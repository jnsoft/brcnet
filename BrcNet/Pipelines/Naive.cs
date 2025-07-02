

namespace BrcNet.Pipelines;

public static class Naive
{
    const int BUFFER_SIZE_1MB = 1024 * 1024;
    public static Dictionary<string, Station> Process_FileStream(string filename, bool verbose = false)
    {
        var stations = new Dictionary<string, Station>();

        byte[] buffer = new byte[BUFFER_SIZE_1MB];
        int bytesRead;
        int lineStart = 0;
        int lineLength = 0;

        using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE_1MB))
        {
            int leftover = 0;
            while ((bytesRead = fs.Read(buffer, leftover, buffer.Length - leftover)) > 0)
            {
                int totalBytes = leftover + bytesRead;
                Span<byte> span = buffer.AsSpan(0, totalBytes);
                int lineStartIdx = 0;

                for (int i = 0; i < totalBytes; i++)
                {
                    if (span[i] == (byte)'\n')
                    {
                        int lineEndIdx = i;
                        // Handle CRLF
                        if (lineEndIdx > lineStartIdx && span[lineEndIdx - 1] == (byte)'\r')
                            lineEndIdx--;

                        var lineSpan = span.Slice(lineStartIdx, lineEndIdx - lineStartIdx);
                        if (!lineSpan.IsEmpty)
                        {
                            string line = Encoding.UTF8.GetString(lineSpan);
                            StationReading reading = Parser.ParseLine(line);
                            addReading(stations, reading);
                        }
                        lineStartIdx = i + 1; // Move to next line start
                    }
                }

                // Move leftover bytes to the beginning of the buffer
                leftover = totalBytes - lineStartIdx;
                if (leftover > 0)
                {
                    span.Slice(lineStartIdx, leftover).CopyTo(buffer);
                }

            }
            // Handle last line if file does not end with newline
            if (leftover > 0)
            {
                var lineSpan = buffer.AsSpan(0, leftover);
                if (!lineSpan.IsEmpty)
                {
                    string line = Encoding.UTF8.GetString(lineSpan);
                    StationReading reading = Parser.ParseLine(line);
                    addReading(stations, reading);
                }
            }
        }

        PrintResults(stations, verbose);
        return stations;
    }

    public static Dictionary<string, Station> Process_StreamReader(string filename, bool verbose = false)
    {
        var stations = new Dictionary<string, Station>();

        using (var reader = new StreamReader(filename, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024 * 1024))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                StationReading reading = Parser.ParseLine(line);
                addReading(stations, reading);
            }
        }
        PrintResults(stations, verbose);
        return stations;
    }

    public static Dictionary<string, Station> Process_ReadLines(string filename, bool verbose = false)
    {
        var stationSums = new Dictionary<string, Station>();

        foreach (var line in File.ReadLines(filename))
            addReading(stationSums, Parser.ParseLine(line));
            
        PrintResults(stationSums, verbose);
        return stationSums; 
    }

    public static void PrintResults(Dictionary<string, Station> stationSums, bool verbose = false)
    {
        if (!verbose)
        {
            Console.WriteLine($"Processed {stationSums.Count} stations.");
            return;
        }

        Console.WriteLine("StationId\tMin\tMax\tSum\tCount");
        Console.WriteLine("---------------------------------------");
    
        foreach (var sum in stationSums.Values.OrderBy(s => s.StationId))
        {
            Console.WriteLine(sum);
        }
    } 
    
    private static void addReading(Dictionary<string, Station> stations, StationReading reading)
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
            stations[reading.StationId] = aggregate(reading, station);
        }
    }

    private static Station aggregate(StationReading reading, Station sum)
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