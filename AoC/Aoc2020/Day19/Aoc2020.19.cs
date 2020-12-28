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

        public abstract class MsgRule {
            public static Dictionary<int, MsgRule> rules = new();

            public static MsgRule Create(string rule) {
                MsgRule newRule;
                var m = Regex.Match(rule, "^([0-9]+): ([0-9]+) ([0-9]+) \\| ([0-9]+) ([0-9]+)$"); // 52: 102 91 | 129 20
                if (m.Success) {
                    int id = int.Parse(m.Groups[1].Value);
                    MsgRule rule_a = new ConcatRule(10000 + id, int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
                    MsgRule rule_b = new ConcatRule(20000 + id, int.Parse(m.Groups[4].Value), int.Parse(m.Groups[5].Value));
                    newRule = new OrRule(id, rule_a.id, rule_b.id);
                    rules.Add(rule_a.id, rule_a);
                    rules.Add(rule_b.id, rule_b);
                } else {
                    m = Regex.Match(rule, "^([0-9]+): ([0-9]+) ([0-9]+)$"); // 52: 102 91
                    if (m.Success) {
                        int id = int.Parse(m.Groups[1].Value);
                        newRule = new ConcatRule(id, int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value));
                    } else {
                        m = Regex.Match(rule, "^([0-9]+): ([0-9]+)$"); // 52: 102
                        if (m.Success) {
                            int id = int.Parse(m.Groups[1].Value);
                            newRule = new RefRule(id, int.Parse(m.Groups[2].Value));
                        } else {
                            m = Regex.Match(rule, "^([0-9]+): \"([a-z])\"$"); // 52: 102
                            if (m.Success) {
                                int id = int.Parse(m.Groups[1].Value);
                                newRule = new CharRule(id, m.Groups[2].Value[0]);
                            } else {
                                m = Regex.Match(rule, "^([0-9]+): ([0-9]+) \\| ([0-9]+)$"); // 52: 102 91 | 129 20
                                if (m.Success) {
                                    int id = int.Parse(m.Groups[1].Value);
                                    MsgRule rule_a = new RefRule(10000 + id, int.Parse(m.Groups[2].Value));
                                    MsgRule rule_b = new RefRule(20000 + id, int.Parse(m.Groups[3].Value));
                                    newRule = new OrRule(id, rule_a.id, rule_b.id);
                                    rules.Add(rule_a.id, rule_a);
                                    rules.Add(rule_b.id, rule_b);
                                } else {
                                    throw new Exception("oh no!");
                                }
                            }
                        }
                    }
                }
                rules.Add(newRule.id, newRule);
                return newRule;
            }

            public int id;

            protected MsgRule(int id) {
                this.id = id;
            }

            public abstract bool Match(string match, ref int pos);

            public bool Matches(string msg) {
                int pos = 0;
                bool match = Match(msg, ref pos);
                return match && pos == msg.Length;
            }

            public virtual string ToStringTop() {
                return ToString();
            }

            public abstract HashSet<string> GenAll(int maxMsgLen);
        }

        public class RefRule : MsgRule {
            private readonly int a;

            public RefRule(int id, int a) : base(id) {
                this.a = a;
            }

            public override bool Match(string match, ref int pos) {
                return rules[a].Match(match, ref pos);
            }

            public override string ToString() {
                return rules[a].ToString();
            }

            public override HashSet<string> GenAll(int maxMsgLen) {
                return rules[a].GenAll(maxMsgLen);
            }
        }

        public class ConcatRule : MsgRule {
            private readonly int a;
            private readonly int b;

            public ConcatRule(int id, int a, int b) : base(id) {
                this.a = a;
                this.b = b;
            }

            public override bool Match(string match, ref int pos) {
                return rules[a].Match(match, ref pos) && rules[b].Match(match, ref pos);
            }

            public override HashSet<string> GenAll(int maxMsgLen) {
                HashSet<string> optsa = rules[a].GenAll(maxMsgLen);
                HashSet<string> optsb = rules[b].GenAll(maxMsgLen);
                HashSet<string> gen = new();
                foreach (var opta in optsa) {
                    foreach (var optb in optsb) {
                        if (opta.Length + optb.Length <= maxMsgLen)
                            gen.Add(opta + optb);
                    }
                }
                return gen;
            }

            public override string ToString() {
                return $"{rules[a]}{rules[b]}";
            }
        }

        public class Concat3Rule : MsgRule {
            private readonly int a;
            private readonly int b;
            private readonly int c;

            public Concat3Rule(int id, int a, int b, int c) : base(id) {
                this.a = a;
                this.b = b;
                this.c = c;
            }

            public override bool Match(string match, ref int pos) {
                return rules[a].Match(match, ref pos) && rules[b].Match(match, ref pos) && rules[c].Match(match, ref pos);
            }

            public override HashSet<string> GenAll(int maxMsgLen) {
                HashSet<string> optsa = rules[a].GenAll(maxMsgLen);
                HashSet<string> optsb = rules[b].GenAll(maxMsgLen);
                HashSet<string> optsc = rules[c].GenAll(maxMsgLen);
                HashSet<string> gen = new();
                foreach (var opta in optsa) {
                    foreach (var optb in optsb) {
                        foreach (var optc in optsc) {
                            if (opta.Length + optb.Length + optc.Length <= maxMsgLen)
                                gen.Add(opta + optb + optc);
                        }
                    }
                }
                return gen;
            }

            public override string ToString() {
                return $"{rules[a]}{rules[b]}{rules[c]}";
            }
        }

        public class OrRule : MsgRule {
            private readonly int a;
            private readonly int b;

            public OrRule(int id, int a, int b) : base(id) {
                this.a = a;
                this.b = b;
            }

            public override bool Match(string match, ref int pos) {
                int pos2 = pos;
                if (rules[a].Match(match, ref pos))
                    return true;
                pos = pos2;
                return rules[b].Match(match, ref pos);
            }

            public override HashSet<string> GenAll(int maxMsgLen) {
                HashSet<string> optsa = rules[a].GenAll(maxMsgLen);
                int minLength = optsa.Select(m => m.Length).Min();
                foreach(var optb in rules[b].GenAll(maxMsgLen - minLength))
                    optsa.Add(optb);
                return optsa;
            }

            public override string ToString() {
                if (id == 8 || id == 11)
                    return id.ToString();
                return ToStringTop();
            }

            public override string ToStringTop() { 
                return $"({rules[a]}|{rules[b]})";
            }
        }

        public class CharRule : MsgRule {
            private readonly char a;

            public CharRule(int id, char a) : base(id) {
                this.a = a;
            }

            public override bool Match(string match, ref int pos) {
                if (pos >= match.Length)
                    return false;
                return match[pos++] == a;
            }

            public override HashSet<string> GenAll(int maxMsgLen) {
                return new HashSet<string> { new string(a, 1) };
            }

            public override string ToString() {
                return $"{a}";
            }
        }

        public static void Day19() {
            Console.WriteLine("Day19: ");
            var rules = File.ReadAllLines("Aoc2020\\Day19\\input.txt").Select(MsgRule.Create).ToArray();
            var messages = File.ReadAllLines("Aoc2020\\Day19\\messages.txt");

            // part 1
            int count = 0;
            foreach(var msg in messages) {
                if (MsgRule.rules[0].Matches(msg))
                    count++;
            }
            Console.WriteLine(count);

            // part 2
            // 0: 8 11
            // 8: 42 | 42 8
            // 11: 42 31 | 42 11 31
            Stopwatch sw = Stopwatch.StartNew();
            int maxMsgLen = messages.Select(m => m.Length).Max();
            HashSet<string> msg42 = MsgRule.rules[42].GenAll(maxMsgLen);
            HashSet<string> msg31 = MsgRule.rules[31].GenAll(maxMsgLen);
            // ooo nice, these all have exactly the same length :-)
            int partLen = msg42.Select(m => m.Length).Concat(msg31.Select(m => m.Length)).Distinct().First();

            // custom matcher: rule 0 is rule8, followed by rule11.
            // this expands to rule42 * n, then rule42 * m then rule31 * m
            // rule42 and rule31 always take the same number of characters (partLen), so we can break the message by that length
            // n (number of repeats of rule8) can range from 1.. (num parts less one repeat of rule 11 (2 parts)).
            // m (number of repeats of rule11) can be derived from n and the number of parts, 
            count = 0;
            foreach (var msg in messages) {
                for (int r8 = 1; r8 <= (msg.Length / partLen) - 2; r8++) {
                    int pos = 0;
                    bool match = true;
                    for (int i = 0; i < r8 && match; i++, pos += partLen)
                        match &= msg42.Contains(msg.Substring(pos, partLen));
                    int r11 = (msg.Length / partLen - r8) / 2;
                    for (int i = 0; i < r11 && match; i++, pos += partLen)
                        match &= msg42.Contains(msg.Substring(pos, partLen));
                    for (int i = 0; i < r11 && match; i++, pos += partLen)
                        match &= msg31.Contains(msg.Substring(pos, partLen));
                    if (match && pos == msg.Length) {
                        //Console.WriteLine($"Match {msg}");
                        count++;
                        break;
                    }
                }
            }
            sw.Stop();
            Console.WriteLine($"{count} in {sw.ElapsedMilliseconds}ms");
        }
    }
}
