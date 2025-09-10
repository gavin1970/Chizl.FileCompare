using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Chizl.FileCompare
{
    public class LineLevel
    {
        static readonly Encoding _defaultEncoding = Encoding.UTF8;

        private StringBuilder _line = new StringBuilder();
        private List<ByteLevel> _bytes = new List<ByteLevel>();
        private DiffType _lineDiffType = DiffType.None;
        private ConcurrentDictionary<DiffType, byte> _diffTypes = new ConcurrentDictionary<DiffType, byte>();
        private FileFormat _fileFormat = FileFormat.Ascii;

        internal LineLevel(FileFormat format, int lineNumber) 
        {
            _fileFormat = format;
            this.LineNumber = lineNumber;
        }

        internal void Clear() 
        { 
            _line.Clear();
            _bytes.Clear();
            _lineDiffType = DiffType.None;
        }

        internal void AddToLine(byte bt, DiffType diff) => AddToLine((char)bt, diff);
        internal void AddToLine(byte[] bt, DiffType diff)
        {
            foreach (byte b in bt)
                AddToLine((char)b, diff);
        }
        internal void AddToLine(string str, DiffType diff)
        {
            foreach (char c in str)
                AddToLine(c, diff);
        }
        internal void AddToLine(char ch, DiffType diff)
        {
            var bl = new ByteLevel(diff, ch);
            _bytes.Add(bl);

            if (_fileFormat.Equals(FileFormat.Ascii))
                _line.Append(bl.Str);
            else
            {
                if (_line.Length > 0)
                    _line.Append(" ");
                _line.Append(bl.Hex);
            }

            CheckLineDiffType(diff);
        }

        public Guid LineID { get; } = Guid.NewGuid();
        public int LineNumber { get; } = 0;

        public ByteLevel[] Bytes { get { return _bytes.ToArray(); } }
        public DiffType LineDiffType { get { return _lineDiffType; } }
        public string LineText { get { return _line.ToString(); } }

        /// <summary>
        /// Attempt to add to dictionary and if successful, validate 
        /// if more than 1 action type, then line is considered Modified.
        /// </summary>
        /// <param name="diff">Char change type</param>
        internal void CheckLineDiffType(DiffType diff)
        {
            // Default is set to None already, we don't 'None' and 'Add or Delete' to mark Line as 'Modified'.
            if (diff.Equals(DiffType.None))
                return;

            if (_diffTypes.TryAdd(diff, 0))
            {
                if (_diffTypes.Count > 1)
                    _lineDiffType = DiffType.Modified;
                else
                    _lineDiffType = diff;
            }
        }
    }
}
