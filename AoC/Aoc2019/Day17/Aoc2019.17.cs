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
        public static void Day17() {
            Console.WriteLine("Day17: ");

            long[] prog = File.ReadAllText("Aoc2019\\Day17\\input.txt").Split(',').Select(c => long.Parse(c)).ToArray();
            var intcode = new IntCode(prog);

            bool show_space = false;

            // part 1
            char[,] space = new char[100, 100];
            int x = 0, y = 0;
            int maxx = 0;
            int maxy = 0;
            intcode.Run(o => {
                space[x, y] = (char)o;
                if (show_space) Console.Write(space[x, y]);
                x++;
                if (o == 10) {
                    maxx = Math.Max(maxx, x);
                    x = 0;
                    y++;
                } else {
                    maxy = y;
                }
            });
            //Console.WriteLine($"{maxx} x {y}");

            // count intersections, calc aligment sum
            int sum = 0;
            for(x = 1; x < maxx - 1; x++) {
                for(y = 1; y < maxy - 1; y++) {
                    if (space[x, y] == '#' && space[x - 1, y] == '#' && space[x + 1, y] == '#' && space[x, y - 1] == '#' && space[x, y + 1] == '#') {
                        sum += x * y;
                    }
                }
            }
            Console.WriteLine(sum);

            // part 2
            // figured it out manually
            // R6L12R6 R6L12R6 L12R6L8L12 R12L10L10 L12R6L8L12 R12L10L10 L12R6L8L12 R12L10L10 L12R6L8L12 R6L12R6
            // A A B C B C B C B A
            string main = "A,A,B,C,B,C,B,C,B,A";
            string progA = "R,6,L,12,R,6";
            string progB = "L,12,R,6,L,8,L,12";
            string progC = "R,12,L,10,L,10";
            string combined = $"{main}\n{progA}\n{progB}\n{progC}\nn\n";
            intcode.Reset().WriteMemory(0, 2);
            foreach (char c in combined)
                intcode.Write((long)c);
            long dust = 0, last = 0;
            intcode.Run(o => {
                if (last == 10 && o == 10) {
                    if (show_space) Console.SetCursorPosition(0, Console.WindowTop);
                }
                if (show_space) Console.Write((char)o);
                last = o;
                if (o != 10) 
                    dust = o; 
            });
            Console.WriteLine(dust);
        }
    }
}
