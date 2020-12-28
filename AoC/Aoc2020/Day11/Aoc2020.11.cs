using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2020
    {
        public static void Day11() {
            Console.WriteLine("Day11: ");
            var input = File.ReadAllLines("Aoc2020\\Day11\\input.txt");

            char[,] seats = new char[input[0].Length, input.Length];
            for (int y = 0; y < input.Length; y++) {
                for (int x = 0; x < input[0].Length; x++) {
                    seats[x, y] = input[y][x];
                }
            }
            int maxx = seats.GetLength(0);
            int maxy = seats.GetLength(1);

            // part 1
            //  If a seat is empty (L) and there are no occupied seats adjacent to it, the seat becomes occupied.
            //  If a seat is occupied(#) and four or more seats adjacent to it are also occupied, the seat becomes empty.
            //  Otherwise, the seat's state does not change.

            int count = 0;
            bool changes = true;
            while (changes) {
                seats = ApplyRules(seats, ref changes);
                count++;
                //Console.WriteLine($"Round {count}");
                //for (int y = 0; y < maxy; y++) {
                //    for (int x = 0; x < maxx; x++) {
                //        Console.Write(seats[x, y]);
                //    }
                //    Console.WriteLine();
                //}
            }
            int occ = 0;
            for (int x = 0; x < maxx; x++) {
                for (int y = 0; y < maxy; y++) {
                    if (seats[x, y] == '#')
                        occ++;
                }
            }
            Console.WriteLine(occ);

            // part 2
            //  If a seat is empty (L) and there are no occupied seats adjacent to it, the seat becomes occupied.
            //  If a seat is occupied(#) and four or more seats adjacent to it are also occupied, the seat becomes empty.
            //  Otherwise, the seat's state does not change.

            for (int y = 0; y < input.Length; y++) {
                for (int x = 0; x < input[0].Length; x++) {
                    seats[x, y] = input[y][x];
                }
            }
            changes = true;
            while (changes) {
                seats = ApplyRules2(seats, ref changes);
                count++;
            }
            occ = 0;
            for (int x = 0; x < maxx; x++) {
                for (int y = 0; y < maxy; y++) {
                    if (seats[x, y] == '#')
                        occ++;
                }
            }
            Console.WriteLine(occ);

        }

        private static char[,] ApplyRules(char[,] seats, ref bool changes) {
            char[,] newSeats = (char[,])seats.Clone();
            int maxx = seats.GetLength(0);
            int maxy = seats.GetLength(1);
            changes = false;
            for (int x = 0; x < maxx; x++) {
                for (int y = 0; y < maxy; y++) {
                    int adj = 0;
                    if (x > 0 && seats[x - 1, y] == '#') adj++;
                    if (x < maxx - 1 && seats[x + 1, y] == '#') adj++;
                    if (y > 0 && seats[x, y - 1] == '#') adj++;
                    if (y < maxy - 1 && seats[x, y + 1] == '#') adj++;

                    if (x > 0 && y > 0 && seats[x - 1, y - 1] == '#') adj++;
                    if (x < maxx - 1 && y > 0 && seats[x + 1, y - 1] == '#') adj++;
                    if (x > 0 && y < maxy - 1 && seats[x - 1, y + 1] == '#') adj++;
                    if (x < maxx - 1 && y < maxy - 1 && seats[x + 1, y + 1] == '#') adj++;

                    if (seats[x, y] == 'L' && adj == 0) {
                        newSeats[x, y] = '#';
                        changes = true;
                    } else if (seats[x, y] == '#' && adj >= 4) {
                        newSeats[x, y] = 'L';
                        changes = true;
                    }
                }
            }
            return newSeats;
        }

        private static char[,] ApplyRules2(char[,] seats, ref bool changes) {
            char[,] newSeats = (char[,])seats.Clone();
            int maxx = seats.GetLength(0);
            int maxy = seats.GetLength(1);
            changes = false;
            for (int x = 0; x < maxx; x++) {
                for (int y = 0; y < maxy; y++) {
                    int adj = 0;
                    adj += IsOccupied(seats, x, y, -1, 0);
                    adj += IsOccupied(seats, x, y, +1, 0);
                    adj += IsOccupied(seats, x, y, 0, -1);
                    adj += IsOccupied(seats, x, y, 0, +1);
                    adj += IsOccupied(seats, x, y, -1, -1);
                    adj += IsOccupied(seats, x, y, +1, -1);
                    adj += IsOccupied(seats, x, y, -1, +1);
                    adj += IsOccupied(seats, x, y, +1, +1);
                    if (seats[x, y] == 'L' && adj == 0) {
                        newSeats[x, y] = '#';
                        changes = true;
                    } else if (seats[x, y] == '#' && adj >= 5) {
                        newSeats[x, y] = 'L';
                        changes = true;
                    }
                }
            }
            return newSeats;
        }

        private static int IsOccupied(char[,] seats, int x, int y, int dx, int dy) {
            int maxx = seats.GetLength(0);
            int maxy = seats.GetLength(1);
            while (true) {
                x += dx;
                y += dy;
                if (x < 0 || x >= maxx || y < 0 || y >= maxy)
                    return 0;
                if (seats[x, y] == '#')
                    return 1;
                if (seats[x, y] == 'L')
                    return 0;
            }
            throw new Exception("Whoops!");
        }
    }
}
