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
        public static void Day22() {
            Console.WriteLine("Day22: ");
            Stopwatch sw = Stopwatch.StartNew();
            var input = File.ReadAllText("Aoc2020\\Day22\\input.txt").Split("\r\n\r\n");
            var player1 = new Queue<char>(input[0].Split("\r\n").Skip(1).Select(c => (char)int.Parse(c)));
            var player2 = new Queue<char>(input[1].Split("\r\n").Skip(1).Select(c => (char)int.Parse(c)));

            // part 1
            while (player1.Count > 0 && player2.Count > 0) {
                var c1 = player1.Dequeue();
                var c2 = player2.Dequeue();
                if (c1 > c2) {
                    player1.Enqueue(c1);
                    player1.Enqueue(c2);
                } else {
                    player2.Enqueue(c2);
                    player2.Enqueue(c1);
                }
            }
            int score = WinningScore(player1, player2);
            sw.Stop();
            Console.WriteLine($"{score} in {sw.ElapsedMilliseconds}ms");

            // part 2
            sw.Restart();

            //Dictionary<string, int> memo_winners = new(); // doesn't actually help !
            player1 = new Queue<char>(input[0].Split("\r\n").Skip(1).Select(c => (char)int.Parse(c)));
            player2 = new Queue<char>(input[1].Split("\r\n").Skip(1).Select(c => (char)int.Parse(c)));
            PlayRecursive(player1, player2);
            sw.Stop();
            score = WinningScore(player1, player2);
            sw.Stop();
            Console.WriteLine($"{score} in {sw.ElapsedMilliseconds}ms");

            static string GameState(Queue<char> player1, Queue<char> player2) {
                char[] state = new char[player1.Count + player2.Count + 1];
                player1.CopyTo(state, 0);
                player2.CopyTo(state, player1.Count + 1);
                return new string(state);
            }

            static int PlayRecursive(Queue<char> player1, Queue<char> player2) {
                //string game_state = GameState(player1, player2);
                //if (memo_winners.TryGetValue(game_state, out int game_winner))
                //    return game_winner;
                var prevRounds = new HashSet<string>();
                while (player1.Count > 0 && player2.Count > 0) {
                    var state = GameState(player1, player2);
                    if (!prevRounds.Add(state)) {
                        player2.Clear();
                        break;
                    }
                    var c1 = player1.Dequeue();
                    var c2 = player2.Dequeue();
                    int winner;
                    if (player1.Count >= c1 && player2.Count >= c2)
                        winner = PlayRecursive(new Queue<char>(player1.Take(c1)), new Queue<char>(player2.Take(c2)));
                    else
                        winner = (c1 > c2) ? 1 : 2;
                    if (winner == 1) {
                        player1.Enqueue(c1);
                        player1.Enqueue(c2);
                    } else {
                        player2.Enqueue(c2);
                        player2.Enqueue(c1);
                    }
                }
                int game_winner = player1.Count > 0 ? 1 : 2;
                //memo_winners[game_state] = game_winner;
                return game_winner;
            }
        }

        private static int WinningScore(Queue<char> player1, Queue<char> player2) {
            Console.WriteLine($"Player {(player1.Count > 0 ? 1 : 2)} wins!");
            var winner = player1.Count > player2.Count ? player1 : player2;
            int score = 0;
            int mult = winner.Count;
            foreach(var c in winner) 
                score += mult-- * c;
            return score;
        }
    }
}
