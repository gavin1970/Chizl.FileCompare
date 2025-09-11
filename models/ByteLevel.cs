using System.Linq;

namespace Chizl.FileCompare
{
    public class ByteLevel
    {
        private readonly byte[] _makeReadable = new byte[] { 9, 10, 13 };  //special case

        internal ByteLevel(DiffType diff, char thisChar) : this(diff, (byte)thisChar) { }
        internal ByteLevel(DiffType diff, byte byteChar)
        {
            this.DiffType = diff;
            this.Byte = byteChar;
            this.Char = (char)byteChar;
            this.Hex = byteChar.ToString("X2");

            if (_makeReadable.Contains(byteChar))
                this.Str = "?";
            else if (byteChar >= 32)  //&& byteChar <= 126
                this.Str = this.Char.ToString();
            else
                this.Str = ".";
        }

        public DiffType DiffType { get; } = DiffType.None;
        public char Char { get; } = '\0';
        public string Str { get; } = string.Empty;
        public byte Byte { get; } = 0;
        public string Hex { get; } = string.Empty;
    }
}
