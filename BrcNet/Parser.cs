namespace BrcNet;

public static class Parser
{
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
}
