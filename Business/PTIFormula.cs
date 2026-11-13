using FinancialFormulas.Models;

namespace FinancialFormulas.Business;

public class PTIFormula : FormulaCalculatorBase, IFormulaCalculator
{
    // Represents the input parameters for the PTI formula.
    // with two fields: MonthlyPayment and GrossMonthlyIncome.
    public sealed record PTIFormulaInput(decimal MonthlyPayment, decimal GrossMonthlyIncome);

    // Executes the PTI formula calculation based on the provided formula parameters.
    public double Execute(FormulaCalculationRequest formula)
    {
        // Parse the formula parameters into a
        // strongly typed PTIFormulaInput object
        var input = Parse(formula);

        if (input.GrossMonthlyIncome == 0)
        {
            throw new ArgumentException("GrossMonthlyIncome cannot be zero.");
        }

        // Calculate PTI as a percentage: (MonthlyPayment / GrossMonthlyIncome) * 100
        return (double)(input.MonthlyPayment / input.GrossMonthlyIncome) * 100.0;
    }

    // Parses the formula parameters into a strongly typed PTIFormulaInput object.
    private static PTIFormulaInput Parse(FormulaCalculationRequest formula)
    {
        var monthlyPayment = GetRequiredDouble(formula.Parameters, "MonthlyPayment");
        var grossMonthlyIncome = GetRequiredDouble(formula.Parameters, "GrossMonthlyIncome");

        return new PTIFormulaInput((decimal)monthlyPayment, (decimal)grossMonthlyIncome);
    }
    
}
