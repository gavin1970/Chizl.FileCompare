using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chizl.FileCompare
{
    internal static class FileCompareExtensions
    {
        static readonly string[] _validMakePrintableTypes = new string[] { "char", "string" };

        /// <summary>
        /// Returns if Exception has value or not.<br/>
        /// Will catch the following as Empty.<br/>
        /// *... = new Exception("")<br/>
        /// *... = new Exception(null)<br/>
        /// *... = null
        /// </summary>
        /// <returns>if Exception var is null, without message, or an "Exception of type Exception".  Occurs when using 'new Exception(null)'.</returns>
        public static bool IsEmpty(this Exception ex)
        {
            bool retVal;

            try
            {
                retVal = ex == null ||
                        string.IsNullOrWhiteSpace(ex.Message) ||
                        ex.Message == "Exception of type 'System.Exception' was thrown.";   //var ex = new Exeception(null);
            }
            catch
            {
                //exception object thows an error, this means it's invalid/empty
                retVal = true;
            }

            return retVal;
        }
        /// <summary>
        /// Generic IsEmpty for classes that might not have the property.
        /// </summary>
        /// <typeparam name="T">Generic ISerializable class</typeparam>
        /// <returns>returns if default value of class or null</returns>
        public static bool IsEmpty<T>(this T @this)
            where T : class, ISerializable
        {
            var t = typeof(T);
            var tDefault = default(T);
            var retVal = false;

            switch (t.Name)
            {
                case nameof(Exception):
                    if (@this == null)
                        retVal = true;
                    else
                    {
                        var ex = (Exception)Convert.ChangeType(@this, typeof(Exception));
                        if (string.IsNullOrWhiteSpace(ex.Message))
                            retVal = true;
                    }
                    break;
                default:
                    //generic
                    if (@this == null || @this == tDefault)
                        retVal = true;
                    break;
            }

            return retVal;
        }
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
        /// <summary>
        /// Convert byte to char based on encoding to use.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static char ToChar(this byte b, Encoding encoding) => encoding.GetString(new byte[] { b })[0];
        public static string ReplaceCrLf(this string @this, char replaceWith)
        {
            if (string.IsNullOrEmpty(@this))
                return @this;

            return !string.IsNullOrEmpty(@this)
                ? @this.Replace("\r", replaceWith.ToString()).Replace("\n", replaceWith.ToString())
                : @this;
        }
        /// <summary>
        /// Validates the is a printable character and replaces it if not with the "replaceWith" arguement.
        /// </summary>
        /// <typeparam name="T">Only string or char accepted, Default: char</typeparam>
        /// <param name="replaceWith">(Optional) character to use if the char isn't printable. Default: '.'</param>
        /// <returns>string or char with printable value.</returns>
        public static T MakePrintable<T>(this char @this, char replaceWith = '.')
        {
            if (!_validMakePrintableTypes.Contains(typeof(T).Name.ToLower()))
                throw new NotSupportedException($"Only cast supported are: ({string.Join(", ", _validMakePrintableTypes)}).");

            var retVal = @this >= 32 && @this <= 126 ? @this : replaceWith;
            return (T)Convert.ChangeType(retVal, typeof(T));
        }
        /// <summary>
        /// Replaces chars in a string into printable characters by replacing them with the "replaceWith" arguement.
        /// <code>
        /// Example:
        ///     var str = "abc\tdefg\n";
        ///     Console.Write(str.MakePrintable('@'));
        /// Results:
        ///     "abc.defg."
        /// </code>
        /// </summary>
        /// <param name="replaceWith">
        /// (Optional) character to use if a char isn't printable.  Default: '.'<br/>
        /// When replaceWith = (char)0, it will replace the character in the string with an blank, "".
        /// </param>
        /// <returns>string with printable value.</returns>
        public static string MakePrintable(this string @this, char replaceWith = '.') => MakeValidRange(@this, (char)32, (char)126, replaceWith);
        /// <summary>
        /// Replaces chars in a string into "replaceWith" arguement when outside the range of firstValid and lastValid arguements.
        /// <code>
        /// Example:
        ///     var str = "abc\tdefg\n";
        ///     Console.Write(str.MakeValidRange((char)32, (char)126, '@'));
        /// Results:
        ///     "abc.defg."
        /// </code>
        /// </summary>
        /// <param name="firstValid">First valid char of the range.</param>
        /// <param name="lastValid">Last valid char of the range.</param>
        /// <param name="replaceWith">
        /// (Optional) character to use if a char isn't printable.  Default: '.'<br/>
        /// When replaceWith = (char)0, it will replace the character in the string with an blank, "".
        /// </param>
        /// <returns>string with printable value.</returns>
        public static string MakeValidRange(this string @this, char firstValid, char lastValid, char replaceWith = '.')
        {
            if (string.IsNullOrEmpty(@this))
                return @this;

            var replace = replaceWith.Equals(0) ? "" : replaceWith.ToString();

            if (firstValid >= lastValid)
                throw new ArgumentException($"{nameof(firstValid)} '{firstValid}' cannot be equal to or after {nameof(lastValid)} '{lastValid}'.");

            var replaceThese = @this.ToCharArray().Distinct().Where(c => (c >= firstValid && c <= lastValid)).Select(s=>$"{s}").ToArray();
            foreach (var c in replaceThese)
                @this = @this.Replace(c, replace);

            return @this;
        }
    }
}
