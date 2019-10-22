using System;
using System.IO;
using System.Linq;
using System.Net;
using PCG.Library.Utilities;

namespace PCG_Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var today = DateTime.Now.ToString("yy-MM-dd hh.mm.ss");
            var grade = 3;

            var nodes = new[]
            {
                //new Tuple<int, int>(100, 1000),
                //new Tuple<int, int>(500, 1000),
                //new Tuple<int, int>(750, 1000),
                new Tuple<int, int>(1000, 100),
                new Tuple<int, int>(2500, 100),
                //new Tuple<int, int>(5000, 50),
                //new Tuple<int, int>(10000, 10),
                //new Tuple<int, int>(20000, 10),
                //new Tuple<int, int>(50000, 10),
                //new Tuple<int, int>(60000, 10)
            };


            File.WriteAllText($"grade-{grade}-{today}.csv", $@"""Nodes count"";""Iterations"";""Method 1 (Total time expected)"";""Method 1  (Avg ticks on 1 iter)"";""Method 1  (Avg ticks on 1 iter Matrix)"";""Method 1  (Avg ticks on 1 iter Calc)"";""Method 1  (Memory)"";""Method 2 (Total time expected)"";""Method 2  (Avg ticks on 1 iter)"";""Method 2  (Memory)""{Environment.NewLine}");

            foreach (var node in nodes)
            {
                var iters = BenchmarkUtilities.GetIterations(node.Item1, grade);
                var result2 = BenchmarkUtilities.Method2(node.Item1, grade, limit: node.Item2);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                var result1 = BenchmarkUtilities.Method1(node.Item1, grade, limit: node.Item2);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                File.AppendAllText($"grade-{grade}-{today}.csv", $@"""{node}"";""{iters}"";""{result1.Sum(x => x.Total) / result1.Count * iters}"";""{result1.Sum(x => x.Total) / result1.Count}"";""{result1.Sum(x => x.TimeSpans[0].Item2) / result1.Count}"";""{result1.Sum(x => x.TimeSpans[1].Item2) / result1.Count}"";""{result1.Sum(x => x.Memory) / result1.Count}"";""{result2.Sum(x => x.Total) / result2.Count * iters}"";""{result2.Sum(x => x.Total) / result2.Count}"";""{result2.Sum(x => x.Memory) / result2.Count}""{Environment.NewLine}");
            }
        }
    }
}