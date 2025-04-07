using HtmlToPdf.core;
using HtmlToPdf.core.Helpers;
using HtmlToPdf.core.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HtmlToPdf.api.Core.DependencyInjections
{
    public static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            services.AddSingleton<IHtmlToPdfService, HtmlToPdfService>();

            services.AddTransient<ITransformHelper, TransformHelper>();

            return services;
        }
    }
}
