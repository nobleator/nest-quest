var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/v0/hello", () => "World");

app.Run();
