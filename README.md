# How to set up a development environment

 - Run `create-empty-dev-db.cmd` to create the database
 - Build the solution
 - Run `gtdpad/run.cmd` to start the server
 - Navigate to `http://localhost:5000/signup` to create an account

 ## Running locally with live DB

 - Create a `config.localproduction.json` file with the live credentials
 - Run `gtdpad\local-production.ps1`
 - If login doesn't work, clear cookies for `localhost`
