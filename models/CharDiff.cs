namespace Chizl.FileCompare
{
    public class CharDiff
    {
        internal CharDiff(DiffType diff, char lineChar)
        {
            this.DiffType = diff;
            this.Char = lineChar;
        }
        public DiffType DiffType { get; } = DiffType.NotSet;
        public char Char { get; } = '\0';
    }
}
