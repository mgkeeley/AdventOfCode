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
        class Food {
            public HashSet<string> ingredients;
            public string[] allergens;

            public Food(string input) {
                //mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
                var m = Regex.Match(input, "^(([a-z]+) )+\\(contains (([a-z]+),? ?)+\\)$");
                ingredients = new HashSet<string>(m.Groups[2].Captures.Select(c => c.Value));
                allergens = m.Groups[4].Captures.Select(c => c.Value).ToArray();
            }
        }

        public static void Day21() {
            Console.WriteLine("Day21: ");
            Stopwatch sw = Stopwatch.StartNew();
            var input = File.ReadAllLines("Aoc2020\\Day21\\input.txt");
            var foods = input.Select(i => new Food(i)).ToArray();

            // part 1
            Dictionary<string, HashSet<string>> allergens = new();
            Dictionary<string, int> ingredients = new();
            foreach (var f in foods) {
                foreach (var a in f.allergens) {
                    if (allergens.TryGetValue(a, out HashSet<string> possible_ingredients))
                        possible_ingredients.IntersectWith(f.ingredients);
                    else
                        allergens.Add(a, new HashSet<string>(f.ingredients));
                }
                foreach(var i in f.ingredients) {
                    ingredients.TryGetValue(i, out int count);
                    ingredients[i] = count + 1;
                }
            }

            var ordered = allergens.OrderBy(kv => kv.Value.Count).ToArray();
            for(int a = 0; a < ordered.Length; a++) {
                if (ordered[a].Value.Count == 1) { // we have a match for allergen -> ingredient, remove from other possibles
                    var i = ordered[a].Value.First();
                    for (int a2 = 0; a2 < ordered.Length; a2++) {
                        if (a != a2)
                            ordered[a2].Value.Remove(i);
                    }
                }
            }
            int total = ingredients.Values.Sum();
            foreach (var a in ordered.SelectMany(kv => kv.Value))
                total -= ingredients[a];
            sw.Stop();
            Console.WriteLine($"{total} in {sw.ElapsedMilliseconds}ms");

            // part 2
            sw.Restart();
            var list = string.Join(",", ordered.OrderBy(kv => kv.Key).Select(kv => kv.Value.First()));

            sw.Stop();
            Console.WriteLine($"{list} in {sw.ElapsedMilliseconds}ms");
        }
    }
}
