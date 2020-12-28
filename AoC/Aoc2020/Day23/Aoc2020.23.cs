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

        public class Cup {
            public int label;
            public Cup next;
        }


        public static void Day23() {
            Console.WriteLine("Day23: ");
            Stopwatch sw = Stopwatch.StartNew();
            int[] cups = { 3, 9, 4, 6, 1, 8, 5, 2, 7 };
            //int[] cups = { 3, 8, 9, 1, 2, 5, 4, 6, 7 };
            Cup[] cupMap;

            Cup BuildCupCircle(int[] cups, int len) {
                Stopwatch sw = Stopwatch.StartNew();
                Cup head = new Cup { label = cups[0] };
                Cup cur = head;
                cupMap = new Cup[len + 1];
                cupMap[cur.label] = cur;
                for (int i = 1; i < cups.Length; i++) {
                    cur.next = new Cup { label = cups[i] };
                    cur = cur.next;
                    cupMap[cur.label] = cur;
                }
                for (int i = cups.Length; i < len; i++) {
                    cur.next = new Cup { label = i + 1 };
                    cur = cur.next;
                    cupMap[cur.label] = cur;
                }
                cur.next = head; // create loop
                Console.WriteLine($"Build Cups: {sw.ElapsedMilliseconds}ms");
                return head;
            }

            void PlayCups(int len, int iterations) {
                Cup cur = BuildCupCircle(cups, len);
                Cup a, b, c;
                for (int i = iterations; i > 0; i--) {
                    a = cur.next;
                    b = a.next;
                    c = b.next;
                    cur.next = c.next; // extract a,b,c from loop
                    int destLabel = cur.label - 1;
                    if (destLabel == 0)
                        destLabel = cupMap.Length - 1;
                    Cup dest = cupMap[destLabel];
                    while (dest == a || dest == b || dest == c) {
                        destLabel--;
                        if (destLabel == 0)
                            destLabel = cupMap.Length - 1;
                        dest = cupMap[destLabel];
                    }
                    c.next = dest.next;
                    dest.next = a;
                    cur = cur.next;
                }
            }

            PlayCups(cups.Length, 100);
            Cup cur = cupMap[1].next;
            for (int j = 0; j < cups.Length - 1; j++) {
                Console.Write(cur.label);
                cur = cur.next;
            }
            sw.Stop();
            Console.WriteLine($" in {sw.ElapsedMilliseconds}ms");

            // part 2
            sw.Restart();

            PlayCups(1000000, 10000000);
            cur = cupMap[1];

            sw.Stop();
            Console.WriteLine($"{(long)cur.next.label * cur.next.next.label} in {sw.ElapsedMilliseconds}ms");
        }

        public static void Day23b() {
            Console.WriteLine("Day23: ");
            Stopwatch sw = Stopwatch.StartNew();
            int[] cups = { 3, 9, 4, 6, 1, 8, 5, 2, 7 };
            //int[] cups = { 3, 8, 9, 1, 2, 5, 4, 6, 7 };
            int[] cupNext;

            int BuildCupCircle(int[] cups, int len) {
                Stopwatch sw = Stopwatch.StartNew();
                int head = cups[0]; 
                int cur = head;
                cupNext = new int[len + 1];
                for (int i = 1; i < cups.Length; i++) {
                    cupNext[cur] = cups[i];
                    cur = cups[i];
                }
                for (int i = cups.Length; i < len; i++) {
                    cupNext[cur] = i + 1;
                    cur = i + 1;
                }
                cupNext[cur] = head; // create loop
                Console.WriteLine($"Build Cups: {sw.ElapsedMilliseconds}ms");
                return head;
            }

            void PlayCups(int len, int iterations) {
                int cur = BuildCupCircle(cups, len);
                int a, b, c;
                for (int i = iterations; i > 0; i--) {
                    a = cupNext[cur];
                    b = cupNext[a];
                    c = cupNext[b];
                    cupNext[cur] = cupNext[c]; // extract a,b,c from loop
                    int dest = cur - 1;
                    if (dest == 0)
                        dest = cupNext.Length - 1;
                    while (dest == a || dest == b || dest == c) {
                        dest--;
                        if (dest == 0)
                            dest = cupNext.Length - 1;
                    }
                    cupNext[c] = cupNext[dest];
                    cupNext[dest] = a;
                    cur = cupNext[cur];
                }
            }

            PlayCups(cups.Length, 100);
            int cur = cupNext[1];
            for (int j = 0; j < cups.Length - 1; j++) {
                Console.Write(cur);
                cur = cupNext[cur];
            }
            sw.Stop();
            Console.WriteLine($" in {sw.ElapsedMilliseconds}ms");

            // part 2
            sw.Restart();

            PlayCups(1000000, 10000000);
            long a = cupNext[1];
            long b = cupNext[a];

            sw.Stop();
            Console.WriteLine($"{a * b} in {sw.ElapsedMilliseconds}ms");
        }
    }
}
