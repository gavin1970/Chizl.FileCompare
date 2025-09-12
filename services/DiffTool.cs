using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Chizl.FileCompare
{
    public sealed class DiffTool
    {
        /// <summary>
        /// Compare two ascii files and return line for line what was added, removed, and modified.<br/>
        /// Each modifiied will have single char brackets around each add or removed.<br/>
        /// By modifying the score threshold will provide the level of detail you are loooking for.<br/>
        /// </summary>
        /// <param name="sourceFile">Older file to compare</param>
        /// <param name="targetFile">Newier file to compare</param>
        /// <param name="scoreThreshold">Default 30%.  Highter percentages will give less modified and show added/deleted lines instead.</param>
        /// <param name="lineLookAhead">For multi line ascii files, how far ahead to look for possible matches?</param>
        /// <returns>
        /// File details and line by line comparison<br/>
        /// Also can return ComparisonResults.HasException bool property where the ComparisonResults.Exception is the object in question.
        /// </returns>
        public static ComparisonResults CompareFiles(string sourceFile, string targetFile, double scoreThreshold = 0.30, byte lineLookAhead = 3)
        {
            var sourceFileInfo = new FileLevel(sourceFile);
            var targetFileInfo = new FileLevel(targetFile);

            if (!sourceFileInfo.Exists)
                return new ComparisonResults(new ArgumentException($"{nameof(sourceFile)}: '{sourceFile}' is not found and/or accessible."));
            if (!targetFileInfo.Exists)
                return new ComparisonResults(new ArgumentException($"{nameof(targetFile)}: '{targetFile}' is not found and/or accessible."));

            var isSrcBinary = sourceFileInfo.IsBinary;
            var isTrgBinary = targetFileInfo.IsBinary;

            //if one of them is binary, view them both as binary else both as ascii.
            if (isSrcBinary || isTrgBinary)
                return BinaryComparer.CompareFiles(sourceFileInfo, targetFileInfo);
            else
                return CompareStringArr(sourceFileInfo.Content, targetFileInfo.Content, scoreThreshold, lineLookAhead);
        }
        /// <summary>
        /// Compare two ascii strings and return line for line what was added, removed, and modified.<br/>
        /// Each modifiied will have single char brackets around each add, modified, or removed.<br/>
        /// Because this is for small one or two line strings, this defaults the threashold to 10%.<br/>
        /// <code>
        /// Example: 
        ///     var fileComparison = DiffTool.CompareString("abcdefjhijklmnopqrstuvwxyz", "abcdfjhijklemnopqrsuvwxtyz");
        ///     foreach (var cmpr in fileComparison.LineComparison)
        ///         Console.WriteLine(cmpr.LineDiffStr);
        /// Results:
        ///     abcd[-e]fjhijkl[+e]mnopqrs[-t]uvwx[+t]yz
        ///     
        /// Binary View:
        ///     foreach (var cmpr in fileComparison.LineComparison)
        ///     {
        ///         var printableText = "";
        ///         foreach(var byteLevel int cmpr.ByteByByteDiff)
        ///         {
        ///             Console.BackgroundColor = ConsoleColor.Green;
        ///             switch (byteLevel.DiffType)
        ///             {
        ///                 case DiffType.Added:
        ///                     Console.BackgroundColor = ConsoleColor.Green;
        ///                     Console.ForegroundColor = ConsoleColor.White;
        ///                     break;
        ///                 case DiffType.Deleted:
        ///                     Console.BackgroundColor = ConsoleColor.Red;
        ///                     Console.ForegroundColor = ConsoleColor.White;
        ///                     break;
        ///                 case DiffType.Modified:
        ///                     Console.BackgroundColor = ConsoleColor.Yellow;
        ///                     Console.ForegroundColor = ConsoleColor.Black;
        ///                     break;
        ///                 default:
        ///                     break;
        ///             }
        ///             
        ///             // - "\x1b[0m" - Resets all FG and BG colors from the point of entry and there after until new color is set within the console screen.
        ///             Console.Write($"{byteLevel.Hex}\x1b[0m ");
        ///             printableText += byteLevel.Str;
        ///         }
        ///         Console.WriteLine($" {printableText}");
        ///     }
        /// </code>
        /// </summary>
        /// <param name="srcText">Older string to compare</param>
        /// <param name="trgText">Newier string to compare</param>
        /// <returns>
        /// File details and byte by comparison<br/>
        /// Also can return ComparisonResults.HasException bool property where the ComparisonResults.Exception is the object in question.
        /// </returns>
        public static ComparisonResults CompareString(string srcText, string trgText)
        {
            var srcLine = srcText.Replace("\r", "").Split('\n');
            var trgLine = trgText.Replace("\r", "").Split('\n');

            return CompareStringArr(srcLine, trgLine, .10);
        }
        /// <summary>
        /// Compare two ascii string arrays and return line for line what was added, removed, and modified.<br/>
        /// Each modifiied will have single char brackets around each add or removed.<br/>
        /// By modifying the score threshold will provide the level of detail you are loooking for.<br/>
        /// </summary>
        /// <param name="linesOld">Older string array to compare</param>
        /// <param name="linesNew">Newier string array to compare</param>
        /// <param name="scoreThreshold">Default 30%.  Highter percentages will give less modified and show added/deleted lines instead.</param>
        /// <param name="lineLookAhead">For the array list, how far ahead to look for possible matches?</param>
        /// <returns>
        /// File details and line by line comparison<br/>
        /// Also can return ComparisonResults.HasException bool property where the ComparisonResults.Exception is the object in question.
        /// </returns>
        public static ComparisonResults CompareStringArr(string[] linesOld, string[] linesNew, double scoreThreshold = 0.30, byte lineLookAhead = 3)
        {
            var retVal = new List<CompareDiff>();
            var lineNo = 0;

            while (scoreThreshold > 1.00) scoreThreshold /= 100.00;
            scoreThreshold = scoreThreshold.ClampTo(0.15, 0.85);

            var lcs = BuildLcsTable(linesOld, linesNew);

            var rawDiff = new List<(string Tag, string Text)>();
            BuildRawDiff(lcs, linesOld, linesNew, linesOld.Length, linesNew.Length, rawDiff);

            var finalDiff = MergeSimilarChanges(rawDiff, lineLookAhead, scoreThreshold); // 3-line lookahead window
            DiffType diffType = DiffType.None;
            var prevSize = 0;
            foreach (var entry in finalDiff)
            {
                switch (entry.Tag)
                {
                    case "+":
                        diffType = DiffType.Added;
                        break;
                    case "-":
                        diffType = DiffType.Deleted;
                        break;
                    case "~":
                        diffType = DiffType.Modified;
                        break;
                    default:
                        diffType = DiffType.None;
                        break;
                }

               
                //get line
                var textLine = entry.Text;

                //if line is zero in size and previous was zero in size, force 30 as a length
                if (prevSize == 0)
                    prevSize = 30;                  //random


                if (textLine.Length == 0)
                    textLine = new string(' ', prevSize);
                
                retVal.Add(new CompareDiff(diffType, ++lineNo, textLine));
                prevSize = textLine.Length;
            }

            return new ComparisonResults(retVal, false);

        }
        /// <summary>
        /// Load files as binary, then convert to string hex format for side by side view of next binary load file.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="returnArr"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static ByteLineLevel[] ShowInHex(string fullPath)
        {
            var binHexViewList = new List<ByteLineLevel>();

            try
            {
                using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    int bytesRead;
                    byte[] buffer = new byte[16]; // Read 16 bytes at a time for a typical hex view layout
                    int offset = 0;
                    int lineNo = 1;
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        var bhView = new ByteLineLevel(FileFormat.Binary, lineNo++);
                        // Display offset
                        bhView.AddOffset($"{offset:X8}:");

                        // Display hex values
                        for (int i = 0; i < bytesRead; i++)
                            bhView.AddToHexStr((char)buffer[i]);

                        binHexViewList.Add(bhView);
                        offset += bytesRead;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception in ShowInHex(\"{fullPath}\"):\n{ex.Message}");
            }

            return binHexViewList.ToArray();
        }

        private static int[,] BuildLcsTable(string[] a, string[] b)
        {
            int m = a.Length, n = b.Length;
            var table = new int[m + 1, n + 1];

            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (a[i - 1] == b[j - 1])
                        table[i, j] = table[i - 1, j - 1] + 1;
                    else
                        table[i, j] = Math.Max(table[i - 1, j], table[i, j - 1]);
                }
            }
            return table;
        }
        private static void BuildRawDiff(int[,] lcs, string[] a, string[] b, int i, int j, List<(string, string)> output)
        {
            if (i > 0 && j > 0 && a[i - 1] == b[j - 1])
            {
                BuildRawDiff(lcs, a, b, i - 1, j - 1, output);
                output.Add((" ", a[i - 1]));
            }
            else if (j > 0 && (i == 0 || lcs[i, j - 1] >= lcs[i - 1, j]))
            {
                BuildRawDiff(lcs, a, b, i, j - 1, output);
                output.Add(("+", b[j - 1]));
            }
            else if (i > 0 && (j == 0 || lcs[i, j - 1] < lcs[i - 1, j]))
            {
                BuildRawDiff(lcs, a, b, i - 1, j, output);
                output.Add(("-", a[i - 1]));
            }
        }
        private static List<(string Tag, string Text)> MergeSimilarChanges(List<(string Tag, string Text)> diff, int windowSize, double scoreThreshold)
        {
            var merged = new List<(string, string)>();

            for (int i = 0; i < diff.Count; i++)
            {
                if (diff[i].Tag == "-")
                {
                    int matchIndex = -1;
                    double bestScore = 0;

                    // look ahead for '+' within the window
                    for (int w = 1; w <= windowSize && i + w < diff.Count; w++)
                    {
                        if (diff[i + w].Tag == "+")
                        {
                            double score = Similarity(diff[i].Text, diff[i + w].Text);
                            if (score > scoreThreshold && score > bestScore)
                            {
                                bestScore = score;
                                matchIndex = i + w;
                            }
                        }
                    }

                    if (matchIndex != -1)
                    {
                        merged.Add(("~", HighlightCharChanges(diff[i].Text, diff[matchIndex].Text)));

                        // copy everything between - and + except the matched + itself
                        for (int k = i + 1; k < matchIndex; k++)
                            merged.Add(diff[k]);

                        i = matchIndex; // skip ahead to after matched +
                        continue;
                    }
                }
                merged.Add(diff[i]);
            }
            return merged;
        }
        private static double Similarity(string a, string b)
        {
            int same = a.Zip(b, (ca, cb) => ca == cb ? 1 : 0).Sum();
            return (double)same / Math.Max(a.Length, b.Length);
        }
        private static string HighlightCharChanges(string oldLine, string newLine)
        {
            var oldChars = oldLine.Select(c => c.ToString()).ToArray();
            var newChars = newLine.Select(c => c.ToString()).ToArray();

            var lcs = BuildLcsTable(oldChars, newChars);
            return BuildHighlightedDiff(lcs, oldChars, newChars, oldChars.Length, newChars.Length);
        }
        private static string BuildHighlightedDiff(int[,] lcs, string[] a, string[] b, int i, int j)
        {
            if (i > 0 && j > 0 && a[i - 1] == b[j - 1])
            {
                return BuildHighlightedDiff(lcs, a, b, i - 1, j - 1) + a[i - 1];
            }
            else if (j > 0 && (i == 0 || lcs[i, j - 1] >= lcs[i - 1, j]))
            {
                return BuildHighlightedDiff(lcs, a, b, i, j - 1) + "[+" + b[j - 1] + "]";
            }
            else if (i > 0 && (j == 0 || lcs[i, j - 1] < lcs[i - 1, j]))
            {
                return BuildHighlightedDiff(lcs, a, b, i - 1, j) + "[-" + a[i - 1] + "]";
            }
            return "";
        }
    }
}
