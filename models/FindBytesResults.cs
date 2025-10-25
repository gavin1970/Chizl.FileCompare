using System;
using System.IO;
using System.Linq;

namespace Chizl.FileCompare
{
    public class FindBytesResults
    {
        private bool _isEmpty = false;
        private bool _verified = false;
        private bool _validated = false;
        private bool _returnOnFirstFind = false;
        private Exception _exception;
        private FileInfo _fileInfo;
        private long[] _foundPositions = new long[0];
        private byte[] _searchForByte = new byte[0];
        private string _searchForHexString = string.Empty;
        private string _alias = string.Empty;

        private FindBytesResults() { _isEmpty = true; }
        internal FindBytesResults(FileInfo fileInfo, string alias, byte[] searchForByte, long bytesScanned, long[] foundPositions, bool returnOnFirstFind)
        {
            _fileInfo = fileInfo;
            _alias = alias;
            _searchForByte = searchForByte;
            _searchForHexString = BitConverter.ToString(searchForByte).Replace("-", " ");
            _foundPositions = foundPositions;
            _returnOnFirstFind = returnOnFirstFind;
            Verified();
        }

        /// <summary>
        /// Allows a non-null class return, with no values set.<br/>
        /// Use FindBytesResults.IsEmpty to verify.
        /// </summary>
        public static FindBytesResults Empty => new FindBytesResults();
        /// <summary>
        /// Validate if object was initialized with FindBytesResults.Empty or through the constructor with arguments..
        /// </summary>
        public bool IsEmpty => _isEmpty;
        /// <summary>
        /// Quick check to see if Exception exists.
        /// </summary>
        public bool HasError => !_exception.IsEmpty();
        /// <summary>
        /// If an exception occurs, this will be the full object.
        /// </summary>
        public Exception Error => _exception;
        /// <summary>
        /// Alias used by user
        /// </summary>
        public string Alias => _alias;
        /// <summary>
        /// File information
        /// </summary>
        public FileInfo FileInfo => _fileInfo;
        /// <summary>
        /// If file existed.
        /// </summary>
        public bool Exists => !_fileInfo.IsEmpty() && _fileInfo.Exists;
        /// <summary>
        /// Search being looked for in bytes
        /// </summary>
        public byte[] SearchForBytes => _searchForByte;
        /// <summary>
        /// All positions search was found.
        /// </summary>
        public long[] Positions => _foundPositions;
        /// <summary>
        /// Formatted response of Positions[].
        /// </summary>
        public string[] PositionsFormatted => _foundPositions.Select(s => s.FormatByComma()).ToArray();
        /// <summary>
        /// Search being looked for in Hex
        /// </summary>
        public string SearchForHexString => _searchForHexString;
        /// <summary>
        /// Flag that states if retun was only on first file or the whole file.
        /// </summary>
        public bool ReturnOnFirstFind => _returnOnFirstFind;
        /// <summary>
        /// Well response with verfied responses by loading each file, useing the Positions[] and the 
        /// length of the SearchForBytes to ensure the expected byte data starts in those positions.
        /// </summary>
        public bool HasPositions => Verified();

        private bool Verified()
        {
            if (_validated || HasError)
                return _verified;

            if (!this.Exists)
            {
                _exception = new FileNotFoundException($"'{_fileInfo.FullName}' not found.");
                return false;
            }

            var retVal = 0;
            try
            {
                _exception = null;
                using (var checkFile = _fileInfo.OpenRead())
                {
                    foreach (var ndx in _foundPositions)
                    {
                        byte[] checkBuff = new byte[_searchForByte.Count()];

                        checkFile.Position = ndx;
                        var readLen = checkFile.Read(checkBuff, 0, checkBuff.Count());

                        if (readLen.Equals(_searchForByte.Length) && checkBuff.EqualTo(_searchForByte))
                            retVal++;
                        else if (retVal > 0)
                            retVal--;
                    }
                }

                _verified = (retVal > 0 && retVal.Equals(_foundPositions.Length));
                _validated = true;
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
            return _verified;
        }
    }
}
