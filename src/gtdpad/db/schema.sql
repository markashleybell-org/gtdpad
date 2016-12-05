CREATE DATABASE gtdpad
GO

USE gtdpad
GO

-- Create users

PRINT 'Creating users'

CREATE TABLE users (
    id UNIQUEIDENTIFIER NOT NULL,
    username NVARCHAR(64) NOT NULL,
    password NVARCHAR(1024) NOT NULL,
    CONSTRAINT PK_users PRIMARY KEY CLUSTERED (
        id ASC
    )
)

-- Create pages

PRINT 'Creating pages'

CREATE TABLE pages (
    id UNIQUEIDENTIFIER NOT NULL,
    user_id UNIQUEIDENTIFIER NOT NULL,
    title NVARCHAR(128) NOT NULL,
    display_order INT NOT NULL CONSTRAINT DF_pages_display_order DEFAULT 2147483647,
    created DATETIME NOT NULL CONSTRAINT DF_pages_created DEFAULT GETDATE(),
    deleted DATETIME NULL,
    CONSTRAINT PK_pages PRIMARY KEY CLUSTERED (
        id ASC
    )
)

ALTER TABLE pages WITH CHECK ADD CONSTRAINT FK_pages_users_user_id
FOREIGN KEY (user_id) REFERENCES users (id)

-- Create lists

PRINT 'Creating lists'

CREATE TABLE lists (
    id UNIQUEIDENTIFIER NOT NULL,
    page_id UNIQUEIDENTIFIER NOT NULL,
    title NVARCHAR(128) NOT NULL,
    display_order INT NOT NULL CONSTRAINT DF_lists_display_order DEFAULT 2147483647,
    created DATETIME NOT NULL CONSTRAINT DF_lists_created DEFAULT GETDATE(),
    deleted DATETIME NULL,
    CONSTRAINT PK_lists PRIMARY KEY CLUSTERED (
        id ASC
    )
)

ALTER TABLE lists WITH CHECK ADD CONSTRAINT FK_lists_pages_page_id
FOREIGN KEY (page_id) REFERENCES pages (id)

-- Create items

PRINT 'Creating items'

CREATE TABLE items (
    id UNIQUEIDENTIFIER NOT NULL,
    list_id UNIQUEIDENTIFIER NOT NULL,
    body NVARCHAR(MAX) NOT NULL,
    title NVARCHAR(MAX) NULL,
    display_order INT NOT NULL CONSTRAINT DF_items_display_order DEFAULT 2147483647,
    created DATETIME NOT NULL CONSTRAINT DF_items_created DEFAULT GETDATE(),
    deleted DATETIME NULL,
    CONSTRAINT PK_items PRIMARY KEY CLUSTERED (
        id ASC
    )
)

ALTER TABLE items WITH CHECK ADD CONSTRAINT FK_items_lists_list_id
FOREIGN KEY (list_id) REFERENCES lists (id)
GO

PRINT 'Completed Schema Generation'
SELECT 'Completed Schema Generation'

GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ReadPageDeep') AND type in (N'P', N'PC'))
DROP PROCEDURE ReadPageDeep
GO

CREATE PROCEDURE ReadPageDeep (
	@id UNIQUEIDENTIFIER
)
AS
BEGIN 
	SELECT TOP 1 
		id,
		title
	FROM 
		pages 
	WHERE 
		deleted IS NULL 
	AND 
		id = @id

	SELECT 
		id, 
		title, 
		display_order,
		created
	INTO 
		#lists
	FROM 
		lists 
	WHERE 
		deleted IS NULL 
	AND 
		page_id = @id

	SELECT 
		id, 
		title
	FROM 
		#lists 
	ORDER BY 
		display_order, 
		created

	SELECT 
		id,
		list_id,
		body
	FROM 
		items i 
	WHERE
		deleted IS NULL 
	AND EXISTS 
		(SELECT * FROM #lists l WHERE l.id = i.list_id) 
	ORDER BY
		display_order, 
		created
END	
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'GetDisplayOrder') AND type = N'TF')
DROP FUNCTION GetDisplayOrder
GO

CREATE FUNCTION GetDisplayOrder (@s NVARCHAR(MAX), @sep CHAR(1)) 
RETURNS @DisplayOrder TABLE (
	id UNIQUEIDENTIFIER, 
	pos INT IDENTITY(0,1)
)
AS
BEGIN
	INSERT INTO @DisplayOrder (id)
	SELECT CONVERT(UNIQUEIDENTIFIER, value) FROM STRING_SPLIT(@s, @sep)
	RETURN
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UpdatePageDisplayOrder') AND type in (N'P', N'PC'))
DROP PROCEDURE UpdatePageDisplayOrder
GO

CREATE PROCEDURE UpdatePageDisplayOrder (
	@userid UNIQUEIDENTIFIER,
	@order NVARCHAR(MAX)
)
AS
BEGIN 
	SELECT id, pos INTO #ids FROM GetDisplayOrder(@order, '|')

	MERGE INTO pages p
	   USING #ids ids 
		  ON p.id = ids.id
		     AND p.user_id = @userid
	WHEN MATCHED THEN
	   UPDATE 
		  SET p.display_order = ids.pos;

	DROP TABLE #ids
END	
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UpdateListDisplayOrder') AND type in (N'P', N'PC'))
DROP PROCEDURE UpdateListDisplayOrder
GO

CREATE PROCEDURE UpdateListDisplayOrder (
	@pageid UNIQUEIDENTIFIER,
	@order NVARCHAR(MAX)
)
AS
BEGIN 
	SELECT id, pos INTO #ids FROM GetDisplayOrder(@order, '|')

	MERGE INTO lists l
	   USING #ids ids 
		  ON l.id = ids.id
		     AND l.page_id = @pageid
	WHEN MATCHED THEN
	   UPDATE 
		  SET l.display_order = ids.pos;

	DROP TABLE #ids
END	
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UpdateItemDisplayOrder') AND type in (N'P', N'PC'))
DROP PROCEDURE UpdateItemDisplayOrder
GO

CREATE PROCEDURE UpdateItemDisplayOrder (
	@listid UNIQUEIDENTIFIER,
	@order NVARCHAR(MAX)
)
AS
BEGIN 
	SELECT id, pos INTO #ids FROM GetDisplayOrder(@order, '|')

	MERGE INTO items i
	   USING #ids ids 
		  ON i.id = ids.id
		     AND i.list_id = @listid
	WHEN MATCHED THEN
	   UPDATE 
		  SET i.display_order = ids.pos;

	DROP TABLE #ids
END	
GO

