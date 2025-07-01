

namespace BrcNet.Pipelines;

public static class Naive
{
    public static Dictionary<string, StationSum> Process_FileStream(string filename)
    {
        var stationSums = new Dictionary<string, StationSum>();
        const int bufferSize = 1024 * 1024; // 1MB buffer
        byte[] buffer = new byte[bufferSize];
        int bytesRead;
        List<byte> lineBuffer = new List<byte>(1024);

        using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize))
        {
            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                byte b = buffer[i];
                if (b == (byte)'\n')
                {
                    // Handle CRLF
                    if (lineBuffer.Count > 0 && lineBuffer[lineBuffer.Count - 1] == (byte)'\r')
                        lineBuffer.RemoveAt(lineBuffer.Count - 1);

                    string line = Encoding.UTF8.GetString(lineBuffer.ToArray());
                    if (!string.IsNullOrEmpty(line))
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
                    lineBuffer.Clear();
                }
                else
                {
                    lineBuffer.Add(b);
                }
            }
        }
        // Handle last line if file does not end with newline
        if (lineBuffer.Count > 0)
        {
            string line = Encoding.UTF8.GetString(lineBuffer.ToArray());
            if (!string.IsNullOrEmpty(line))
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
        }
    }
        PrintResults(stationSums);
        return stationSums; 
    }

    public static Dictionary<string, StationSum> Process_StreamReader(string filename)
    {
        var stationSums = new Dictionary<string, StationSum>();

        using (var reader = new StreamReader(filename))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
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
        }
        PrintResults(stationSums);
        return stationSums;
    }

    public static Dictionary<string, StationSum> Process_ReadLines(string filename)
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