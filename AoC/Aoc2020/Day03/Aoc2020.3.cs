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
        public static void Day3() {
            Console.WriteLine("Day3: ");
            string[] trees = File.ReadAllLines("Aoc2020\\Day03\\input.txt");
            // part 1
            int bang = CountBangs(trees, 3, 1);
            Console.WriteLine(bang);
            // part 2
            int bang1 = CountBangs(trees, 1, 1);
            int bang2 = CountBangs(trees, 3, 1);
            int bang3 = CountBangs(trees, 5, 1);
            int bang4 = CountBangs(trees, 7, 1);
            int bang5 = CountBangs(trees, 1, 2);
            Console.WriteLine(bang1 * bang2 * bang3 * bang4 * bang5);
        }

        private static int CountBangs(string[] trees, int dx, int dy) {
            int x = 0, y = 0, bang = 0;
            while (y < trees.Length) {
                x = (x + dx) % trees[0].Length;
                y += dy;
                if (y >= trees.Length)
                    break;
                if (trees[y][x] == '#')
                    bang++;
            }
            return bang;
        }
    }
}
