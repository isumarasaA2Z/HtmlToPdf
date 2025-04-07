using Microsoft.Extensions.Options;

namespace HtmlToPdf.api.Core.DependencyInjections
{
    public static class ConfigurationServiceCollectionExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration config)
        {
            //services.Configure<CosmosDbConfiguration>(config.GetSection("CosmosDbSettings"));
            //services.AddSingleton<IValidateOptions<CosmosDbConfiguration>, CosmosDbConfigurationValidation>();
            //var cosmosDbConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<CosmosDbConfiguration>>().Value;
            //services.AddSingleton<ICosmosDbConfiguration>(cosmosDbConfiguration);

            return services;
        }
    }
}
