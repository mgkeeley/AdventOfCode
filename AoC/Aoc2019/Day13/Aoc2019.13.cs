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
        public static void Day13() {
            Console.WriteLine("Day13: ");

            long[] prog = File.ReadAllText("Aoc2019\\Day13\\input.txt").Split(',').Select(c => long.Parse(c)).ToArray();
            var intcode = new IntCode(prog);

            // part 1
            BlockingCollection<int> game = new();
            Task.Run(() => {
                intcode.Run(o => game.Add((int)o));
                game.CompleteAdding();
            });
            int num_blocks = 0;
            int max_x = 0, max_y = 0;
            while (true) {
                if (!game.TryTake(out int x, 10000))
                    break;
                int y = game.Take();
                int t = game.Take();
                max_x = Math.Max(max_x, x);
                max_y = Math.Max(max_y, y);
                if (t == 2)
                    num_blocks++;
            }
            Console.WriteLine(num_blocks);

            // part 2
            game = new();
            bool show_board = false;
            if (show_board) {
                for (int y = 0; y < Console.WindowHeight + 2; y++)
                    Console.WriteLine();
                Console.CursorVisible = false;
            }
            int high_score = 0;
            intcode.Reset();
            intcode.WriteMemory(0, 2); // play game
            Task.Run(() => {
                intcode.Run(o => game.Add((int)o));
                game.CompleteAdding();
            });
            bool play_manually = false;
            if (play_manually) { // play manually
                Task.Run(() => {
                    while (!game.IsCompleted) {
                        var k = Console.ReadKey();
                        if (k.Key == ConsoleKey.LeftArrow)
                            intcode.Write(-1);
                        else if (k.Key == ConsoleKey.RightArrow)
                            intcode.Write(1);
                        else
                            intcode.Write(0);
                    }
                });
            }
            int last_x = -1;
            int paddle_x = 0;
            bool auto_play = false;
            while (true) {
                if (!game.TryTake(out int x, -1))
                    break;
                int y = game.Take();
                int t = game.Take();
                if (x == -1) {
                    if (show_board) Console.SetCursorPosition(0, Console.WindowTop);
                    if (show_board) Console.Write("Score: " + t + "            ");
                    high_score = t;
                    continue;
                }
                if (show_board) Console.SetCursorPosition(x, y + Console.WindowTop + 1);
                if (t == 0) {
                    if (show_board) Console.Write(' ');
                } else if (t == 1) {
                    if (show_board) Console.Write('▓');
                } else if (t == 2) {
                    if (show_board) Console.Write('░');
                } else if (t == 3) {
                    if (show_board) Console.Write('▀');
                    paddle_x = x;
                } else if (t == 4) {
                    if (show_board) Console.Write('o');
                    if (!play_manually) {
                        if (paddle_x == last_x)
                            auto_play = true;
                        if (!auto_play)
                            intcode.Write(0);
                        else if (last_x > x)
                            intcode.Write(-1);
                        else if (last_x < x)
                            intcode.Write(1);
                        last_x = x;
                        //Thread.Sleep(10);
                    }
                }
            }
            if (show_board) Console.SetCursorPosition(0, Console.WindowTop + max_y + 2);
            Console.WriteLine("Game Over!  High Score = " + high_score);
            Console.CursorVisible = true;
        }
    }
}
