namespace BrcNet.Workers;

public static class BufferParser
{
    public static void Parse(byte[] parseBuffer, ByteResult result)
    {
        Span<byte> span = parseBuffer;
        int lineStartIdx = 0;
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == Parser.ASCII_NEWLINE)
            {
                int lineEndIdx = i;
                if (lineEndIdx > lineStartIdx && span[lineEndIdx - 1] == Parser.ASCII_CARRIAGE_RETURN)
                    lineEndIdx--;

                var lineSpan = span.Slice(lineStartIdx, lineEndIdx - lineStartIdx);
                if (!lineSpan.IsEmpty)
                {
                    var reading = Parser.ParseLine(lineSpan);
                    result.Add(reading);
                }
                lineStartIdx = i + 1;
            }
        }
    }
}
