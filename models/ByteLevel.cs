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
            else
                this.Str = this.Char.MakePrintable<string>('.');
        }
        /// <summary>
        /// Byte state: None, Added, Modified, Deleted
        /// </summary>
        public DiffType DiffType { get; } = DiffType.None;
        /// <summary>
        /// Char representation of the byte.
        /// </summary>
        public char Char { get; } = '\0';
        /// <summary>
        /// Single character string representation of the byte.
        /// </summary>
        public string Str { get; } = string.Empty;
        /// <summary>
        /// Byte found in file.
        /// </summary>
        public byte Byte { get; } = 0;
        /// <summary>
        /// Hex version of the Byte
        /// </summary>
        public string Hex { get; } = string.Empty;
    }
}
