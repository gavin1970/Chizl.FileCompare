/*  

// Add forward slash at beginning of line 1 to uncomment all code below.
// This code doesn't have an issue, it's return isn't readable for side by side comparisons.

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Chizl.FileCompare
{
    public class AnchorDiff
    {
        public static void CompareFiles(string sourceFile, string targetFile)
        {
            var linesOld = File.ReadAllLines(sourceFile);
            var linesNew = File.ReadAllLines(targetFile);
            var rawDiff = BuildRawDiffWithAnchors(linesOld, linesNew, 5); // window = 5 lines

            foreach (var entry in rawDiff)
                Console.WriteLine($"{entry.Tag} {entry.Text}");
        }

        private static List<(string Tag, string Text)> BuildRawDiffWithAnchors(string[] oldLines, string[] newLines, int window)
        {
            var diff = new List<(string Tag, string Text)>();
            int i = 0, j = 0;

            while (i < oldLines.Length || j < newLines.Length)
            {
                if (i < oldLines.Length && j < newLines.Length && oldLines[i] == newLines[j])
                {
                    diff.Add((" ", oldLines[i]));
                    i++; j++;
                    continue;
                }

                // Lookahead anchor search
                int bestOld = -1, bestNew = -1;
                double bestScore = 0;
                for (int di = 0; di <= window && i + di < oldLines.Length; di++)
                {
                    for (int dj = 0; dj <= window && j + dj < newLines.Length; dj++)
                    {
                        double score = Similarity(oldLines[i + di], newLines[j + dj]);
                        if (score > 0.6 && score > bestScore)
                        {
                            bestScore = score;
                            bestOld = i + di;
                            bestNew = j + dj;
                        }
                    }
                }

                if (bestOld != -1 && bestNew != -1)
                {
                    // Lines before the anchor are removals/additions
                    for (int k = i; k < bestOld; k++)
                        diff.Add(("-", oldLines[k]));

                    for (int k = j; k < bestNew; k++)
                        diff.Add(("+", newLines[k]));

                    // Highlight difference at anchor line if not exact match
                    if (oldLines[bestOld] != newLines[bestNew])
                        diff.Add(("~", HighlightCharChanges(oldLines[bestOld], newLines[bestNew])));
                    else
                        diff.Add((" ", oldLines[bestOld]));

                    i = bestOld + 1;
                    j = bestNew + 1;
                }
                else
                {
                    // No good anchor, treat as add/remove
                    if (i < oldLines.Length)
                    {
                        diff.Add(("-", oldLines[i]));
                        i++;
                    }

                    if (j < newLines.Length)
                    {
                        diff.Add(("+", newLines[j]));
                        j++;
                    }
                }
            }
            return diff;
        }

        private static double Similarity(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
                return 0;
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
        
        private static int[,] BuildLcsTable(string[] a, string[] b)
        {
            int m = a.Length, n = b.Length;
            var table = new int[m + 1, n + 1];

            for (int i = 1; i <= m; i++)
                for (int j = 1; j <= n; j++)
                    table[i, j] = a[i - 1] == b[j - 1] ? table[i - 1, j - 1] + 1 : Math.Max(table[i - 1, j], table[i, j - 1]);

            return table;
        }
        
        private static string BuildHighlightedDiff(int[,] lcs, string[] a, string[] b, int i, int j)
        {
            if (i > 0 && j > 0 && a[i - 1] == b[j - 1])
                return BuildHighlightedDiff(lcs, a, b, i - 1, j - 1) + a[i - 1];
            else if (j > 0 && (i == 0 || lcs[i, j - 1] >= lcs[i - 1, j]))
                return BuildHighlightedDiff(lcs, a, b, i, j - 1) + "[+" + b[j - 1] + "]";
            else if (i > 0 && (j == 0 || lcs[i, j - 1] < lcs[i - 1, j]))
                return BuildHighlightedDiff(lcs, a, b, i - 1, j) + "[-" + a[i - 1] + "]";

            return "";
        }
    }
}
/**/