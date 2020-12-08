using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC
{
    public partial class Aoc2020
    {
        public class BagRule {
            private static readonly Regex parser = new Regex("^([a-z ]+) bags contain (([0-9]+) ([a-z ]+) bags?[, .]+)*|no other bags.$");

            public string Color { get; set; }
            public Dictionary<string, int> ContentRules { get; set; }

            public BagRule(string rule) {
                var m = parser.Match(rule);
                Color = m.Groups[1].Value;
                ContentRules = new();
                for(int i = 0; i < m.Groups[3].Captures.Count; i++) {
                    ContentRules.Add(m.Groups[4].Captures[i].Value, int.Parse(m.Groups[3].Captures[i].Value));
                }
            }
        }

        public static void Day7() {
            Console.WriteLine("Day7: ");
            Dictionary<string, BagRule> rules = File.ReadAllLines("Aoc2020\\Day07\\input.txt").Select(rule => new BagRule(rule)).ToDictionary(rule => rule.Color);

            // part 1
            bool CanContain(string outercolor, string color) {
                BagRule outerBag = rules[outercolor];
                if (outerBag.ContentRules.ContainsKey(color))
                    return true;
                foreach (var innerBag in outerBag.ContentRules) {
                    if (CanContain(innerBag.Key, color))
                        return true;
                }
                return false;
            }

            int count = 0;
            foreach(var bag in rules.Keys) {
                if (CanContain(bag, "shiny gold"))
                    count++;
            }
            Console.WriteLine(count);

            // part 2
            int CountContents(string color) {
                BagRule bag = rules[color];
                int count = 0;
                foreach (var innerBag in bag.ContentRules) {
                    count += innerBag.Value * CountContents(innerBag.Key) + innerBag.Value;
                }
                return count;
            }

            Console.WriteLine(CountContents("shiny gold"));
        }
    }
}
