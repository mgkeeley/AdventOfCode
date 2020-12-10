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
        public static void Day9() {
            Console.WriteLine("Day9: ");
            var codes = File.ReadAllLines("Aoc2020\\Day09\\input.txt").Select(long.Parse).ToArray();

            // part 1
            HashSet<long> preamble = new(codes.AsSpan(0, 25).ToArray());
            long invalid = -1;

            bool HasSum(HashSet<long> preamble, long sum) {
                foreach(int addend in preamble) {
                    if (addend != sum / 2 && preamble.Contains(sum - addend))
                        return true;
                }
                return false;
            }

            for (int i = 25; i < codes.Length; i++) {
                if (!HasSum(preamble, codes[i])) {
                    invalid = codes[i];
                    break;
                }
                preamble.Remove(codes[i - 25]);
                preamble.Add(codes[i]);
            }
            Console.WriteLine(invalid);

            //part2
            int start = 0, end = 0;
            long sum = 0;
            while (sum != invalid) {
                if (sum < invalid) {
                    sum += codes[end];
                    end++;
                } else {
                    sum -= codes[start];
                    start++;
                }
            }
            Console.WriteLine($"Found range {start}..{end - 1} summing to {invalid}");
            long min = long.MaxValue, max = 0;
            for(int i = start; i < end; i++) {
                min = Math.Min(min, codes[i]);
                max = Math.Max(max, codes[i]);
            }
            Console.WriteLine(min + max);
        }

        
    }
}
