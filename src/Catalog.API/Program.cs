using Catalog.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppConfigurations();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
