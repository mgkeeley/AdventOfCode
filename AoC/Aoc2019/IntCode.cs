using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;

namespace AoC {
    public class IntCode {
        readonly long[] program;
        long[] memory;
        int pc;
        int argmode;
        long relbase;
        BlockingCollection<long> input;
        ManualResetEventSlim idle = new ManualResetEventSlim(false);

        public IntCode(long[] program) {
            this.program = program;
            Reset();
        }

        public bool IsBusy => !idle.IsSet;

        public bool Running { get; private set; }

        public IntCode Reset() {
            memory = new long[8192];
            program.CopyTo(memory, 0);
            pc = 0;
            argmode = 0;
            relbase = 0;
            input = new BlockingCollection<long>();
            idle.Set();
            return this;
        }

        public IntCode FastReset() {
            pc = 0;
            argmode = 0;
            relbase = 0;
            input = new BlockingCollection<long>();
            idle.Set();
            return this;
        }

        public IntCode SetParams(int param1, int param2) {
            return WriteMemory(1, param1).WriteMemory(2, param2);
        }

        public long ReadMemory(int addr) {
            return memory[addr];
        }

        public IntCode WriteMemory(int addr, long value) {
            memory[addr] = value;
            return this;
        }

        public IntCode Write(long input_value) {
            input.Add(input_value);
            return this;
        }

        public long PopInput() {
            return input.Take();
        }

        public IntCode Run(Action<long> output = null) {
            idle.Reset();
            Running = true;
            while (pc >= 0 && pc < memory.Length && memory[pc] != 99) {
                int op = PCReadOpCode();
                if (op == 1) {
                    long arg1 = PCReadValue();
                    long arg2 = PCReadValue();
                    long dest = PCReadAddr();
                    memory[dest] = arg1 + arg2;
                } else if (op == 2) {
                    long arg1 = PCReadValue();
                    long arg2 = PCReadValue();
                    long dest = PCReadAddr();
                    memory[dest] = arg1 * arg2;
                } else if (op == 3) {
                    long dest = PCReadAddr();
                    idle.Set();
                    memory[dest] = PopInput();
                    idle.Reset();
                } else if (op == 4) {
                    long arg1 = PCReadValue();
                    output(arg1);
                } else if (op == 5) {
                    long arg1 = PCReadValue();
                    long dest = PCReadValue();
                    if (arg1 != 0)
                        pc = (int)dest;
                } else if (op == 6) {
                    long arg1 = PCReadValue();
                    long dest = PCReadValue();
                    if (arg1 == 0)
                        pc = (int)dest;
                } else if (op == 7) {
                    long arg1 = PCReadValue();
                    long arg2 = PCReadValue();
                    long dest = PCReadAddr();
                    memory[dest] = (arg1 < arg2) ? 1 : 0;
                } else if (op == 8) {
                    long arg1 = PCReadValue();
                    long arg2 = PCReadValue();
                    long dest = PCReadAddr();
                    memory[dest] = (arg1 == arg2) ? 1 : 0;
                } else if (op == 9) {
                    relbase += PCReadValue();
                } else {
                    throw new Exception("Intcode opcode error");
                }
            }
            if (memory[pc] != 99)
                throw new Exception("Intcode pc address error");
            Running = false;
            return this;
        }

        public void WaitWhileBusy() {
            idle.Wait();
        }

        private int PCReadOpCode() {
            int op = (int)(memory[pc] % 100);
            argmode = (int)(memory[pc] / 100);
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
