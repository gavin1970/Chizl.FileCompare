using System.IO;

namespace Chizl.FileCompare
{
    public class FileLevel
    {
        const int _maxContentFileSize = ((1024 * 1024) * 5);   //5MB
        //private List<LineLevel> _lineLevels = new List<LineLevel>();

        internal FileLevel(string fullPath)
        {
            var fi = new FileInfo(fullPath);

            this.FullPath = fi.FullName;
            this.Name = fi.Name;
            this.Folder = fi.DirectoryName;
            this.Exists = fi.Exists;

            if (this.Exists)
            {
                this.Bytes = File.ReadAllBytes(fi.FullName);
                this.Format = Common.IsBinary(this.Bytes, this.Bytes.Length, out string err)
                              && string.IsNullOrWhiteSpace(err) 
                                ? FileFormat.Binary 
                                : FileFormat.Ascii;

                if (fi.Length > int.MaxValue)
                    this.Size = new FileSize(int.MaxValue);
                else
                    this.Size = new FileSize((int)fi.Length);

                if (this.Format.Equals(FileFormat.Ascii) && this.Size.Format_ByteSize < _maxContentFileSize)
                    this.Content = File.ReadAllLines(fi.FullName);

                this.Pointer = 0;
            }
        }
        internal int Pointer { get; set; }
        public byte[] Bytes { get; }
        public string[] Content { get; }
        public string FullPath { get; }
        public string Name { get; }
        public string Folder { get; }
        public FileSize Size { get; }
        public bool Exists { get; }
        public bool IsBinary { get { return Format.Equals(FileFormat.Binary); } }
        public bool IsAscii { get { return Format.Equals(FileFormat.Ascii); } }
        public FileFormat Format { get; } = FileFormat.NotSet;
        //public LineLevel[] Lines { get { return _lineLevels.ToArray(); } }

        //internal void AddLine(LineLevel level) => _lineLevels.Add(level);
        //internal void RemoveLine(Guid lineID)
        //{
        //    var lineLevel = _lineLevels.FirstOrDefault(w => w.LineID.Equals(lineID));
        //    if (lineLevel.Bytes.Length > 0)
        //        RemoveLine(lineLevel);
        //}
        //internal void RemoveLine(LineLevel lineLevel) => _lineLevels.Remove(lineLevel);
        //internal void Clear() => _lineLevels.Clear();
    }
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
