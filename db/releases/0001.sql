USE gtdpad
GO

-- Alter items

PRINT 'Applying Release script'

IF COL_LENGTH('items','title') IS NULL
BEGIN
	ALTER TABLE items ADD
		title NVARCHAR(MAX) NULL

	ALTER TABLE items ALTER COLUMN
		body NVARCHAR(MAX) NOT NULL
END

PRINT 'Release script applied'
SELECT 'Release script applied'

GO
