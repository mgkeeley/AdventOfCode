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
        public static void Day25() {
            Console.WriteLine("Day25: ");

            long[] prog = File.ReadAllText("Aoc2019\\Day25\\input.txt").Split(',').Select(c => long.Parse(c)).ToArray();
            var intcode = new IntCode(prog);

            // part 1
            bool play_manually = false;
            bool done = false;
            intcode.Reset();
            List<char> line = new List<char>();
            string lastline = "";
            Task.Run(() => {
                intcode.Run(o => {
                    if (play_manually)
                        Console.Write((char)o);
                    else if (o == 10) {
                        lastline = new string(line.ToArray());
                        line.Clear();
                    } else
                        line.Add((char)o);
                });
                done = true;
            });
            if (!play_manually) {
                string cmds = @"north
north
north
take astrolabe
south
south
take sand
south
west
north
take shell
south
south
west
take ornament
west
south
south
";
                cmds = cmds.Trim().Replace("\r", "");
                for (int i = 0; i < cmds.Length; i++)
                    intcode.Write(cmds[i]);
                intcode.Write(10);
                while (!done)
                    Thread.Sleep(10);
                Console.WriteLine(lastline);
            } else {
                while (!done) {
                    var cmd = Console.ReadLine().Trim();
                    if (cmd == "n")
                        cmd = "north";
                    if (cmd == "s")
                        cmd = "south";
                    if (cmd == "e")
                        cmd = "east";
                    if (cmd == "w")
                        cmd = "west";

                    for (int i = 0; i < cmd.Length; i++)
                        intcode.Write(cmd[i]);
                    intcode.Write(10);
                }
            }
        }
    }
}
