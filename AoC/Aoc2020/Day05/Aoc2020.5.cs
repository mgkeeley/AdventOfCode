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
        public static void Day5() {
            Console.WriteLine("Day5: ");
            string[] passes = File.ReadAllLines("Aoc2020\\Day05\\input.txt");

            // part 1
            int max_seat = 0;
            int min_seat = int.MaxValue;
            int sum_seat = 0;
            foreach (string pass in passes) {
                int seat = DecodeSeatNum(pass);
                max_seat = Math.Max(seat, max_seat);
                min_seat = Math.Min(seat, min_seat);
                sum_seat += seat;
            }
            Console.WriteLine(max_seat);

            // part 2
            Console.WriteLine(SumToN(max_seat) - SumToN(min_seat - 1) - sum_seat);
        }

        private static int SumToN(int n) => n * (n + 1) / 2;

        private static int DecodeSeatNum(string pass) {
            int row = 0;
            if (pass[0] == 'B') row += 64;
            if (pass[1] == 'B') row += 32;
            if (pass[2] == 'B') row += 16;
            if (pass[3] == 'B') row += 8;
            if (pass[4] == 'B') row += 4;
            if (pass[5] == 'B') row += 2;
            if (pass[6] == 'B') row += 1;

            int col = 0;
            if (pass[7] == 'R') col += 4;
            if (pass[8] == 'R') col += 2;
            if (pass[9] == 'R') col += 1;

            return row * 8 + col;
        }
    }
}
