using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Chizl.FileCompare
{
    public class CompareDiff
    {
        static readonly Encoding _defaultEncoding = Encoding.UTF8;
        private ByteLevel[] _textBreakDown = new ByteLevel[0];
        private ByteLevel[] _lineDiffBytes = new ByteLevel[0];
        private string _lineDiffStr = string.Empty;
        private string _lineHexString = string.Empty;

        private CompareDiff() {}

        internal CompareDiff(Encoding enc) : this(DiffType.None, 0, enc) { }
        private CompareDiff(DiffType diff, int line, Encoding enc)
        {
            if (line < 0)
                throw new ArgumentException("'Diff Start' cannot be below 0.");

            DiffType = diff;
            LineNumber = line;
            EncodeType = enc;
        }
        internal CompareDiff(ByteLevel[] byteLevelDiff, int line) : this(byteLevelDiff, line, _defaultEncoding) { }
        internal CompareDiff(ByteLevel[] byteLevelDiff, int line, Encoding enc) : this(DiffType.None, line, enc)
        {
            if (byteLevelDiff == null || byteLevelDiff.Length > 0)
                throw new ArgumentException("Byte level data is required.");

            _lineDiffBytes = byteLevelDiff;
            _lineDiffStr = enc.GetString(byteLevelDiff.Select(s=>s.Byte).ToArray()).ReplaceCrLf('?'); 
            _lineHexString = ToHexString(_lineDiffBytes);
        }
        internal CompareDiff(DiffType diff, int line, ReadOnlySpan<byte> diffSpan) : this(diff, line, _defaultEncoding, diffSpan.ToArray()) { }
        internal CompareDiff(DiffType diff, int line, Encoding enc, byte[] diffArray) : this(diff, line, enc)
        {
            if (diffArray == null || diffArray.Length.Equals(0))
                throw new ArgumentException("Byte data is required.");

            _lineDiffBytes = diffArray.Select(s => new ByteLevel(diff, s)).ToArray();
            _lineDiffStr = enc.GetString(diffArray).ReplaceCrLf('?');
            _lineHexString = ToHexString(_lineDiffBytes);
        }
        internal CompareDiff(DiffType diff, int line, string diffStr) : this(diff, line, _defaultEncoding, diffStr) { }
        internal CompareDiff(DiffType diff, int line, Encoding enc, string diffStr) : this(diff, line, enc)
        {
            _lineDiffStr = diffStr.ReplaceCrLf('?');
            if (diffStr != null && diffStr.Length > 0)
                _lineDiffBytes = enc.GetBytes(diffStr).Select(s => new ByteLevel(diff, s)).ToArray();
            _lineHexString = ToHexString(_lineDiffBytes);
        }

        /// <summary>
        /// Create an empty Model.  IsEmpty will be set to True.
        /// </summary>
        public static CompareDiff Empty { get; } = new CompareDiff();
        /// <summary>
        /// LineBreakDown is used for Modified lines and will be empty for New, Deleted, or unmodified lines.<br/>
        /// This is the help translate modified lines in a more structured manner.
        /// </summary>
        public ByteLevel[] TextBreakDown { get { return GetByteLevel(); } }
        /// <summary>
        /// If CompareDiff wasn't intialized, True will be returned.
        /// </summary>
        public bool IsEmpty { get { return this.DiffType.Equals(DiffType.None); } }
        /// <summary>
        /// Type of modification found.  Add, Delete, Modified, or None
        /// </summary>
        public DiffType DiffType { get; } = DiffType.None;
        /// <summary>
        /// Line number created for UI matching and may not match line numbers in file.
        /// </summary>
        public int LineNumber { get; } = -1;
        /// <summary>
        /// Hex string of line value.
        /// </summary>
        public string LineHexString { get { return _lineHexString; } }
        /// <summary>
        /// Single line in byte[] format.
        /// </summary>
        public ByteLevel[] LineDiffBytes { get { return _lineDiffBytes; } }
        /// <summary>
        /// Single line in string format.
        /// </summary>
        public string LineDiffStr { get { return _lineDiffStr; } }
        /// <summary>
        /// Encoding used for string to byte[] and back to string conversion.<br/>
        /// Default: UTF8
        /// </summary>
        public Encoding EncodeType { get; } = _defaultEncoding;
        
        internal void AddByteLevels(ByteLevel[] byteLevelDiff)
        {
            if (byteLevelDiff == null || byteLevelDiff.Length > 0)
                throw new ArgumentException("Byte level data is required.");

            _lineDiffBytes = byteLevelDiff;
            _lineDiffStr = EncodeType.GetString(byteLevelDiff.Select(s => s.Byte).ToArray()).ReplaceCrLf('?');
            _lineHexString = ToHexString(LineDiffBytes);
        }
        /// <summary>
        /// Breaks down the strings, building a list of ByteLevel 
        /// instead of a string being parsed by caller.
        /// </summary>
        private ByteLevel[] GetByteLevel()
        {
            if (_textBreakDown.Length > 0)
                return _textBreakDown;

            var charArray = LineDiffStr.ToCharArray();
            var charList = new List<ByteLevel>();

            if (this.DiffType != DiffType.Modified)
                return _lineDiffBytes;

            for (int i = 0; i < charArray.Length; i++)
            {
                char thisChar = charArray[i];
                char nextChar = i + 1 < charArray.Length ? charArray[i + 1] : '\0';
                char endMod = i + 3 < charArray.Length ? charArray[i + 3] : '\0';

                if (thisChar == '[' && nextChar == '+' && endMod == ']')
                {
                    i += 2;
                    charList.Add(new ByteLevel(DiffType.Added, charArray[i]));
                    i++;    //skip end Mod
                }
                else if (thisChar == '[' && nextChar == '-' && endMod == ']')
                {
                    i += 2;
                    charList.Add(new ByteLevel(DiffType.Deleted, charArray[i]));
                    i++;    //skip end Mod
                }
                else
                    charList.Add(new ByteLevel(DiffType.Modified, thisChar));
            }

            _textBreakDown = charList.ToArray();
            return _textBreakDown;
        }
        /// <summary>
        /// Hex string from byte array.
        /// </summary>
        /// <returns>Hex string</returns>
        private string ToHexString(ByteLevel[] byteArray)
        {
            var sb = new StringBuilder();

            foreach (var b in byteArray.Select(s => s.Byte))
                sb.Append($"{b:X2} ");

            return sb.ToString().Trim();
        }
    }
}
