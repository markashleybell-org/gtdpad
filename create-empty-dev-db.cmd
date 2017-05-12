@ECHO OFF
CLS
COLOR 0E
ECHO.
CHOICE /M "WARNING: This will completely destroy any existing gtdpad database. Proceed?"
IF ERRORLEVEL 2 GOTO END
IF ERRORLEVEL 1 GOTO CREATE
:CREATE
COLOR
ECHO.
COPY /Y "db\nocount.sql" + "db\kill.sql" + "db\schema.sql" + "db\procedures.sql" create-and-seed-db.sql >NUL
ECHO Creating database...
sqlcmd -i create-and-seed-db.sql
DEL /Q create-and-seed-db.sql
ECHO Database gtdpad created successfully
ECHO.
PAUSE
:END
COLOR