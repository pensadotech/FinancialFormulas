using Avro;
using Avro.Generic;
using FinancialFormulas.Models;

namespace FinancialFormulas.Messages.Producers;

// This class is responsible for mapping a FormulaBatchResponse object to a GenericRecord based
// on the provided Avro schema. It handles the mapping of nested records and union types,
// ensuring that the data is correctly represented in the Avro format.
public static class AvroFormulaRequestMapper
{
    // This method maps a FormulaBatchResponse object to a GenericRecord based on the provided Avro schema.
    public static GenericRecord Map(FormulaBatchResponse response, RecordSchema responseSchema)
    {
        // Create a new GenericRecord for the response using the provided schema.
        var responseRecord = new GenericRecord(responseSchema);

        // Map the statistics field from the response to the GenericRecord.
        var statisticsFieldSchema = (RecordSchema)GetFieldSchema(responseSchema, "statistics");
        var statisticsRecord = new GenericRecord(statisticsFieldSchema);
        statisticsRecord.Add("total", response.Statistics.Total);
        statisticsRecord.Add("succeeded", response.Statistics.Succeeded);
        statisticsRecord.Add("failed", response.Statistics.Failed);

        // Add the statistics record to the response record.
        responseRecord.Add("statistics", statisticsRecord);

        // Map the responses field from the response to the GenericRecord.
        var responsesArraySchema = (ArraySchema)GetFieldSchema(responseSchema, "responses");
        var responseItemSchema = (RecordSchema)responsesArraySchema.ItemSchema;
        var responseItems = new List<object>();

        // Iterate through each FormulaResponse in the response and map it to a GenericRecord.
        foreach (var item in response.Responses)
        {
            var itemRecord = new GenericRecord(responseItemSchema);
            itemRecord.Add("calculationId", item.CalculationId);
            itemRecord.Add("formulaName", item.FormulaName);
            itemRecord.Add("formulaVersion", item.FormulaVersion);
            itemRecord.Add("result", item.Result);

            // Map the error field, which is a union type, to the GenericRecord.
            // If the error is null, add null; otherwise, create a new record for the
            // error and add it to the item record.
            if (item.Error is null)
            {
                itemRecord.Add("error", null);
            }
            else
            {
                var errorUnionSchema = (UnionSchema)GetFieldSchema(responseItemSchema, "error");
                var errorRecordSchmema = (RecordSchema)errorUnionSchema
                                        .Schemas.First(s => s.Tag == Schema.Type.Record);
                var errorRecord = new GenericRecord(errorRecordSchmema);
                errorRecord.Add("reason", item.Error.Reason);
                itemRecord.Add("error", errorRecord);
            }

            // Add the mapped item record to the list of response items.
            responseItems.Add(item);
        }

        // Add the list of response items to the response record.
        responseRecord.Add("responses", responseItems);

        return responseRecord;
    }

    private static Schema GetFieldSchema(RecordSchema recordSchema, string fieldName)
    {
        return recordSchema.Fields.FirstOrDefault(field => field.Name == fieldName)?.Schema;
    }

}
