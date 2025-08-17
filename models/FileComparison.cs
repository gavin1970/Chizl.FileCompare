using System.Collections.Generic;
using System.Linq;

namespace Chizl.FileCompare
{
    public class FileComparison
    {
        private FileComparison() { IsEmpty = true; }
        internal FileComparison(List<CompareDiff> compareDiffs)
        {
            LineComparison = compareDiffs.ToArray();
            Added = compareDiffs.Where(w => w.DiffType == DiffType.Added).Count();
            Deleted = compareDiffs.Where(w => w.DiffType == DiffType.Deleted).Count();
            Modified = compareDiffs.Where(w => w.DiffType == DiffType.Modified).Count();
            Identical = compareDiffs.Where(w => w.DiffType == DiffType.None).Count();
        }

        public static FileComparison Empty { get; } = new FileComparison();
        public bool IsEmpty { get; }

        public CompareDiff[] LineComparison { get; }
        public int Added { get; }
        public int Deleted { get; }
        public int Modified { get; }
        public int Identical { get; }
    }
}
