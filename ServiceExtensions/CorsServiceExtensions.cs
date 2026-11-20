namespace FinancialFormulas.ServiceExtensions;

public static class CorsServiceExtensions
{
    public static IServiceCollection AddConfiguredCors(this IServiceCollection services, string localOnlyCorsPolicy)
    {
        // Add CORS policy to allow only local requests
        services.AddCors(options =>
        {
            options.AddPolicy(localOnlyCorsPolicy, policy =>
            {
                policy
                   .SetIsOriginAllowed(origin =>
                   {
                       if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                       {
                           return false;
                       }

                       return uri.IsLoopback || string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase);

                   })
                  .WithOrigins("http://localhost", "http://127.0.0.1","mydomain.com")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
            });
        });

        return services;
    }
}
