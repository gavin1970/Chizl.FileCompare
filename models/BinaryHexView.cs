//*
namespace Chizl.FileCompare
{
    //public class Binary2HexView
    //{
    //    private string _offset = "";
    //    internal void AddOffset(string str) => _offset = str;

    //    private string _hexValues = "";
    //    internal void AddHexStr(string str) => _hexValues += (_hexValues.Length > 0 ? $" {str}" : str);

    //    private string _printableChars = "";
    //    internal void AddPChar(string str) => _printableChars += str;

    //    //prevent initialization
    //    internal Binary2HexView() {}

    //    public string Offset { get { return _offset.Trim(); } }
    //    public string HexValues { get { return _hexValues.Trim(); } }
    //    public string PrintableChars { get { return _printableChars.Trim(); } }
    //}
}
/**/

/*
namespace Chizl.FileCompare
{
    public class BinaryHexView
    {
        internal DiffType _diffType = DiffType.None;
        internal void AddDiffType(DiffType dt) => _diffType = dt;

        private string _offset = "";
        internal void AddOffset(string str) => _offset = str;

        private string _hexValues = "";
        internal void AddHexStr(string str) => _hexValues += (_hexValues.Length > 0 ? $" {str}" : str);

        private string _printableChars = "";
        internal void AddPChar(string str) => _printableChars += str;

        //prevent initialization
        internal BinaryHexView() {}

        public DiffType DiffType { get { return _diffType; } }
        public string Offset { get { return _offset.Trim(); } }
        public string HexValues { get { return _hexValues.Trim(); } }
        public string PrintableChars { get { return _printableChars.Trim(); } }
    }
} 
/**/