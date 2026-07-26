using Azure.Identity;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Behaviors;
using Order.API.Controllers;
using Order.API.ExceptionHandlers;
using Order.API.Infrastructure;

namespace Order.API.Extensions
{
    public static class Extension
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddProblemDetails();

            builder.Configuration.AddAzureKeyVault(
                new Uri(builder.Configuration["KeyVaultUrl"] ?? throw new InvalidOperationException("KeyVaultUrl is not configured")),
                new DefaultAzureCredential());

            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);

                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            builder.Services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb"));
            });

            builder.Services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<OrderDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(builder.Configuration.GetConnectionString("AzureServiceBus"));

                    cfg.ConfigureEndpoints(context);
                });
            });

            builder.Services.AddScoped<OrderServices>();
        }
    }
}
