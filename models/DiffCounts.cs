namespace Chizl.FileCompare
{
    public class DiffCounts
    {
        internal DiffCounts(int added, int deleted, int modified, int identical)
        {
            Added = added;
            Deleted = deleted;
            Modified = modified;
            Identical = identical;
        }
        public int Added { get; } = 0;
        public int Deleted { get; } = 0;
        public int Modified { get; } = 0;
        public int Identical { get; } = 0;
    }
}
