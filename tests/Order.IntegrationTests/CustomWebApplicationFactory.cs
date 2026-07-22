using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Order.API.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using Testcontainers.MsSql;

namespace Order.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer = 
            new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
                .Build();

        public async Task InitializeAsync() => await _msSqlContainer.StartAsync();
        async Task IAsyncLifetime.DisposeAsync() => await _msSqlContainer.DisposeAsync();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var discriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<OrderDbContext>));
                if (discriptor != null)
                    services.Remove(discriptor);

                services.AddDbContext<OrderDbContext>(options =>
                {
                    options.UseSqlServer(_msSqlContainer.GetConnectionString());
                });

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();

                scope.ServiceProvider.GetRequiredService<OrderDbContext>().Database.Migrate();
            });
        }
    }
}
