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

        int processorCount = Environment.ProcessorCount;


        Stopwatch sw = Stopwatch.StartNew();
        if (true)
        { 
        var res1 = Naive.Process_FileStream(filename);
        sw.Stop();
        Console.WriteLine($"Process_FileStream completed in {sw.ElapsedMilliseconds} ms");
        Console.WriteLine(res1);

        sw.Restart();
        var res2 = Naive.Process_StreamReader(filename);
        sw.Stop();
        Console.WriteLine($"Process_StreamReader completed in {sw.ElapsedMilliseconds} ms");
        Console.WriteLine(res2);

        sw.Restart();
        var res3 = Naive.Process_ReadLines(filename);
        sw.Stop();
        Console.WriteLine($"Process_ReadLines completed in {sw.ElapsedMilliseconds} ms");
        Console.WriteLine(res3);
    }

        sw.Restart();
        var res4 = Simple.Process(filename);
        //res4.PrintResults();
        sw.Stop();
        Console.WriteLine($"Simple completed in {sw.ElapsedMilliseconds} ms");
        Console.WriteLine(res4);

        sw.Restart();
        var res5 = MemoryMapped.Process(filename, processorCount);
        //res5.PrintResults();
        sw.Stop();
        Console.WriteLine($"MemoryMapped completed in {sw.ElapsedMilliseconds} ms");
        Console.WriteLine(res5);

        sw.Restart();
        var res6 = SimpleLinq.AggregateMeasurements(filename);
        //res6.PrintResults();
        sw.Stop();
        Console.WriteLine($"SimpleLinq completed in {sw.ElapsedMilliseconds} ms");
        Console.WriteLine(res6.Count);
        //foreach (var kvp in res6.OrderBy(m => m.Key))
        //    Console.WriteLine($"{kvp.Key}={kvp.Value}");

  

        if (false)
        {
            Console.WriteLine("Results:");
            //res1.PrintResults();
            //res2.PrintResults();
            //res3.PrintResults();
            res4.PrintResults();
        }
    }

}
