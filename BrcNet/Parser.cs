using System.Xml;

namespace BrcNet;

public static class Parser
{
    public const byte ASCII_NEWLINE = 10; // '\n';
    public const byte ASCII_CARRIAGE_RETURN = 13; // '\r'

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

    public static StationReading ParseLine(ReadOnlySpan<byte> line)
    {
        
        string stationId = Encoding.UTF8.GetString(span.Slice(lineStartIdx, semicolonIndex));
        string tempStr = Encoding.UTF8.GetString(span.Slice(lineStartIdx + semicolonIndex + 1, lineEndIdx - lineStartIdx - semicolonIndex - 1));

        if (double.TryParse(tempStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double temp))
        {
            return new StationReading
            {
                StationId = stationId,
                Temperature = (int)Math.Round(temp * 10)
            };
        }
        else
        {
            throw new FormatException($"Invalid temperature value: {tempStr}");
        }
    }
}
