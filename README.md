# Todo API

> API for managing todo tasks
* Can be integrated with an openid enabled authorization server. 
* Has api endpoints with cookie authentication and csrf protection for SPAs.
* Has api endpoints with JWT bearer authorization and no csrf protection for native clients and servers.
* Employs best practices such as [SOLID](https://en.wikipedia.org/wiki/SOLID) and [Onion architecture](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/).
* Comprehensive Swagger documentation.


### Build Setup ###

* Ensure you have installed [dotnet 5.0](https://dotnet.microsoft.com/download/dotnet/5.0), [Entity Framework tools](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet) and [Microsoft SQL Server](https://docs.microsoft.com/en-us/sql/database-engine/install-windows/install-sql-server) at least developer edition.
* Open the Web folder of the solution, copy appsettings.example.json and save as appsettings.json. Set the DefaultConnection field in the ConnectionStrings section of appsettings.json to point to a database for the app in Microsoft SQL Server. Set your desired value for other fields in appsettings.json
* From your CLI, navigate to Infrastructure folder of the solution and run the following commands:

```bash
# run migrations
$ dotnet ef database update --startup-project ../Web/Web.csproj

# serve at http://localhost:5002 and https://localhost:5003
$ dotnet run --project ../Web/Web.csproj

# Trust dotnet developer certificates. If the browser is still not trusting the certificate, read more [here](https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-5.0&tabs=visual-studio#trust-the-aspnet-core-https-development-certificate-on-windows-and-macos)
$ dotnet dev-certs https --clean
$ dotnet dev-certs https --trust
```
* Open https://localhost:5003/swagger in your browser to see swagger doc.
* In Microsoft SQL Server, enable [full text search](https://docs.microsoft.com/en-us/sql/relational-databases/search/get-started-with-full-text-search)
 on NormalizedName column of TodoItems table.


### Who do I talk to? ###

*  [Muyiwa Awoniyi](mailto:muyiwaawoniyi@yahoo.com)