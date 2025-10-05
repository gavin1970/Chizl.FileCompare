using System;
using System.Net;
using System.Text;
using System.Security;
using System.Security.Cryptography;

namespace Chizl.FileCompare
{
    public sealed class CHash
    {
        private enum HashType
        {
            MD5,
            SHA256,
            FNV1a
        }
        
        private static readonly Encoding _enc = Encoding.ASCII;

        /// <summary>
        /// Returns MD5 Has string of string being passed in.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="salt"></param>
        /// <param name="removeDashes"></param>
        /// <returns></returns>
        public static string GetMD5Hash(byte[] text, SecureString salt = null, bool removeDashes = true) => GetMD5Hash(_enc.GetString(text), salt, removeDashes);
        public static string GetMD5Hash(string text, SecureString salt = null, bool removeDashes = true) => GetHash(text, HashType.MD5, salt, removeDashes);

        public static string GetShaHash(byte[] text, SecureString salt = null, bool removeDashes = true) => GetShaHash(_enc.GetString(text), salt, removeDashes);
        public static string GetShaHash(string text, SecureString salt = null, bool removeDashes = true) => GetHash(text, HashType.SHA256, salt, removeDashes);

        public static string GetFnv1aHash(byte[] text, SecureString salt = null, bool removeDashes = true) => GetFnv1aHash(_enc.GetString(text), salt, removeDashes);
        public static string GetFnv1aHash(string text, SecureString salt = null, bool removeDashes = true) => GetHash(text, HashType.FNV1a, salt, removeDashes);

        /// <summary>
        /// FNV-1a Algorithm: The logic used is the FNV-1a (Fowler-Noll-Vo) hash function. It's known for being 
        /// fast and having a good distribution of hash values, which minimizes collisions for small data sets.
        /// </summary>
        /// <param name="data">byte array</param>
        /// <returns>hash value</returns>
        private static byte[] ComputeFnv1aHash(byte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                return _enc.GetBytes(hash.ToString("X2"));
            }
        }
        private static string GetHash(string text, HashType hashType, SecureString salt = null, bool removeDashes = true)
        {
            var bHash = new byte[0];
            var b = _enc.GetBytes(text + new NetworkCredential(null, salt).Password);

            switch (hashType)
            {
                case HashType.MD5:
                    using (var md5 = MD5.Create())
                        bHash = md5.ComputeHash(b);
                    break;
                case HashType.FNV1a:
                    bHash = ComputeFnv1aHash(b);
                    break;
                case HashType.SHA256:
                default:
                    using (var sha = SHA256.Create())
                        bHash = sha.ComputeHash(b);
                    break;
            }

            var retVal = BitConverter.ToString(bHash);
            if (removeDashes)
                retVal = retVal.Replace("-", "");

            return retVal;
        }
    }

}
