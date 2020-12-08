using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC {
    public partial class Aoc2019 {
        public static void Day2() {
            Console.WriteLine("Day2: ");
            long[] prog = "1,12,2,3,1,1,2,3,1,3,4,3,1,5,0,3,2,10,1,19,1,19,6,23,2,13,23,27,1,27,13,31,1,9,31,35,1,35,9,39,1,39,5,43,2,6,43,47,1,47,6,51,2,51,9,55,2,55,13,59,1,59,6,63,1,10,63,67,2,67,9,71,2,6,71,75,1,75,5,79,2,79,10,83,1,5,83,87,2,9,87,91,1,5,91,95,2,13,95,99,1,99,10,103,1,103,2,107,1,107,6,0,99,2,14,0,0"
                .Split(',').Select(c => long.Parse(c)).ToArray();
            // part 1
            var intcode = new IntCode(prog);
            var result = intcode.Reset().SetParams(12, 2).Run().ReadMemory(0);
            Console.WriteLine(result);
            // part 2
            for (int p1 = 0; p1 <= 99; p1++) {
                for (int p2 = 0; p2 <= 99; p2++) {
                    result = intcode.Reset().SetParams(p1, p2).Run().ReadMemory(0);
                    if (result == 19690720)
                        Console.WriteLine(100 * p1 + p2);
                }
            }
        }
    }
}
