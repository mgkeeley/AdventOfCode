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

        public static void Day25() {
            Console.WriteLine("Day25: ");
            Stopwatch sw = Stopwatch.StartNew();
            int pubCard = 1965712;
            int pubDoor = 19072108;

            // test pubCard = 5764801;
            // test pubDoor = 17807724;

            int Transform(int subj, int loops) {
                var val = 1;
                for (int i = 0; i < loops; i++) {
                    val = (int)(((long)val * subj) % 20201227);
                }
                return val;
            }

            int Decode(int subj, int target) {
                var val = 1;
                var loops = 0;
                while(val != target) { 
                    val = (int)(((long)val * subj) % 20201227);
                    loops++;
                }
                return loops;
            }

            int cardLoops = Decode(7, pubCard);
            //int doorLoops = Decode(7, pubDoor);

            int key = Transform(pubDoor, cardLoops);

            sw.Stop();
            Console.WriteLine($"{key} in {sw.ElapsedMilliseconds}ms");
        }
    }
}
