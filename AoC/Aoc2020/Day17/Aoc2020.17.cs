using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC {
    public partial class Aoc2020 {


        public static void Day17() {
            Console.WriteLine("Day17: ");
            var input = File.ReadAllLines("Aoc2020\\Day17\\input.txt");

            bool[,,] active = new bool[32, 32, 32];

            for (int y = 0; y < input.Length; y++) {
                for (int x = 0; x < input[0].Length; x++) {
                    active[x + 12, y + 12, 16] = (input[y][x] == '#');
                }
            }
            int maxx = active.GetLength(0);
            int maxy = active.GetLength(1);
            int maxz = active.GetLength(2);

            // part 1
            for (int i = 0; i < 6; i++) {
                active = Cycle3D(active);
            }
            int count = 0;
            for (int x = 0; x < maxx; x++) {
                for (int y = 0; y < maxy; y++) {
                    for (int z = 0; z < maxz; z++) {
                        if (active[x, y, z])
                            count++;
                    }
                }
            }

            Console.WriteLine(count);

            // part 2
            Stopwatch sw = Stopwatch.StartNew();
            /*
            bool[,,,] active4D = new bool[32, 32, 32, 32];

            for (int y = 0; y < input.Length; y++) {
                for (int x = 0; x < input[0].Length; x++) {
                    active4D[x + 12, y + 12, 16, 16] = (input[y][x] == '#');
                }
            }
            int maxw = active4D.GetLength(3);
            for (int i = 0; i < 6; i++) {
                active4D = Cycle4D(active4D);
            }
            count = 0;
            for (int x = 0; x < maxx; x++) {
                for (int y = 0; y < maxy; y++) {
                    for (int z = 0; z < maxz; z++) {
                        for (int w = 0; w < maxw; w++) {
                            if (active4D[x, y, z, w]) {
                                count++;
                            }
                        }
                    }
                }
            }
            sw.Stop();
            Console.WriteLine($"{count} in {sw.ElapsedMilliseconds}ms");*/

            int[] active4Db = new int[32*32*32*32];
            sw.Restart();
            for (int y = 0; y < input.Length; y++) {
                for (int x = 0; x < input[0].Length; x++) {
                    active4Db[CycleIndex(x + 12, y + 12, 16, 16)] = (input[y][x] == '#') ? 1 : 0;
                }
            }
            for (int i = 0; i < 6; i++) {
                active4Db = Cycle4Db(active4Db);
            }
            count = 0;
            for (int x = 0; x < active4Db.Length; x++) {
                count += active4Db[x];
            }
            sw.Stop();
            Console.WriteLine($"{count} in {sw.ElapsedMilliseconds}ms");
        }

        private static bool[,,] Cycle3D(bool[,,] nodes) {
            bool[,,] nodes2 = (bool[,,])nodes.Clone();
            int maxx = nodes.GetLength(0);
            int maxy = nodes.GetLength(1);
            int maxz = nodes.GetLength(2);
            for (int x = 1; x < maxx - 1; x++) {
                for (int y = 1; y < maxy - 1; y++) {
                    for (int z = 1; z < maxz - 1; z++) {
                        int count = 0;
                        for (int dx = -1; dx <= 1; dx++) {
                            for (int dy = -1; dy <= 1; dy++) {
                                for (int dz = -1; dz <= 1; dz++) {
                                    if (dx != 0 || dy != 0 || dz != 0) {
                                        if (nodes[x + dx, y + dy, z + dz])
                                            count++;
                                    }
                                }
                            }
                        }
                        if (nodes[x, y, z]) {
                            nodes2[x, y, z] = (count == 2 || count == 3);
                        } else {
                            nodes2[x, y, z] = (count == 3);
                        }
                    }
                }
            }
            return nodes2;
        }

        private static bool[,,,] Cycle4D(bool[,,,] nodes) {
            bool[,,,] nodes2 = (bool[,,,])nodes.Clone();
            int maxx = nodes.GetLength(0);
            int maxy = nodes.GetLength(1);
            int maxz = nodes.GetLength(2);
            int maxw = nodes.GetLength(2);
            for (int x = 1; x < maxx - 1; x++) {
                for (int y = 1; y < maxy - 1; y++) {
                    for (int z = 1; z < maxz - 1; z++) {
                        for (int w = 1; w < maxw - 1; w++) {
                            int count = 0;
                            if (nodes[x - 1, y - 1, z - 1, w - 1]) count++;
                            if (nodes[x - 1, y - 1, z - 1, w + 0]) count++;
                            if (nodes[x - 1, y - 1, z - 1, w + 1]) count++;
                            if (nodes[x - 1, y - 1, z + 0, w - 1]) count++;
                            if (nodes[x - 1, y - 1, z + 0, w + 0]) count++;
                            if (nodes[x - 1, y - 1, z + 0, w + 1]) count++;
                            if (nodes[x - 1, y - 1, z + 1, w - 1]) count++;
                            if (nodes[x - 1, y - 1, z + 1, w + 0]) count++;
                            if (nodes[x - 1, y - 1, z + 1, w + 1]) count++;
                            if (nodes[x - 1, y + 0, z - 1, w - 1]) count++;
                            if (nodes[x - 1, y + 0, z - 1, w + 0]) count++;
                            if (nodes[x - 1, y + 0, z - 1, w + 1]) count++;
                            if (nodes[x - 1, y + 0, z + 0, w - 1]) count++;
                            if (nodes[x - 1, y + 0, z + 0, w + 0]) count++;
                            if (nodes[x - 1, y + 0, z + 0, w + 1]) count++;
                            if (nodes[x - 1, y + 0, z + 1, w - 1]) count++;
                            if (nodes[x - 1, y + 0, z + 1, w + 0]) count++;
                            if (nodes[x - 1, y + 0, z + 1, w + 1]) count++;
                            if (nodes[x - 1, y + 1, z - 1, w - 1]) count++;
                            if (nodes[x - 1, y + 1, z - 1, w + 0]) count++;
                            if (nodes[x - 1, y + 1, z - 1, w + 1]) count++;
                            if (nodes[x - 1, y + 1, z + 0, w - 1]) count++;
                            if (nodes[x - 1, y + 1, z + 0, w + 0]) count++;
                            if (nodes[x - 1, y + 1, z + 0, w + 1]) count++;
                            if (nodes[x - 1, y + 1, z + 1, w - 1]) count++;
                            if (nodes[x - 1, y + 1, z + 1, w + 0]) count++;
                            if (nodes[x - 1, y + 1, z + 1, w + 1]) count++;
                            if (nodes[x + 0, y - 1, z - 1, w - 1]) count++;
                            if (nodes[x + 0, y - 1, z - 1, w + 0]) count++;
                            if (nodes[x + 0, y - 1, z - 1, w + 1]) count++;
                            if (nodes[x + 0, y - 1, z + 0, w - 1]) count++;
                            if (nodes[x + 0, y - 1, z + 0, w + 0]) count++;
                            if (nodes[x + 0, y - 1, z + 0, w + 1]) count++;
                            if (nodes[x + 0, y - 1, z + 1, w - 1]) count++;
                            if (nodes[x + 0, y - 1, z + 1, w + 0]) count++;
                            if (nodes[x + 0, y - 1, z + 1, w + 1]) count++;
                            if (nodes[x + 0, y + 0, z - 1, w - 1]) count++;
                            if (nodes[x + 0, y + 0, z - 1, w + 0]) count++;
                            if (nodes[x + 0, y + 0, z - 1, w + 1]) count++;
                            if (nodes[x + 0, y + 0, z + 0, w - 1]) count++;
                            if (nodes[x + 0, y + 0, z + 0, w + 0]) count++;
                            if (nodes[x + 0, y + 0, z + 0, w + 1]) count++;
                            if (nodes[x + 0, y + 0, z + 1, w - 1]) count++;
                            if (nodes[x + 0, y + 0, z + 1, w + 0]) count++;
                            if (nodes[x + 0, y + 0, z + 1, w + 1]) count++;
                            if (nodes[x + 0, y + 1, z - 1, w - 1]) count++;
                            if (nodes[x + 0, y + 1, z - 1, w + 0]) count++;
                            if (nodes[x + 0, y + 1, z - 1, w + 1]) count++;
                            if (nodes[x + 0, y + 1, z + 0, w - 1]) count++;
                            if (nodes[x + 0, y + 1, z + 0, w + 0]) count++;
                            if (nodes[x + 0, y + 1, z + 0, w + 1]) count++;
                            if (nodes[x + 0, y + 1, z + 1, w - 1]) count++;
                            if (nodes[x + 0, y + 1, z + 1, w + 0]) count++;
                            if (nodes[x + 0, y + 1, z + 1, w + 1]) count++;
                            if (nodes[x + 1, y - 1, z - 1, w - 1]) count++;
                            if (nodes[x + 1, y - 1, z - 1, w + 0]) count++;
                            if (nodes[x + 1, y - 1, z - 1, w + 1]) count++;
                            if (nodes[x + 1, y - 1, z + 0, w - 1]) count++;
                            if (nodes[x + 1, y - 1, z + 0, w + 0]) count++;
                            if (nodes[x + 1, y - 1, z + 0, w + 1]) count++;
                            if (nodes[x + 1, y - 1, z + 1, w - 1]) count++;
                            if (nodes[x + 1, y - 1, z + 1, w + 0]) count++;
                            if (nodes[x + 1, y - 1, z + 1, w + 1]) count++;
                            if (nodes[x + 1, y + 0, z - 1, w - 1]) count++;
                            if (nodes[x + 1, y + 0, z - 1, w + 0]) count++;
                            if (nodes[x + 1, y + 0, z - 1, w + 1]) count++;
                            if (nodes[x + 1, y + 0, z + 0, w - 1]) count++;
                            if (nodes[x + 1, y + 0, z + 0, w + 0]) count++;
                            if (nodes[x + 1, y + 0, z + 0, w + 1]) count++;
                            if (nodes[x + 1, y + 0, z + 1, w - 1]) count++;
                            if (nodes[x + 1, y + 0, z + 1, w + 0]) count++;
                            if (nodes[x + 1, y + 0, z + 1, w + 1]) count++;
                            if (nodes[x + 1, y + 1, z - 1, w - 1]) count++;
                            if (nodes[x + 1, y + 1, z - 1, w + 0]) count++;
                            if (nodes[x + 1, y + 1, z - 1, w + 1]) count++;
                            if (nodes[x + 1, y + 1, z + 0, w - 1]) count++;
                            if (nodes[x + 1, y + 1, z + 0, w + 0]) count++;
                            if (nodes[x + 1, y + 1, z + 0, w + 1]) count++;
                            if (nodes[x + 1, y + 1, z + 1, w - 1]) count++;
                            if (nodes[x + 1, y + 1, z + 1, w + 0]) count++;
                            if (nodes[x + 1, y + 1, z + 1, w + 1]) count++;
                            if (nodes[x, y, z, w]) {
                                nodes2[x, y, z, w] = (count == 2 + 1 || count == 3 + 1); // we count ourselves when active
                            } else {
                                nodes2[x, y, z, w] = (count == 3);
                            }
                        }
                    }
                }
            }
            return nodes2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CycleIndex(int x, int y, int z, int w) => (x + 1) << 15 | (y + 1) << 10 | (z + 1) << 5 | (w + 1);

        private static int[] Cycle4Db(int[] nodes) {
            int[] nodes2 = (int[])nodes.Clone();
            int maxi = nodes.Length - CycleIndex(1, 1, 1, 1);
            for (int i = 0 + CycleIndex(1, 1, 1, 1); i < maxi; i++) {
                int count = 0;
                count += nodes[i + CycleIndex(-1, -1, -1, -1)];
                count += nodes[i + CycleIndex(-1, -1, -1, +0)];
                count += nodes[i + CycleIndex(-1, -1, -1, +1)];
                count += nodes[i + CycleIndex(-1, -1, +0, -1)];
                count += nodes[i + CycleIndex(-1, -1, +0, +0)];
                count += nodes[i + CycleIndex(-1, -1, +0, +1)];
                count += nodes[i + CycleIndex(-1, -1, +1, -1)];
                count += nodes[i + CycleIndex(-1, -1, +1, +0)];
                count += nodes[i + CycleIndex(-1, -1, +1, +1)];
                count += nodes[i + CycleIndex(-1, +0, -1, -1)];
                count += nodes[i + CycleIndex(-1, +0, -1, +0)];
                count += nodes[i + CycleIndex(-1, +0, -1, +1)];
                count += nodes[i + CycleIndex(-1, +0, +0, -1)];
                count += nodes[i + CycleIndex(-1, +0, +0, +0)];
                count += nodes[i + CycleIndex(-1, +0, +0, +1)];
                count += nodes[i + CycleIndex(-1, +0, +1, -1)];
                count += nodes[i + CycleIndex(-1, +0, +1, +0)];
                count += nodes[i + CycleIndex(-1, +0, +1, +1)];
                count += nodes[i + CycleIndex(-1, +1, -1, -1)];
                count += nodes[i + CycleIndex(-1, +1, -1, +0)];
                count += nodes[i + CycleIndex(-1, +1, -1, +1)];
                count += nodes[i + CycleIndex(-1, +1, +0, -1)];
                count += nodes[i + CycleIndex(-1, +1, +0, +0)];
                count += nodes[i + CycleIndex(-1, +1, +0, +1)];
                count += nodes[i + CycleIndex(-1, +1, +1, -1)];
                count += nodes[i + CycleIndex(-1, +1, +1, +0)];
                count += nodes[i + CycleIndex(-1, +1, +1, +1)];
                count += nodes[i + CycleIndex(+0, -1, -1, -1)];
                count += nodes[i + CycleIndex(+0, -1, -1, +0)];
                count += nodes[i + CycleIndex(+0, -1, -1, +1)];
                count += nodes[i + CycleIndex(+0, -1, +0, -1)];
                count += nodes[i + CycleIndex(+0, -1, +0, +0)];
                count += nodes[i + CycleIndex(+0, -1, +0, +1)];
                count += nodes[i + CycleIndex(+0, -1, +1, -1)];
                count += nodes[i + CycleIndex(+0, -1, +1, +0)];
                count += nodes[i + CycleIndex(+0, -1, +1, +1)];
                count += nodes[i + CycleIndex(+0, +0, -1, -1)];
                count += nodes[i + CycleIndex(+0, +0, -1, +0)];
                count += nodes[i + CycleIndex(+0, +0, -1, +1)];
                count += nodes[i + CycleIndex(+0, +0, +0, -1)];
              //count += nodes[i + CycleIndex(+0, +0, +0, +0)];
                count += nodes[i + CycleIndex(+0, +0, +0, +1)];
                count += nodes[i + CycleIndex(+0, +0, +1, -1)];
                count += nodes[i + CycleIndex(+0, +0, +1, +0)];
                count += nodes[i + CycleIndex(+0, +0, +1, +1)];
                count += nodes[i + CycleIndex(+0, +1, -1, -1)];
                count += nodes[i + CycleIndex(+0, +1, -1, +0)];
                count += nodes[i + CycleIndex(+0, +1, -1, +1)];
                count += nodes[i + CycleIndex(+0, +1, +0, -1)];
                count += nodes[i + CycleIndex(+0, +1, +0, +0)];
                count += nodes[i + CycleIndex(+0, +1, +0, +1)];
                count += nodes[i + CycleIndex(+0, +1, +1, -1)];
                count += nodes[i + CycleIndex(+0, +1, +1, +0)];
                count += nodes[i + CycleIndex(+0, +1, +1, +1)];
                count += nodes[i + CycleIndex(+1, -1, -1, -1)];
                count += nodes[i + CycleIndex(+1, -1, -1, +0)];
                count += nodes[i + CycleIndex(+1, -1, -1, +1)];
                count += nodes[i + CycleIndex(+1, -1, +0, -1)];
                count += nodes[i + CycleIndex(+1, -1, +0, +0)];
                count += nodes[i + CycleIndex(+1, -1, +0, +1)];
                count += nodes[i + CycleIndex(+1, -1, +1, -1)];
                count += nodes[i + CycleIndex(+1, -1, +1, +0)];
                count += nodes[i + CycleIndex(+1, -1, +1, +1)];
                count += nodes[i + CycleIndex(+1, +0, -1, -1)];
                count += nodes[i + CycleIndex(+1, +0, -1, +0)];
                count += nodes[i + CycleIndex(+1, +0, -1, +1)];
                count += nodes[i + CycleIndex(+1, +0, +0, -1)];
                count += nodes[i + CycleIndex(+1, +0, +0, +0)];
                count += nodes[i + CycleIndex(+1, +0, +0, +1)];
                count += nodes[i + CycleIndex(+1, +0, +1, -1)];
                count += nodes[i + CycleIndex(+1, +0, +1, +0)];
                count += nodes[i + CycleIndex(+1, +0, +1, +1)];
                count += nodes[i + CycleIndex(+1, +1, -1, -1)];
                count += nodes[i + CycleIndex(+1, +1, -1, +0)];
                count += nodes[i + CycleIndex(+1, +1, -1, +1)];
                count += nodes[i + CycleIndex(+1, +1, +0, -1)];
                count += nodes[i + CycleIndex(+1, +1, +0, +0)];
                count += nodes[i + CycleIndex(+1, +1, +0, +1)];
                count += nodes[i + CycleIndex(+1, +1, +1, -1)];
                count += nodes[i + CycleIndex(+1, +1, +1, +0)];
                count += nodes[i + CycleIndex(+1, +1, +1, +1)];

                if (nodes[i + CycleIndex(+0, +0, +0, +0)] == 1) {
                    nodes2[i + CycleIndex(+0, +0, +0, +0)] = (count == 2 || count == 3) ? 1 : 0;
                } else {
                    nodes2[i + CycleIndex(+0, +0, +0, +0)] = (count == 3) ? 1 : 0;
                }
            }
            return nodes2;
        }
    }
}
