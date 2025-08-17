using System;

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
    }
}
