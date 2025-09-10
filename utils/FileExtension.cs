namespace Chizl.FileCompare
{
    public static class FileExtension
    {
        internal static readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public static string FormatByComma(this int inSize) => inSize.ToString("N0");
        public static string FormatByComma(this long inSize) => inSize.ToString("N0");
        public static string FormatByteSize(this int intBytes) => ((double)intBytes).FormatByteSize();
        public static string FormatByteSize(this long intBytes) => ((double)intBytes).FormatByteSize();
        public static string FormatByteSize(this double dblBytes)
        {
            int idx = 0;
            double num = dblBytes;

            while (num > 1024)
            {
                num /= 1024;
                idx++;
            }
            return string.Format("{0:n2} {1}", num, suffixes[idx]);
        }
    }
}
