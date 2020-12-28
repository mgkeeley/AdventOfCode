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

        public class TileEdges {
            public int left, right, top, bottom;
            public int left_match, right_match, top_match, bottom_match;

            public IEnumerable<(int, int)> EdgeRange(int x1, int y1, int x2, int y2) {
                int dx = Math.Sign(x2 - x1);
                int dy = Math.Sign(y2 - y1);
                while (x1 != x2 || y1 != y2) {
                    yield return (x1, y1);
                    x1 += dx;
                    y1 += dy;
                }
                yield return (x2, y2);
            }

            public TileEdges(string[] data, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4) {
                //foreach ((int x, int y) in EdgeRange(x1, y1, x2, y2)) top = (top << 1) | (data[y][x] == '#' ? 1 : 0);    // we actually don't need these, we only match one way
                foreach ((int x, int y) in EdgeRange(x2, y2, x3, y3)) right = (right << 1) | (data[y][x] == '#' ? 1 : 0);
                foreach ((int x, int y) in EdgeRange(x3, y3, x4, y4)) bottom = (bottom << 1) | (data[y][x] == '#' ? 1 : 0);
                //foreach ((int x, int y) in EdgeRange(x4, y4, x1, y1)) left = (left << 1) | (data[y][x] == '#' ? 1 : 0);

                foreach ((int x, int y) in EdgeRange(x1, y1, x2, y2).Reverse()) top_match = (top_match << 1) | (data[y][x] == '#' ? 1 : 0);
                //foreach ((int x, int y) in EdgeRange(x2, y2, x3, y3).Reverse()) right_match = (right_match << 1) | (data[y][x] == '#' ? 1 : 0);
                //foreach ((int x, int y) in EdgeRange(x3, y3, x4, y4).Reverse()) bottom_match = (bottom_match << 1) | (data[y][x] == '#' ? 1 : 0);
                foreach ((int x, int y) in EdgeRange(x4, y4, x1, y1).Reverse()) left_match = (left_match << 1) | (data[y][x] == '#' ? 1 : 0);
            }
        }

        public class Tile {
            public long id;
            public string[] data;
            public bool used;

            public TileEdges[] orientations;

            public Tile(int id, string[] data) {
                this.id = id;
                this.data = data;
                orientations = new TileEdges[8] {
                    new TileEdges(data, 0, 0, 9, 0, 9, 9, 0, 9), 
                    new TileEdges(data, 9, 0, 9, 9, 0, 9, 0, 0),
                    new TileEdges(data, 9, 9, 0, 9, 0, 0, 9, 0),
                    new TileEdges(data, 0, 9, 0, 0, 9, 0, 9, 9),
                
                    new TileEdges(data, 9, 0, 0, 0, 0, 9, 9, 9),
                    new TileEdges(data, 9, 9, 0, 9, 0, 0, 0, 9),
                    new TileEdges(data, 0, 9, 9, 9, 9, 0, 0, 0),
                    new TileEdges(data, 0, 0, 0, 9, 9, 9, 9, 0)
                };
            }

            public static Tile CreateTile(string input) {
                var input_lines = input.Split("\r\n");
                return new Tile(int.Parse(input_lines[0].Substring(5, input_lines[0].Length - 6)), input_lines.Skip(1).ToArray());
            }

            internal bool ReadPix(int x, int y, int o) {
                if (o == 0) return data[0 + y][0 + x] == '#';
                if (o == 1) return data[0 + x][9 - y] == '#'; // rot 90
                if (o == 2) return data[9 - y][9 - x] == '#'; // rot 180
                if (o == 3) return data[9 - x][0 + y] == '#'; // rot 270
                
                if (o == 4) return data[0 + y][9 - x] == '#'; // flip
                if (o == 5) return data[9 - x][9 - y] == '#'; // flip + rot 90
                if (o == 6) return data[9 - y][0 + x] == '#'; // flip + rot 180
                if (o == 7) return data[0 + x][0 + y] == '#'; // flip + rot 270

                throw new Exception("wtf o");
            }
        }

        public static void Day20() {
            Console.WriteLine("Day20: ");
            Stopwatch sw = Stopwatch.StartNew();
            var tiles = File.ReadAllText("Aoc2020\\Day20\\input.txt").Split("\r\n\r\n").Select(Tile.CreateTile).ToArray();
            
            // part 1
            int imgsize = (int)Math.Sqrt(tiles.Length);
            int[,] image = new int[imgsize, imgsize];
            bool ok = BuildImage(tiles, imgsize, 0, 0, image);

            //for (int y = 0; y < imgsize; y++) {
            //    for (int x = 0; x < imgsize; x++)
            //        Console.Write($"{tiles[image[x, y] / 8].id}({image[x, y] % 8}) ");
            //    Console.WriteLine();
            //}
            sw.Stop();
            Console.WriteLine($"{ok}: {tiles[image[0, 0] / 8].id * tiles[image[imgsize - 1, 0] / 8].id * tiles[image[0, imgsize - 1] / 8].id * tiles[image[imgsize - 1, imgsize - 1] / 8].id} in {sw.ElapsedMilliseconds}ms");

            // part 2
            sw.Restart();
            int fullsize = imgsize * 8;
            int[,] full = new int[fullsize, fullsize];
            int total_pix = 0;
            for(int y = 0; y < fullsize; y++) {
                for(int x = 0; x < fullsize; x++) {
                    full[x, y] = tiles[image[x / 8, y / 8] / 8].ReadPix(x % 8 + 1, y % 8 + 1, image[x / 8, y / 8] % 8) ? 1 : 0;
                    total_pix += full[x, y];
                }
            }

            // find the monster
            string[] monster = new string[] {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };

            int ReadFull(int x, int y, int o) {
                int f = fullsize - 1;
                if (o == 0) return full[0 + x, 0 + y];
                if (o == 1) return full[f - y, 0 + x]; // rot 90
                if (o == 2) return full[f - x, f - y]; // rot 180
                if (o == 3) return full[0 + y, f - x]; // rot 270

                if (o == 4) return full[f - x, 0 + y]; // flip
                if (o == 5) return full[f - y, f - x]; // flip + rot 90
                if (o == 6) return full[0 + x, f - y]; // flip + rot 180
                if (o == 7) return full[0 + y, 0 + x]; // flip + rot 270
                throw new Exception("wtf o");
            }

            int total_monsters = 0;
            for(int o = 0; o < 8; o++) {
                for (int fy = 0; fy < fullsize - monster.Length; fy++) {
                    for (int fx = 0; fx < fullsize - monster[0].Length; fx++) {
                        int match = 0;
                        for (int y = 0; y < monster.Length; y++) {
                            for (int x = 0; x < monster[0].Length; x++) {
                                if (monster[y][x] == '#' && ReadFull(fx + x, fy + y, o) != 0) {
                                    match++;
                                }
                            }
                        }
                        if (match == 15)
                            total_monsters++;
                    }
                }
                //Console.WriteLine($"{o}: {total_monsters} monsters");
            }
            int total_water = total_pix - total_monsters * 15;
            sw.Stop();

            Console.WriteLine($"{total_monsters} monsters, with {total_water} water in {sw.ElapsedMilliseconds}ms");
        }


        private static bool BuildImage(Tile[] tiles, int imgsize, int x, int y, int[,] img) {
            // find a tile for x, y
            if (x == imgsize)
                return true;
            int nexty = y + 1;
            int nextx = x;
            if (nexty == imgsize) {
                nexty = 0;
                nextx++;
            }
            for (int t = 0; t < tiles.Length; t++) {
                if (!tiles[t].used) {
                    tiles[t].used = true;
                    for (int o = 0; o < 8; o++) {
                        if (x > 0 && tiles[img[x - 1, y] / 8].orientations[img[x - 1, y] % 8].right != tiles[t].orientations[o].left_match) continue;
                        if (y > 0 && tiles[img[x, y - 1] / 8].orientations[img[x, y - 1] % 8].bottom != tiles[t].orientations[o].top_match) continue;
                        //bool match = true;
                        //if (x > 0) {
                        //    ref Tile t2 = ref tiles[img[x - 1, y] / 8];
                        //    int o2 = img[x - 1, y] % 8;
                        //    for (int i = 0; i < 10 && match; i++) match &= t2.ReadPix(9, i, o2) == tiles[t].ReadPix(0, i, o);
                        //}
                        //if (y > 0) {
                        //    ref Tile t2 = ref tiles[img[x, y - 1] / 8];
                        //    int o2 = img[x, y - 1] % 8;
                        //    for (int i = 0; i < 10 && match; i++) match &= t2.ReadPix(i, 9, o2) == tiles[t].ReadPix(i, 0, o);
                        //}
                        //if (!match)
                        //    continue;
                        img[x, y] = t * 8 + o;
                        if (BuildImage(tiles, imgsize, nextx, nexty, img))
                            return true;
                    }
                    tiles[t].used = false;
                }
            }
            return false;
        }
    }
}
