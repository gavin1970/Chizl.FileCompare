using Chizl.FileCompare;
using System;

namespace ConsoleDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SingleLineTest();
        }

        static void SingleLineTest()
        {
            var src = "abcdefjhijklmnop qrstuvwxyz123465789!";
            var trg = "abcdfjhijklemnop qrsuvwxtyz123456789!";


            Console.WriteLine("----====[ Single Line String Compare ]====----\n");

            Console.WriteLine($"Source: {src.Replace("\n", "?")}");
            Console.WriteLine($"Target: {trg.Replace("\n", "?")}\n");

            Console.WriteLine("Ascii DIY View:");

            var fileComparison = DiffTool.CompareString(src, trg);
            foreach (var cmpr in fileComparison.LineComparison)
                Console.WriteLine(cmpr.LineDiffStr.Replace("\n", "?"));

            Console.WriteLine("\nBinary View (As Overlay):");

            var line = 0;
            var hexCount = 0;
            var printableText = "";
            foreach (var cmpr in fileComparison.LineComparison)
            {
                foreach (var byteLevel in cmpr.TextBreakDown)
                {
                    if (hexCount == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write($"{line:X8}h \x1b[0m");
                        line += 16;
                        printableText = "";
                    }

                    hexCount++;

                    switch (byteLevel.DiffType)
                    {
                        case DiffType.Added:
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case DiffType.Deleted:
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case DiffType.Modified:
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        default:
                            break;
                    }

                    // - "\x1b[0m" - Resets all FG and BG colors from the point of entry
                    // and there after until new color is set within the console screen.
                    Console.Write($"{byteLevel.Hex} \x1b[0m");
                    printableText += (byteLevel.Char == 9 || byteLevel.Char == 10 || byteLevel.Char == 13) ? "." : byteLevel.Str;

                    if (hexCount == 16)
                    {
                        hexCount = 0;
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" {printableText} \x1b[0m");
                    }
                }
            }

            if (hexCount < 16)
            {
                var padHex = new string(' ', (16 * 3) - (hexCount * 3));
                var padPrint = new string(' ', 16 - hexCount);
                
                Console.Write(padHex);
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($" {printableText}{padPrint} \x1b[0m");
                hexCount = 0;
            }


            Console.ReadKey(true);
        }
    }
}
