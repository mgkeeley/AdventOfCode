using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC
{
    public partial class Aoc2020
    {
        public static void Day4() {
            Console.WriteLine("Day4: ");
            string[] passport_data = File.ReadAllText("Aoc2020\\Day04\\input.txt").Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Replace("\r\n", " ")).ToArray();

            List<Dictionary<string, string>> passports = passport_data.Select(s =>
                 s.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                 .Select(s => s.Split(':'))
                 .ToDictionary(x => x[0], x => x[1])).ToList();

            string[] req_fields = { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
            int count = 0;
            int count_verified = 0;
            foreach(var passport in passports) {
                // part 1
                bool valid = true;
                foreach(var f in req_fields) {
                    if (!passport.ContainsKey(f))
                        valid = false;
                }
                // part 2
                if (valid) {
                    count++;
                    if (passport["byr"].Length != 4 || !int.TryParse(passport["byr"], out int byr) || byr < 1920 || byr > 2002)
                        valid = false;
                    if (passport["iyr"].Length != 4 || !int.TryParse(passport["iyr"], out int iyr) || iyr < 2010 || iyr > 2020)
                        valid = false;
                    if (passport["eyr"].Length != 4 || !int.TryParse(passport["eyr"], out int eyr) || eyr < 2020 || eyr > 2030)
                        valid = false;
                    var m = Regex.Match(passport["hgt"], "^([0-9]+)(cm|in)$");
                    if (!m.Success
                        || (m.Groups[2].Value == "cm" && (int.Parse(m.Groups[1].Value) < 150 || int.Parse(m.Groups[1].Value) > 193))
                        || (m.Groups[2].Value == "in" && (int.Parse(m.Groups[1].Value) < 59 || int.Parse(m.Groups[1].Value) > 76))) {
                        valid = false;
                    }
                    if (!Regex.IsMatch(passport["hcl"], "^#[0-9a-f]{6}$"))
                        valid = false;
                    if (!Regex.IsMatch(passport["ecl"], "^(amb|blu|brn|gry|grn|hzl|oth)$"))
                        valid = false;
                    if (!Regex.IsMatch(passport["pid"], "^[0-9]{9}$"))
                        valid = false;
                    if (valid)
                        count_verified++;
                }
            }

            Console.WriteLine(count);
            Console.WriteLine(count_verified);
        }

    }
}
