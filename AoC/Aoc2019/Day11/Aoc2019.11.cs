using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    public partial class Aoc2019
    {
        public static void Day11() {
            Console.WriteLine("Day11: ");

            long[] prog = File.ReadAllText("Aoc2019\\Day11\\input.txt").Split(',').Select(c => long.Parse(c)).ToArray();
            var intcode = new IntCode(prog);

            // part 1
            Dictionary<long, bool> hull = new Dictionary<long, bool>();
            int panels_painted = PaintHull(intcode, hull);
            Console.WriteLine(panels_painted);

            // part 2
            hull.Clear();
            hull.Add(0, true);
            int painted = PaintHull(intcode, hull);
            bool[,] pic = new bool[50, 10];
            foreach (var p in hull) {
                pic[p.Key / 100, p.Key % 100] = p.Value;
            }
            for (int y = 0; y < 6; y++) {
                for (int x = 0; x < 50; x++) {
                    Console.Write(pic[x, y] ? '▓' : ' ');
                }
                Console.WriteLine();
            }
        }

        private static int PaintHull(IntCode intcode, Dictionary<long, bool> hull) {
            intcode.Reset();
            BlockingCollection<long> output = new BlockingCollection<long>();
            var task = Task.Run(() => {
                intcode.Run(o => output.Add(o));
                output.CompleteAdding();
            });

            int loc = 0;
            int dir = 0;
            int panels_painted = 0;
            hull.TryGetValue(loc, out bool iswhite);
            intcode.Write(iswhite ? 1 : 0);
            while (output.TryTake(out long paint, 1000)) {
                if (!hull.ContainsKey(loc))
                    panels_painted++;
                hull[loc] = paint == 1;
                long turn = output.Take();
                if (turn == 0)
                    dir = (dir + 3) % 4;
                else
                    dir = (dir + 1) % 4;
                if (dir == 0)
                    loc--;
                else if (dir == 1)
                    loc += 100;
                else if (dir == 2)
                    loc++;
                else if (dir == 3)
                    loc -= 100;
                hull.TryGetValue(loc, out iswhite);
                intcode.Write(iswhite ? 1 : 0);
            }

            return panels_painted;
        }
    }
}
