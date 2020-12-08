using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AoC {
    public class OpCodeComputer {
        public enum OpCode {
            nop,
            acc,
            jmp
        }

        public class OpWord {
            public OpCode OpCode { get; set; }
            public int Arg { get; set; }
            
            public OpWord(OpCode op, int arg) {
                OpCode = op;
                Arg = arg;
            }
        }

        public OpWord[] Program { get; }
        public int Accumulator { get; set; }

        public event Action<int, CancelEventArgs> PreExecute;

        long[] memory;
        int pc;
        int argmode;
        long relbase;
        BlockingCollection<long> input;
        ManualResetEventSlim idle = new ManualResetEventSlim(false);

        public OpCodeComputer(string[] program) {
            Program = program.Select(i => {
                string[] ops = i.Split(' ');
                return new OpWord(Enum.Parse<OpCode>(ops[0]), int.Parse(ops[1]));
            }).ToArray();
            Reset();
        }

        public OpCodeComputer Reset() {
            memory = new long[4096];
            pc = 0;
            Accumulator = 0;
            argmode = 0;
            relbase = 0;
            input = new BlockingCollection<long>();
            idle.Set();
            return this;
        }

        public OpCodeComputer SetParams(int param1, int param2) {
            return WriteMemory(1, param1).WriteMemory(2, param2);
        }

        public long ReadMemory(int addr) {
            return memory[addr];
        }

        public OpCodeComputer WriteMemory(int addr, long value) {
            memory[addr] = value;
            return this;
        }

        public OpCodeComputer Write(long input_value) {
            input.Add(input_value);
            return this;
        }

        public long PopInput() {
            return input.Take();
        }

        public OpCodeComputer Run(Action<long> output = null) {
            idle.Reset();
            while (pc >= 0 && pc < Program.Length) {
                var cancel = new CancelEventArgs(false);
                PreExecute?.Invoke(pc, cancel);
                OpWord op = PCReadOpWord();
                if (cancel.Cancel)
                    break;
                if (op.OpCode == OpCode.nop) {
                    //nop
                } else if (op.OpCode == OpCode.acc) {
                    Accumulator += op.Arg;
                } else if (op.OpCode == OpCode.jmp) {
                    pc += op.Arg - 1;
                } else {
                    throw new Exception("Opcode error");
                }
            }
            return this;
        }

        public void WaitWhileBusy() {
            idle.Wait();
        }

        private OpWord PCReadOpWord() {
            OpWord op = Program[pc];
            pc++;
            return op;
        }

        private long PCReadAddr() {
            int mode = argmode % 10;
            argmode /= 10;
            long arg = memory[pc++];
            if (mode == 0)
                return arg;
            if (mode == 1)
                throw new Exception("Invalid arg mode for Read Addr!");
            if (mode == 2)
                return arg + relbase;
            throw new Exception("Incode invalid arg mode");
        }

        private long PCReadValue() {
            int mode = argmode % 10;
            argmode /= 10;
            long arg = memory[pc++];
            if (mode == 0)
                return memory[arg];
            if (mode == 1)
                return arg;
            if (mode == 2)
                return memory[arg + relbase];
            throw new Exception("Incode invalid arg mode");
        }
    }
}
