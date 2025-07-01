using System;

const int NO_OF_LOCATIONS = 10;

if (args.Length != 2)
{
    Console.WriteLine("Usage: dotnet run <number_of_lines> <filename>");
    return;
}

if (!int.TryParse(args[0], out int numberOfLines) || numberOfLines < 1)
{
    Console.WriteLine("Please provide a valid positive integer for the number of lines.");
    return;
}

string filename = args[1];

if (string.IsNullOrWhiteSpace(filename))
{
    Console.WriteLine("Please provide a valid filename.");
    return;
}
else
{
    Directory.CreateDirectory(Path.GetDirectoryName(filename) ?? "");
}

    

var locations = getLocations(NO_OF_LOCATIONS);
var random = new Random();

try
{
    using (var writer = new StreamWriter(filename))
    {
        for (int i = 1; i <= numberOfLines; i++)
        {
            string location = locations[random.Next(0, NO_OF_LOCATIONS)];
            string temp = getTemp();
            string rowData = $"{location};{temp}";
            writer.Write(rowData);
            if (i < numberOfLines)
            {
                writer.WriteLine();
            }
        }
    }
    Console.WriteLine($"Successfully wrote {numberOfLines} lines to {filename}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error writing to file: {ex.Message}");
}

static List<string> getLocations(int size)
{
    var res = new List<string>(size);
    var random = new Random();
    for (int i = 0; i < size; i++)
    {
        int nameLength = random.Next(3, 15);
        res.Add(getRandomName(nameLength));
    }
    return res;
}

static string getRandomName(int length)
{
    if (length < 1) return string.Empty;
    const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string lower = "abcdefghijklmnopqrstuvwxyz";
    var random = new Random();
    char firstChar = upper[random.Next(upper.Length)];
    var chars = new char[length];
    chars[0] = firstChar;
    for (int i = 1; i < length; i++)
    {
        chars[i] = lower[random.Next(lower.Length)];
    }
    return new string(chars);
}

static string getTemp()
{
    var random = new Random();
    int t = random.Next(-999, 1000);
    double f = t / 10.0;
    return f.ToString("F1");
}

