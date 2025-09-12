using System;
using System.IO;

namespace Chizl.FileCompare
{
    internal static class Common
    {
        const int charsToCheck = 1024;

        /// <summary>
        /// Used LINQ.Where().Count()>1 originally, but it's scans the 
        /// whole buffer while looping is a few milliseconds faster.
        /// </summary>
        public static bool IsBinary(byte[] bytes, int bytesRead, out string errorMsg)
        {
            int nullCount = 0;
            errorMsg = string.Empty;

            try
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    if (bytes[i] == '\0')
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
            catch (Exception ex)
            {
                errorMsg = $"IsBinary(byte[]) Exception:\n{ex.Message}";
            }

            return false; // Found one or less null, likely text
        }
        /// <summary>
        /// Used LINQ.Where().Count()>1 originally, but it's scans the 
        /// whole buffer while looping is a few milliseconds faster.
        /// </summary>
        public static bool IsBinary(string fullPath, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                using (var fs = File.OpenRead(fullPath))
                {
                    byte[] buffer = new byte[charsToCheck];
                    int bytesRead = fs.Read(buffer, 0, charsToCheck);

                    return IsBinary(buffer, bytesRead, out errorMsg);
                }
            }
            catch (Exception ex)
            {
                errorMsg = $"IsBinary('{fullPath}') Exception:\n{ex.Message}";
            }

            return false; // Found one or less null, likely text
        }
        /// <summary>
        /// FNV-1a Algorithm: The logic used is the FNV-1a (Fowler-Noll-Vo) hash function. It's known for being 
        /// fast and having a good distribution of hash values, which minimizes collisions for small data sets.         
        /// </summary>
        /// <param name="data">byte array</param>
        /// <returns>Hex representation of hash</returns>
        public static string GetHashString(params byte[] data) => ComputeHash(data).ToString("X2");
        /// <summary>
        /// FNV-1a Algorithm: The logic used is the FNV-1a (Fowler-Noll-Vo) hash function. It's known for being 
        /// fast and having a good distribution of hash values, which minimizes collisions for small data sets.         
        /// </summary>
        /// <param name="data">byte array</param>
        /// <returns>hash value</returns>
        public static int ComputeHash(params byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                return hash;
            }
        }
    }
}
