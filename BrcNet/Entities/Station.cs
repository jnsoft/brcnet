namespace BrcNet.Entities;

public record struct Station
{
    public Station()
    {
        StationId = string.Empty;
        Sum = 0;
        Count = 0;
    }

    public string StationId { get; init; }
    public long Sum { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
    public int Count { get; set; }

    public readonly double AverageTemperature => Count > 0 ? (double)Sum / (Count * 10) : 0.0;

    public override readonly string ToString()
    {
        return $"{StationId}={(double)Min / 10}/{AverageTemperature:F1}/{(double)Max / 10}";
    }
}

