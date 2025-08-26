using System;
using System.Collections.Generic;
using System.Text;

namespace Chizl.FileCompare
{
    public class BinaryHexView
    {
        internal void AddOffset(string str) => _offset = str;
        private string _offset = "";
        internal void AddHexStr(string str) => _hexValues += (_hexValues.Length > 0 ? $" {str}" : str); 
        private string _hexValues = "";
        internal void AddPChar(string str) => _printableChars += str;
        private string _printableChars = "";

        //prevent initialization
        internal BinaryHexView() {}

        public string Offset { get { return _offset.Trim(); } }
        public string HexValues { get { return _hexValues.Trim(); } }
        public string PrintableChars { get { return _printableChars.Trim(); } }
    }
}
