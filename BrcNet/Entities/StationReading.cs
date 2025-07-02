namespace BrcNet.Entities;

public readonly record struct StationReading
{
    public StationReading()
    {
        StationId = string.Empty;
        Temperature = 0;
    }

    public string StationId { get; init; }
    public int Temperature { get; init; }
}

public readonly record struct ByteStationReading
{
    public ByteStationReading()
    {
        StationId = Memory<byte>.Empty;
        Temperature = 0;
    }

    public Memory<byte> StationId { get; init; }
    public int Temperature { get; init; }

    public readonly int HashCode
    {
        get
        {
            var hash = new HashCode();
            foreach (var b in StationId.Span)
                hash.Add(b);
            return hash.ToHashCode();
        }
    }

    public readonly int HashCode_simple
    {
        get
        {
            unchecked
            {
                int hash = 17;
                foreach (var b in StationId.Span)
                    hash = hash * 31 + b;
                return hash;
            }
        }
    }
}