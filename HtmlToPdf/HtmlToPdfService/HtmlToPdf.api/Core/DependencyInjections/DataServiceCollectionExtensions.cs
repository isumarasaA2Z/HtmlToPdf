using HtmlToPdf.api.Data;
using HtmlToPdf.api.Data.Repositories;
using HtmlToPdf.core;
using HtmlToPdf.core.Data;
using HtmlToPdf.core.Data.Repositories;
using HtmlToPdf.core.Helpers;
using HtmlToPdf.core.Interfaces;
using HtmlToPdf.core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HtmlToPdf.api.Core.DependencyInjections
{
    public static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register existing services
            services.AddSingleton<IHtmlToPdfService, HtmlToPdfService>();
            services.AddTransient<ITransformHelper, TransformHelper>();
            services.AddSingleton<ITemplateLoader, TemplateLoader>();

            // Register Database Context
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    options.UseSqlServer(connectionString);
                }
            });

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPdfRepository, PdfRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();

            return services;
        }
    }
}
