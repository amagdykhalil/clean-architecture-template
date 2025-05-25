using Asp.Versioning;

namespace SolutionName.API.Extensions.Startup
{
    public static class APIVersioningExtentions
    {
        public static void AddAPIVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(apiVersion =>
            {
                apiVersion.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                apiVersion.AssumeDefaultVersionWhenUnspecified = true;
                apiVersion.ReportApiVersions = true;
                apiVersion.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Version"),
                    new MediaTypeApiVersionReader("X-Version"));
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
        }
    }
}
