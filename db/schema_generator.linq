<Query Kind="FSharpProgram" />

// Generates CREATE/ALTER TABLE statements for specified tables with common audit fields and constraints

type StatementType = ALTER | CREATE

type ColType = INT | GUID | BIT | DATE | MONEY | TEXT | CHR of int

type Default = NONE | NULL | TRUE | FALSE | NOW | VAL of int

type ColSpec = 
    | Null of string * ColType
    | NotNull of string * ColType * Default
    | Identity of string * ColType * int * int

type ConstraintSpec = 
    | ASC of string
    | DESC of string

type FKSpec = ForeignKey of string * string * string

type Table = {name:string; stype:StatementType; cols:ColSpec list; constraints:ConstraintSpec list; fks:FKSpec list}

// Common column and foreign key definitions
let commoncols = []

let commonfks = []

/////////////////////////////////////////////////////////////////////////////////////
// Begin table specifications
/////////////////////////////////////////////////////////////////////////////////////
let users = {
    name="users"
    stype=CREATE
    cols=[NotNull("id", GUID, NONE)
          NotNull("username", CHR(64), NONE)
          NotNull("password", CHR(1024), NONE)]
    constraints = [ASC("id")]
    fks = []
}

let pages = {
    name="pages"
    stype=CREATE
    cols=[NotNull("id", GUID, NONE)
          NotNull("user_id", GUID, NONE)
          NotNull("name", CHR(128), NONE)
          NotNull("display_order", INT, VAL(Int32.MaxValue))
          Null("deleted", DATE)]
    constraints = [ASC("id")]
    fks = [ForeignKey("user_id", "users", "id")]
}

let lists = {
    name="lists"
    stype=CREATE
    cols=[NotNull("id", GUID, NONE)
          NotNull("page_id", GUID, NONE)
          NotNull("name", CHR(128), NONE)
          NotNull("display_order", INT, VAL(Int32.MaxValue))
          Null("deleted", DATE)]
    constraints = [ASC("id")]
    fks = [ForeignKey("page_id", "pages", "id")]
}

let items = {
    name="items"
    stype=CREATE
    cols=[NotNull("id", GUID, NONE)
          NotNull("list_id", GUID, NONE)
          NotNull("text", CHR(1024), NONE)
          NotNull("display_order", INT, VAL(Int32.MaxValue))
          Null("deleted", DATE)]
    constraints = [ASC("id")]
    fks = [ForeignKey("list_id", "lists", "id")]
}

// A list of all the tables we need to create SQL for
let tables = [
    users
    pages
    lists
    items
]
/////////////////////////////////////////////////////////////////////////////////////
// End table specifications
/////////////////////////////////////////////////////////////////////////////////////

let indent = sprintf "    %s" 
let indent2 = indent >> indent 
let br = Environment.NewLine // Shorthand for newline
let brbr = br + br
let commabr = sprintf ",%s" br // Shorthand for comma followed by newline

let coltype t = 
    match t with
    | INT -> "INT"
    | GUID -> "UNIQUEIDENTIFIER"
    | BIT -> "BIT"
    | MONEY -> "DECIMAL(18,2)"
    | DATE -> "DATETIME"
    | CHR l -> sprintf "NVARCHAR(%i)" l
    | TEXT -> "NVARCHAR(MAX)"
    
let col n t = sprintf "%s %s" (indent n) (coltype t)

let def tblname n d =
    let cn = sprintf " CONSTRAINT DF_%s_%s DEFAULT %s" tblname n 
    match d with
    | NONE -> ""
    | NULL -> cn "NULL"
    | TRUE -> cn "1"
    | FALSE -> cn "0"
    | NOW -> cn "GETDATE()"
    | VAL i -> cn (sprintf "%i" i)

// Column definition statement
let coldef tbl c = 
    match c with
    | Null (n, t) -> sprintf "%s NULL" (col n t)
    | NotNull (n, t, d) -> sprintf "%s NOT NULL%s" (col n t) (def tbl.name n d)
    | Identity (n, t, a, b) -> sprintf "%s IDENTITY(%i,%i) NOT NULL" (col n t) a b
    
// All column definitions for a table
let cols tbl = 
    let ccols = match tbl.stype with 
                | CREATE -> commoncols
                | ALTER -> []
    ccols
    |> List.append tbl.cols
    |> List.map (coldef tbl)
    |> String.concat commabr
    |> (fun s -> match tbl.stype with 
                 | CREATE -> sprintf "%s," s
                 | ALTER -> sprintf "%s" s)

// PK constraint field
let cnstr c = 
    let cf col dir = indent2 (sprintf "%s %s" col dir)
    match c with 
    | ASC s -> cf s "ASC"
    | DESC s -> cf s "DESC"

// List of PK constraint fields
let cnstrs cnstrlist = 
    cnstrlist
    |> List.map cnstr 
    |> String.concat commabr

// FK constraint statement
let fk tblname fk =
    let fkd = sprintf "ALTER TABLE %s WITH CHECK ADD CONSTRAINT FK_%s_%s_%s%sFOREIGN KEY (%s) REFERENCES %s (%s)" 
    match fk with
    | ForeignKey (c, kt, kc) -> fkd tblname tblname kt c br c kt kc

// List of foreign key statements for a table
let fks tbl = 
    let cfks = match tbl.stype with 
               | CREATE -> commonfks
               | ALTER -> []
    cfks
    |> List.append tbl.fks
    |> List.map (fk tbl.name)
    |> String.concat br

// CREATE and ALTER statement generators
let create tbl = 
    let com = sprintf "-- Create %s" tbl.name
    let create = sprintf "CREATE TABLE %s (" tbl.name
    let constr = sprintf "    CONSTRAINT PK_%s PRIMARY KEY CLUSTERED (" tbl.name
    let wth = sprintf "    )%s)" br
    let msg = sprintf "PRINT 'Creating %s'" tbl.name

    [com; ""; msg; ""; create; (cols tbl); constr; (cnstrs tbl.constraints); wth]
    |>  (fun l -> match tbl.fks with
                  | [] -> l
                  | _ -> l @ [""; (fks tbl)])
    |> String.concat br

let alter tbl = 
    let com = sprintf "-- Alter %s" tbl.name
    let alter = sprintf "ALTER TABLE %s ADD" tbl.name
    let msg = sprintf "PRINT 'Altering %s'" tbl.name

    [com; ""; msg; ""; alter; (cols tbl)] 
    |>  (fun l -> match tbl.fks with
                  | [] -> l
                  | _ -> l @ [""; (fks tbl)])
    |> String.concat br

let stmt tbl =
    match tbl.stype with
    | CREATE -> create tbl
    | ALTER -> alter tbl

let stmts tbllist =
    tbllist
    |> List.map stmt
    |> String.concat brbr

// Make sure we are using this script's path as the working directory
Directory.SetCurrentDirectory (Path.GetDirectoryName Util.CurrentQueryPath)

let outputlist = [
    "CREATE DATABASE gtdpad"
    "GO"
    ""
    "USE gtdpad"
    "GO"
    ""
    (stmts tables)
    ""
    "GO"
    ""
    "PRINT 'Completed Schema Generation'"
    "SELECT 'Completed Schema Generation'"
    ""
    "INSERT INTO users (id, username, password) VALUES ('47d2911f-c127-40c8-a39a-fb13634d2ae9', 'admin', 'admin')"
    ""
]

let output = sprintf "%s" (outputlist |> String.concat br)
                
output |> Dump |> ignore

File.WriteAllText("schema.sql", output)