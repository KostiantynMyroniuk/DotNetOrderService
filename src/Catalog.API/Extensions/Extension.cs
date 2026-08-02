using Catalog.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Extensions
{
    public static class Extension
    {
        public static void AddAppConfigurations(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<CatalogDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb"));
            });

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            });
        }
    }
}
