IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'gtdpad')
BEGIN
	DECLARE @kill varchar(8000) = '';
	SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), spid) + ';'
	FROM master..sysprocesses 
	WHERE dbid = db_id('gtdpad')

	EXEC(@kill)

	DROP DATABASE gtdpad
END
GO