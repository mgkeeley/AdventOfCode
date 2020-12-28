using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC {
    public partial class Aoc2020 {
        class TicketRule {
            public string name;
            public int from1, to1, from2, to2;

            public TicketRule(string rule) {
                // zone: 39-786 or 807-969
                var m = Regex.Match(rule, "^(.*): ([0-9]+)-([0-9]+) or ([0-9]+)-([0-9]+)$");

                name = m.Groups[1].Value;
                from1 = int.Parse(m.Groups[2].Value);
                to1 = int.Parse(m.Groups[3].Value);
                from2 = int.Parse(m.Groups[4].Value);
                to2 = int.Parse(m.Groups[5].Value);
            }

            public bool IsValid(int field) {
                return (field >= from1 && field <= to1) || (field >= from2 && field <= to2);
            }
        }

        public static void Day16() {
            Console.WriteLine("Day16: ");
            var rules = File.ReadAllLines("Aoc2020\\Day16\\rules.txt").Select(rule => new TicketRule(rule)).ToArray();
            var tickets = File.ReadAllLines("Aoc2020\\Day16\\tickets.txt").Select(s => s.Split(',').Select(int.Parse).ToArray()).ToArray();
            var myticket = File.ReadAllText("Aoc2020\\Day16\\myticket.txt").Split(',').Select(int.Parse).ToArray();

            // part one
            List<int[]> validTickets = new();
            long error_code = 0;
            foreach (var ticket in tickets) {
                bool valid = true;
                foreach (var field in ticket) {
                    if (!rules.Any(rule => rule.IsValid(field))) {
                        valid = false;
                        error_code += field;
                    }
                }
                if (valid)
                    validTickets.Add(ticket);
            }
            Console.WriteLine(error_code);

            // part two
            HashSet<TicketRule>[] candidates = new HashSet<TicketRule>[tickets[0].Length]; // set of rules valid for each field
            for (int f = 0; f < tickets[0].Length; f++) 
                candidates[f] = new HashSet<TicketRule>(rules);
            foreach (var ticket in validTickets) {
                for (int f = 0; f < ticket.Length; f++) {
                    foreach (var rule in candidates[f].ToArray())
                        if (!rule.IsValid(ticket[f]))
                            candidates[f].Remove(rule);
                }
            }
            foreach(var c in candidates.OrderBy(c => c.Count)) {
                if (c.Count == 1) {
                    foreach(var c2 in candidates) {
                        if (c != c2)
                            c2.Remove(c.First());
                    }
                } else {
                    throw new Exception("oh no!");
                }
            }
            long prod = 1;
            for(int f = 0; f < candidates.Length; f++) {
                var rule = candidates[f].First();
                if (rule.name.StartsWith("departure"))
                    prod *= myticket[f];
                //Console.WriteLine($"Rule {rule.name}: {candidates[f].Count}");
            }
            Console.WriteLine(prod);
        }
    }
}
