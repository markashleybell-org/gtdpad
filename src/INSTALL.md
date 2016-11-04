
# Running under IIS

- Install [.NET Core Windows Server Hosting Bundle](https://go.microsoft.com/fwlink/?LinkID=827547) on the Server
- Restart the IIS server (`net stop/start` or `iisreset`)
- Create app pool with .NET CLR set to `No Managed Code`, running as a user which can access the `dotnet` command
- Provision a Data Protection Key hive by running https://github.com/aspnet/DataProtection/blob/dev/Provision-AutoGenKeys.ps1 with the new app pool name
- Build the project and then run `dotnet publish`, which will create a `publish` folder containing all the binaries and the correct `web.config` settings
- Copy the contents of the `publish` folder into the server web root
- Create new IIS site pointing at the new folder, using the app pool created previously

