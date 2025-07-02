namespace BrcNet.Entities;

public record class Result
{
    readonly Dictionary<string, Station> stations = [];

    public void Add(StationReading reading)
    {
        if (!stations.TryGetValue(reading.StationId, out var station))
        {
            stations[reading.StationId] = new Station
            {
                StationId = reading.StationId,
                Min = reading.Temperature,
                Max = reading.Temperature,
                Sum = reading.Temperature,
                Count = 1
            };
        }
        else
        {
            station.Sum += reading.Temperature;
            if (station.Min > reading.Temperature)
            {
                station.Min = reading.Temperature;
            }
            if (station.Max < reading.Temperature)
            {
                station.Max = reading.Temperature;
            }
            station.Count++;
        }
    }

    public override string ToString()
    {
        return $"Processed {stations.Count} stations.";
    }

    public void PrintResults()
    {
        Console.WriteLine("StationId\tMin\tMax\tSum\tCount");
        Console.WriteLine("---------------------------------------");

        foreach (var sum in stations.Values.OrderBy(s => s.StationId))
        {
            Console.WriteLine(sum);
        }
    } 
}