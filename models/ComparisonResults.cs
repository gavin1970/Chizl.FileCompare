using System;
using System.Collections.Generic;
using System.Linq;

namespace Chizl.FileCompare
{
    public class ComparisonResults
    {
        private ComparisonResults() { IsEmpty = true; }
        internal ComparisonResults(Exception ex) 
        { 
            HasException = true;
            IsEmpty = true; 
            Exception = ex; 
        }
        internal ComparisonResults(List<CompareDiff> compareDiffs)
        {
            this.LineComparison = compareDiffs.ToArray();
            this.Diffs = new DiffCounts(
                compareDiffs.Where(w => w.DiffType == DiffType.Added).Count(),
                compareDiffs.Where(w => w.DiffType == DiffType.Deleted).Count(),
                compareDiffs.Where(w => w.DiffType == DiffType.Modified).Count(),
                compareDiffs.Where(w => w.DiffType == DiffType.None).Count()
            );
        }

        public static ComparisonResults Empty { get; } = new ComparisonResults();
        public bool IsEmpty { get; } = false;
        public bool HasException { get; } = false;

        public Exception Exception { get; } = new Exception();
        public CompareDiff[] LineComparison { get; } = new CompareDiff[0];
        public DiffCounts Diffs { get; } = new DiffCounts(0, 0, 0, 0);
    }
}
