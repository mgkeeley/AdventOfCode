using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2020 {
        public static void Day13() {
            Console.WriteLine("Day13: ");
            var input = File.ReadAllLines("Aoc2020\\Day13\\test.txt");

            int minTime = int.Parse(input[0]);
            int[] busses = input[1].Split(',').Select(x => x == "x" ? 0 : int.Parse(x)).ToArray();

            int bestT = int.MaxValue;
            int bestBus = 0;
            foreach (int b in busses.Where(b => b != 0)) {
                int t = (minTime + b - 1) / b * b;
                if (t < bestT) {
                    bestT = t;
                    bestBus = b;
                }
            }
            Console.WriteLine((bestT - minTime) * bestBus);

            // part 2 
            Stopwatch sw = Stopwatch.StartNew();
            long cycle = busses[0];
            long timestamp = 0;
            for (int b = 1; b < busses.Length; b++) {
                int t2cyc = busses[b];
                if (t2cyc == 0)
                    continue;
                // init this bus's cycle with current cycle position less offset
                long t2 = timestamp / t2cyc * t2cyc - b;
                // increase timestamps until they sync
                while (timestamp != t2) {
                    if (timestamp < t2)
                        timestamp += cycle;
                    else
                        t2 += (timestamp - t2 + t2cyc - 1) / t2cyc * t2cyc;
                }
                // bus numbers are all prime (how convenient!), so we can just multiply the cycle by the bus number to get the next cycle length
                cycle *= busses[b];
                // Console.WriteLine($"Cycle: {cycle}");
            }
            sw.Stop();
            Console.WriteLine($"{timestamp} in {sw.ElapsedTicks * 1000d / Stopwatch.Frequency}ms");
        }
    }
}
