using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public partial class Aoc2019
    {
        public static void Day8() {
            Console.WriteLine("Day8: ");
            string pix = File.ReadAllText("Aoc2019\\Day08\\input.txt");

            // part 1
            int min0 = int.MaxValue;
            int mul12 = 0;
            
            for(int i = 0; i < pix.Length / (25*6); i++) {
                string layer = pix.Substring(i * 25 * 6, 25 * 6);
                int count0 = layer.Count(c => c == '0');
                int count1 = layer.Count(c => c == '1');
                int count2 = layer.Count(c => c == '2');
                if (count0 < min0) {
                    min0 = count0;
                    mul12 = count1 * count2;
                }
            }
            Console.WriteLine(mul12);

            // part 2
            char[] img = new char[25*6];
            Array.Fill(img, ' ');
            for (int i = 0; i < pix.Length / (25 * 6); i++) {
                string layer = pix.Substring(i * 25 * 6, 25 * 6).Replace('2', ' ').Replace('0', '_').Replace('1', '▓');
                for(int x = 0; x < 25*6; x++) {
                    if (img[x] == ' ')
                        img[x] = layer[x];
                }
            }
            for (int y = 0; y < 6; y++) 
                Console.WriteLine(new string(img.AsSpan(y*25, 25)));
        }
    }
}
