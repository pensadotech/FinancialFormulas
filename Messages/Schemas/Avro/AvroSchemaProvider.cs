using Avro;
using System.IO;

namespace FinancialFormulas.Messages.Schemas.Avro
{   
    // Implementation of IAvroSchemaProvider that loads Avro schemas from files in the specified directory.
    public class AvroSchemaProvider : IAvroSchemaProvider
    {
        public RecordSchema FormulaRequestSchema { get; }
        public RecordSchema FormulaResponseSchema { get; }

        public AvroSchemaProvider(IHostEnvironment hostEnvironment)
        {
            var schemaRoot = Path.Combine(hostEnvironment.ContentRootPath, "Messages/Schemas/Avro");

            FormulaRequestSchema = LoadRecordSchema(Path.Combine(schemaRoot, "FormulaRequest.avsc"));
            FormulaResponseSchema = LoadRecordSchema(Path.Combine(schemaRoot, "FormulaResponse.avsc"));
        }

        private static RecordSchema LoadRecordSchema(string path)
        { 
            var schemaJson = File.ReadAllText(path);

            return (RecordSchema)RecordSchema.Parse(schemaJson);
        }
    }
}