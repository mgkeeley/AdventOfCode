using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AoC
{
    public partial class Aoc2019
    {
        public static void Day21() {
            Console.WriteLine("Day21: ");

            long[] prog = File.ReadAllText("Aoc2019\\Day21\\input.txt").Split(',').Select(c => long.Parse(c)).ToArray();
            var intcode = new IntCode(prog);

            bool show_space = false;

            // part 1
            int damage = 0;
            var springscript = @"
NOT A J
NOT B T
OR T J
NOT C T
OR T J
AND D J

WALK
";
            RunScript(intcode, springscript);

            // part 2
            springscript = @"
NOT A J
NOT B T
OR T J
NOT C T
OR T J
AND D J

NOT E T
NOT T T
OR H T
AND T J

RUN
";
            RunScript(intcode, springscript);

            void RunScript(IntCode intcode, string springscript) {
                intcode.Reset();
                foreach (var instr in springscript.Split("\r\n").Select(s => s.Trim()).Where(s => s != "")) {
                    foreach (var c in instr + "\n")
                        intcode.Write(c);
                }
                intcode.Run(o => {
                    if (o > 128)
                        damage = (int)o;
                    else if (show_space) 
                        Console.Write((char)o);
                });
                Console.WriteLine($"Damage: {damage}");
            }
        }
    }
}
