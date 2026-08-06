var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
