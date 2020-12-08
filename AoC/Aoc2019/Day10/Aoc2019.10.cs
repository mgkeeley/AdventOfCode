using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2019
    {
        public class Asteroid {
            public int X, Y;
            public double dist;
            public double angle;
        }

        public static void Day10() {
            Console.WriteLine("Day10: ");

            string[] input = File.ReadAllLines("Aoc2019\\Day10\\input.txt");
            List<Asteroid> asteroids = new List<Asteroid>();

            // part 1
            for (int y = 0; y < input.Length; y++) {
                for (int x = 0; x < input[0].Length; x++) {
                    if (input[y][x] == '#') {
                        asteroids.Add(new Asteroid { X = x, Y = y });
                    }
                }
            }
            int best_count = 0;
            Asteroid best = null;
            foreach (var a1 in asteroids) {
                HashSet<double> angles = new HashSet<double>();
                foreach (var a2 in asteroids) {
                    if (a1 != a2) {
                        double slope = Math.Round(Math.Atan2(a1.Y - a2.Y, a1.X - a2.X), 8);
                        angles.Add(slope);
                    }
                }
                if (best_count < angles.Count) {
                    best_count = angles.Count;
                    best = a1;
                }
            }
            Console.WriteLine($"{best_count} @ {best.X},{best.Y}");

            // part 2
            Dictionary<double, List<Asteroid>> fire_list = new Dictionary<double, List<Asteroid>>();
            foreach (var a2 in asteroids) {
                if (best != a2) {
                    a2.angle = Math.Round(Math.Atan2(a2.X - best.X, best.Y - a2.Y), 8);
                    if (a2.angle < 0)
                        a2.angle += Math.PI * 2.0;
                    a2.dist = (best.Y - a2.Y) * (best.Y - a2.Y) + (best.X - a2.X) * (best.X - a2.X);
                    if (!fire_list.TryGetValue(a2.angle, out List<Asteroid> fire_line)) {
                        fire_line = new List<Asteroid>();
                        fire_list.Add(a2.angle, fire_line);
                    }
                    fire_line.Add(a2);
                }
            }
            foreach(var entry in fire_list) {
                entry.Value.Sort((a, b) => -Comparer<double>.Default.Compare(a.dist, b.dist));
            }
            Asteroid num200 = null;
            int fire_count = 0;
            while (fire_count < 200) {
                foreach (var entry in fire_list.OrderBy(v => v.Key)) {
                    if (fire_count < 200 && entry.Value.Count > 0) {
                        num200 = entry.Value[entry.Value.Count - 1];
                        entry.Value.RemoveAt(entry.Value.Count - 1);
                        fire_count++;
                    }
                }
            }
            Console.WriteLine($"{num200.X},{num200.Y} = {num200.X * 100 + num200.Y}");
        }
    }
}
