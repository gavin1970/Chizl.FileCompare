using System;
using System.Collections.Generic;
using System.Linq;

namespace Chizl.FileCompare
{
    public class ComparisonResults
    {
        /// <summary>
        /// Only used by static property "ComparisonResults.Empty"
        /// </summary>
        private ComparisonResults() { IsEmpty = true; }
        internal ComparisonResults(Exception ex) 
        { 
            IsEmpty = true; 
            Exception = ex;
            IsBinary = false;
        }
        internal ComparisonResults(List<CompareDiff> compareDiffs, bool isBinary = false)
        {
            this.IsBinary = isBinary;
            this.LineComparison = compareDiffs.ToArray();
            this.Diffs = new DiffCounts(
                compareDiffs.Where(w => w.DiffType == DiffType.Added).Count(),
                compareDiffs.Where(w => w.DiffType == DiffType.Deleted).Count(),
                compareDiffs.Where(w => w.DiffType == DiffType.Modified).Count(),
                compareDiffs.Where(w => w.DiffType == DiffType.None).Count()
            );
        }

        /// <summary>
        /// Create an empty Model.  IsEmpty will be set to True.
        /// </summary>
        public static ComparisonResults Empty { get; } = new ComparisonResults();
        /// <summary>
        /// If ComparisonResults wasn't intialized, True will be returned.
        /// </summary>
        public bool IsEmpty { get; } = false;
        /// <summary>
        /// If files compared were binary or not.
        /// </summary>
        public bool IsBinary { get; } = false;
        /// <summary>
        /// If an Exception occurs when files are to be loaded, this will be set to true.<br/>
        /// "Exception" property for this class will be set to exception reason.
        /// </summary>
        public bool HasException { get { return this.Exception != null; } }
        /// <summary>
        /// Only used when one of the files is missing when CompareFiles() is called.<br/>
        /// <code>
        /// if (!sourceFileInfo.Exists)
        ///     return new ComparisonResults(new ArgumentException($"{nameof(sourceFile)}: '{sourceFile}' is not found and/or accessible."));
        /// if (!targetFileInfo.Exists)
        ///     return new ComparisonResults(new ArgumentException($"{nameof(targetFile)}: '{targetFile}' is not found and/or accessible."));
        /// </code>
        /// </summary>
        public Exception Exception { get; } = null;
        /// <summary>
        /// Holds each line with each byte showing any differences that may exist.
        /// </summary>
        public CompareDiff[] LineComparison { get; } = new CompareDiff[0];
        /// <summary>
        /// Shows how many of each different change types were found.<br/>
        /// Add, Modified, Delete, or No Change
        /// </summary>
        public DiffCounts Diffs { get; } = new DiffCounts(0, 0, 0, 0);
    }
}
