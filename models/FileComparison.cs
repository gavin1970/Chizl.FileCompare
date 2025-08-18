using System.Collections.Generic;
using System.Linq;

namespace Chizl.FileCompare
{
    public class FileComparison
    {
        private FileComparison() { IsEmpty = true; }
        internal FileComparison(List<CompareDiff> compareDiffs)
        {
            this.LineComparison = compareDiffs.ToArray();
            this.Diffs = new Counts(
                compareDiffs.Where(w => w.DiffType == DiffType.Added).Count(),
                compareDiffs.Where(w => w.DiffType == DiffType.Deleted).Count(),
                compareDiffs.Where(w => w.DiffType == DiffType.Modified).Count(),
                compareDiffs.Where(w => w.DiffType == DiffType.None).Count()
            );
        }

        public static FileComparison Empty { get; } = new FileComparison();
        public bool IsEmpty { get; }

        public CompareDiff[] LineComparison { get; }
        public Counts Diffs { get; }

        public class Counts
        {
            internal Counts(int added, int deleted, int modified, int identical)
            {
                Added = added;
                Deleted = deleted;
                Modified = modified;
                Identical = identical;
            }
            public int Added { get; }
            public int Deleted { get; }
            public int Modified { get; }
            public int Identical { get; }
        }
    }
}
