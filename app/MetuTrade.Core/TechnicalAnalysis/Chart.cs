using MetuTrade.Core.Entities;

namespace MetuTrade.Core.TechnicalAnalysis;

public class Chart
{
    public string? Symbol { get; set; }
    public string? Interval { get; set; }

    public List<Bar> Bars { get; set; }

    public Chart()
    {
        Bars = new List<Bar>();
    }

    public Chart SubChart(int barCount)
    {
        Chart toReturn = new Chart();
        toReturn.Symbol = Symbol;
        toReturn.Interval = Interval;

        foreach (var item in Bars.TakeLast(barCount))
        {
            toReturn.Bars.Add(item);
        }

        return toReturn;
    }

    public List<double> GetOpenValues()
    {
        return Bars.Select(b => b.Open).ToList();
    }

    public List<double> GetHighValues()
    {
        return Bars.Select(b => b.High).ToList();
    }

    public List<double> GetLowValues()
    {
        return Bars.Select(b => b.Low).ToList();
    }

    public List<double> GetCloseValues()
    {
        return Bars.Select(b => b.Close).ToList();
    }

    public List<double> GetVolumes()
    {
        return Bars.Select(b => b.Volume).ToList();
    }

    public List<long> GetOpenTimesAsLong()
    {
        return Bars.Select(b => b.OpenTime).ToList();
    }

    public List<DateTime> GetOpenTimesAsDateTime()
    {
        return Bars.Select(b => DateTimeOffset.FromUnixTimeMilliseconds(b.OpenTime).DateTime).ToList();
    }
}