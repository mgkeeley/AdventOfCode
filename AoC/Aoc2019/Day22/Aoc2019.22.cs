using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AoC
{
    public partial class Aoc2019 {
        struct DealRule {
            public int mult;
            public int add;
        }

        public static void Day22() {
            Console.WriteLine("Day22: ");
            Stopwatch sw = Stopwatch.StartNew();

            string[] input = File.ReadAllLines("Aoc2019\\Day22\\input.txt");
            DealRule[] rules = new DealRule[input.Length];
            // deal with increment 34
            // deal into new stack
            // cut 1712

            // use modulo algebra, each rule is of the form pos = (pos * mult + add) % deckSize
            for (int i = 0; i < input.Length; i++) {
                if (input[i] == "deal into new stack") {
                    rules[i].mult = -1;
                    rules[i].add = -1;
                } else if (input[i].StartsWith("cut")) {
                    rules[i].mult = 1;
                    rules[i].add = -int.Parse(input[i].Substring(4));
                } else if (input[i].StartsWith("deal with increment")) {
                    rules[i].mult = int.Parse(input[i].Substring(20));
                    rules[i].add = 0;
                }
            }
            long deckSize = 10007;
            // we can combine the rules into one rule using modulo rules
            // a1 = a0 * p + q    (mod n)
            // a2 = a1 * r + s    (mod n)
            // a2 = (a0 * p + q) * r + s   (mod n)
            //    = a0 * p * r + q * r + s (mod n)
            //  so  p => p * r
            //      q => q * r + s

            // ab mod n = [(a mod n)(b mod n)] mod n

            long mult = 1;
            long add = 0;
            foreach (var rule in rules) {
                mult = (mult % deckSize) * rule.mult;
                add = (add * rule.mult) % deckSize + rule.add;
            }
            if (add < 0)
                add += deckSize; // ensure it's positive

            long pos = 2019;
            pos = (pos * mult + add) % deckSize;

            sw.Stop();
            Console.WriteLine($"{pos} in {sw.ElapsedMilliseconds}ms");

            // part2
            sw.Restart();
            pos = Part2();
            long Part2() {
                long deckSize = 119315717514047; // prime
                long count = 101741582076661; // prime

                // recalculate with new decksize
                BigInteger mult = 1;
                BigInteger add = 0;
                foreach (var rule in rules) {
                    mult = (mult % deckSize) * rule.mult;
                    add = (add * rule.mult) % deckSize + rule.add;
                }
                if (add < 0)
                    add += deckSize; // ensure it's positive

                // now we need to go backwards, we have pos and need card
                // pos = (card * mult + add) % decksize
                // pos - add = card * mult   (mod decksize)
                // pos * 1/mult - add * 1/mult = card   (mod decksize)
                // we need to find the multicative inverse of mult (mod decksize)...use the euclidean algorithm
                mult = ModInverse((long)mult, deckSize);
                add = ((-add + deckSize) * mult) % deckSize;

                BigInteger pos = 2020;
                // go through bits of count, applying 2^i cycles when each bit is set
                for (int i = 0; i < 64; i++) {
                    if ((count & (1L << i)) != 0) 
                        pos = (pos * mult + add) % deckSize;
                    // apply current mult/add twice -> doubles cycles covered
                    // p1 = p0 * mult + add  (mod deckSize)
                    // p2 = p1 * mult + add  (mod deckSize)
                    //    = (p0 * mult + add) * mult + add
                    //    = p0 * mult^2 + add*mult + add
                    add = (add * mult + add) % deckSize;
                    mult = (mult * mult) % deckSize;
                }
                return (long)pos;
            }

            long ModInverse(long a, long n) {
                long t = 0, newt = 1;
                long r = n, newr = a;
                while (newr != 0) {
                    long quotient = r / newr;
                    long t2 = newt;
                    newt = t - quotient * newt;
                    t = t2;
                    long r2 = newr;
                    newr = r - quotient * newr;
                    r = r2;
                }
                if (r > 1)
                    throw new Exception("a is not invertible");
                if (t< 0)
                    t += n;
                return t;
            }
           
            Console.WriteLine($"{pos} in {sw.ElapsedMilliseconds}ms");
        }
    }
}
