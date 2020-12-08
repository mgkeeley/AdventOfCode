using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2020
    {
        public static void Day1() {
            Console.WriteLine("Day1: ");
            int[] data = File.ReadAllLines("Aoc2020\\Day01\\input.txt").Select(c => int.Parse(c)).ToArray();

            // part 1
            HashSet<int> nums = new HashSet<int>(data);
            for (int i = 0; i < data.Length; i++) {
                if (nums.Contains(2020 - data[i])) {
                    Console.WriteLine(data[i] * (2020-data[i]));
                    break;
                }
            }

            // part 2
            for (int i = 0; i < data.Length; i++) {
                for (int j = 0; j < data.Length; j++) {
                    if (nums.Contains(2020 - data[i] - data[j])) {
                        Console.WriteLine(data[i] * data[j] * (2020 - data[i] - data[j]));
                        i = data.Length;
                        break;
                    }
                }
            }
        }
    }
}
