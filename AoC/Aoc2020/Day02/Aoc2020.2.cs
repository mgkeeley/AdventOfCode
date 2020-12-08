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
        public static void Day2() {
            Console.WriteLine("Day2: ");
            string[] passes = File.ReadAllLines("Aoc2020\\Day02\\input.txt");

            // part 1
            int good = 0;
            Regex parser = new Regex("^([0-9]+)-([0-9]+) ([a-z]): ([a-z]+)$");  // 3-19 f: pxmznsfdhzjqrdfjqrd
            foreach (var pass in passes) {
                var m = parser.Match(pass);
                if (!m.Success)
                    throw new Exception(pass);
                int min = int.Parse(m.Groups[1].Value);
                int max = int.Parse(m.Groups[2].Value);
                char c = m.Groups[3].Value[0];
                string p = m.Groups[4].Value;
                int count = p.Count(pc => pc == c);
                if (count >= min && count <= max)
                    good++;
            }
            Console.WriteLine(good);

            // part 2
            good = 0;
            foreach (var pass in passes) {
                var m = parser.Match(pass);
                if (!m.Success)
                    throw new Exception(pass);
                int min = int.Parse(m.Groups[1].Value);
                int max = int.Parse(m.Groups[2].Value);
                char c = m.Groups[3].Value[0];
                string p = m.Groups[4].Value;
                if ((p[min-1] == c) != (p[max-1] == c))
                    good++;
            }
            Console.WriteLine(good);
        }
    }
}
