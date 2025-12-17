using HtmlToPdf.api.Core.DependencyInjections;
using Microsoft.Net.Http.Headers;

namespace HtmlToPdf.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string ToAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAppConfiguration(Configuration).AddDataServices(Configuration);

            services.AddCors(options =>
            {
                options.AddPolicy(name: ToAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin().AllowAnyMethod().WithHeaders(HeaderNames.ContentType, "ApimSubscriptionKey");
                                  });
            });

            services.AddControllers();
            services.AddSwaggerGen();
        }
    }
}
