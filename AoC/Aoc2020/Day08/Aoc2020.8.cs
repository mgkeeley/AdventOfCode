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
        public static void Day8() {
            Console.WriteLine("Day8: ");
            var occ = new OpCodeComputer(File.ReadAllLines("Aoc2020\\Day08\\input.txt"));

            // part 1
            HashSet<int> executed = new();
            bool infiniteLoop = false;
            occ.PreExecute += (op, ev) => {
                if (executed.Contains(op)) {
                    ev.Cancel = true;
                    infiniteLoop = true;
                }
                executed.Add(op);
            };
            occ.Run();
            Console.WriteLine(occ.Accumulator);

            // part 2
            // find a nop/jmp to change to jmp/nop
            HashSet<int> originalExecuted = new HashSet<int>(executed);
            foreach(var pc in originalExecuted) {
                var op = occ.Program[pc];
                if (op.OpCode == OpCodeComputer.OpCode.nop) {
                    if (!originalExecuted.Contains(pc + op.Arg)) {
                        //Console.WriteLine($"Found possible breakout at {pc}");
                        op.OpCode = OpCodeComputer.OpCode.jmp;
                        infiniteLoop = false;
                        executed.Clear();
                        occ.Reset().Run();
                        if (!infiniteLoop)
                            break;
                        op.OpCode = OpCodeComputer.OpCode.nop;
                    }
                }
                if (op.OpCode == OpCodeComputer.OpCode.jmp) {
                    if (!originalExecuted.Contains(pc + 1)) {
                        //Console.WriteLine($"Found possible jmp skip at {pc}");
                        op.OpCode = OpCodeComputer.OpCode.nop;
                        infiniteLoop = false;
                        executed.Clear();
                        occ.Reset().Run();
                        if (!infiniteLoop)
                            break;
                        op.OpCode = OpCodeComputer.OpCode.jmp;
                    }
                }
            }
            if (infiniteLoop)
                Console.WriteLine("Nope.");
            else
                Console.WriteLine(occ.Accumulator);
        }
    }
}
