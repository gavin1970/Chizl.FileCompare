using System;
using System.Linq;
using System.Text;

namespace Chizl.FileCompare
{
    internal static class FileCompareExtensions
    {
        /// <summary>
        /// Since netstandard2.0 doesn't have Math.Clamp, this will do it.
        /// </summary>
        /// <typeparam name="T">Generic IComparable type</typeparam>
        /// <param name="value">this var</param>
        /// <param name="min">Miniumum value based on type</param>
        /// <param name="max">Maximum value based on type</param>
        /// <returns>Forced bounds value based on type</returns>
        public static T ClampTo<T>(this T value, T min, T max)
            where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }
        /// <summary>
        /// Validates current byte array against another and returns if the bytes are identical in each or not.
        /// <code>
        /// var byteArray1 = new byte[2] {71, 97}
        /// var byteArray2 = new byte[2] {71, 97}
        /// if (byteArray1.EqualTo(byteArray2)) 
        ///     Console.WriteLine("Identical byte array");
        /// </code>
        /// </summary>
        /// <param name="byteArr2">Second byte array to compare with</param>
        /// <returns>true: each byte in the array is identical.  false: the two arrays are not identical.</returns>
        public static bool EqualTo(this byte[] @this, byte[] byteArr2) => @this.Where((x, i) => i < byteArr2.Length && x != byteArr2[i]).Count().Equals(0);
        public static bool EqualTo(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> other) => span.SequenceEqual(other); // super fast, optimized in the runtime
        public static char ToChar(this byte b, Encoding encoding) => encoding.GetString(new byte[] { b })[0];
    }
}
