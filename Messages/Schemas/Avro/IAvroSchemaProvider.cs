using Avro;

namespace FinancialFormulas.Messages.Schemas.Avro;

public interface IAvroSchemaProvider
{
    RecordSchema FormulaRequestSchema { get; }
    RecordSchema FormulaResponseSchema { get; }
}
