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
}