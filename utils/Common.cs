using System;
using System.IO;

namespace Chizl.FileCompare
{
    internal static class Common
    {
        /// <summary>
        /// Used LINQ.Where().Count()>1 originally, but it's scans the 
        /// whole buffer while looping is a few milliseconds faster.
        /// </summary>
        public static bool IsBinary(string fullPath, out string errorMsg)
        {
            const int CharsToCheck = 1024;
            int nullCount = 0;
            errorMsg = string.Empty;

            try
            {
                using (var fs = File.OpenRead(fullPath))
                {
                    byte[] buffer = new byte[CharsToCheck];
                    int bytesRead = fs.Read(buffer, 0, CharsToCheck);

                    for (int i = 0; i < bytesRead; i++)
                    {
                        if (buffer[i] == '\0')
                        {
                            nullCount++;

                            // There are some malformed ascii text files with 1
                            // string terminator.  So we look for more than 1.
                            // Its been found, most of the time at the end of the file.
                            if (nullCount > 1)
                                // Found more than one string terminator, most definitely binary
                                return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = $"IsBinary('{fullPath}') Exception:\n{ex.Message}";
            }

            return false; // Found one or less null, likely text
        }
    }
}
