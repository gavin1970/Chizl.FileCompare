using System;
using System.IO;

namespace Chizl.FileCompare
{
    public class FileLevel : IDisposable
    {
        const int _maxContentFileSize = ((1024 * 1024) * 5);   //5MB
        private bool disposedValue;
        //private LoadContentType _loadedType;
        private byte[] _bytes;
        private string[] _content;
        //private FileStream _fileStream;

        #region Constructor/Deconstructor
        public FileLevel(string fullPath)
        {
            //working on DiffTool when this is a parameter.
            LoadContentType loadContentType = LoadContentType.InMemory;
            var fi = new FileInfo(fullPath);

            this.FullPath = fi.FullName;
            this.Name = fi.Name;
            this.Folder = fi.DirectoryName;
            this.Exists = fi.Exists;

            if (this.Exists)
            {
                if (fi.Length > int.MaxValue)
                    this.Size = new FileSize(int.MaxValue);
                else
                    this.Size = new FileSize((int)fi.Length);

                var sizeFits = this.Size.Format_ByteSize <= _maxContentFileSize;

                if (!sizeFits)  //remove this in the future, will auto set if too large.
                    throw new Exception($"File: '{fi.FullName}' is too large '{this.Size.Format_ByteSize.FormatByteSize()}' in " +
                                        $"size to load into memory at this time.  Max is set to '{_maxContentFileSize.FormatByteSize()}'.  " +
                                        $"Future versions will be streaming content.");

                if (loadContentType == LoadContentType.InMemory)
                {
                    //if (sizeFits) 
                    //{
                        _bytes = File.ReadAllBytes(fi.FullName);
                        //_loadedType = LoadContentType.InMemory;
                    //}
                    //else
                    //{
                    //    _loadedType = LoadContentType.Stream;
                    //    _fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                    //}
                }
                //else if (loadContentType == LoadContentType.InMemory && !sizeFits)

                this.Format = Common.IsBinary(this.Bytes, this.Bytes.Length, out string err)
                                && string.IsNullOrWhiteSpace(err)
                                ? FileFormat.Binary
                                : FileFormat.Ascii;

                if (loadContentType == LoadContentType.InMemory
                    && this.Format.Equals(FileFormat.Ascii) && sizeFits)
                    _content = File.ReadAllLines(fi.FullName);

                this.Pointer = 0;
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //_fileStream?.Close();
                    //_fileStream?.Dispose();
                }

                disposedValue = true;
            }
        }
        ~FileLevel() => Dispose(disposing: false);
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        internal int Pointer { get; set; }

        //public LoadContentType LoadedType { get { return _loadedType; } }
        //public FileStream FileStream { get { return _fileStream; } }
        public byte[] Bytes { get { return _bytes; } }
        public string[] Content { get { return _content; } }
        public string FullPath { get; }
        public string Name { get; }
        public string Folder { get; }
        public FileSize Size { get; }
        public bool Exists { get; }
        public bool IsBinary { get { return Format.Equals(FileFormat.Binary); } }
        public bool IsAscii { get { return Format.Equals(FileFormat.Ascii); } }
        public FileFormat Format { get; } = FileFormat.NotSet;
    }
}
