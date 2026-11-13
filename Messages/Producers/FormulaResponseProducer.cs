using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using FinancialFormulas.Messages.Configuration;
using FinancialFormulas.Messages.Schemas.Avro;
using FinancialFormulas.Models;
using Microsoft.Extensions.Options;

namespace FinancialFormulas.Messages.Producers;

// Kakfa producer
public class FormulaResponseProducer : IFormulaResponseProducer, IDisposable
{
    private const string ProducerMessageKey = "FinancialFormulas";
    private const string CorrelationIdHeaderName = "correlationId";

    private readonly KafkaMessagingOptions _options;
    private readonly IAvroSchemaProvider _schemaProvider;
    private readonly IProducer<string, GenericRecord> _producer;
    
    public FormulaResponseProducer(
        IOptions<KafkaMessagingOptions> options,
        IAvroSchemaProvider schemaProvider,
        ISchemaRegistryClient schemaRegistryClient)
    {
        _options = options.Value;
        _schemaProvider = schemaProvider;

        // Configure the Kafka producer with the necessary settings,
        // including bootstrap servers, acknowledgment settings, idempotence, and retry configurations.
        var producetConfig = new ProducerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000
        };

        // Build the Kafka producer using the ProducerBuilder,
        // specifying the key and value types (string and GenericRecord).
        _producer = new ProducerBuilder<string, GenericRecord>(producetConfig)
            .SetValueSerializer(new AvroSerializer<GenericRecord>(schemaRegistryClient))
            .Build();
    }

    public async Task ProduceAsync(string? correlationId, FormulaBatchResponse response, Headers? sourceHeaders,
                                  CancellationToken cancellationToken)
    {
        // Map the FormulaBatchResponse object to a GenericRecord
        // using the Avro schema provided by the IAvroSchemaProvider.
        var responseRecord = AvroFormulaRequestMapper.Map(response, _schemaProvider.FormulaResponseSchema);

        // If sourceHeaders is null, create a new Headers object;
        // otherwise, use the provided sourceHeaders.
        var headers = CopyHeaders(sourceHeaders);
        SetCorrelationIdHeader(headers, correlationId);

        // Create a new Kafka message with the specified key,
        // value (the mapped GenericRecord), and headers.
        var message = new Message<string, GenericRecord>
        {
            Key = ProducerMessageKey,
            Value = responseRecord,
            Headers = headers
        };

        // Produce the message to the specified response topic asynchronously,
        // passing in the cancellation token for cancellation support.
        await _producer.ProduceAsync(_options.ResponseTopic, message, cancellationToken);

        // to manually force sending teh message, you can use the following line instead of ProduceAsync
        // _producer.Produce() or use _producer.Flush(). However, this will have perfromance
        // implications. It is better to use ProduceAsync() for sending a message.

    }

    // This method copies the headers from the source headers to a new Headers object.
    // If the source headers are null, it returns an empty Headers object. It iterates
    // through each header in the source headers and adds them to the target Headers object.
    private static Headers CopyHeaders(Headers? headers)
    {
        var target = new Headers();

        if (headers is null)
        {
            return target;
        }

        foreach (var header in headers)
        {
            target.Add(header.Key, header.GetValueBytes());
        }

        return target;
    }

    // This method sets the correlation ID header in the provided Headers object.
    // If the correlation ID is null or whitespace, it does nothing. Otherwise,
    // it removes any existing correlation ID header and adds a new one with the provided value.
    private static void SetCorrelationIdHeader(Headers headers, string? correlationId)
    {
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            return;
        }

        headers.Remove(CorrelationIdHeaderName);
    }

    // This method sets the correlation ID header in the provided Headers object.     
    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }

}
