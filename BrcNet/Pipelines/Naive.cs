

namespace BrcNet.Pipelines;

public static class Naive
{
    const int BUFFER_SIZE_1MB = 1024 * 1024;

    public static ByteResult Process_FileStream(string filename, bool verbose = false)
    {
        ByteResult result = new();

        byte[] buffer = new byte[BUFFER_SIZE_1MB];
        int bytesRead;

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
                    if (span[i] == Parser.ASCII_NEWLINE)
                    {
                        int lineEndIdx = i;
                        // Handle CRLF
                        if (lineEndIdx > lineStartIdx && span[lineEndIdx - 1] == Parser.ASCII_CARRIAGE_RETURN)
                            lineEndIdx--;

                        var lineSpan = span.Slice(lineStartIdx, lineEndIdx - lineStartIdx);
                        if (!lineSpan.IsEmpty)
                        {
                            ByteStationReading reading = Parser.ParseLine(lineSpan);
                            result.Add(reading);
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
                    ByteStationReading reading = Parser.ParseLine(lineSpan);
                        result.Add(reading);
                }
            }
        }
        return result;
    }

    public static Result Process_StreamReader(string filename, bool verbose = false)
    {
        Result result = new();
        using (var reader = new StreamReader(filename, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024 * 1024))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                StationReading reading = Parser.ParseLine(line);
                result.Add(reading);
            }
        }
        return result;
    }

    public static Result Process_ReadLines(string filename, bool verbose = false)
    {
        Result result = new();

        foreach (var line in File.ReadLines(filename))
            result.Add(Parser.ParseLine(line));

        return result;
    }
    
}