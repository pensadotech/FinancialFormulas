using FinancialFormulas.Models;

namespace FinancialFormulas.Business;

public class LTVFormula : FormulaCalculatorBase, IFormulaCalculator
{
    // Represents the input parameters for the LTV formula.
    // with two fields: LoanAmount and PropertyValue.
    public sealed record LTVFormulaInput(decimal LoanAmount, decimal PropertyValue);

    // Executes the LTV formula calculation based on the provided formula parameters.
    public double Execute(FormulaCalculationRequest formula)
    {
        // Parse the formula parameters into a
        // strongly typed LTVFormulaInput object
        var input = Parse(formula);

        if (input.PropertyValue == 0)
        {
            throw new ArgumentException("PropertyValue cannot be zero.");
        }

        // caclulate LTV as a percentage: (LoanAmount / PropertyValue) * 100
        return (double)(input.LoanAmount / input.PropertyValue) * 100.0;
    }

    // Parses the formula parameters into a strongly typed LTVFormulaInput object.
    private static LTVFormulaInput Parse(FormulaCalculationRequest formula)
    {
        var loanAmount = GetRequiredDouble(formula.Parameters, "LoanAmount");
        var propertyValue = GetRequiredDouble(formula.Parameters, "PropertyValue");

        return new LTVFormulaInput((decimal)loanAmount, (decimal)propertyValue);
    }

}
