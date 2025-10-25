using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Chizl.FileCompare
{
    /// <summary>
    /// <code>
    /// Diff engine based on the Myers algorithm (O(ND) shortest edit script).
    /// 
    /// - Core algorithm: Myers diff (produces insert/delete operations).
    /// - Extension: A similarity check is applied post-process to collapse
    ///              adjacent delete+insert pairs into "modify" operations
    ///              when line similarity exceeds a threshold.
    /// 
    /// Input handling:
    /// - Files are read fully into arrays of lines (string[]).
    ///   This keeps the algorithm correct and efficient, since Myers requires
    ///   random access to both sequences.
    /// - Memory usage is linear in the number of lines, not quadratic as in
    ///   the LCS matrix approach. Large multi-MB files are now practical.
    /// 
    /// Notes:
    /// - This implementation is still "Myers" at its core, with an additional
    ///   similarity-based refinement layer.
    /// - A true streaming version (line-by-line without ToArray) is not feasible
    ///   for exact Myers, since the algorithm requires arbitrary lookahead.
    ///   Chunked/streamed approaches would be "Myers-inspired" rather than pure.
    /// </code>
    /// </summary>
    public sealed class DiffTool
    {
        /// <summary>
        /// Reutrns the file content in Binary Hex
        /// </summary>
        /// <param name="fullPath">Path to file to view as Hex</param>
        /// <returns>ByteLineLevel array.  Each ByteLineLevel holds offset, hex values, and printable characters.</returns>
        /// <exception cref="Exception">Any exception during access, open, or read</exception>
        public static IEnumerable<ByteLineLevel> ShowInHex(string fullPath)
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

                    yield return bhView;
                    offset += bytesRead;
                }
            }
        }
        /// <summary>
        /// Compares two files, either ASCII/text or binary.<br/>
        /// - For binary files, differences are computed using a custom Fnv1 algorythm. The result indicates IsBinary and can be displayed appropriately.<br/>
        /// - For ASCII files, differences are computed using Myers diff with optional modify detection.
        /// </summary>
        /// <param name="sourceFile">The source file (considered the older file).</param>
        /// <param name="targetFile">The target file (considered the newer file).</param>
        /// <param name="scoreThreshold">
        /// ASCII COMPARE ONLY: The similarity threshold (0–1) for a line to be considered a modification (~)<br/>
        /// instead of a separate Add (+) or Delete (-).
        /// </param>
        /// <param name="lineLookAhead">
        /// ASCII COMPARE ONLY: The number of subsequent lines to inspect when merging adjacent<br/>
        /// delete+insert pairs into modifications.
        /// </param>
        /// <returns>
        /// A <see cref="ComparisonResults"/> object with details for each line,<br/>
        /// and inline character-level highlights for modified lines.
        /// </returns>
        public static ComparisonResults CompareFiles(string sourceFile, string targetFile, double scoreThreshold = 0.30, byte lineLookAhead = 3)
        {
            if (string.IsNullOrWhiteSpace(sourceFile))
                return new ComparisonResults(new ArgumentException($"{nameof(sourceFile)}: cannot be empty."));
            if (string.IsNullOrWhiteSpace(targetFile))
                return new ComparisonResults(new ArgumentException($"{nameof(targetFile)}: cannot be empty."));

            try
            {
                var sourceFileInfo = new FileLevel(sourceFile);
                var targetFileInfo = new FileLevel(targetFile);

                if (!sourceFileInfo.Exists)
                    return new ComparisonResults(new ArgumentException($"{nameof(sourceFile)}: '{sourceFile}' is not found and/or accessible."));
                if (!targetFileInfo.Exists)
                    return new ComparisonResults(new ArgumentException($"{nameof(targetFile)}: '{targetFile}' is not found and/or accessible."));

                // Binary path unchanged
                if (sourceFileInfo.IsBinary || targetFileInfo.IsBinary)
                    return BinaryComparer.CompareFiles(sourceFileInfo, targetFileInfo);
                else
                    return CompareStringArr(sourceFileInfo, targetFileInfo, scoreThreshold, lineLookAhead);
            }
            catch (Exception ex) 
            {
                return new ComparisonResults(ex);
            }
        }
        /// <summary>
        /// Builds results off of scripts from Myers, then merges with an LCS to seperate Added, Deleted, and Modified lines.
        /// </summary>
        private static ComparisonResults CompareStringArr(FileLevel sourceFileInfo, FileLevel targetFileInfo, double scoreThreshold = 0.30, byte lineLookAhead = 3)
        {
            // Read text lines (Myers is O(N+M) memory). If you already have arrays, call CompareStringArr directly.
            string[] linesOld = sourceFileInfo.Content;
            string[] linesNew = targetFileInfo.Content;

            var retVal = new List<CompareDiff>();
            var lineNo = 0;

            while (scoreThreshold > 1.00) scoreThreshold /= 100.00;
            scoreThreshold = scoreThreshold.ClampTo(0.15, 0.85);

            // Get raw edit script from Myers
            var rawDiff = MyersDiff(linesOld, linesNew);

            // Merge nearby -/+ into ~ when similar (your previous logic)
            var finalDiff = MergeSimilarChanges(rawDiff, lineLookAhead, scoreThreshold);

            DiffType diffType = DiffType.None;
            var prevSize = 0;
            foreach (var entry in finalDiff)
            {
                switch (entry.Tag)
                {
                    case "+": diffType = DiffType.Added; break;
                    case "-": diffType = DiffType.Deleted; break;
                    case "~": diffType = DiffType.Modified; break;
                    default: diffType = DiffType.None; break;
                }

                var textLine = entry.Text;
                if (prevSize == 0) prevSize = 32;
                if (textLine.Length == 0) textLine = new string(' ', prevSize);

                retVal.Add(new CompareDiff(diffType, ++lineNo, textLine));
                prevSize = textLine.Length;
            }

            return new ComparisonResults(retVal, sourceFileInfo, targetFileInfo);
        }
        /// <summary>
        /// Robust Myers implementation (array V, trace)
        /// </summary>
        private static List<(string Tag, string Text)> MyersDiff(IList<string> a, IList<string> b)
        {
            int N = a.Count;
            int M = b.Count;
            int max = N + M;
            int offset = max;
            int size = 2 * max + 1;
            var V = new int[size];
            // V[offset + 1] = 0;  // default 0

            var trace = new List<int[]>();

            for (int d = 0; d <= max; d++)
            {
                var Vd = new int[size];
                Array.Copy(V, Vd, size);

                for (int k = -d; k <= d; k += 2)
                {
                    int idx = k + offset;
                    int x;
                    // choose down or right
                    if (k == -d || (k != d && V[idx - 1] < V[idx + 1]))
                        x = V[idx + 1];       // down (insertion)
                    else
                        x = V[idx - 1] + 1;   // right (deletion)

                    int y = x - k;

                    // snake: follow equal elements
                    while (x < N && y < M && string.Equals(a[x], b[y], StringComparison.Ordinal))
                    {
                        x++; y++;
                    }

                    Vd[idx] = x;

                    if (x >= N && y >= M)
                    {
                        trace.Add(Vd);
                        goto BACKTRACK;
                    }
                }

                V = Vd;
                trace.Add(Vd);
            }

        BACKTRACK:
            int rx = N, ry = M;
            var edits = new List<(char op, int i1, int i2, int j1, int j2)>();

            for (int d = trace.Count - 1; d >= 0; d--)
            {
                var Vd = trace[d];
                int k = rx - ry;
                int idx = k + offset;

                int prevK;
                if (k == -d || (k != d && Vd[idx - 1] < Vd[idx + 1]))
                    prevK = k + 1;
                else
                    prevK = k - 1;

                int prevIdx = prevK + offset;
                int prevX = Vd[prevIdx];
                int prevY = prevX - prevK;

                // snake (matches)
                while (rx > prevX && ry > prevY)
                {
                    edits.Add((' ', rx - 1, rx, ry - 1, ry));
                    rx--; ry--;
                }

                if (d > 0)
                {
                    if (rx == prevX)
                    {
                        // insertion(s) in b
                        edits.Add(('+', rx, rx, prevY, ry));
                    }
                    else
                    {
                        // deletion(s) from a
                        edits.Add(('-', prevX, rx, ry, ry));
                    }
                }

                rx = prevX;
                ry = prevY;
            }

            edits.Reverse();

            // Flatten segments to tag/text entries
            var result = new List<(string, string)>();
            foreach (var e in edits)
            {
                if (e.op == ' ')
                {
                    for (int i = e.i1; i < e.i2; i++)
                        result.Add((" ", $"{a[i]}{Environment.NewLine}"));
                }
                else if (e.op == '-')
                {
                    for (int i = e.i1; i < e.i2; i++)
                        result.Add(("-", $"{a[i]}{Environment.NewLine}"));
                }
                else if (e.op == '+')
                {
                    for (int j = e.j1; j < e.j2; j++)
                        result.Add(("+", $"{b[j]}{Environment.NewLine}"));
                }
            }
            return result;
        }
        /// <summary>
        /// Highlight character-level changes for modified lines.
        /// Uses a small dynamic-programming LCS table (safe since lines are short).
        /// This is separate from the main file-level Myers diff.
        /// </summary>        
        private static string HighlightCharChanges(string oldLine, string newLine)
        {
            var oldChars = oldLine.Select(c => c.ToString()).ToArray();
            var newChars = newLine.Select(c => c.ToString()).ToArray();

            var lcs = BuildLcsTable(oldChars, newChars);
            return BuildHighlightedDiff(lcs, oldChars, newChars, oldChars.Length, newChars.Length);
        }
        /// <summary>
        /// Post-processing step for Myers diff results.
        /// - Scans for delete+insert pairs within a lookahead window.
        /// - If similarity exceeds a threshold, merges them into a "~" (modified).
        /// - Calls HighlightCharChanges to compute fine-grained edits.
        /// 
        /// This is not part of Myers; it is a refinement layer.
        /// </summary>
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
        /// <summary>
        /// Simple similarity heuristic for line-level comparison.
        /// Counts matching characters in the same position.
        /// </summary>
        private static double Similarity(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)) return 1.0;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0.0;

            int same = a.Zip(b, (ca, cb) => ca == cb ? 1 : 0).Sum();
            return (double)same / Math.Max(a.Length, b.Length);
        }
        /// <summary>
        /// Build a small LCS table for character-level diff inside a single line.
        /// Complexity is trivial because lines are short.
        /// </summary>
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
        /// <summary>
        /// Recursively build a string with inline [+insert] and [-delete] markers
        /// based on the per-line LCS table.
        /// </summary>
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
