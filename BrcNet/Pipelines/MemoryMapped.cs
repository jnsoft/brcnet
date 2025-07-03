using System.IO.MemoryMappedFiles;

namespace BrcNet.Pipelines;

public static class MemoryMapped
{

    public static ByteResult Process(string filename, int parallell, bool verbose = false)
    {
        ByteResult result = new();
        var tasks = new List<Task>();

        using (var mmf = MemoryMappedFile.CreateFromFile(filename, FileMode.Open, null, 0, MemoryMappedFileAccess.Read))
        {
            using (var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read))
            {
                long length = accessor.Capacity;
                
                long chunkSize = length / parallell;
                byte[] buffer = new byte[length];
                accessor.ReadArray(0, buffer, 0, (int)length);

                // Find chunk boundaries at newlines
                long[] chunkStarts = new long[parallell + 1];
                chunkStarts[0] = 0;
                chunkStarts[parallell] = length;

                // Set chunkStarts[i] to the next newline after the ideal split
                for (int i = 1; i < parallell; i++)
                {
                    long pos = i * chunkSize;
                    while (pos < length && buffer[pos] != Parser.ASCII_NEWLINE)
                        pos++;
                    chunkStarts[i] = pos + 1 < length ? pos + 1 : length;
                }

                for (int i = 0; i < parallell; i++)
                {
                    long start = chunkStarts[i];
                    long end = chunkStarts[i + 1];
                    if (start >= end) continue;

                    tasks.Add(Task.Run(() =>
                    {
                        var span = buffer.AsSpan((int)start, (int)(end - start));
                        int lineStartIdx = 0;
                        for (int j = 0; j < span.Length; j++)
                        {
                            if (span[j] == Parser.ASCII_NEWLINE)
                            {
                                int lineEndIdx = j;
                                if (lineEndIdx > lineStartIdx && span[lineEndIdx - 1] == Parser.ASCII_CARRIAGE_RETURN)
                                    lineEndIdx--;

                                var lineSpan = span[lineStartIdx..lineEndIdx];
                                if (!lineSpan.IsEmpty)
                                {
                                    var reading = Parser.ParseLine(lineSpan);
                                    result.Add(reading);
                                }
                                lineStartIdx = j + 1;
                            }
                        }
                        // Handle last line in chunk if not newline-terminated
                        if (lineStartIdx < span.Length)
                        {
                            var lineSpan = span[lineStartIdx..];
                            if (!lineSpan.IsEmpty)
                            {
                                var reading = Parser.ParseLine(lineSpan);
                                result.Add(reading);
                            }
                        }
                    }));
                }
            }
        }

        Task.WaitAll(tasks.ToArray());
        return result;
    }
}
