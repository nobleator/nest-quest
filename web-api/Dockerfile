# Use .NET SDK for building the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy project and restore dependencies
COPY ./*.csproj ./
RUN dotnet restore

# Build the app
COPY . ./
RUN dotnet publish -c Release -o out

# Use runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "HelloWorldApi.dll"]
