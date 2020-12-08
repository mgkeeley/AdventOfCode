using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC
{
    public partial class Aoc2019 {
       
        public struct Moon {
            public int x, y, z;
            public int vx, vy, vz;

            public Moon(int[] xyz) : this() {
                x = xyz[0];
                y = xyz[1];
                z = xyz[2];
                vx = vy = vz = 0;
            }

            public int TotalEnergy() {
                return (Math.Abs(x) + Math.Abs(y) + Math.Abs(z)) * (Math.Abs(vx) + Math.Abs(vy) + Math.Abs(vz));
            }
        }

        public class System {
            public Moon[] moons;

            public System(Moon[] moons) {
                this.moons = moons;
            }

            public override bool Equals(object obj) {
                return obj is System system && moons.SequenceEqual(system.moons);
            }

            public override int GetHashCode() {
                return HashCode.Combine(moons[0], moons[1], moons[2], moons[3]);
            }

            public static bool operator ==(System left, System right) {
                return left.moons.SequenceEqual(right.moons);
            }

            public static bool operator !=(System left, System right) {
                return !(left == right);
            }

            public System Clone() {
                return new System((Moon[])moons.Clone());
            }

            public long TotalEnergy() {
                return moons.Sum(m => m.TotalEnergy());
            }
        }

        public static void Day12() {
            Console.WriteLine("Day12: ");

            Moon[] moons = File.ReadAllLines("Aoc2019\\Day12\\input.txt").Select(s => Regex.Replace(s, "[<>xyz= ]", "")).Select(s => {
                var xyz = s.Split(',').Select(s => int.Parse(s)).ToArray();
                return new Moon(xyz);
            }).ToArray();
            System sys0 = new System(moons);

            // part 1
            System sys = sys0.Clone();
            for (int t = 0; t < 1000; t++)
                Timestep2(sys.moons);
            Console.WriteLine(sys.TotalEnergy());

            //part 2
            System sysX = sys0.Clone();
            for (int i = 0; i < sysX.moons.Length; i++) {
                sysX.moons[i].y = 0;
                sysX.moons[i].z = 0;
            }
            System sysY = sys0.Clone();
            for (int i = 0; i < sysY.moons.Length; i++) {
                sysY.moons[i].x = 0;
                sysY.moons[i].z = 0;
            }
            System sysZ = sys0.Clone();
            for (int i = 0; i < sysZ.moons.Length; i++) {
                sysZ.moons[i].x = 0;
                sysZ.moons[i].y = 0;
            }
            long xcyc = CheckCycleAt0(sysX);
            long ycyc = CheckCycleAt0(sysY);
            long zcyc = CheckCycleAt0(sysZ);
            long x = 0, y = 0, z = 0;
            // this is the answer, just to speed up running this program. Comment these three lines to recalculate.
            x = 374307970285176 - xcyc;
            y = 374307970285176 - ycyc;
            z = 374307970285176 - zcyc;
            // A faster way than this loop would be to calculate the Least Common Multiple of xcyc, ycyc, zcyc
            while (true) {
                if (x < y) {
                    if (x < z)
                        x += xcyc;
                    else
                        z += zcyc;
                } else {
                    if (y < z)
                        y += ycyc;
                    else
                        z += zcyc;
                }
                if (x == y && y == z)
                    break;
            }
            Console.WriteLine(x);
        }

        private static long CheckCycleAt0(System sys0) {
            System sys = sys0.Clone();
            long t2 = 0;
            while (true) {
                Timestep2(sys.moons);
                t2++;
                if (sys == sys0)
                    break;
            }
            Console.WriteLine($"Found cycle at {t2}");
            return t2;
        }

        // this version checks for cycles that might start after position 0.
        private static long CheckCycle(System sys0) {
            Dictionary<int, (long, System)> possible_matches = new Dictionary<int, (long, System)>();
            System sys = sys0.Clone();
            long t2 = 0;
            long check_step = 10000000;
            long deltastep;
            while (true) {
                Timestep2(sys.moons);
                t2++;
                int hash = sys.GetHashCode();
                if (possible_matches.TryGetValue(hash, out var prevt)) {
                    //Console.WriteLine($"Found possible previous cycle at {prevt.Item1}");
                    if (prevt.Item2 == sys) {
                        Console.WriteLine($"Found cycle at {t2} with time-delta {t2 - prevt.Item1}");
                        deltastep = t2 - prevt.Item1;
                        break;
                    }
                }
                if (t2 % check_step == 0) {
                    possible_matches.Add(hash, (t2, sys.Clone()));
                    Console.WriteLine(t2 + " " + possible_matches.Count);
                }
            }
            sys = sys0.Clone();
            t2 = 0;
            foreach (var prev in possible_matches) {
                if (prev.Value.Item1 < deltastep && t2 < prev.Value.Item1) {
                    t2 = prev.Value.Item1;
                    sys = prev.Value.Item2.Clone();
                    Console.WriteLine($"Advancing system to prev {t2}");
                }
            }
            while (t2 < deltastep) {
                Timestep2(sys.moons);
                t2++;
                if (t2 % check_step == 0)
                    Console.WriteLine($"Advancing system to {t2}");
            }
            var sys1 = sys0.Clone();
            while (sys1 != sys) {
                Timestep2(sys.moons);
                Timestep2(sys1.moons);
                t2++;
                if (t2 % check_step == 0)
                    Console.WriteLine($"Advancing delta systems to {t2}");
            }
            Console.WriteLine($"Cycle starts at {t2 - deltastep} repeating at {t2}");
            return deltastep;
        }

        private static void Timestep(Moon[] moons) {
            int l = moons.Length;
            for (int m1 = 0; m1 < l; m1++) {
                for (int m2 = 0; m2 < l; m2++) {
                    if (m1 == m2)
                        continue;
                    // calc delta for moon m1
                    moons[m1].vx += (int)-Math.Sign(moons[m1].x - moons[m2].x);
                    moons[m1].vy += (int)-Math.Sign(moons[m1].y - moons[m2].y);
                    moons[m1].vz += (int)-Math.Sign(moons[m1].z - moons[m2].z);
                }
            }
            for (int m1 = 0; m1 < l; m1++) {
                moons[m1].x += moons[m1].vx;
                moons[m1].y += moons[m1].vy;
                moons[m1].z += moons[m1].vz;
            }
        }

        private static void Timestep2(Moon[] moons) {
            ref Moon moons0 = ref moons[0];
            ref Moon moons1 = ref moons[1];
            ref Moon moons2 = ref moons[2];
            ref Moon moons3 = ref moons[3];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int Sign(int v) {
                return v > 0 ? 1 : (int)(v >> 31);
            }

            // unroll loops
            moons0.vx += (int)(Sign(moons1.x - moons0.x) + Sign(moons2.x - moons0.x) + Sign(moons3.x - moons0.x));
            moons0.vy += (int)(Sign(moons1.y - moons0.y) + Sign(moons2.y - moons0.y) + Sign(moons3.y - moons0.y));
            moons0.vz += (int)(Sign(moons1.z - moons0.z) + Sign(moons2.z - moons0.z) + Sign(moons3.z - moons0.z));
            moons1.vx += (int)(Sign(moons0.x - moons1.x) + Sign(moons2.x - moons1.x) + Sign(moons3.x - moons1.x));
            moons1.vy += (int)(Sign(moons0.y - moons1.y) + Sign(moons2.y - moons1.y) + Sign(moons3.y - moons1.y));
            moons1.vz += (int)(Sign(moons0.z - moons1.z) + Sign(moons2.z - moons1.z) + Sign(moons3.z - moons1.z));
            moons2.vx += (int)(Sign(moons0.x - moons2.x) + Sign(moons1.x - moons2.x) + Sign(moons3.x - moons2.x));
            moons2.vy += (int)(Sign(moons0.y - moons2.y) + Sign(moons1.y - moons2.y) + Sign(moons3.y - moons2.y));
            moons2.vz += (int)(Sign(moons0.z - moons2.z) + Sign(moons1.z - moons2.z) + Sign(moons3.z - moons2.z));
            moons3.vx += (int)(Sign(moons0.x - moons3.x) + Sign(moons1.x - moons3.x) + Sign(moons2.x - moons3.x));
            moons3.vy += (int)(Sign(moons0.y - moons3.y) + Sign(moons1.y - moons3.y) + Sign(moons2.y - moons3.y));
            moons3.vz += (int)(Sign(moons0.z - moons3.z) + Sign(moons1.z - moons3.z) + Sign(moons2.z - moons3.z));

            moons0.x += moons0.vx;
            moons0.y += moons0.vy;
            moons0.z += moons0.vz;
            moons1.x += moons1.vx;
            moons1.y += moons1.vy;
            moons1.z += moons1.vz;
            moons2.x += moons2.vx;
            moons2.y += moons2.vy;
            moons2.z += moons2.vz;
            moons3.x += moons3.vx;
            moons3.y += moons3.vy;
            moons3.z += moons3.vz;
        }
    }
}
