using Order.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers();

app.UseSwagger();

app.UseSwaggerUI();

app.Run();
