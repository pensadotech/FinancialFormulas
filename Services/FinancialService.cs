using FinancialFormulas.Business;
using FinancialFormulas.Models;
using Microsoft.Net.Http.Headers;

namespace FinancialFormulas.Services;

public class FinancialService : IFinancialService
{
    private readonly PTIFormula _ptiFormula = new();
    private readonly LTVFormula _ltvFormula = new();

    public IReadOnlyCollection<FormulaResponse> ExecuteFormulas(FormulaRequest request)
    {
        var responses = new List<FormulaResponse>();

        foreach (var formula in request.Formulas)
        {
            try
            {
                // Condensed form of the switch statement using pattern
                // matching and expression-bodied members
                //double result = formula.FormulaName switch
                //{
                //    "PTI" => _ptiFormula.Execute(formula),
                //    "LTV" => _ltvFormula.Execute(formula),
                //    _ => throw new ArgumentException($"Unsupported formula '{formula.FormulaName}'."),
                //};

                // Traditional switch statement for clarity and maintainability
                var formulaResponse = new FormulaResponse();

                if(string.IsNullOrWhiteSpace(formula.FormulaName))
                {
                    throw new ArgumentException("FormulaName cannot be null or empty.");
                }

                var normalizedFormulaName = formula.FormulaName.Trim().ToUpperInvariant();

                double result;

                switch(normalizedFormulaName)
                {
                    case "PTI":
                        result = _ptiFormula.Execute(formula);
                        break;
                    case "LTV":
                        result = _ltvFormula.Execute(formula);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported formula '{formula.FormulaName}'.");
                }

                // Add the successful response to the list of responses
                responses.Add(new FormulaResponse
                {
                    CalculationId = formula.CalculationId,
                    FormulaName = formula.FormulaName,
                    FormulaVersion = formula.FormulaVersion,
                    Result = result,
                    Error = null
                });
            }
            catch (ArgumentException ex)
            {
                // Add the error response to the list of responses for argument exceptions
                responses.Add(new FormulaResponse
                {
                    CalculationId = formula.CalculationId,
                    FormulaName = formula.FormulaName,
                    FormulaVersion = formula.FormulaVersion,
                    Result = null,
                    Error = new FormulaErrorResponse { Reason = ex.Message }
                });
            }
            catch (Exception ex)
            {
                // Add the error response to the list of responses for general exceptions
                responses.Add(new FormulaResponse
                {
                    CalculationId = formula.CalculationId,
                    FormulaName = formula.FormulaName,
                    FormulaVersion = formula.FormulaVersion,
                    Result = null,
                    Error = new FormulaErrorResponse { Reason = ex.Message }
                });
            }
        }
        return responses;
    }
}
