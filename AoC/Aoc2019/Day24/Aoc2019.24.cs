using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AoC
{
    public partial class Aoc2019 {
        struct Adjacency {
            public int level;
            public int cell;
        }

        class AdjacencyList : List<Adjacency> {
            public void Add(int level, int cell) {
                Add(new Adjacency { level = level, cell = cell });
            }
        }

        public static void Day24() {
            Console.WriteLine("Day24: ");
            Stopwatch sw = Stopwatch.StartNew();

            string data = File.ReadAllText("Aoc2019\\Day24\\input.txt").Replace("\r\n", "");

            void Part1() {
                long tile = 0;
                for (int i = 0; i < data.Length; i++) {
                    if (data[i] == '#')
                        tile |= 1L << i;
                }

                HashSet<long> prevTiles = new();
                while (true) {
                    if (prevTiles.Contains(tile))
                        break;
                    prevTiles.Add(tile);
                    //WriteTile("tile", tile);

                    long left = (tile << 1) & 0b1111011110111101111011110;
                    long right = (tile >> 1) & 0b0111101111011110111101111;
                    long top = tile << 5;
                    long bottom = tile >> 5;
                    //WriteTile("left", left);
                    //WriteTile("right", right);
                    //WriteTile("top", top);
                    //WriteTile("bottom", bottom);

                    long any = left | right | top | bottom;
                    long odd = left ^ right ^ top ^ bottom;
                    long even = any & ~odd;
                    long four = left & right & top & bottom;
                    long two = even & ~four;
                    long oneH = (left ^ right) & ~(top | bottom);
                    long oneV = (top ^ bottom) & ~(right | left);
                    long one = oneH | oneV;
                    //WriteTile("any", any);
                    //WriteTile("odd", odd);
                    //WriteTile("even", even);
                    //WriteTile("four", four);
                    //WriteTile("two", two);
                    //WriteTile("one", one);

                    // A bug dies (becoming an empty space) unless there is exactly one bug adjacent to it.
                    // An empty space becomes infested with a bug if exactly one or two bugs are adjacent to it.
                    long dead = tile & ~one;
                    long infested = ~tile & (one | two);
                    tile = (tile & ~dead) | infested;
                    tile = tile & 0b1111111111111111111111111;
                }
                sw.Stop();
                Console.WriteLine($"Found duplicate tile {tile} in {sw.ElapsedMilliseconds}ms");
            }
            Part1();

            // part 2
            sw.Restart();
            int[,] tile = new int[400, 25];
            for (int i = 0; i < data.Length; i++) {
                if (data[i] == '#')
                    tile[200, i] = 1;
            }

            AdjacencyList[] adj = new AdjacencyList[25];
            /* part 1
            adj[0] = new AdjacencyList {                        { 0, 1 },  { 0, 5 }, };
            adj[1] = new AdjacencyList {             { 0, 0 },  { 0, 2 },  { 0, 6 } };
            adj[2] = new AdjacencyList {             { 0, 1 },  { 0, 3 },  { 0, 7 } };
            adj[3] = new AdjacencyList {             { 0, 2 },  { 0, 4 },  { 0, 8 } };
            adj[4] = new AdjacencyList {             { 0, 3 },             { 0, 9 } };
            adj[5] = new AdjacencyList {  { 0, 0 },             { 0, 6 },  { 0, 10 } };
            adj[6] = new AdjacencyList {  { 0, 1 },  { 0, 5 },  { 0, 7 },  { 0, 11 } };
            adj[7] = new AdjacencyList {  { 0, 2 },  { 0, 6 },  { 0, 8 },  { 0, 12 } };
            adj[8] = new AdjacencyList {  { 0, 3 },  { 0, 7 },  { 0, 9 },  { 0, 13 } };
            adj[9] = new AdjacencyList {  { 0, 4 },  { 0, 8 },             { 0, 14 } };
            adj[10] = new AdjacencyList { { 0, 5 },             { 0, 11 }, { 0, 15 } };
            adj[11] = new AdjacencyList { { 0, 6 },  { 0, 10 }, { 0, 12 }, { 0, 16 } };
            adj[12] = new AdjacencyList { { 0, 7 },  { 0, 11 }, { 0, 13 }, { 0, 17 } };
            adj[13] = new AdjacencyList { { 0, 8 },  { 0, 12 }, { 0, 14 }, { 0, 18 } };
            adj[14] = new AdjacencyList { { 0, 9 },  { 0, 13 },            { 0, 19 } };
            adj[15] = new AdjacencyList { { 0, 10 },            { 0, 16 }, { 0, 20 } };
            adj[16] = new AdjacencyList { { 0, 11 }, { 0, 15 }, { 0, 17 }, { 0, 21 } };
            adj[17] = new AdjacencyList { { 0, 12 }, { 0, 16 }, { 0, 18 }, { 0, 22 } };
            adj[18] = new AdjacencyList { { 0, 13 }, { 0, 17 }, { 0, 19 }, { 0, 23 } };
            adj[19] = new AdjacencyList { { 0, 14 }, { 0, 18 },            { 0, 24 } };
            adj[20] = new AdjacencyList { { 0, 15 },            { 0, 21 } };
            adj[21] = new AdjacencyList { { 0, 16 }, { 0, 20 }, { 0, 22 } };
            adj[22] = new AdjacencyList { { 0, 17 }, { 0, 21 }, { 0, 23 } };
            adj[23] = new AdjacencyList { { 0, 18 }, { 0, 22 }, { 0, 24 } };
            adj[24] = new AdjacencyList { { 0, 19 }, { 0, 23 } };
            */

            // 0  1  2  3  4
            // 5  6  7  8  9
            // 10 11 [] 13 14
            // 15 16 17 18 19
            // 20 21 22 23 24

            // part 2
            adj[0] = new AdjacencyList { { 0, 1 }, { 0, 5 }, { -1, 7 }, { -1, 11 } };
            adj[1] = new AdjacencyList { { 0, 0 }, { 0, 2 }, { 0, 6 }, { -1, 7 } };
            adj[2] = new AdjacencyList { { 0, 1 }, { 0, 3 }, { 0, 7 }, { -1, 7 } };
            adj[3] = new AdjacencyList { { 0, 2 }, { 0, 4 }, { 0, 8 }, { -1, 7 } };
            adj[4] = new AdjacencyList { { 0, 3 }, { 0, 9 }, { -1, 7 }, { -1, 13 } };
            adj[5] = new AdjacencyList { { 0, 0 }, { 0, 6 }, { 0, 10 }, { -1, 11 } };
            adj[6] = new AdjacencyList { { 0, 1 }, { 0, 5 }, { 0, 7 }, { 0, 11 } };
            adj[7] = new AdjacencyList { { 0, 2 }, { 0, 6 }, { 0, 8 }, { 1, 0 }, { 1, 1 }, { 1, 2 }, { 1, 3 }, { 1, 4 } };
            adj[8] = new AdjacencyList { { 0, 3 }, { 0, 7 }, { 0, 9 }, { 0, 13 } };
            adj[9] = new AdjacencyList { { 0, 4 }, { 0, 8 }, { 0, 14 }, { -1, 13 } };
            adj[10] = new AdjacencyList { { 0, 5 }, { 0, 11 }, { 0, 15 }, { -1, 11 } };
            adj[11] = new AdjacencyList { { 0, 6 }, { 0, 10 }, { 0, 16 }, { 1, 0 }, { 1, 5 }, { 1, 10 }, { 1, 15 }, { 1, 20 } };
            adj[12] = new AdjacencyList { };
            adj[13] = new AdjacencyList { { 0, 8 }, { 0, 14 }, { 0, 18 }, { 1, 4 }, { 1, 9 }, { 1, 14 }, { 1, 19 }, { 1, 24 } };
            adj[14] = new AdjacencyList { { 0, 9 }, { 0, 13 }, { 0, 19 }, { -1, 13 } };
            adj[15] = new AdjacencyList { { 0, 10 }, { 0, 16 }, { 0, 20 }, { -1, 11 } };
            adj[16] = new AdjacencyList { { 0, 11 }, { 0, 15 }, { 0, 17 }, { 0, 21 } };
            adj[17] = new AdjacencyList { { 0, 16 }, { 0, 18 }, { 0, 22 }, { 1, 20 }, { 1, 21 }, { 1, 22 }, { 1, 23 }, { 1, 24 } };
            adj[18] = new AdjacencyList { { 0, 13 }, { 0, 17 }, { 0, 19 }, { 0, 23 } };
            adj[19] = new AdjacencyList { { 0, 14 }, { 0, 18 }, { 0, 24 }, { -1, 13 } };
            adj[20] = new AdjacencyList { { 0, 15 }, { 0, 21 }, { -1, 11 }, { -1, 17 } };
            adj[21] = new AdjacencyList { { 0, 16 }, { 0, 20 }, { 0, 22 }, { -1, 17 } };
            adj[22] = new AdjacencyList { { 0, 17 }, { 0, 21 }, { 0, 23 }, { -1, 17 } };
            adj[23] = new AdjacencyList { { 0, 18 }, { 0, 22 }, { 0, 24 }, { -1, 17 } };
            adj[24] = new AdjacencyList { { 0, 19 }, { 0, 23 }, { -1, 13 }, { -1, 17 } };

            int minLayer = 199;
            int maxLayer = 201;

            for (int cycle = 0; cycle < 200; cycle++) {
                tile = RunSimulation(tile, adj, ref minLayer, ref maxLayer);
            }
            int count = 0;
            for (int layer = minLayer; layer <= maxLayer; layer++) {
                for (int cell = 0; cell < 25; cell++) {
                    count += tile[layer, cell];
                }
            }
            sw.Stop();
            Console.WriteLine($"{count} bugs in {sw.ElapsedMilliseconds}ms");
        }

        private static void WriteTile(string title, long tile) {
            for (int i = 0; i < 25; i++) {
                if (i % 5 == 0)
                    Console.WriteLine();
                if ((tile & (1 << i)) != 0) Console.Write('#'); else Console.Write('.');
            }
            Console.WriteLine();
            Console.WriteLine(title);
        }

        private static int[,] RunSimulation(int[,] tile, AdjacencyList[] adj, ref int minLayer, ref int maxLayer) {
            int[,] next = new int[400, 25];
            for (int layer = minLayer; layer <= maxLayer; layer++) {
                for (int cell = 0; cell < 25; cell++) {
                    int adjacent = adj[cell].Select(a => tile[layer + a.level, a.cell]).DefaultIfEmpty().Sum();
                    // A bug dies (becoming an empty space) unless there is exactly one bug adjacent to it.
                    // An empty space becomes infested with a bug if exactly one or two bugs are adjacent to it.
                    if (tile[layer, cell] == 0 && (adjacent == 1 || adjacent == 2)) {
                        if (layer - 1 < minLayer) minLayer = layer - 1;
                        if (layer + 1 > maxLayer) maxLayer = layer + 1;
                        next[layer, cell] = 1;
                    } else if (tile[layer, cell] == 1 && adjacent == 1) {
                        next[layer, cell] = 1;
                    }
                }
            }
            return next;
        }
    }
}
