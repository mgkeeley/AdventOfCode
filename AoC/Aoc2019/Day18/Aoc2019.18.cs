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

namespace AoC
{
    public partial class Aoc2019
    {
        public class Memo {
            public int Steps;
            public string Trail;
        }

        [DebuggerDisplay("{Tag}")]
        public class Key {
            public char Tag { get; init; }
            public Point Pos { get; init; }
            public List<NextKey> Next { get; } = new();  // list of all keys and dist to them, with required keys
            public Dictionary<HashSet<char>, Memo> Memo { get; } = new();  // memo of previously found keys and dist so far to this key

            public bool TryGetMemo(HashSet<char> keyRing, out Memo best) {
                foreach(var m in Memo) {
                    if (m.Key.Count == keyRing.Count && m.Key.All(k => keyRing.Contains(k))) {
                        best = m.Value;
                        return true;
                    }
                }
                best = null;
                return false;
            }

            internal void AddMemo(HashSet<char> keyRing, int best, string trail) {
                Memo[new HashSet<char>(keyRing)] = new Memo { Steps = best, Trail = trail };
            }
        }

        [DebuggerDisplay("{Key.Tag}")]
        public class NextKey {
            public NextKey(Key key, IEnumerable<char> requiredKeys, IEnumerable<char> collectedKeys, int steps) {
                Key = key;
                RequiredKeys = new(requiredKeys);
                CollectedKeys = new(collectedKeys);
                Steps = steps;
            }

            public Key Key { get; init; }
            public int Steps { get; init; }
            public HashSet<char> RequiredKeys { get; }
            public HashSet<char> CollectedKeys { get; }
        }

        public class Trail {
            public int X, Y, Steps;
            public List<char> RequiredKeys, CollectedKeys;

            public Trail(int x, int y, int v, List<char> requiredKeys, List<char> collectedKeys) {
                X = x;
                Y = y;
                Steps = v;
                RequiredKeys = requiredKeys;
                CollectedKeys = collectedKeys;
            }
        }

        public static void Day18() {
            Console.WriteLine("Day18: ");

            string[] input = File.ReadAllLines("Aoc2019\\Day18\\test2.txt");
            int maxx = input[0].Length;
            int maxy = input.Length;

            // find keys
            Dictionary<char, Key> keys = new();
            Point start = new Point(-1, -1);
            for(int y = 0; y < maxy; y++) {
                for(int x = 0; x < maxx; x++) {
                    if (input[y][x] >= 'a' && input[y][x] <= 'z') {
                        Key key = new Key {
                            Tag = input[y][x],
                            Pos = new Point(x, y)
                        };
                        keys.Add(key.Tag, key);
                    } else if (input[y][x] == '@') {
                        start = new Point(x, y);
                    }
                }
            }

            foreach(var key in keys.Values)
                key.Next.AddRange(FindNext(key.Pos));
            HashSet<char> keyRing = new();
            int overallBest = int.MaxValue;
            string overallBestTrail = "";
            (int minSteps, string minTrail) = FindMinSteps(0, FindNext(start));
            
            Console.WriteLine($"{overallBest}: {overallBestTrail}");

            (int, string) FindMinSteps(int steps, List<NextKey> nextKeys) {
                List<NextKey> options = nextKeys.Where(nk => !keyRing.Contains(nk.Key.Tag) && nk.RequiredKeys.All(rk => keyRing.Contains(rk))).ToList();
                if (steps >= overallBest || options.Count == 0)
                    return (steps, "");
                int best = int.MaxValue;
                string bestTrail = "";
                foreach (var opt in options.OrderBy(nk => nk.Steps)) {
                    Console.WriteLine($"Try {string.Concat(keyRing)} to {opt.Key.Tag} + {string.Concat(opt.CollectedKeys)}  {steps}+{opt.Steps}={steps + opt.Steps} ");

                    List<char> toRemove = new();
                    foreach (var c in opt.CollectedKeys) {
                        if (keyRing.Add(c))
                            toRemove.Add(c);
                    }
                    int s;
                    string trail;
                    if (opt.Key.TryGetMemo(keyRing, out Memo memo)) {
                        s = memo.Steps + steps + opt.Steps;
                        trail = string.Concat(keyRing) + memo.Trail;
                        Console.WriteLine($"  {string.Concat(keyRing)} = +{s - steps - opt.Steps} = {s} (-)");
                    } else {
                        (s, trail) = FindMinSteps(steps + opt.Steps, opt.Key.Next);
                        opt.Key.AddMemo(keyRing, s - steps - opt.Steps, trail);
                        //trail = string.Concat(keyRing) + trail;
                        Console.WriteLine($"  {string.Concat(keyRing)} = +{s - steps - opt.Steps} = {s} (+)");
                    }
                    Console.WriteLine($"s={s} best={best} trail={bestTrail}");
                    if (s < best) {
                        best = s;
                        bestTrail = trail;
                        if (best < overallBest) {
                            Console.WriteLine($"{string.Concat(keyRing)} = {best} **");
                            overallBestTrail = bestTrail;
                            overallBest = best;
                        }
                    }
                    foreach (var c in toRemove)
                        keyRing.Remove(c);
                }
                return (best, bestTrail);
            }

            List<NextKey> FindNext(Point start) {
                List<NextKey> next = new();
                Queue<Trail> queue = new();
                queue.Enqueue(new Trail(start.X, start.Y, 0, new List<char>(), new List<char>()));
                bool[,] visited = new bool[maxx, maxy];
                visited[start.X, start.Y] = true;
                char startC = input[start.Y][start.X];
                while (queue.Count > 0) {
                    var t = queue.Dequeue();
                    var c = input[t.Y][t.X];
                    visited[t.X, t.Y] = true;
                    if (c >= 'a' && c <= 'z' && c != startC) {
                        t.CollectedKeys = new List<char>(t.CollectedKeys) { c };
                        next.Add(new NextKey(keys[c], t.RequiredKeys, t.CollectedKeys, t.Steps));
                    }
                    if (c >= 'A' && c <= 'Z' && !next.Any(nk => nk.Key.Tag == c))
                        t.RequiredKeys = new List<char>(t.RequiredKeys) { char.ToLower(c) };
                    if (!visited[t.X - 1, t.Y] && input[t.Y][t.X - 1] != '#') queue.Enqueue(new Trail(t.X - 1, t.Y, t.Steps + 1, t.RequiredKeys, t.CollectedKeys));
                    if (!visited[t.X + 1, t.Y] && input[t.Y][t.X + 1] != '#') queue.Enqueue(new Trail(t.X + 1, t.Y, t.Steps + 1, t.RequiredKeys, t.CollectedKeys));
                    if (!visited[t.X, t.Y - 1] && input[t.Y - 1][t.X] != '#') queue.Enqueue(new Trail(t.X, t.Y - 1, t.Steps + 1, t.RequiredKeys, t.CollectedKeys));
                    if (!visited[t.X, t.Y + 1] && input[t.Y + 1][t.X] != '#') queue.Enqueue(new Trail(t.X, t.Y + 1, t.Steps + 1, t.RequiredKeys, t.CollectedKeys));
                }
                return next;
            }
        }
    }
}
