using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC
{
    public partial class Aoc2019
    {
        public static void Day7() {
            Console.WriteLine("Day7: ");

            long[] prog = File.ReadAllText("Aoc2019\\Day07\\input.txt").Split(',').Select(c => long.Parse(c)).ToArray();
            var intcode = new IntCode(prog);

            // part 1
            long max_thrust = 0;
            string best = "?";

            for (int A = 0; A <= 4; A++) {
                for (int B = 0; B <= 4; B++) {
                    for (int C = 0; C <= 4; C++) {
                        for (int D = 0; D <= 4; D++) {
                            for (int E = 0; E <= 4; E++) {
                                if (A == B || A == C || A == D || A == E || B == C || B == D || B == E || C == D || C == E || D == E)
                                    continue;
                                Stack<long> inp = new Stack<long>();
                                long thrust = 0;
                                intcode.Reset().Write(A).Write(thrust).Run(o => thrust = o);
                                intcode.Reset().Write(B).Write(thrust).Run(o => thrust = o);
                                intcode.Reset().Write(C).Write(thrust).Run(o => thrust = o);
                                intcode.Reset().Write(D).Write(thrust).Run(o => thrust = o);
                                intcode.Reset().Write(E).Write(thrust).Run(o => thrust = o);
                                if (thrust > max_thrust) {
                                    max_thrust = thrust;
                                    best = $"{A}{B}{C}{D}{E}";
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine(max_thrust);

            // part 2
            var ampA = new IntCode(prog);
            var ampB = new IntCode(prog);
            var ampC = new IntCode(prog);
            var ampD = new IntCode(prog);
            var ampE = new IntCode(prog);
            max_thrust = 0;
            for (int A = 5; A <= 9; A++) {
                for (int B = 5; B <= 9; B++) {
                    for (int C = 5; C <= 9; C++) {
                        for (int D = 5; D <= 9; D++) {
                            for (int E = 5; E <= 9; E++) {
                                if (A == B || A == C || A == D || A == E || B == C || B == D || B == E || C == D || C == E || D == E)
                                    continue;
                                ampA.Reset().Write(A).Write(0);
                                ampB.Reset().Write(B);
                                ampC.Reset().Write(C);
                                ampD.Reset().Write(D);
                                ampE.Reset().Write(E);
                                Task.WaitAll(
                                    Task.Run(() => ampA.Run(o => ampB.Write(o))),
                                    Task.Run(() => ampB.Run(o => ampC.Write(o))),
                                    Task.Run(() => ampC.Run(o => ampD.Write(o))),
                                    Task.Run(() => ampD.Run(o => ampE.Write(o))),
                                    Task.Run(() => ampE.Run(o => ampA.Write(o))));
                                long thrust = ampA.PopInput();
                                if (thrust > max_thrust) {
                                    max_thrust = thrust;
                                    best = $"{A}{B}{C}{D}{E}";
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine(max_thrust);
        }
    }
}
