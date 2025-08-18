using System;
using System.Text;

namespace Chizl.FileCompare
{
    public class CompareDiff
    {
        static readonly Encoding _defaultEncoding = Encoding.UTF8;
        private string _lineDiffStr = string.Empty;

        private CompareDiff() {}
        private CompareDiff(DiffType diff, int line, Encoding enc)
        {
            if (diff.Equals(DiffType.NotSet))
                throw new ArgumentException("'DiffType.NotSet' is not a valid DiffType.");
            if (line < 0)
                throw new ArgumentException("'Diff Start' cannot be below 0.");

            DiffType = diff;
            LineNumber = line;
            EncodeType = enc;
        }

        internal CompareDiff(DiffType diff, int line, byte[] diffArray) : this(diff, line, _defaultEncoding, diffArray) { }
        internal CompareDiff(DiffType diff, int line, Encoding enc, byte[] diffArray) : this(diff, line, enc)
        {
            LineDiffBytes = diffArray;
            if (diffArray != null)
                LineDiffStr = EncodeType.GetString(diffArray);

            if (diff != DiffType.None)
                SetStringDiff();
        }

        internal CompareDiff(DiffType diff, int line, string diffStr) : this(diff, line, _defaultEncoding, diffStr) { }
        internal CompareDiff(DiffType diff, int line, Encoding enc, string diffStr) : this(diff, line, enc)
        {
            LineDiffStr = diffStr;
            if (!string.IsNullOrWhiteSpace(diffStr))
                LineDiffBytes = EncodeType.GetBytes(diffStr);

            if (diff != DiffType.None)
                SetStringDiff();
        }

        /// <summary>
        /// TODO: Not ready..
        /// </summary>
        private void SetStringDiff()
        {
            //var charDiff = new List<CharDiff>();

            //var a = LineDiffStr.IndexOf("[+");
            //var d = LineDiffStr.IndexOf("[-");
            //var e = 0;

            //while (a > -1 || d > -1)
            //{

            //    e = LineDiffStr.IndexOf("]", e);

            //    if (a > d)
            //    {
            //        a += 2;
            //        charDiff.Add(new CharDiff(DiffType.Added, LineDiffStr[a]));
            //        a++;
            //    }
            //    else
            //    {
            //        d += 2;
            //        charDiff.Add(new CharDiff(DiffType.Deleted, LineDiffStr[d]));
            //        d++;
            //    }

            //    a = LineDiffStr.IndexOf("[+", a + 1);
            //    d = LineDiffStr.IndexOf("[-", d + 1);
            //}
        }

        public static CharDiff[] LineBreakDown { get; } = new CharDiff[0];
        /// <summary>
        /// Create an empty Model.  IsEmpty will be set to True.
        /// </summary>
        public static CompareDiff Empty { get; } = new CompareDiff();
        
        /// <summary>
        /// If CompareDiff wasn't intialized, True will be returned.
        /// </summary>
        public bool IsEmpty { get { return this.DiffType.Equals(DiffType.NotSet); } }
        /// <summary>
        /// Type of modification found.  Add, Delete, Modified, or None
        /// </summary>
        public DiffType DiffType { get; } = DiffType.NotSet;
        /// <summary>
        /// Line number created for UI matching and may not match line numbers in file.
        /// </summary>
        public int LineNumber { get; } = -1;
        /// <summary>
        /// Single line in byte[] format.
        /// </summary>
        public byte[] LineDiffBytes { get; } = new byte[0];
        /// <summary>
        /// Single line in string format.
        /// </summary>
        public string LineDiffStr { get; } = string.Empty;
        /// <summary>
        /// Encoding used for string to byte[] and back to string conversion.<br/>
        /// Default: UTF8
        /// </summary>
        public Encoding EncodeType { get; } = _defaultEncoding;
    }
}
