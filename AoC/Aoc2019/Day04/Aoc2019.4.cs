using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2019
    {
        public static void Day4() {
            Console.WriteLine("Day4: ");
            int min = 134564;
            int max = 585159;

            // part 1
            int good = 0;
            for (int p = min; p <= max; p++) {
                if (IsValidPass(p))
                    good++;
            }
            Console.WriteLine(good);

            // part 2
            good = 0;
            for (int p = min; p <= max; p++) {
                if (IsValidPass2(p))
                    good++;
            }
            Console.WriteLine(good);
        }

        private static bool IsValidPass(int p) {
            string s = p.ToString();
            bool has_double = false;
            for (int i = 0; i < s.Length - 1; i++) {
                if (s[i + 1] < s[i])
                    return false;
                if (s[i + 1] == s[i])
                    has_double = true;
            }
            return has_double;
        }

        private static bool IsValidPass2(int p) {
            string s = p.ToString() + "X";
            char cur = 'x';
            int count = 0;
            bool has_double = false;
            for (int i = 0; i < s.Length; i++) {
                if (i + 1 < s.Length && s[i + 1] < s[i])
                    return false;
                if (s[i] == cur)
                    count++;
                else {
                    if (count == 2)
                        has_double = true;
                    count = 1;
                    cur = s[i];
                }
            }
            return has_double;
        }
    }
}
