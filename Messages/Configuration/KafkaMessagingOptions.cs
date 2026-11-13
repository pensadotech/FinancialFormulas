
namespace FinancialFormulas.Messages.Configuration;

public class KafkaMessagingOptions
{
    public const string SectionName = "Kafka";
    public string BootstrapServers { get; set; } = string.Empty;
    public string SchemaRegistryUrl { get; set; } = string.Empty;
    public string RequestTopic { get; set; } = string.Empty;
    public string ResponseTopic { get; set; } = string.Empty;
    public string ConsumerGroupId { get; set; } = "financial-formulas-consumer";
    public string AutoOffsetReset { get; set; } = "Earliest";
}
