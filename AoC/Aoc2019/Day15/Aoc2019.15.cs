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
        public static void Day15() {
            Console.WriteLine("Day15: ");

            long[] prog = File.ReadAllText("Aoc2019\\Day15\\input.txt").Split(',').Select(c => long.Parse(c)).ToArray();
            var intcode = new IntCode(prog);

            // part 1
            // we need to explore the game space; and then count the steps to the origin.
            // idea1: wall following
            // idea2: traverse to the nearest unexplored space (also could be used to get back to origin)
            // idea3: keep track of min distance to origin at each square.

            int[,] board = new int[100,100];
            BlockingCollection<int> game = new();
            int x = 25, y = 25, dx = 0, dy = -1, dist = 1, max_dist = 0;
            bool hitWall = false, findOxygen;
            bool show_board = false;

            if (show_board) {
                Console.WindowHeight = 50;
                //Console.WindowWidth = 100;
                for (int cy = 0; cy < Console.WindowHeight; cy++)
                    Console.WriteLine();
                Console.CursorVisible = false;
            }

            // part 1
            findOxygen = true;
            board[x, y] = dist;
            intcode.Reset();
            Task.Run(() => {
                intcode.Run(o => game.Add((int)o));
                game.CompleteAdding();
            });
            RunSimulation(show_board, autoplay: FollowWall);

            if (show_board)
                Console.SetCursorPosition(0, Console.WindowTop + Console.WindowHeight - 1);

            Console.Write($"Found Oxygen at {x},{y} dist to origin = {dist}");

            // part 2
            findOxygen = false;
            // reset dists
            dist = 0;
            max_dist = 0;
            ClearDists(board);
            RunSimulation(show_board, autoplay: FollowWall);

            if (show_board) {
                Console.SetCursorPosition(0, Console.WindowTop + Console.WindowHeight);
                Console.CursorVisible = true;
            }
            Console.WriteLine($"Max time for oxygen dispersion: {max_dist}");

            void RunSimulation(bool show_board, Action autoplay) {
                while (true) {
                    if (show_board && board[x,y] != -2) {
                        Console.SetCursorPosition(x, y + Console.WindowTop);
                        Console.Write('D');
                        Thread.Sleep(10);
                    }
                    autoplay?.Invoke();
                    if (!game.TryTake(out int r, -1))
                        break;
                    if (show_board && board[x, y] != -2) {
                        Console.SetCursorPosition(x, y + Console.WindowTop);
                        Console.Write('.');
                    }
                    hitWall = false;
                    x += dx;
                    y += dy;
                    if (show_board) Console.SetCursorPosition(x, y + Console.WindowTop);
                    if (r == 0) {
                        if (show_board) Console.Write('▓');
                        board[x, y] = -1;
                        x -= dx;
                        y -= dy;
                        hitWall = true;
                    } else if (r == 1) {
                        if (board[x, y] == 0)
                            board[x, y] = ++dist;
                        else
                            dist = board[x, y];
                        if (dist > max_dist)
                            max_dist = dist;
                    } else if (r == 2) {
                        if (show_board) Console.Write('@');
                        board[x, y] = -2;
                        if (findOxygen)
                            break;
                    }
                    if (!findOxygen && x == 25 && y == 25) // back to the starting position
                        break;
                }
            }

            void FollowWall() {
                if (hitWall) { // we smacked into a wall, turn right
                    var ox = dx;
                    dx = -dy;
                    dy = ox;
                } else if (board[x + dy, y - dx] != -1) { // no wall to left - turn left
                    var ox = dx;
                    dx = dy;
                    dy = -ox;
                } else {
                    while (board[x + dx, y + dy] == -1) { // wall ahead - turn right
                        var ox = dx;
                        dx = -dy;
                        dy = ox;
                    }
                }
                int next = (dy < 0) ? 1 : (dy > 0) ? 2 : (dx < 0) ? 3 : 4;
                intcode.Write(next);
            }

            void ClearDists(int[,] board) {
                for(int x = 0; x < board.GetLength(0); x++) {
                    for (int y = 0; y < board.GetLength(1); y++) {
                        if (board[x, y] > 0)
                            board[x, y] = 0;
                    }
                }
            }

            void PlayManually() {
                var k = Console.ReadKey(true);
                dx = dy = 0;
                if (k.Key == ConsoleKey.UpArrow) {
                    dy = -1;
                    intcode.Write(1);
                } else if (k.Key == ConsoleKey.DownArrow) {
                    dy = 1;
                    intcode.Write(2);
                } else if (k.Key == ConsoleKey.LeftArrow) {
                    dx = -1;
                    intcode.Write(3);
                } else if (k.Key == ConsoleKey.RightArrow) {
                    dx = 1;
                    intcode.Write(4);
                } else if (k.Key == ConsoleKey.Q) {
                    intcode.Write(0);
                }
            }
        }
    }
}
