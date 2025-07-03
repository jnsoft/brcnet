using System.Xml;

namespace BrcNet;

public static class Parser
{
    public const byte ASCII_NEWLINE = 10; // '\n';
    public const byte ASCII_CARRIAGE_RETURN = 13; // '\r'
    public const byte ASCII_SEMICOLON = 59; // ';'
    public const byte ASCII_MINUS = 45; // '-'
    public const byte ASCII_ZERO = 48; // '0'
    public const byte ASCII_DOT = 46; // '.'

    public static StationReading ParseLine2(string line)
    {
        var parts = line.Split(';');
        double temp = double.Parse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture);

        return new StationReading
        {
            StationId = parts[0],
            Temperature = (int)(temp * 10)
        };
    }

    public static StationReading ParseLine(string line)
    {
        var parts = line.Split(';');
        if (parts.Length == 2 &&
            double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double temp))
        {
            return new StationReading
            {
                StationId = parts[0],
                Temperature = (int)Math.Round(temp * 10)
            };
        }
        else
        {
            throw new FormatException($"Invalid line format: {line}");
        }
    }

    public static ByteStationReading ParseLine(ReadOnlySpan<byte> line)
    {
        int splitIx = 0;
        bool negative = false;

        while (splitIx < line.Length && line[splitIx] != ASCII_SEMICOLON)
        {
            splitIx++;
        }

        ReadOnlySpan<byte> stationIdBytes = line[..splitIx];

        if (line[splitIx + 1] == ASCII_MINUS)
        {
            negative = true;
            splitIx++;
        }

        int fac = 1;
        int n = 0;
        for (int i = line.Length - 1; i > splitIx; i--)
        {
            if (line[i] != ASCII_DOT)
            {
                n += fac * (line[i]- ASCII_ZERO);
                fac *= 10;
            }
        }

        if (negative)
        {
            n = -n;
        }

        return new ByteStationReading
        {
            StationId = stationIdBytes.ToArray(),
            Temperature = n
        };
    }
}
