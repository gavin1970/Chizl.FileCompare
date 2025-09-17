namespace Chizl.FileCompare
{
    public class FileSize
    {
        internal FileSize(int sizeInBytes)
        {
            this.Format_ByteSize = sizeInBytes;
            this.Format_ByteSize_Comma = sizeInBytes.FormatByComma();
            this.Format_Standard = sizeInBytes.FormatByteSize();
        }
        public int Format_ByteSize { get; }
        public string Format_ByteSize_Comma { get; }
        public string Format_Standard { get; }
    }
}
