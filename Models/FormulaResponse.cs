namespace FinancialFormulas.Models;

public class FormulaResponse
{
    public string CalculationId { get; set; } = string.Empty;

    public string FormulaName { get; set; } = string.Empty;

    public string FormulaVersion { get; set; } = string.Empty;

    public double? Result { get; set; }

    public FormulaErrorResponse? Error { get; set; }

}

public class FormulaErrorResponse
{
    public string Reason { get; set; } = string.Empty;
}