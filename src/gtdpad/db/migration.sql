USE gtdpad_old
GO

SELECT 
	id as oldid, 
	NEWID() as id,
	'47D2911F-C127-40C8-A39A-FB13634D2AE9' as user_id,  -- Update this GUID with the real ID of your user account
	title as title,
	displayorder as display_order,
	created_at as created
INTO #pages
FROM pages
WHERE user_id = 7
AND deleted = 0

SELECT 
	i.id as oldid,
	NEWID() as id,
	p.id as page_id,
	i.title as title,
	i.displayorder as display_order,
	i.created_at as created
INTO #lists
FROM items i
INNER JOIN #pages p ON p.oldid = i.page_id
WHERE i.user_id = 7
AND deleted = 0

SELECT 
	li.id as oldid,
	NEWID() as id,
	l.id as list_id,
	li.body as body,
	li.displayorder as display_order,
	li.created_at as created
INTO #items
FROM listitems li
INNER JOIN #lists l ON l.oldid = li.item_id
WHERE li.user_id = 7
AND deleted = 0

--SELECT * FROM #pages
--ORDER BY display_order, created

--SELECT * FROM #lists
--ORDER BY display_order, created

--SELECT * FROM #items
--ORDER BY display_order, created

INSERT INTO gtdpad.dbo.pages (id, user_id, title, display_order, created)
SELECT id, user_id, title, display_order, created FROM #pages

INSERT INTO gtdpad.dbo.lists (id, page_id, title, display_order, created)
SELECT id, page_id, title, display_order, created FROM #lists

INSERT INTO gtdpad.dbo.items (id, list_id, body, display_order, created)
SELECT id, list_id, body, display_order, created FROM #items

DROP TABLE #items
DROP TABLE #lists
DROP TABLE #pages