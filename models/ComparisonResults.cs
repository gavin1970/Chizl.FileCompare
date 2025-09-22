using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Chizl.FileCompare
{
    /// <summary>
    /// Represents the results of comparing two files (ASCII or binary).
    /// Provides detailed line-by-line differences, counts of change types,
    /// and status information such as exceptions or binary detection.
    /// </summary>
    public class ComparisonResults
    {
        /// <summary>
        /// Private constructor for creating an empty ComparisonResults instance.
        /// Used by the static <see cref="Empty"/> property.
        /// </summary>
        private ComparisonResults()
        {
            IsEmpty = true;
        }

        /// <summary>
        /// Internal constructor used when an exception occurs while loading files.
        /// </summary>
        /// <param name="ex">The exception that occurred during comparison.</param>
        internal ComparisonResults(Exception ex)
        {
            IsEmpty = true;
            Exception = ex;
            IsBinary = false;
        }

        /// <summary>
        /// Internal constructor used when comparison completes successfully.
        /// </summary>
        /// <param name="compareDiffs">List of per-line comparison results.</param>
        /// <param name="isBinary">True if the files compared were binary; otherwise false.</param>
        internal ComparisonResults(List<CompareDiff> compareDiffs, FileLevel sourceFile, FileLevel targetFile)
        {
            this.SourceFile = sourceFile;
            this.TargetFile = targetFile;
            this.IsBinary = sourceFile.IsBinary || targetFile.IsBinary;
            this.LineComparison = compareDiffs.ToArray();

            //binary needs to be done by byte, not line
            if (this.IsBinary)
            {
                this.Diffs = new DiffCounts(
                    compareDiffs.Where(w => w.DiffType == DiffType.Added).Select(s => s.ByteByByteDiff.Length).Sum(),
                    compareDiffs.Where(w => w.DiffType == DiffType.Deleted).Select(s => s.ByteByByteDiff.Length).Sum(),
                    compareDiffs.Where(w => w.DiffType == DiffType.Modified).Select(s => s.ByteByByteDiff.Length).Sum(),
                    compareDiffs.Where(w => w.DiffType == DiffType.None).Select(s => s.ByteByByteDiff.Length).Sum()
                );
            }
            else
            { 
                this.Diffs = new DiffCounts(
                    compareDiffs.Count(w => w.DiffType == DiffType.Added),
                    compareDiffs.Count(w => w.DiffType == DiffType.Deleted),
                    compareDiffs.Count(w => w.DiffType == DiffType.Modified),
                    compareDiffs.Count(w => w.DiffType == DiffType.None)
                );
            }
        }

        /// <summary>
        /// Returns an empty <see cref="ComparisonResults"/> instance.
        /// The <see cref="IsEmpty"/> property will be set to true.
        /// </summary>
        public static ComparisonResults Empty { get; } = new ComparisonResults();

        /// <summary>
        /// Source is usually reguarding the oldest file and the first argument in DiffTools.CompareFiles().  This holds information about the source file.
        /// </summary>
        public FileLevel SourceFile { get; }

        /// <summary>
        /// Target is usually reguarding the newest file and the second argument in DiffTools.CompareFiles().  This holds information about the target file.
        /// </summary>
        public FileLevel TargetFile { get; }

        /// <summary>
        /// Indicates whether this instance contains meaningful comparison data.
        /// True if the instance is empty or created due to an exception.
        /// </summary>
        public bool IsEmpty { get; } = false;

        /// <summary>
        /// Indicates whether the files compared were binary.
        /// </summary>
        public bool IsBinary { get; } = false;

        /// <summary>
        /// Returns true if an exception occurred during file comparison.
        /// </summary>
        public bool HasException => this.Exception != null;

        /// <summary>
        /// Provides the exception that occurred when comparing files, if any.
        /// This is typically set when a file is missing or cannot be read.
        /// </summary>
        public Exception Exception { get; } = null;

        /// <summary>
        /// Array of per-line comparison results.
        /// Each <see cref="CompareDiff"/> indicates the line content and type of change.
        /// </summary>
        public CompareDiff[] LineComparison { get; } = new CompareDiff[0];

        /// <summary>
        /// Aggregated counts of differences found in the comparison.
        /// Tracks number of Added, Deleted, Modified, and Unchanged lines.
        /// </summary>
        public DiffCounts Diffs { get; } = new DiffCounts(0, 0, 0, 0);
    }
}