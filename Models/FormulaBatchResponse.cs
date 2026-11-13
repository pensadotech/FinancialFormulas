namespace FinancialFormulas.Models;

public class FormulaBatchResponse
{
    public FormulaBatchStatistics Statistics { get; set;} = new();

    public IReadOnlyCollection<FormulaResponse> Responses { get; set; } = [];

}

public class  FormulaBatchStatistics
{
    public int Total { get; set; }

    public int Succeeded {  get; set; }

    public int Failed { get; set; }
}
