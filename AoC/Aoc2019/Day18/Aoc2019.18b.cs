using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices {
    public class IsExternalInit { }
}

namespace AoC
{
    public partial class Aoc2019
    {
        [DebuggerDisplay("{Tag}")]
        public class Key {
            public int Tag { get; init; }
            public Point Pos { get; init; }
            public List<NextKey> Next { get; } = new();  // list of all keys and dist to them, with required keys
        }

        [DebuggerDisplay("{Key.Tag}")]
        public class NextKey {
            public NextKey(int tag, int requiredKeys, int collectedKeys, int steps) {
                Tag = tag;
                RequiredKeys = requiredKeys;
                CollectedKeys = collectedKeys;
                Steps = steps;
            }

            public int Tag { get; init; }
            public int Steps { get; init; }
            public int RequiredKeys { get; }  // bit-set: each bit corresponds to a key (a..z = bits 0..25)
            public int CollectedKeys { get; }
        }

        public class Trail {
            public int X, Y, Steps;
            public int RequiredKeys, CollectedKeys;

            public Trail(int x, int y, int v, int requiredKeys, int collectedKeys) {
                X = x;
                Y = y;
                Steps = v;
                RequiredKeys = requiredKeys;
                CollectedKeys = collectedKeys;
            }
        }

        public struct RobotState : IEquatable<RobotState> {
            public int Robots { get; }
            public int Keys { get; }

            public RobotState(int robots, int keys = 0) {
                Robots = robots;
                Keys = keys;
            }

            public override bool Equals(object obj) {
                return obj is RobotState state && Equals(state);
            }

            public bool Equals(RobotState other) {
                return Robots == other.Robots &&
                       Keys == other.Keys;
            }

            public override int GetHashCode() {
                return HashCode.Combine(Robots, Keys);
            }
        }

        static void FindPath(string[] input) {
            Stopwatch sw = Stopwatch.StartNew();
            int maxx = input[0].Length;
            int maxy = input.Length;

            // find keys
            Dictionary<int, Key> keys = new();
            int robots = 0;
            for (int y = 0; y < maxy; y++) {
                for (int x = 0; x < maxx; x++) {
                    if (input[y][x] >= 'a' && input[y][x] <= 'z') {
                        Key key = new Key {
                            Tag = 1 << (input[y][x] - 'a'),
                            Pos = new Point(x, y)
                        };
                        keys.Add(key.Tag, key);
                    } else if (input[y][x] == '@') {
                        Key start = new Key {
                            Tag = 1 << (26 + robots),
                            Pos = new Point(x, y)
                        };
                        keys.Add(start.Tag, start);
                        robots++;
                    }
                }
            }

            foreach (var key in keys.Values)
                FindNext(key);

            //Console.WriteLine($"Found key steps in {sw.ElapsedMilliseconds}ms");

            PriorityQueue<RobotState> queue = new PriorityQueue<RobotState>();
            Dictionary<RobotState, int> visited = new Dictionary<RobotState, int>();

            int allKeys = (1 << (keys.Count - robots)) - 1;
            int minSteps = 0;
            int robotState = 0;
            for(int r = 0; r < robots; r++)
                robotState = (robotState << 1) | (1 << 26);
            queue.Add(new RobotState(robotState), 0);
            visited[queue.Peek()] = 0;
            while (queue.TryRemoveRoot(out RobotState state, out int dist)) {
                if (state.Keys == allKeys) {
                    minSteps = dist;
                    break;
                }
                for (int r = 1 << 30; r > 0; r >>= 1) {
                    if ((state.Robots & r) != 0) {
                        foreach (var next in keys[r].Next) {
                            if ((state.Keys & next.Tag) == 0 && (next.RequiredKeys & state.Keys) == next.RequiredKeys) {
                                var nextState = new RobotState((state.Robots & ~r) | next.Tag, state.Keys | next.CollectedKeys);
                                if (visited.TryGetValue(nextState, out int prevDist) && prevDist <= dist + next.Steps)
                                    continue; // ignore, we already found a better path previously
                                visited[nextState] = dist + next.Steps;
                                queue.Add(nextState, dist + next.Steps);
                            }
                        }
                    }
                }
            }
            sw.Stop();
            Console.WriteLine($"{minSteps} in {sw.ElapsedMilliseconds}ms");

            void FindNext(Key start) {
                Queue<Trail> queue = new();
                queue.Enqueue(new Trail(start.Pos.X, start.Pos.Y, 0, 0, 0));
                bool[,] visited = new bool[maxx, maxy];
                visited[start.Pos.X, start.Pos.Y] = true;
                while (queue.Count > 0) {
                    var t = queue.Dequeue();
                    var c = input[t.Y][t.X];
                    if (c >= 'a' && c <= 'z') {
                        int tag = 1 << (c - 'a');
                        t.CollectedKeys |= tag;
                        start.Next.Add(new NextKey(tag, t.RequiredKeys, t.CollectedKeys, t.Steps));
                    }
                    if (c >= 'A' && c <= 'Z')
                        t.RequiredKeys |= 1 << (c - 'A');
                    TryEnqueue(t, -1, 0);
                    TryEnqueue(t, +1, 0);
                    TryEnqueue(t, 0, -1);
                    TryEnqueue(t, 0, +1);

                    void TryEnqueue(Trail t, int dx, int dy) {
                        if (!visited[t.X + dx, t.Y + dy] && input[t.Y + dy][t.X + dx] != '#') {
                            visited[t.X + dx, t.Y + dy] = true;
                            queue.Enqueue(new Trail(t.X + dx, t.Y + dy, t.Steps + 1, t.RequiredKeys, t.CollectedKeys));
                        }
                    }
                }
            }

        }

        public static void Day18b() {
            Console.WriteLine("Day18: ");

            string[] input = File.ReadAllLines("Aoc2019\\Day18\\input.txt");

            FindPath(input);

            // part 2
            for (int y = 0; y < input.Length; y++) {
                int pos = input[y].IndexOf(".@.");
                if (pos != -1) {
                    input[y - 1] = input[y - 1].Remove(pos, 3).Insert(pos, "@#@");
                    input[y + 0] = input[y + 0].Remove(pos, 3).Insert(pos, "###");
                    input[y + 1] = input[y + 1].Remove(pos, 3).Insert(pos, "@#@");
                }
            }
            FindPath(input);
        }
    }
}
