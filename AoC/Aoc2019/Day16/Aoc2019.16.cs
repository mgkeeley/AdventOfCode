using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AoC
{
    public partial class Aoc2019 {
        public static void Day16() {
            Console.WriteLine("Day16: ");

            byte[] signal = File.ReadAllText("Aoc2019\\Day16\\input.txt").Select(c => (byte)(c - '0')).ToArray();

            byte[] input = signal;
            byte[] next;
            for (int phase = 0; phase < 100; phase++) {
                next = new byte[input.Length];
                for (int i = 0; i < input.Length; i++) {
                    next[i] = ApplyFFTRound(input, 0, i + 1);
                }
                input = next;
                //Console.WriteLine($"Phase {phase}: {string.Concat(input)}");
            }
            Console.WriteLine($"{string.Concat(input).Substring(0, 8)}");

            int offset = int.Parse(string.Concat(signal.AsSpan(0, 7).ToArray()));
            input = new byte[signal.Length * 10000];
            for (int i = 0; i < 10000; i++)
                signal.CopyTo(input, i * signal.Length); // this is a bit wasteful, we only need offset..10000*signal.Length in input[]
            // for big offsets (> half input length), we don't need to worry about the positive/negative phases, it's all positive addition
            Stopwatch sw = Stopwatch.StartNew();
            byte[] mod10 = new byte[100];
            for(int x = 0; x < 10; x++) {
                for (int y = 0; y < 10; y++) {
                    mod10[x * 10 + y] = (byte)((x + y) % 10); // slightly faster to precompute x + y mod 10
                }
            }
            next = new byte[input.Length];
            for (int phase = 0; phase < 100; phase++) {
                byte sum = 0;
                for (int i = input.Length - 1; i >= offset; i--) {
                    sum = mod10[sum * 10 + input[i]];
                    next[i] = sum;
                }
                // swap arrays to avoid realloc
                var tmp = input;
                input = next;
                next = tmp;
            }
            sw.Stop();
            Console.WriteLine($"{string.Concat(input.AsSpan(offset, 8).ToArray())} in {sw.ElapsedMilliseconds}ms");
        }

        private static byte ApplyFFTRound(byte[] input, int start, int round) {
            int sum = 0;

            // basemult is [0, 1, 0, -1]
            // we can skip steps with basemult == 0, and group the 1 and -1 segments together.
            int plusstart = round - 1;
            while (plusstart + round < start)
                plusstart += round * 4; // should use maths here!
            
            int negstart = plusstart + round * 2;
            while (plusstart < input.Length) {
                int plusend = Math.Min(plusstart + round, input.Length);
                for (int j = plusstart; j < plusend; j++)
                    sum += input[j];
                int negend = Math.Min(negstart + round, input.Length);
                for (int j = negstart; j < negend; j++) {
                    sum -= input[j];
                }
                plusstart += 4 * round;
                negstart += 4 * round;
            }
            return (byte)(Math.Abs(sum) % 10);
        }
    }
}
