using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AoC
{
    public partial class Aoc2019
    {
        public static void Day23() {
            Console.WriteLine("Day23: ");

            long[] prog = File.ReadAllText("Aoc2019\\Day23\\input.txt").Split(',').Select(c => long.Parse(c)).ToArray();
            IntCode[] intcode = Enumerable.Range(0, 50).Select(addr => new IntCode(prog).Write(addr)).ToArray();

            long natX = 0, natY = -3;

            Thread[] tasks = intcode.Select(i => {
                var thread = new Thread(() => {
                    int n = 0;
                    while (i != intcode[n])
                        n++;
                    long[] output = new long[3];
                    int state = 0;
                    i.Write(-1);
                    try {
                        i.Run(o => {
                            output[state++] = o;
                            if (state == 3) {
                                if (output[0] == 255) {
                                    if (natY == -3)
                                        Console.WriteLine($"First Y for 255 = {output[2]}");
                                    natX = output[1];
                                    natY = output[2];
                                    //Console.WriteLine($"Wrote {output[1]},{output[2]} to NAT from {n}");
                                } else {
                                    intcode[output[0]].Write(output[1]).Write(output[2]);
                                    Thread.Sleep(1); // doesn't seem to sync properly without this sleep ?
                                    //Console.WriteLine($"Wrote {output[1]},{output[2]} to {output[0]} from {n}");
                                }
                                state = 0;
                            }
                        });
                    }
                    catch { }
                });
                thread.Start();
                return thread;
            }).ToArray();

            long lastY = 0;
            while (true) {
                while (natY == -2 || natY == -3 || intcode.Any(i => i.IsBusy))
                    Thread.Sleep(10);
                if (!intcode.Any(i => i.IsBusy)) {
                    //Console.WriteLine($"Writing {natX},{natY} to 0");
                    intcode[0].Write(natX).Write(natY);
                    if (natY == lastY) {
                        Console.WriteLine($"Wrote {natY} twice to 0");
                        break;
                    }
                    lastY = natY;
                    natY = -2;
                }
            }
            foreach (Thread t in tasks)
                t.Interrupt();
        }
    }
}
