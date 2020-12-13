using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2020
    {
        public static void Day12() {
            Console.WriteLine("Day12: ");
            var input = File.ReadAllLines("Aoc2020\\Day12\\input.txt");

            int x = 0, y = 0;
            int a = 0;
            foreach (string dir in input) {
                int dist = int.Parse(dir.Substring(1));
                switch (dir[0]) {
                    case 'N':
                        y -= dist;
                        break;
                    case 'S':
                        y += dist;
                        break;
                    case 'E':
                        x += dist;
                        break;
                    case 'W':
                        x -= dist;
                        break;
                    case 'F':
                        x += (int)Math.Round(Math.Cos(Math.PI * a / 180.0) * dist);
                        y += (int)Math.Round(Math.Sin(Math.PI * a / 180.0) * dist);
                        break;
                    case 'R':
                        a += dist;
                        break;
                    case 'L':
                        a -= dist;
                        break;
                }
                //Console.WriteLine($"{dir}: {x},{y}");
            }
            Console.WriteLine(Math.Abs(x) + Math.Abs(y));

            x = 0;
            y = 0;
            int wx = 10;
            int wy = -1;
            foreach (string dir in input) {
                int dist = int.Parse(dir.Substring(1));
                switch (dir[0]) {
                    case 'N':
                        wy -= dist;
                        break;
                    case 'S':
                        wy += dist;
                        break;
                    case 'E':
                        wx += dist;
                        break;
                    case 'W':
                        wx -= dist;
                        break;
                    case 'F':
                        x += wx * dist;
                        y += wy * dist;
                        break;
                    case 'R':
                        double angle = Math.Atan2(wy, wx) + (Math.PI * dist / 180.0);
                        double mag = Math.Sqrt(wy * wy + wx * wx);
                        wx = (int)Math.Round(Math.Cos(angle) * mag);
                        wy = (int)Math.Round(Math.Sin(angle) * mag);
                        break;
                    case 'L':
                        angle = Math.Atan2(wy, wx) - (Math.PI * dist / 180.0);
                        mag = Math.Sqrt(wy * wy + wx * wx);
                        wx = (int)Math.Round(Math.Cos(angle) * mag);
                        wy = (int)Math.Round(Math.Sin(angle) * mag);
                        break;
                }
                //Console.WriteLine($"{dir}: {x},{y} ({wx},{wy})");
            }
            Console.WriteLine(Math.Abs(x) + Math.Abs(y));
        }
    }
}
