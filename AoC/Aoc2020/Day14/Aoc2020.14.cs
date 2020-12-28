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
        public static void Day14() {
            Console.WriteLine("Day14: ");
            var input = File.ReadAllLines("Aoc2020\\Day14\\input.txt");

            long mask1 = 0;
            long mask0 = 0;
            Dictionary<long, long> mem = new();
            foreach(var ins in input) {
                if (ins.StartsWith("mask = ")) {
                    mask1 = 0;
                    mask0 = 0;
                    for (int i = 7; i < ins.Length; i++) {
                        mask1 *= 2;
                        mask0 *= 2;
                        if (ins[i] == '1')
                            mask1++;
                        else if (ins[i] == '0')
                            mask0++;
                    }
                }
                else {
                    var m = Regex.Match(ins, "mem\\[([0-9]+)\\] = ([0-9]+)$");
                    int addr = int.Parse(m.Groups[1].Value);
                    long val = long.Parse(m.Groups[2].Value);
                    val |= mask1;
                    val &= ~mask0;
                    mem[addr] = val;
                }
            }

            long sum = mem.Sum(m => m.Value);
            Console.WriteLine(sum);

            // part 2
            mem = new();
            long maskX = 0;
            int numXBits = 0;
            List<int> toggleBits = new();
            foreach (var ins in input) {
                if (ins.StartsWith("mask = ")) {
                    mask1 = 0;
                    maskX = 0;
                    toggleBits.Clear();
                    for (int i = 7; i < ins.Length; i++) {
                        mask1 *= 2;
                        maskX *= 2;
                        if (ins[i] == '1')
                            mask1++;
                        else if (ins[i] == 'X') {
                            maskX++;
                            toggleBits.Add(ins.Length - i - 1);
                        }
                    }
                    numXBits = toggleBits.Count;
                } else {
                    var m = Regex.Match(ins, "mem\\[([0-9]+)\\] = ([0-9]+)$");
                    long addr = long.Parse(m.Groups[1].Value);
                    long val = long.Parse(m.Groups[2].Value);
                    addr |= mask1;
                    addr &= ~maskX;
                    for (int i = (1 << numXBits) - 1; i >= 0; i--) {
                        long addr2 = addr;
                        for (int t = numXBits - 1; t >= 0; t--) {
                            if ((i & (1 << t)) != 0)
                                addr2 |= 1L << toggleBits[t];
                        }
                        mem[addr2] = val;
                    }
                }
            }
            sum = mem.Sum(m => m.Value);
            Console.WriteLine(sum);
        }
    }
}
