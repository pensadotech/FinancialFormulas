using FinancialFormulas.Models;
using System.Globalization;
using System.Text.Json;

namespace FinancialFormulas.Business;

public abstract class FormulaCalculatorBase
{
    // Retrieves a required parameter from the list of parameters and converts it to a double.
    protected static double GetRequiredDouble(IEnumerable<FormulaParameterRequest> parameters, string parameterName)
    {
        var parameter = parameters.FirstOrDefault(p =>
            string.Equals(p.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase));

        if (parameter == null)
        {
            throw new ArgumentException($"Missing required parameter '{parameterName}'.");
        }

        return ConvertJsonElementToDouble(parameter.Value, parameterName);
    }

    // Converts a JsonElement to a double, throwing an exception if the conversion fails.
    // Supports:
    // - JsonValueKind.Number (e.g. 1500.00)
    // - numeric strings (e.g. "1500.00")
    // Uses invariant culture so decimal parsing is stable across different locales.
    protected static double ConvertJsonElementToDouble(JsonElement value, string parameterName)
    {
        // The value can be a number or a string that can be parsed to a number.
        // We need to handle both cases.
        if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out decimal decimalValue))
        {
            return (double)decimalValue;
        }

        if (value.ValueKind == JsonValueKind.String &&
            double.TryParse(value.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var stringNumberValue))
        {
            return stringNumberValue;
        }

        throw new ArgumentException($"Invalid value for parameter '{parameterName}'. Expected a number or a string that can be parsed to a number.");
    }
}
