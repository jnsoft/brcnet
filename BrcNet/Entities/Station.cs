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

public struct ByteStation
{
    public ByteStation()
    {
        StationId = Memory<byte>.Empty;
        Sum = 0;
        Count = 0;
        _stationName = null;
    }

    public Memory<byte> StationId { get; init; }
    public long Sum { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
    public int Count { get; set; }

    [NonSerialized]
    private string? _stationName;

    public string StationName
    {
        get
        {
            _stationName ??= StationId.Length == 0
                    ? string.Empty
                    : Encoding.UTF8.GetString(StationId.Span);
            return _stationName;
        }
    }

    public readonly double AverageTemperature => Count > 0 ? (double)Sum / (Count * 10) : 0.0;

    public override string ToString()
    {
        return $"{StationName}={(double)Min / 10}/{AverageTemperature:F1}/{(double)Max / 10}";
    }
}

