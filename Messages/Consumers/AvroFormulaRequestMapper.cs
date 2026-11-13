using Avro.Generic;
using FinancialFormulas.Models;
using System.Text.Json;

namespace FinancialFormulas.Messages.Consumers;

internal static class AvroFormulaRequestMapper
{
    // Maps a GenericRecord representing a FormulaRequest to a FormulaRequest model object.
    public static FormulaRequest Map(GenericRecord message)
    {
        var formulas = new List<FormulaCalculationRequest>();

        // Iterate through the "Formulas" field of the message, which is expected to be
        // an enumerable of GenericRecords.
        foreach (var formulaObject in GetEnumerableField(message, "Formulas"))
        {
            var formulaRecord = formulaObject as GenericRecord;
            var parameters = new List<FormulaParameterRequest>();

            // Iterate through the parameters field of the formula record, which is expected
            // to be an enumerable of GenericRecords.
            foreach (var parameterObject in GetEnumerableField(formulaRecord, "parameters"))
            {
                var parameterRecord = parameterObject as GenericRecord;
                var parameterName = Convert.ToString(parameterRecord["parameterName"]) ?? string.Empty;
                var valueObject = parameterRecord["value"];

                parameters.Add(new FormulaParameterRequest
                {
                    ParameterName = parameterName,
                    // Serialize the value to a JsonElement for flexible type handling
                    Value = JsonSerializer.SerializeToElement(valueObject) 
                });
            }

            // helper method to safly retreive an enumerable field from a GeericRecord
            formulas.Add( new FormulaCalculationRequest
            {
                CalculationId = formulaRecord["calculationId"]?.ToString() ?? string.Empty,
                FormulaName = formulaRecord["formulaName"]?.ToString() ?? string.Empty,
                FormulaVersion = formulaRecord["formulaVersion"]?.ToString() ?? string.Empty,
                Parameters = parameters
            });
        }

        return new FormulaRequest
        {
            Formulas = formulas
        };
    }

    // Helper method to get an enumerable field from a GenericRecord, returning an empty
    // list if the field is not found or is not an enumerable
    private static IEnumerable<object> GetEnumerableField(GenericRecord record, string fieldName)
    {
        if (record[fieldName] is IEnumerable<object> enumerable)
        {
            return enumerable;
        }
        return [];
    }

}
