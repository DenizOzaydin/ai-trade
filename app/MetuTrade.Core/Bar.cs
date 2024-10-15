namespace MetuTrade.Core;

public class Bar
{
    public long OpenTime { get; set; }

    public string? Symbol { get; set; }

    public string? Interval { get; set; }

    public double Open { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Close { get; set; }

    public double Volume { get; set; }
}