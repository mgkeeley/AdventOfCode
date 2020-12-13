using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2020
    {
        public static void Day10() {
            Console.WriteLine("Day10: ");
            var adapters = File.ReadAllLines("Aoc2020\\Day10\\input.txt").Select(int.Parse).ToList();

            // part 1
            adapters.Add(0);
            adapters.Sort();
            adapters.Add(adapters.Max() + 3);
            int diff1 = 0, diff3 = 0;
            for(int i = 1; i < adapters.Count; i++) {
                if (adapters[i] - adapters[i - 1] == 1)
                    diff1++;
                if (adapters[i] - adapters[i - 1] == 3)
                    diff3++;
            }
            Console.WriteLine($"max={adapters.Max()} d1={diff1}, d2={diff3}\n{diff1 * diff3}");

            // part 2
            Stopwatch sw = Stopwatch.StartNew();
            long[] memo = new long[adapters.Count];
            // run through list, see if can skip. memoize results along the way
            memo[0] = 1;
            long CountWays(int i) {
                if (memo[i] > 0)
                    return memo[i];
                long ways = CountWays(i - 1);
                if (i - 2 >= 0 && adapters[i] - adapters[i - 2] <= 3)
                    ways += CountWays(i - 2);
                if (i - 3 >= 0 && adapters[i] - adapters[i - 3] <= 3)
                    ways += CountWays(i - 3);
                memo[i] = ways;
                return ways;
            }
            long ways = CountWays(adapters.Count - 1);
            sw.Stop();
            Console.WriteLine($"{ways} ({sw.ElapsedTicks/(double)Stopwatch.Frequency}s)");
        }
    }
}
