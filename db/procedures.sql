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

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'GetDisplayOrder') AND type = N'IF')
DROP FUNCTION GetDisplayOrder
GO

CREATE FUNCTION GetDisplayOrder (@s NVARCHAR(4000), @sep CHAR(1)) RETURNS TABLE
AS RETURN (
	WITH ids(pos, start, stop) AS (
		SELECT
			0,
			1,
			CHARINDEX(@sep, @s)
		UNION ALL
		SELECT
			pos + 1,
			stop + 1,
			CHARINDEX(@sep, @s, stop + 1)
		FROM
			ids
		WHERE
			stop > 0
    )
    SELECT
		SUBSTRING(@s, start, CASE WHEN stop > 0 THEN stop - start ELSE 4000 END) AS id,
		pos
    FROM
		ids
)
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UpdatePageDisplayOrder') AND type in (N'P', N'PC'))
DROP PROCEDURE UpdatePageDisplayOrder
GO

CREATE PROCEDURE UpdatePageDisplayOrder (
	@userid UNIQUEIDENTIFIER,
	@order NVARCHAR(4000)
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
	@order NVARCHAR(4000)
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
	@order NVARCHAR(4000)
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
