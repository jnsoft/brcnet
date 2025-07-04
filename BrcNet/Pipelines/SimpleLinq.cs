namespace BrcNet.Pipelines;

public static class SimpleLinq
{
    public static SortedDictionary<string, ResultRow> AggregateMeasurements(string filePath)
    {
        return File.ReadLines(filePath)
            .Select(line => new Measurement(line.Split(';')))
            .GroupBy(m => m.Station)
            .Aggregate(
                new SortedDictionary<string, ResultRow>(),
                (dict, group) =>
                {
                    var agg = group.Aggregate(
                        new MeasurementAggregator(),
                        (a, m) =>
                        {
                            a.Min = Math.Min(a.Min, m.Value);
                            a.Max = Math.Max(a.Max, m.Value);
                            a.Sum += m.Value;
                            a.Count++;
                            return a;
                        }
                    );

                    double avg = Math.Round(agg.Sum * 10.0) / 10.0 / agg.Count;

                    dict[group.Key] = new ResultRow(agg.Min, avg, agg.Max);
                    return dict;
                }
            );
    }

    public class Measurement(string[] parts)
    {
        public string Station { get; } = parts[0];
        public double Value { get; } = double.Parse(parts[1], CultureInfo.InvariantCulture);
    }

    public class MeasurementAggregator
    {
        public double Min { get; set; } = double.MaxValue;
        public double Max { get; set; } = double.MinValue;
        public double Sum { get; set; } = 0;
        public int Count { get; set; } = 0;
    }

    public class ResultRow(double min, double avg, double max)
    {
        public double Min { get; } = min;
        public double Avg { get; } = avg;
        public double Max { get; } = max;

        public override string ToString()
            => $"Min: {Min:F1}, Avg: {Avg:F1}, Max: {Max:F1}";
    }
}
