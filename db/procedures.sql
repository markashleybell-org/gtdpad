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
		title,
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
