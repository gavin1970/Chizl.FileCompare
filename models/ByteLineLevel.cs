using System.Collections.Generic;
using System.Linq;

namespace Chizl.FileCompare
{
    public class ByteLineLevel : LineLevel
    {
        private string _offset = "";
        // private List<ByteLevel> _byteLevelDiff = new List<ByteLevel>();
        // private string _hexValues = "";
        // private string _printableChars = "";

        internal void AddOffset(string str) => _offset = str;
        internal void AddToHexStr(byte bt, DiffType dt = DiffType.None) => AddToHexStr((char)bt, dt);
        internal void AddToHexStr(char ch, DiffType dt = DiffType.None) => AddToLine(ch, dt);
        // {
        // AddToLine(ch, dt);
        // var bl = new ByteLevel(dt, ch);
        // _hexValues += (_hexValues.Length > 0 ? $" {bl.Hex}" : $"{bl.Hex}");
        // _byteLevelDiff.Add(bl);
        // AddToStr(bl.Str);
        // }

        // private void AddToStr(byte str) => AddToStr((char)str);
        // private void AddToStr(char str) => _printableChars += $"{str}";
        // private void AddToStr(string str) => _printableChars += str;

        // prevent initialization
        internal ByteLineLevel(FileFormat format, int lineNumber) : base(format, lineNumber) { }

        // public DiffType DiffType { get { return _diffType; } }
        // public ByteLevel[] ByteLine { get {  return _byteLevelDiff.ToArray(); } }
        public string Offset { get { return _offset.Trim(); } }
        public string HexValues { get { return GetHexValues(); } }
        public string PrintableChars { get { return string.Join("",Bytes.ToList().Select(s=>s.Str).ToList()); } }

        private string GetHexValues()
        {
            var lineLength = (16 * 3) - 1; // 16 bytes, (2) byte hex, (1) space, - 1 end space..
            var line = LineText;
            if (line.Length.Equals(lineLength))
                return line;
            else if(lineLength>line.Length)
                return $"{line}{(new string(' ', lineLength - line.Length))}";
            else
                return $"{line.Substring(0, lineLength)}";  // invalid data, so, we are showing invalid hex.
        }
    }
}
