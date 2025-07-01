if (args.Length != 1)
{
    Console.WriteLine("Usage: dotnet run <filename>");
    return;
}

string filename = args[0];

if (!File.Exists(filename))
{
    Console.WriteLine($"File not found: {filename}");
    return;
}

foreach (var line in File.ReadLines(filename))
{
    Console.WriteLine(line);
}
