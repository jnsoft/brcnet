using System;
using System.Diagnostics;
using System.IO;
using BrcNet.Pipelines;

namespace BrcNet;

class Program
{
    static void Main(string[] args)
    {
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

        Stopwatch sw = Stopwatch.StartNew();
        Naive.Process_FileStream(filename);
        sw.Stop();
        Console.WriteLine($"Process_FileStream completed in {sw.ElapsedMilliseconds} ms");
        sw.Restart();
        Naive.Process_StreamReader(filename);
        sw.Stop();
        Console.WriteLine($"Process_StreamReader completed in {sw.ElapsedMilliseconds} ms");
        sw.Restart();
        Naive.Process_ReadLines(filename);
        sw.Stop();
        Console.WriteLine($"Process_ReadLines completed in {sw.ElapsedMilliseconds} ms");
    }

}
