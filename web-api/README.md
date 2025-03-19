## Local Setup
This service uses Entity Framework code-first database migrations. The first time you run this service locally you will need to create the SQLite database:
> dotnet ef migrations add InitialCreate

Migrations will be automatically applied on startup.