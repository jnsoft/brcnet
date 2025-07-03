namespace BrcNet.Entities;

public class Result
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

public class ByteResult
{
    readonly ConcurrentDictionary<int, ByteStation> stations = [];
    public void Add(ByteStationReading reading)
    {
        if (!stations.TryGetValue(reading.HashCode_simple, out var station))
        {
            stations[reading.HashCode_simple] = new ByteStation
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

        foreach (var sum in stations.Values.OrderBy(s => s.StationName))
        {
            Console.WriteLine(sum);
        }
    } 
}