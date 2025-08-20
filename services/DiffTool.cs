using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
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
        /// Also can return FileComparison.HasException bool property where the FileComparison.Exception is the object in question.
        /// </returns>
        public static FileComparison CompareFiles(string sourceFile, string targetFile, double scoreThreshold = 0.30, byte lineLookAhead = 3)
        {
            if (!File.Exists(sourceFile))
                return new FileComparison(new ArgumentException($"{nameof(sourceFile)}: '{sourceFile}' is not found or accessible."));
            if (!File.Exists(targetFile))
                return new FileComparison(new ArgumentException($"{nameof(targetFile)}: '{targetFile}' is not found or accessible."));

            if (IsBinyary(sourceFile))
                return new FileComparison(new Exception($"'{sourceFile}' is a binary file."));
            else if (IsBinyary(sourceFile))
                return new FileComparison(new Exception($"'{targetFile}' is a binary file."));
            else
            {
                var linesOld = File.ReadAllLines(sourceFile);
                var linesNew = File.ReadAllLines(targetFile);
                return CompareStringArr(linesOld, linesNew, scoreThreshold, lineLookAhead);
            }
        }
        /// <summary>
        /// Compare two ascii strings and return line for line what was added, removed, and modified.<br/>
        /// Each modifiied will have single char brackets around each add or removed.<br/>
        /// By modifying the score threshold will provide the level of detail you are loooking for.<br/>
        /// <code>
        /// Example: 
        ///     var fileComparison = DiffTool.CompareString("abcdefjhijklmnopqrstuvwxyz", "abcdfjhijklemnopqrsuvwxtyz");
        ///     foreach (var cmpr in fileComparison.LineComparison)
        ///         Console.WriteLine(cmpr.LineDiffStr);
        /// Results:
        ///     abcd[-e]fjhijkl[+e]mnopqrs[-t]uvwx[+t]yz
        /// </code>
        /// </summary>
        /// <param name="srcText">Older string to compare</param>
        /// <param name="trgText">Newier string to compare</param>
        /// <returns>
        /// File details and byte by comparison<br/>
        /// Also can return FileComparison.HasException bool property where the FileComparison.Exception is the object in question.
        /// </returns>
        public static FileComparison CompareString(string srcText, string trgText) =>
            CompareStringArr(new string[] { srcText }, new string[] { trgText }, .10);
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
        /// Also can return FileComparison.HasException bool property where the FileComparison.Exception is the object in question.
        /// </returns>
        public static FileComparison CompareStringArr(string[] linesOld, string[] linesNew, double scoreThreshold = 0.30, byte lineLookAhead = 3)
        {
            var retVal = new List<CompareDiff>();
            var lineNo = 0;

            while (scoreThreshold > 1.00) scoreThreshold /= 100.00;
            scoreThreshold = scoreThreshold.ClampTo(0.15, 0.85);

            var lcs = BuildLcsTable(linesOld, linesNew);

            var rawDiff = new List<(string Tag, string Text)>();
            BuildRawDiff(lcs, linesOld, linesNew, linesOld.Length, linesNew.Length, rawDiff);

            var finalDiff = MergeSimilarChanges(rawDiff, lineLookAhead, scoreThreshold); // 3-line lookahead window
            DiffType diffType = DiffType.NotSet;

            foreach (var entry in finalDiff)
            {
                lineNo++;
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
                retVal.Add(new CompareDiff(diffType, lineNo, entry.Text));
            }

            return new FileComparison(retVal);

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
        private static bool IsBinyary(string fullPath)
        {
            var maxRead = 1024;
            var buffer = new byte[maxRead];

            using (var strm = File.OpenRead(fullPath))
            {
                if (strm.Length < maxRead)
                    maxRead = (int)strm.Length;

                strm.Read(buffer, 0, maxRead);
                if (buffer.Where(c => c == '\0').Count() > 0)
                    return true;
            }

            return false;
        }
    }
}
