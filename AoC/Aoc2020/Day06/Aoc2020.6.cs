using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC
{
    public partial class Aoc2020
    {
        public static void Day6() {
            Console.WriteLine("Day6: ");
            string[][] groups = File.ReadAllText("Aoc2020\\Day06\\input.txt").Split("\r\n\r\n").Select(g => g.Split("\r\n").ToArray()).ToArray();

            // part 1
            int count = 0;
            for (int g = 0; g < groups.Length; g++) {
                count += string.Join("", groups[g]).Distinct().Count();
            }
            Console.WriteLine(count);

            // part 2;
            count = 0;
            for (int g = 0; g < groups.Length; g++) {
                count += groups[g].Aggregate((a, b) => new string(a.Intersect(b).ToArray())).Count();
            }
            Console.WriteLine(count);

        }

    }
}
