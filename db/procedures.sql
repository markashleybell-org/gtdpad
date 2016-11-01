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
