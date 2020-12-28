using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AoC
{
    public partial class Aoc2019
    {
        public static void Day20() {
            Console.WriteLine("Day20: ");

            string[] input = File.ReadAllLines("Aoc2019\\Day20\\input.txt");
            int maxx = input[0].Length;
            int maxy = input.Length;

            Dictionary<string, int> warps = new();
            int[] board = new int[maxx * maxy];
            for(int y = 0; y < maxy; y++) {
                for(int x = 0; x < maxx; x++) {
                    if (input[y][x] == '#')
                        board[y * maxx + x] = 1;
                    else if (input[y][x] == '.')
                        board[y * maxx + x] = 2;
                }
            }

            // warps: outer
            int innerTop = 0, innerBottom = 0;
            for (int x = 0; x < maxx; x++) {
                if (input[2][x] == '.')
                    warps.Add("o" + input[0][x] + input[1][x], 2 * maxx + x);
                if (input[maxy - 3][x] == '.')
                    warps.Add("o" + input[maxy - 2][x] + input[maxy - 1][x], (maxy - 3) * maxx + x);
            }
            for (int y = 0; y < maxy; y++) {
                if (input[y][2] == '.')
                    warps.Add("o" + input[y][0] + input[y][1], y * maxx + 2);
                if (input[y][maxx - 3] == '.')
                    warps.Add("o" + input[y][maxx - 2] + input[y][maxx - 1], y * maxx + maxx - 3);
                if (innerTop == 0 && y > 3 && input[y].IndexOf(' ', 3) < maxx / 2)
                    innerTop = y;
                if (y < maxy - 4 && input[y].IndexOf(' ', 3) < maxx / 2)
                    innerBottom = y;
            }
            // warps: inner
            int innerLeft = input[innerTop].IndexOf(' ', 3);
            int innerRight = input[innerTop].IndexOf('#', innerLeft) - 1;
            for (int x = innerLeft; x < innerRight; x++) {
                int y = innerTop;
                if (input[y][x] != ' ')
                    warps.Add("i" + input[y][x] + input[y + 1][x], (y - 1) * maxx + x);
                y = innerBottom;
                if (input[y][x] != ' ')
                    warps.Add("i" + input[y - 1][x] + input[y][x], (y + 1) * maxx + x);
            }
            for (int y = innerTop; y < innerBottom; y++) {
                int x = innerLeft;
                if (input[y][x] != ' ')
                    warps.Add("i" + input[y][x] + input[y][x + 1], y * maxx + x - 1);
                x = innerRight;
                if (input[y][x] != ' ')
                    warps.Add("i" + input[y][x - 1] + input[y][x], y * maxx + x + 1);
            }

            int startPos = 0, endPos = 0;
            // marry up warps into board
            foreach(var warp in warps) {
                if (warp.Key.Contains("AA"))
                    startPos = warp.Value;
                else if (warp.Key.Contains("ZZ"))
                    endPos = warp.Value; 
                else {
                    var otherWarp = warps[(warp.Key.StartsWith("i") ? "o" : "i") + warp.Key.Substring(1)];
                    board[warp.Value] = otherWarp;
                    board[otherWarp] = warp.Value;
                }
            }


            Part1();
            void Part1() {
                Stopwatch sw = Stopwatch.StartNew();

                PriorityQueue<int> queue = new PriorityQueue<int>();
                Dictionary<int, int> visited = new Dictionary<int, int>();

                int minSteps = 0;
                queue.Add(startPos, 0);
                visited[queue.Peek()] = 0;
                while (queue.TryRemoveRoot(out int pos, out int dist)) {
                    if (pos == endPos) {
                        minSteps = dist;
                        break;
                    }
                    if (board[pos] > 2) TryVisit(board[pos]);
                    if (board[pos + 1] >= 2) TryVisit(pos + 1);
                    if (board[pos - 1] >= 2) TryVisit(pos - 1);
                    if (board[pos + maxx] >= 2) TryVisit(pos + maxx);
                    if (board[pos - maxx] >= 2) TryVisit(pos - maxx);

                    void TryVisit(int nextPos) {
                        if (visited.TryGetValue(nextPos, out int prevDist) && prevDist <= dist + 1)
                            return;
                        visited[nextPos] = dist + 1;
                        queue.Add(nextPos, dist + 1);
                    }
                }
                sw.Stop();
                Console.WriteLine($"{minSteps} in {sw.ElapsedMilliseconds}ms");

            }

            Part2();
            void Part2() {
                Stopwatch sw = Stopwatch.StartNew();

                PriorityQueue<int> queue = new PriorityQueue<int>();
                Dictionary<int, int> visited = new Dictionary<int, int>();

                int minSteps = 0;
                queue.Add(startPos, 0);
                visited[queue.Peek()] = 0;
                while (queue.TryRemoveRoot(out int pos, out int dist)) {
                    if (pos == endPos) {
                        minSteps = dist;
                        break;
                    }
                    if (board[pos & 0x0ffff] > 2) TryRecursiveVisit(pos);
                    if (board[(pos & 0x0ffff) + 1] >= 2) TryVisit(pos + 1);
                    if (board[(pos & 0x0ffff) - 1] >= 2) TryVisit(pos - 1);
                    if (board[(pos & 0x0ffff) + maxx] >= 2) TryVisit(pos + maxx);
                    if (board[(pos & 0x0ffff) - maxx] >= 2) TryVisit(pos - maxx);

                    void TryRecursiveVisit(int pos) {
                        int x = (pos & 0x0ffff) % maxx;
                        int y = (pos & 0x0ffff) / maxx;
                        int level = pos >> 16;
                        bool outer = x == 2 || x == maxx - 3 || y == 2 || y == maxy - 3;
                        if (outer && level > 0)
                            TryVisit(((level - 1) << 16) + board[pos & 0x0ffff]);
                        if (!outer)
                            TryVisit(((level + 1) << 16) + board[pos & 0x0ffff]);
                    }

                    void TryVisit(int nextPos) {
                        if (visited.TryGetValue(nextPos, out int prevDist) && prevDist <= dist + 1)
                            return;
                        visited[nextPos] = dist + 1;
                        queue.Add(nextPos, dist + 1);
                    }
                }
                sw.Stop();
                Console.WriteLine($"{minSteps} in {sw.ElapsedMilliseconds}ms");

            }

        }
    }
}
