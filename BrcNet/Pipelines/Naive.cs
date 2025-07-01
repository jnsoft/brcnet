namespace BrcNet.Pipelines;

public static class Naive
{
    public static Dictionary<string, StationSum> Process(string filename)
    {
        var stationSums = new Dictionary<string, StationSum>();

        foreach (var line in File.ReadLines(filename))
        {
            StationReading reading = Parser.ParseLine(line);
            if (!stationSums.TryGetValue(reading.StationId, out var sum))
            {
                stationSums[reading.StationId] = new StationSum
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
                stationSums[reading.StationId] = Aggregate(reading, sum);
            }
        }
        PrintResults(stationSums);
        return stationSums; 
    }

    public static void PrintResults(Dictionary<string, StationSum> stationSums)
    {
        foreach (var sum in stationSums.Values.OrderBy(s => s.StationId))
        {
            Console.WriteLine(sum);
        }
    } 
    
    public static StationSum Aggregate(StationReading reading, StationSum sum)
    {
        sum.Sum += reading.Temperature;
        if (sum.Min > reading.Temperature)
        {
            sum.Min = reading.Temperature;
        }
        if (sum.Max < reading.Temperature)
        {
            sum.Max = reading.Temperature;
        }
        sum.Count++;

        return sum;
    }
}