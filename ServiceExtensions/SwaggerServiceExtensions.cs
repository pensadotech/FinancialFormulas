using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FinancialFormulas.Filters;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace FinancialFormulas.ServiceExtensions
{
    public static class SwaggerServiceExtensions
    {
        // AddConfiguredSwagger() is a custom extension method that configures Swagger/OpenAPI
        // for the application. It sets up the necessary services and options to generate API
        // documentation and provide an interactive UI for testing API endpoints. This method
        // is typically defined in a separate class (e.g., ServiceExtensions) and is used to
        // centralize and organize the configuration of Swagger/OpenAPI for better maintainability
        // and readability of the code.
        public static IServiceCollection AddConfiguredSwagger(this IServiceCollection services)
        {
            // Disvover versionin within the application
            var provider = services.BuildServiceProvider()
                                   .GetRequiredService<IApiVersionDescriptionProvider>();

            services.AddSwaggerGen(c =>
            {
                if (provider != null)
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        c.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo()
                        {
                            Title = $"My API {description.ApiVersion}",
                            Version = description.ApiVersion.ToString(),
                            Description = description.IsDeprecated ? "This API version has been deprecated." : null,
                            Contact = new OpenApiContact()
                            {
                                Email = "Pensadotech@gmail.com",
                                Name = "Pensadotech",
                                Url = new Uri("https://pensadotech.github.io/ ")
                            },
                            License = new OpenApiLicense()
                            {
                                Name = "MIT License",
                                Url = new Uri("https://opensource.org/licenses/MIT")
                            }
                        });
                    }
                }

                // Add classes XML comments as part of OpenAPI documentation
                // Configure the project file PropertyGroup (e.g. FinancialFormulas.csproj -> PropertyGroup)
                // Set the <GenerateDocumentationFile> property, which is required to enable the
                // generation of the XML documentation file. Without this setting,
                // the compiler does not produce the XML file that Swagger expects.
                // Example  :
                //   <GenerateDocumentationFile>true</GenerateDocumentationFile> 
                // Optional: For a mre specific control to specify the XML file name, add
                //   <DocumentationFile>$(OutputPath)FinancialFormulas.xml</DocumentationFile> 
                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                c.IncludeXmlComments(xmlCommentsFullPath);

                // FILTERS
                // Add custom operation filter to remove text/plain filter in web page
                c.OperationFilter<RemovePlainTextMediaTypeFilter>();

                // Using Swashbuckle.AspNetCore.Annotations and adding to 
                // each method, [SwaggerOperation(Summary = "myDescription, OperationId = "Post_Person")] 
                // REf: https://medium.com/c-sharp-progarmming/configure-annotations-in-swagger-documentation-for-asp-net-core-api-8215596907c7
                c.EnableAnnotations();

            });

            return services;
        }

        // AddConfiguredVersioning() is a custom extension method that configures API versioning
        // for the application. It sets up the necessary services and options to support versioning
        // of API endpoints, allowing clients to specify which version of the API they want to use.
        // This method is typically defined in a separate class (e.g., ServiceExtensions) and is
        // used to centralize and organize the configuration of API versioning for better
        // maintainability and readability of the code.
        public static IServiceCollection AddConfiguredVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            }).AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                //options.AssumeDefaultVersionWhenUnspecified = true;
            });

            return services;
        }
    }
}
