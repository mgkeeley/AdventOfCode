using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC {
    public partial class Aoc2020 {

        public enum Mathop {
            Enter,
            Add,
            Multiply
        }

        public static void Day18() {
            Console.WriteLine("Day18: ");
            var input = File.ReadAllLines("Aoc2020\\Day18\\input.txt");

            // part 1
            long sum = 0;
            foreach (var eqn in input) {
                var expr = MathParser.ExprParser.ParsePart1(eqn);
                //int pos = 0;
                //long val = Parse(eqn + " ", ref pos);
                long val = expr.Evaluate();
                sum += val;
            }
            Console.WriteLine(sum);

            sum = 0;
            foreach (var eqn in input) {
                var expr = MathParser.ExprParser.ParsePart2(eqn);
                long val = expr.Evaluate();
                sum += val;
            }
            Console.WriteLine(sum);

            // only works for part 1 - no operator precedence!
            long Parse(string eqn, ref int pos) {
                long val = 0;
                long acc = 0;
                Mathop op = Mathop.Enter;
                while (pos < eqn.Length) {
                    switch (eqn[pos]) {
                        case '(':
                            pos++;
                            val = Parse(eqn, ref pos);
                            break;
                        case ')':
                            if (op == Mathop.Enter)
                                acc = val;
                            else if (op == Mathop.Add)
                                acc = acc + val;
                            else if (op == Mathop.Multiply)
                                acc = acc * val;
                            return acc;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            val *= 10;
                            val += eqn[pos] - '0';
                            break;
                        case ' ':
                            if (op == Mathop.Enter)
                                acc = val;
                            else if (op == Mathop.Add)
                                acc = acc + val;
                            else if (op == Mathop.Multiply)
                                acc = acc * val;
                            val = 0;
                            break;
                        case '+':
                            op = Mathop.Add;
                            pos++;
                            break;
                        case '*':
                            op = Mathop.Multiply;
                            pos++;
                            break;
                    }
                    pos++;
                }
                return acc; // shouldn't happen?
            }
        }
    }
}
