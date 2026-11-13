using FinancialFormulas.Models;

namespace FinancialFormulas.Services;

public interface IFinancialService
{
    IReadOnlyCollection<FormulaResponse> ExecuteFormulas(FormulaRequest request);
}

