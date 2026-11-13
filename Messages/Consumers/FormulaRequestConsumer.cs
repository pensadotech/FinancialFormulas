using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using FinancialFormulas.Messages.Configuration;
using FinancialFormulas.Messages.Producers;
using FinancialFormulas.Models;
using FinancialFormulas.Services;
using Microsoft.Extensions.Options;

namespace FinancialFormulas.Messages.Consumers;

public class FormulaRequestConsumer : BackgroundService
{
    private readonly KafkaMessagingOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IFormulaResponseProducer _responseProducer;
    private readonly ILogger<FormulaRequestConsumer> _logger;
    private readonly IConsumer<string, byte[]> _consumer;
    private readonly AvroDeserializer<GenericRecord> _avroDeserializer;

    public FormulaRequestConsumer(
        IOptions<KafkaMessagingOptions> options,  // IOptions
        IServiceScopeFactory scopeFactory,
        IFormulaResponseProducer responseProducer,
        ISchemaRegistryClient schemaregistryClient,
        ILogger<FormulaRequestConsumer> logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _responseProducer = responseProducer;
        _logger = logger;

        // Itialize the Avro deserializer with the schema registry client
        // for deserializing messages from Kafka
        _avroDeserializer = new AvroDeserializer<GenericRecord>(schemaregistryClient);

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.ConsumerGroupId,
            AutoOffsetReset = ParseAutoOffsetReset(_options.AutoOffsetReset),
            EnableAutoCommit = false  // Disable auto-commit to manage offsets manually
        };
    
        _consumer = new ConsumerBuilder<string, byte[]>(consumerConfig).Build();
    }

    // The main execution loop of the consumer, which runs until the service is stopped or canceled.
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) 
    {
        // Subscribe to the request topic to start consuming messages
        _consumer.Subscribe(_options.RequestTopic);

        // Run the consumer loop until cancellation is requested, processing messages and producing responses
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Consume a message from the request topic
                var consumeResult = _consumer.Consume(stoppingToken);

                if (consumeResult?.Message?.Value is null)
                {
                    _logger.LogWarning("Received a null message from Kafka.");
                    continue;
                }
                
                // Deserialize the avro message to a GenericREcord
                var message = await _avroDeserializer.DeserializeAsync(
                    consumeResult.Message.Value, 
                    false, 
                    new SerializationContext(MessageComponentType.Value, 
                                              consumeResult.Topic, 
                                              consumeResult.Message.Headers));

                if (message is null)
                {
                    _logger.LogWarning("Deserialized message is null.");
                    continue;
                }

                // Convert teh Avro GenericREcord to a FormulaRequest object
                var formulaRequest = AvroFormulaRequestMapper.Map(message);

                // Execute the formulas using the financial service
                var response = ExecuteFormulas(formulaRequest);

                // Retrieve the correlationId from the message headers to maintain traceability
                var correlationId = GetHeaderValue(consumeResult.Message.Headers, "correlationId");

                // Produce the response to the response topic, including the correlationId and original headers
                await _responseProducer.ProduceAsync(
                    correlationId, 
                    response, 
                    consumeResult.Message.Headers, 
                    stoppingToken);

                // Commit Offset after processing the message successfully
                _consumer.Commit(consumeResult);

            }
            catch(OperationCanceledException)
            {
                // Graceful shutdown, no action needed
                _logger.LogInformation("Kafka consumer operation canceled.");
                break;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error from topic {Topic} with reason {ErrorReason}", 
                                _options.RequestTopic,
                                 ex.Error.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while processing Kafka message.");
            }
        }

    }

    // Dispose method to clean up resources when the consumer is stopped or disposed
    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }

    // helper method to execute formulas using the financial service within a new scope
    private FormulaBatchResponse ExecuteFormulas(FormulaRequest request)
    {
        // Create a new scope to resolve the IFinancialService dependency for executing formulas
        using var scope = _scopeFactory.CreateScope();

        // Resolve the IFinancialService from the service provider within the scope
        var financialService = scope.ServiceProvider.GetRequiredService<IFinancialService>();

        // Execute the formulas and get the responses
        var responses = financialService.ExecuteFormulas(request);

        // Create and return a FormulaBatchResponse containing the statistics and individual responses
        return new FormulaBatchResponse
        {
            Statistics = new FormulaBatchStatistics
            {
                Total = responses.Count,
                Succeeded = responses.Count(item => item.Error is null),
                Failed = responses.Count(item => item.Error is  not null)
            },
            Responses = responses
        };
    }

    // Helper method to parse the AutoOffsetReset value from configuration,
    // defaulting to Earliest if parsing fails
    private static AutoOffsetReset ParseAutoOffsetReset(string value)
    {
        return Enum.TryParse<AutoOffsetReset>(value, true, out var autoOffsetReset)
            ? autoOffsetReset
            : AutoOffsetReset.Earliest;
    }

    // Helper method to retrieve a header value from Kafka message headers,
    // returning null if the header is not found
    private static string? GetHeaderValue(Headers? headers, string headerName)
    {
        if (headers is null)
            return null;

        var header = headers.FirstOrDefault( item =>
            string.Equals(item.Key, headerName, StringComparison.OrdinalIgnoreCase));
        
        if (header is null) return null;

        var value = header.GetValueBytes();

        return value is null ? null : System.Text.Encoding.UTF8.GetString(value);
    }

}
