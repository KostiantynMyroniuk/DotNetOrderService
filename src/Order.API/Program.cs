using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Order.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();

builder.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers();

app.UseSwagger();

app.UseSwaggerUI();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
