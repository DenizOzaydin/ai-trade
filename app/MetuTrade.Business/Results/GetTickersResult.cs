namespace MetuTrade.Business.Results;

public class GetTickersResult : ResultBase
{
    public List<SymbolResult> Symbols { get; set; }

    public GetTickersResult()
    {
        Symbols = new List<SymbolResult>();
    }
}

public class SymbolResult
{
    public string Symbol { get; set; }
    public string Currency { get; set; }
    public string BaseCurrency { get; set; }
}