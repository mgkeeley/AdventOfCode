using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC
{
    public partial class Aoc2019 {

        public class Reaction {
            public string Chemical { get; }
            public int Amount { get; }

            public Dictionary<string, int> Ingredients { get; }

            public Reaction(string formula) {
                // 2 PQTVS, 9 VMNJ => 9 TXZCM
                Match m = Regex.Match(formula, "^(([0-9]+) ([A-Z]+)(\\, )?)+ => ([0-9]+) ([A-Z]+)$");
                Amount = int.Parse(m.Groups[5].Value);
                Chemical = m.Groups[6].Value;
                Ingredients = new();
                for(int i = 0; i < m.Groups[2].Captures.Count; i++) 
                    Ingredients.Add(m.Groups[3].Captures[i].Value, int.Parse(m.Groups[2].Captures[i].Value));
            }
        }

        public class Factory {
            private Dictionary<string, long> Excess;
            public Dictionary<string, Reaction> Reactions;

            public Factory(Reaction[] reactions) {
                Reactions = reactions.ToDictionary(r => r.Chemical);
                RequiredOre = 0;
                Excess = new Dictionary<string, long>();
            }

            public long RequiredOre { get; private set; }

            public void Produce(long amount, string chemical) {
                if (chemical == "ORE") {
                    RequiredOre += amount;
                    return;
                }
                Reaction reaction = Reactions[chemical];
                Excess.TryGetValue(chemical, out long excess);
                long from_excess = Math.Min(amount, excess);
                amount -= from_excess;
                excess -= from_excess;
                long qty = (amount + reaction.Amount - 1) / reaction.Amount;
                excess += reaction.Amount * qty - amount;
                Excess[chemical] = excess;
                if (qty > 0) {
                    foreach (var ing in reaction.Ingredients)
                        Produce(ing.Value * qty, ing.Key);
                }
            }

            public void Reset() {
                RequiredOre = 0;
                Excess.Clear();
            }
        }

        public static void Day14() {
            Console.WriteLine("Day14: ");

            string[] input = File.ReadAllLines("Aoc2019\\Day14\\input.txt");
            Factory f = new Factory(input.Select(i => new Reaction(i)).ToArray());

            // part one
            f.Produce(1, "FUEL");
            Console.WriteLine(f.RequiredOre);

            // part two
            long min_fuel = 1000000000000 / f.RequiredOre;
            long max_fuel = min_fuel;
            while (f.RequiredOre < 1000000000000) {
                f.Reset();
                f.Produce(max_fuel, "FUEL");
                max_fuel *= 2;
            }
            // binary search
            while (min_fuel < max_fuel - 1) {
                long mid = (min_fuel + max_fuel) / 2;
                f.Reset();
                f.Produce(mid, "FUEL");
                if (f.RequiredOre < 1000000000000)
                    min_fuel = mid;
                else
                    max_fuel = mid;
            }
            Console.WriteLine($"{min_fuel}");
        }
    }
}
