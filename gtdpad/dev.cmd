start node-sass -w --output-style expanded static\css\app.scss static\css\app.css
start tsc -w -p .
SET ASPNETCORE_ENVIRONMENT=Development
dotnet watch run