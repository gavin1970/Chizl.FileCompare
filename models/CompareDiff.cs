using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Chizl.FileCompare
{
    public class CompareDiff
    {
        static readonly Encoding _defaultEncoding = Encoding.UTF8;

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
        }

        internal CompareDiff(DiffType diff, int line, string diffStr) : this(diff, line, _defaultEncoding, diffStr) { }
        internal CompareDiff(DiffType diff, int line, Encoding enc, string diffStr) : this(diff, line, enc)
        {
            LineDiffStr = diffStr;
            if (!string.IsNullOrWhiteSpace(diffStr))
                LineDiffBytes = EncodeType.GetBytes(diffStr);
        }

        /// <summary>
        /// Create an empty Model.  IsEmpty will be set to True.
        /// </summary>
        public static CompareDiff Empty { get; } = new CompareDiff();

        /// <summary>
        /// LineBreakDown is used for Modified lines and will be empty for New, Deleted, or unmodified lines.<br/>
        /// This is the help translate modified lines in a more structured manner.
        /// </summary>
        public CharDiff[] LineBreakDown { get { return SetStringDiff(); } }
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

        /// <summary>
        /// TODO: Not ready..
        /// </summary>
        private CharDiff[] SetStringDiff()
        {
            if(!this.DiffType.Equals(DiffType.Modified))
                return new CharDiff[0];

            var charArray = LineDiffStr.ToCharArray();
            var charList = new List<CharDiff>();

            for (int i = 0; i < charArray.Length; i++)
            {
                char thisChar = charArray[i];
                char nextChar = i + 1 < charArray.Length ? charArray[i + 1] : '\0';
                char endMod = i + 3 < charArray.Length ? charArray[i + 3] : '\0';

                if (thisChar == '[' && nextChar == '+' && endMod == ']')
                {
                    i += 2;
                    charList.Add(new CharDiff(DiffType.Added, charArray[i]));
                    i++;    //skip end Mod
                }
                else if (thisChar == '[' && nextChar == '-' && endMod == ']')
                {
                    i += 2;
                    charList.Add(new CharDiff(DiffType.Deleted, charArray[i]));
                    i++;    //skip end Mod
                }
                else
                    charList.Add(new CharDiff(DiffType.Modified, thisChar));
            }

            return charList.ToArray();
        }
    }
}
