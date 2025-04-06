## Local Setup
This service uses Entity Framework code-first database migrations. This requires installation of the dotnet EF CLI tools:
> dotnet tool install --global dotnet-ef

The first time you run this service locally you will need to create the SQLite database:
> dotnet ef migrations add InitialCreate

Any time the schema is updated, you will need to run:
> dotnet ef migrations add UpdateSchema

Any pending migrations should be automatically applied on app startup.
