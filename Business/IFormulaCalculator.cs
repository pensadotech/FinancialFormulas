
using FinancialFormulas.Models;

namespace FinancialFormulas.Business;

public interface IFormulaCalculator
{
    double Execute(FormulaCalculationRequest formula);
}
