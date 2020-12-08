using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2019
    {
        public static void Day5() {
            Console.WriteLine("Day5: ");

            long[] prog = File.ReadAllText("Aoc2019\\Day05\\input.txt").Split(',').Select(c => long.Parse(c)).ToArray();

            // part 1
            List<long> output = new List<long>();
            var intcode = new IntCode(prog);
            intcode.Write(1).Run(o => output.Add(o));
            Console.WriteLine(string.Join(',', output));

            // part 2
            output.Clear();
            intcode.Reset().Write(5).Run(o => output.Add(o));
            Console.WriteLine(string.Join(',', output));
        }
    }
}
