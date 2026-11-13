using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace FinancialFormulas.ServiceExtensions;

public static class ControllerServiceExtensions
{
    // Configur controllers with custom options
    public static IServiceCollection ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            // General errors across all controllers: 400, 406, 500
            // This errors are wil apply possible bad request errors accross all controllers
            // This behavior will overwrite API conventions 
            options.Filters.Add(
                new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
            options.Filters.Add(
                new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
            options.Filters.Add(
                new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
            options.Filters.Add(
                new ProducesDefaultResponseTypeAttribute());

            // Returns a 406 not acceptable, if requested media type is not supported
            options.ReturnHttpNotAcceptable = true;

            // Remove "text/json" media type
            var jsonOutputFormatter = options.OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>()
                .FirstOrDefault();

            if (jsonOutputFormatter != null)
            {
                jsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
            }

            var jsonInputFormatter = options.InputFormatters
                .OfType<SystemTextJsonInputFormatter>()
                .FirstOrDefault();

            if (jsonInputFormatter != null)
            {
                jsonInputFormatter.SupportedMediaTypes.Remove("text/json");
            }

        })
        .AddXmlDataContractSerializerFormatters(); // Acept XML format as well as JSON format

        return services;
    }

    // Configuew MVC options for input and output formatters
    public static IServiceCollection AddConfiguredMVCOptions(this IServiceCollection services)
    {

        services.PostConfigure<MvcOptions>(options =>
        {
            options.OutputFormatters.RemoveType<StringOutputFormatter>();

            var jsonOutputFormatter = options.OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>()
                .FirstOrDefault();

            if (jsonOutputFormatter != null)
            {
                jsonOutputFormatter.SupportedMediaTypes.Clear();
                jsonOutputFormatter.SupportedMediaTypes.Add("application/json");
            }

            var jsonInputFormatter = options.InputFormatters
                .OfType<SystemTextJsonInputFormatter>()
                .FirstOrDefault();

            if(jsonInputFormatter != null)
            {
                jsonInputFormatter.SupportedMediaTypes.Clear();
                jsonInputFormatter.SupportedMediaTypes.Add("application/json");
            }

            var xmlOutputFormatter = options.OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()
                .FirstOrDefault();

            if (xmlOutputFormatter != null)
            {
                xmlOutputFormatter.SupportedMediaTypes.Clear();
                xmlOutputFormatter.SupportedMediaTypes.Add("application/xml");
            }

            var xmlInputFormatter = options.InputFormatters
                .OfType<XmlDataContractSerializerInputFormatter>()
                .FirstOrDefault();

            if (xmlInputFormatter != null)
            {
                xmlInputFormatter.SupportedMediaTypes.Clear();
                xmlInputFormatter.SupportedMediaTypes.Add("application/xml");
            }
        });


        return services;
    }


}
