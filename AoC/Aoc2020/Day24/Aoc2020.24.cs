using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Pidgin;

namespace AoC {
    public partial class Aoc2020 {

        public static void Day24() {
            Console.WriteLine("Day24: ");
            Stopwatch sw = Stopwatch.StartNew();
            var input = File.ReadAllLines("Aoc2020\\Day24\\input.txt");

            int n = 200;
            // the board is the hexagonal space, skewed horizontally / / so that a NW tile is now directly N.  
            // This makes the space orthogonal, NW is (0,-1), NE is (+1,-1), W is (-1,0), E is (+1,0), SW is (-1,+1) and SE is (0,+1)
            // we make the board big enough to cater for all the modifications required
            int[,] board = new int[n, n];
            int count = 0;
            foreach(var steps in input) {
                int x = n / 2, y = n / 2;
                for(int i = 0; i < steps.Length; i++) {
                    if (steps[i] == 'n') {
                        y--;
                        i++;
                        if (steps[i] == 'e')
                            x++;
                    } else if (steps[i] == 's') {
                        y++;
                        i++;
                        if (steps[i] == 'w')
                            x--;
                    } else {
                        if (steps[i] == 'w')
                            x--;
                        else if (steps[i] == 'e')
                            x++;
                    }
                }
                if (board[x, y] == 0)
                    count++;
                else
                    count--;
                board[x, y] = 1 - board[x, y];
            }
            sw.Stop();
            Console.WriteLine($"{count} in {sw.ElapsedMilliseconds}ms");

            // part 2
            sw.Restart();
            int[,] next = (int[,])board.Clone();
            for (int i = 0; i < 100; i++) {
                for (int x = 1; x < n-1; x++) {
                    for (int y = 1; y < n-1; y++) {
                        count = 0;
                        count += board[x - 1, y];
                        count += board[x - 1, y + 1];
                        count += board[x, y - 1];
                        count += board[x, y + 1];
                        count += board[x + 1, y - 1];
                        count += board[x + 1, y];
                        if (board[x, y] == 0)
                            next[x, y] = (count == 2) ? 1 : 0;
                        else
                            next[x, y] = (count == 0 || count > 2) ? 0 : 1;
                    }
                }
                var tmp = board;
                board = next;
                next = tmp;
            }
            count = 0;
            for (int x = 1; x < n - 1; x++) {
                for (int y = 1; y < n - 1; y++) {
                    count += board[x, y];
                }
            }
            sw.Stop();
            Console.WriteLine($"{count} in {sw.ElapsedMilliseconds}ms");
        }
    }
}
