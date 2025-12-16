using HtmlToPdf.core;
using HtmlToPdf.core.Helpers;
using HtmlToPdf.core.Interfaces;
using HtmlToPdf.core.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HtmlToPdf.api.Core.DependencyInjections
{
    public static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            services.AddSingleton<IHtmlToPdfService, HtmlToPdfService>();

            services.AddTransient<ITransformHelper, TransformHelper>();

            services.AddSingleton<ITemplateLoader, TemplateLoader>();

            return services;
        }
    }
}
