using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2019
    {
        public static void Day1() {
            Console.WriteLine("Day1: ");
            int[] data = File.ReadAllLines("Aoc2019\\Day01\\input.txt").Select(c => int.Parse(c)).ToArray();

            // part 1
            int fuel = data.Select(mass => mass / 3 - 2).Sum();
            Console.WriteLine(fuel);

            // part 2
            static int CalcFuel(int mass) {
                int f = Math.Max(mass / 3 - 2, 0);
                if (f > 0)
                    f += CalcFuel(f);
                return f;
            }
            fuel = data.Select(mass => CalcFuel(mass)).Sum();
            Console.WriteLine(fuel);
        }
    }
}
