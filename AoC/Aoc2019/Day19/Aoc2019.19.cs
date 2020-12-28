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
        public static void Day19() {
            Console.WriteLine("Day19: ");

            long[] prog = File.ReadAllText("Aoc2019\\Day19\\input.txt").Split(',').Select(c => long.Parse(c)).ToArray();
            var intcode = new IntCode(prog);

            bool IsBeam(int x, int y) {
                bool isBeam = false;
                intcode.Reset().Write(x).Write(y).Run(o => isBeam = (o == 1));
                return isBeam;
            }

            // part 1
            int x = 0, y = 0;
            long count = 0;
            for (y = 0; y < 50; y++) {
                for (x = 0; x < 50; x++) {
                    if (IsBeam(x, y))
                        count++;
                }
            }
            Console.WriteLine(count);

            // part 2
            x = 0;
            y = 100; // skip some of the start - the beam is not contiguous there
            while (true) {
                //Console.WriteLine($"{x},{y} = {x * 10000 + y}");
                if (!IsBeam(x, y))
                    x++;
                else if (!IsBeam(x + 99, y))
                    y++;
                else if (!IsBeam(x, y + 99))
                    x++;
                else if (!IsBeam(x + 99, y + 99))
                    y++; // shouldn't happen!
                else {
                    // found it
                    break;
                }
            }
            Console.WriteLine($"{x},{y} = {x * 10000 + y}");

        }
    }
}
