using Confluent.Kafka;
using FinancialFormulas.Models;

namespace FinancialFormulas.Messages.Producers;

public interface IFormulaResponseProducer
{
    Task ProduceAsync(string? correlationId, FormulaBatchResponse response, Headers? sourceHeaders, CancellationToken cancellationToken);
}
