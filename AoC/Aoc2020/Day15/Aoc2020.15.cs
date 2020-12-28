using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC
{
    public partial class Aoc2020 {
        struct Spoken {
            public int time, prev_time;
        }

        public static void Day15() {
            Console.WriteLine("Day15: ");
            var input = File.ReadAllText("Aoc2020\\Day15\\test.txt").Split(',').Select(int.Parse).ToArray();
            // part 1
            int lastNum = PlaySpokenGame2(2020);
            Console.WriteLine(lastNum);
            // part 2
            Stopwatch sw = Stopwatch.StartNew();
            lastNum = PlaySpokenGame2(30000000);
            sw.Stop();
            Console.WriteLine($"{lastNum} in {sw.ElapsedMilliseconds}ms");


            int PlaySpokenGame(int rounds) {
                Spoken[] lastSpoken = new Spoken[rounds];
                int i, lastNum = -1;
                for (i = 0; i < input.Length; i++) {
                    lastNum = input[i];
                    lastSpoken[lastNum].time = i + 1;
                }
                for (; i < rounds; i++) {
                    if (lastSpoken[lastNum].time == 0)
                        lastNum = input[i % input.Length];
                    else {
                        if (lastSpoken[lastNum].prev_time == 0)
                            lastNum = 0;
                        else
                            lastNum = lastSpoken[lastNum].time - lastSpoken[lastNum].prev_time;
                    }
                    lastSpoken[lastNum].prev_time = lastSpoken[lastNum].time;
                    lastSpoken[lastNum].time = i + 1;
                }

                return lastNum;
            }

            int PlaySpokenGame2(int rounds) {
                int[] lastSpoken = new int[rounds];
                int i, prev = -1, next = -1;
                for (i = 0; i < input.Length; i++) {
                    prev = input[i];
                    lastSpoken[prev] = i + 1;
                }
                next = 0;
                for (; i < rounds; i++) {
                    if (lastSpoken[prev] == 0)
                        next = 0;
                    else {
                        next = i + 1 - lastSpoken[prev];
                        lastSpoken[prev] = i + 1;
                    }
                }

                return next;
            }
        }
    }
}
