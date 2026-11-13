using System.Text.Json;

namespace FinancialFormulas.Models;

// The FormulaRequest class represents a request to perform calculations based
// on specified formulas. It contains a list of FormulaCalculationRequest objects,
// each representing a specific formula calculation request with its associated parameters.
// The FormulaParameterRequest class allows for flexible parameter values using JsonElement,
// enabling the passing of various data types.
public class FormulaRequest
{
    public List<FormulaCalculationRequest> Formulas { get; set; } = [];
}

// The FormulaCalculationRequest class represents a single formula calculation request,
// including the calculation ID, formula name, version, and a list of parameters.
// Each parameter is represented by the FormulaParameterRequest class, which allows for
// flexible value types using JsonElement.
public class FormulaCalculationRequest
{
    public string CalculationId { get; set; } = string.Empty;

    public string FormulaName { get; set; } = string.Empty;

    public string FormulaVersion { get; set; } = string.Empty;

    public List<FormulaParameterRequest> Parameters { get; set;} = new List<FormulaParameterRequest>();

}

// The FormulaParameterRequest class represents a single parameter for a formula calculation request.
// It includes the parameter name and its value, which is represented as a JsonElement to allow
// for any type of value to be passed in, including numbers, strings, arrays, and objects.
public class FormulaParameterRequest
{
    public string ParameterName { get; set; } = string.Empty;

    // Important: The value is a JsonElement to allow for any type of value to be passed in,
    // inc;luding numbers, strings, arrays, and objects
    public JsonElement Value { get; set;}
}