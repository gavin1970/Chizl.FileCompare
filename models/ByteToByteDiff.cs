using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Chizl.FileCompare
{
    internal class ByteToByteDiff : IDisposable
    {
        private readonly FileInfo _fileInfo;

        private readonly bool _isEmpty = false;
        private readonly string _alias = string.Empty;
        private readonly bool _isBinary = false;
        private readonly string _errMessage = string.Empty;
        private FileStream _fileStream = null;
        private bool _isStaticLoad = false;
        private bool disposedValue;

        private ByteToByteDiff() { _isEmpty = true; }
        private ByteToByteDiff(FileInfo fileInfo, string alias, bool isStatic) : this(fileInfo, alias) { _isStaticLoad = isStatic; }
        public ByteToByteDiff(FileInfo fileInfo, string alias)
        {
            if (!fileInfo.Exists)
                throw new FileNotFoundException($"'{fileInfo.FullName}' not found.");

            if (string.IsNullOrWhiteSpace(alias))
                alias = Path.GetFileNameWithoutExtension(fileInfo.FullName);

            _alias = alias;
            _fileInfo = fileInfo;
            _isBinary = Common.IsBinary(fileInfo.FullName, out string errMsg);

            if (!string.IsNullOrWhiteSpace(errMsg))
                _errMessage = errMsg;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                }

                disposedValue = true;
            }
        }
        ~ByteToByteDiff() => Dispose(disposing: false);
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        public void Close()
        {
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream.Dispose();
                _fileStream = null;
            }
        }

        public static ByteToByteDiff Empty => new ByteToByteDiff();
        public bool IsEmpty => _isEmpty;
        public string ErrorMessage => _errMessage;
        public string Alias => _alias;
        public string Name => _fileInfo.Name;
        public string Directory => _fileInfo.DirectoryName;
        public string FullName => _fileInfo.FullName;
        public long FileSize => Exists ? _fileInfo.Length : 0;
        public bool Exists => _fileInfo.Exists;
        public bool IsBinary => _isBinary;

        public static Task<FindBytesResults> FindPositions(FileInfo fileInfo, string alias, byte[] query, long start = 0, bool returnOnFirstFind = true)
        {
            var btbDiff = new ByteToByteDiff(fileInfo, alias, true);
            return btbDiff.FindPositions(query, start, returnOnFirstFind);
        }
        public Task<FindBytesResults> FindPositions(byte[] query, long start = 0, bool returnOnFirstFind = true)
        {
            bool allowBackTrack = true;
            double threshHoldPerc = 0.33f;  //0.25f, 0.33f
            long maxReadBuffer = 4096;
            long totalBytesSearched = start;
            List<long> foundPositions = new List<long>();
            byte[] buffer = new byte[maxReadBuffer];

            int bytesRead;
            int locCount = 0;
            int threshHold = (int)Math.Round(((double)query.Length) * threshHoldPerc);

            if (_fileStream == null)
                _fileStream = _fileInfo.OpenRead();

            if (_fileStream.CanRead)
            {
                _fileStream.Position = start;

                while ((bytesRead = _fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        // Debug.WriteLineIf(i >= 315 && i <= 350, $"{i} := query: {query[locCount]} <-> buffer: {buffer[i]}");

                        if (query[locCount] != buffer[i])
                        {
                            if (!allowBackTrack || locCount <= threshHold)
                                locCount = 0;
                            else
                            {
                                // This resolves an issue of long duplicate bytes, where farther within, is what is being looked for.
                                // Backing pointer up to the start + 1 byte to check again.  This is a low percentage case, but can occur.
                                var startOver = locCount;
                                i -= startOver;
                                totalBytesSearched -= (startOver - 1);
                                locCount = 0;
                                continue;
                            }
                        }
                        else
                            locCount++;

                        totalBytesSearched++;

                        if (locCount.Equals(query.Length))
                        {
                            foundPositions.Add(totalBytesSearched - locCount);

                            if (returnOnFirstFind)
                                return Task.FromResult(
                                                new FindBytesResults(_fileInfo, _alias, query, totalBytesSearched,
                                                                        foundPositions.ToArray(), returnOnFirstFind));

                            locCount = 0;
                        }
                    }
                }
            }

            if (_isStaticLoad)
                this.Dispose();

            return Task.FromResult(
                            new FindBytesResults(_fileInfo, _alias, query, totalBytesSearched,
                                                    foundPositions.ToArray(), returnOnFirstFind));
        }
    }
}
