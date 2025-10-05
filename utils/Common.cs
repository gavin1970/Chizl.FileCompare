using System;
using System.IO;

namespace Chizl.FileCompare
{
    internal static class Common
    {
        /// <summary>
        /// IsBinary(): Has BOM detection for UTF encodings UTF-8 BOM, UTF-16 LE/BE and future UTF-32 LE/BE ASCII<br/>
        /// </summary>
        public static bool IsBinary(string fullPath, out string errorMsg)
        {
            const int BytesToCheck = 1024;
            int nullCount = 0;
            errorMsg = string.Empty;

            try
            {
                using (var fs = File.OpenRead(fullPath))
                {
                    byte[] buffer = new byte[BytesToCheck];
                    int bytesRead = fs.Read(buffer, 0, BytesToCheck);

                    // BOM detection for UTF encodings
                    if (bytesRead >= 2)
                    {
                        if (buffer[0] == 0xFF && buffer[1] == 0xFE) return false; // UTF-16 LE
                        if (buffer[0] == 0xFE && buffer[1] == 0xFF) return false; // UTF-16 BE
                    }
                    if (bytesRead >= 3)
                    {
                        if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF) return false; // UTF-8 BOM
                    }
                    if (bytesRead >= 4)
                    {
                        if (buffer[0] == 0xFF && buffer[1] == 0xFE && buffer[2] == 0x00 && buffer[3] == 0x00) return false; // UTF-32 LE
                        if (buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0xFE && buffer[3] == 0xFF) return false; // UTF-32 BE
                    }

                    for (int i = 0; i < bytesRead; i++)
                    {
                        if (buffer[i] == 0x00)
                        {
                            nullCount++;
                            if (nullCount > 1)
                                return true; // definitely binary
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = $"IsBinary('{fullPath}') Exception:\n{ex.Message}";
            }

            return false; // one or fewer string terminators '0x00' → likely text
        }
    }
}
