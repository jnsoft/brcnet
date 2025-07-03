namespace BrcNet.Pipelines;

public static class Simple
{
    const int BUFFER_SIZE = 1024 * 1024;

    public static ByteResult Process(string filename, bool verbose = false)
    {
        ByteResult result = new();
        var tasks = new List<Task>();

        byte[] buffer = new byte[BUFFER_SIZE];
        int bytesRead;
        byte[] leftover = Array.Empty<byte>();

        using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE))
        {
            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                // Combine leftover from previous read with current buffer
                int totalLength = leftover.Length + bytesRead;
                byte[] combined = new byte[totalLength];
                if (leftover.Length > 0)
                    Buffer.BlockCopy(leftover, 0, combined, 0, leftover.Length);
                Buffer.BlockCopy(buffer, 0, combined, leftover.Length, bytesRead);

                // Find last newline in combined buffer
                int lastNewline = Array.LastIndexOf(combined, Parser.ASCII_NEWLINE);
                if (lastNewline == -1)
                {
                    // No newline found, all is leftover
                    leftover = combined;
                    continue;
                }

                // Prepare leftover for next round
                int nextLeftoverLen = totalLength - (lastNewline + 1);
                leftover = nextLeftoverLen > 0 ? combined[(lastNewline + 1)..] : Array.Empty<byte>();

                // Pass the complete lines to a task for parsing
                var parseBuffer = combined.AsSpan(0, lastNewline + 1).ToArray();
                tasks.Add(Task.Run(() => Workers.BufferParser.Parse(parseBuffer, result)));
            }
        }

        // Handle any leftover bytes as the last line
        if (leftover.Length > 0)
        {
            var lineSpan = leftover.AsSpan();
            if (!lineSpan.IsEmpty)
            {
                var reading = Parser.ParseLine(lineSpan);
                result.Add(reading);
            }
        }

        Task.WaitAll(tasks.ToArray());
        
        return result;
    }
}