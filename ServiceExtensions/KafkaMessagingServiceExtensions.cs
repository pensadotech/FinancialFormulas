using Confluent.SchemaRegistry;
using FinancialFormulas.Messages.Configuration;
using FinancialFormulas.Messages.Consumers;
using FinancialFormulas.Messages.Producers;
using FinancialFormulas.Messages.Schemas.Avro;
using Microsoft.Extensions.Options;

namespace FinancialFormulas.ServiceExtensions;

public static class KafkaMessagingServiceExtensions
{
    public static IServiceCollection AddKafkaMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        // Register KafkaMessagingOptions configuration using IOptions with validation for properties
        services.AddOptions<KafkaMessagingOptions>()
            .Bind(configuration.GetSection(KafkaMessagingOptions.SectionName))
            .Validate(options => !string.IsNullOrEmpty(options.BootstrapServers), "BootstrapServers is required.")
            .Validate(options => !string.IsNullOrEmpty(options.SchemaRegistryUrl), "SchemaRegistryUrl is required.")
            .Validate(options => !string.IsNullOrEmpty(options.RequestTopic), "RequestTopic is required.")
            .Validate(options => !string.IsNullOrEmpty(options.ResponseTopic), "ResponseTopic is required.")
            .ValidateOnStart();

        // Register the Schema Registry client as a singleton service,
        // configured with the URL from KafkaMessagingOptions
        services.AddSingleton<ISchemaRegistryClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<KafkaMessagingOptions>>().Value;

            // here more specific configuration can be added if needed, such as authentication or other settings
            return new CachedSchemaRegistryClient(new SchemaRegistryConfig
            {
                Url = options.SchemaRegistryUrl
            });
        });

        // register Kafka procedures and consumers
        services.AddSingleton<IAvroSchemaProvider, AvroSchemaProvider>();
        services.AddSingleton< IFormulaResponseProducer , FormulaResponseProducer>();
        services.AddSingleton<FormulaRequestConsumer>();

        return services;
    }
}
