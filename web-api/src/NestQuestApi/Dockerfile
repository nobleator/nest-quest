# Use .NET SDK for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project and restore dependencies
COPY ./*.csproj ./
RUN dotnet restore

# Build the app
COPY . ./
RUN dotnet publish NestQuestApi.csproj -c Release -o out

# Use runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/out .

# Apply EF Core migrations and start the app
ENTRYPOINT ["sh", "-c", "dotnet NestQuestApi.dll; dotnet ef database update --no-build"]
