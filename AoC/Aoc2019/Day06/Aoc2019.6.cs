using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2019 {
        public class Orbit {
            public Orbit(string name) {
                Name = name;
                Children = new List<Orbit>();
            }

            public string Name { get; }
            public Orbit Parent { get; set; }
            public List<Orbit> Children { get; }

            public int Data { get; set; }

            public int TotalDepth(int depth = 0) {
                if (Children.Count == 0)
                    return depth;
                return depth + Children.Sum(child => child.TotalDepth(depth + 1));
            }

            public void Dump(int depth = 0) {
                Console.WriteLine(new string(' ', depth) + Name);
                foreach (var child in Children) {
                    child.Dump(depth + 2);
                }
            }
        }

        public static void Day6() {
            Console.WriteLine("Day6: ");

            string[] orbits = File.ReadAllLines("Aoc2019\\Day06\\input.txt");

            Dictionary<string, Orbit> system = new Dictionary<string, Orbit>();
            Orbit rootOrbit = new Orbit("COM");
            system.Add(rootOrbit.Name, rootOrbit);

            // part 1
            foreach (string orbit in orbits) {
                string root = orbit.Split(')')[0];
                string name = orbit.Split(')')[1];
                if (!system.TryGetValue(root, out Orbit parent)) {
                    parent = new Orbit(root);
                    system.Add(root, parent);
                }
                if (!system.TryGetValue(name, out Orbit child)) {
                    child = new Orbit(name);
                    system.Add(name, child);
                }
                child.Parent = parent;
                parent.Children.Add(child);
            }

            //rootOrbit.Dump();
            Console.WriteLine(rootOrbit.TotalDepth());

            // part 2
            int transfers = 0;
            if (system.TryGetValue("YOU", out Orbit you)) {
                if (system.TryGetValue("SAN", out Orbit santa)) {
                    while (you != null) {
                        you.Data = transfers++;
                        you = you.Parent;
                    }
                    transfers = 0;
                    while (santa != null) {
                        if (santa.Data != 0) {
                            transfers += santa.Data;
                            break;
                        }
                        transfers++;
                        santa = santa.Parent;
                    }
                }
            }
            Console.WriteLine(transfers-2);
        }
    }
}
