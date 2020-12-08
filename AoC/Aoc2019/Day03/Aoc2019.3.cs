using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2019
    {
        enum Direction {
            L,
            R,
            U,
            D
        }
        private class Wire {
            public Direction direction;
            public int dist;
            public int x1, y1, x2, y2;
            public bool Horz => direction == Direction.L || direction == Direction.R;
        }

        public static void Day3() {
            Console.WriteLine("Day3: ");
            string[] wires = File.ReadAllLines("Aoc2019\\Day03\\input.txt");
            Wire[] wires1 = LoadWires(wires[0]).ToArray();
            Wire[] wires2 = LoadWires(wires[1]).ToArray();

            // find intersections - part one
            int best_dist = int.MaxValue;
            foreach (var w1 in wires1) {
                foreach (var w2 in wires2) {
                    if (w1.Horz && !w2.Horz && Between(w1.y1, w2.y1, w2.y2, out int y) && Between(w2.x1, w1.x1, w1.x2, out int x)
                        || (!w1.Horz && w2.Horz && Between(w2.y1, w1.y1, w1.y2, out y) && Between(w1.x1, w2.x1, w2.x2, out x))) {
                        if (Math.Abs(x) + Math.Abs(y) < best_dist) {
                            best_dist = Math.Abs(x) + Math.Abs(y);
                        }
                    }

                }
            }
            Console.WriteLine(best_dist);

            // find intersections - part two
            best_dist = int.MaxValue;
            foreach (var w1 in wires1) {
                foreach (var w2 in wires2) {
                    int dist = best_dist;
                    if (w1.Horz && !w2.Horz && Between(w1.y1, w2.y1, w2.y2, out int y) && Between(w2.x1, w1.x1, w1.x2, out int x)) {
                        dist = w1.dist + Math.Abs(y - w2.y1) + w2.dist + Math.Abs(x - w1.x1);
                    }
                    if (!w1.Horz && w2.Horz && Between(w2.y1, w1.y1, w1.y2, out y) && Between(w1.x1, w2.x1, w2.x2, out x)) {
                        dist = w2.dist + Math.Abs(y - w1.y1) + w1.dist + Math.Abs(x - w2.x1);
                    }

                    if (dist < best_dist) {
                        best_dist = dist;
                    }
                }
            }
            Console.WriteLine(best_dist);
        }

        private static bool Between(int v, int a, int b, out int i) {
            i = v;
            return (v >= Math.Min(a, b) && v <= Math.Max(a, b));
        }

        private static IEnumerable<Wire> LoadWires(string wire) {
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0, dist = 0;
            foreach(var seg in wire.Split(',')) {
                int len = int.Parse(seg[1..]);
                Direction dir = Enum.Parse<Direction>(seg.Substring(0, 1));
                if (dir == Direction.L)
                    x2 = x1 - len;
                else if (dir == Direction.R)
                    x2 = x1 + len;
                else if (dir == Direction.U)
                    y2 = y1 - len;
                else if (dir == Direction.D)
                    y2 = y1 + len;

                yield return new Wire { direction = dir, x1 = x1, y1 = y1, x2 = x2, y2 = y2, dist = dist };
                dist += len;
                x1 = x2;
                y1 = y2;
            }
        }
    }
}
