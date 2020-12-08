using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2019
    {
        public static void Day9() {
            Console.WriteLine("Day9: ");

            long[] prog = File.ReadAllText("Aoc2019\\Day09\\input.txt").Split(',').Select(c => long.Parse(c)).ToArray();

            // part 1
            List<long> output = new List<long>();
            var intcode = new IntCode(prog);
            intcode.Write(1).Run(o => output.Add(o));
            Console.WriteLine(string.Join(',', output));

            // part 2
            output.Clear();
            intcode = new IntCode(prog);
            intcode.Reset().Write(2).Run(o => output.Add(o));
            Console.WriteLine(string.Join(',', output));
        }
    }
}
