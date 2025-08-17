using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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
        /// <returns>File details and line by line comparison</returns>
        public static FileComparison CompareFiles(string sourceFile, string targetFile, double scoreThreshold = 0.30)
        {
            if (!File.Exists(sourceFile))
                throw new ArgumentException($"{nameof(sourceFile)}: '{sourceFile}' is not found or accessible.");
            if (!File.Exists(targetFile))
                throw new ArgumentException($"{nameof(targetFile)}: '{targetFile}' is not found or accessible.");

            while (scoreThreshold > 1.00) scoreThreshold /= 100.00;
            scoreThreshold = scoreThreshold.ClampTo(0.15, 0.85);

            var lineNo = 0;
            var retVal = new List<CompareDiff>();
            var linesOld = File.ReadAllLines(sourceFile);
            var linesNew = File.ReadAllLines(targetFile);
            var lcs = BuildLcsTable(linesOld, linesNew);

            var rawDiff = new List<(string Tag, string Text)>();
            BuildRawDiff(lcs, linesOld, linesNew, linesOld.Length, linesNew.Length, rawDiff);

            var finalDiff = MergeSimilarChanges(rawDiff, 3, scoreThreshold); // 3-line lookahead window
            DiffType diffType = DiffType.NotSet;

            foreach (var entry in finalDiff)
            {
                // Debug.WriteLine($"{entry.Tag} {entry.Text}");
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
        
        public static string CompareString(string str1, string str2) => HighlightCharChanges(str1, str2);

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
