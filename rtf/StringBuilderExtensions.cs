using System;
using System.Text;
using System.Diagnostics.Contracts;

namespace Chizl.Rtf
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Overload for convenience<br/>
        /// AsSpan() is used for matching StringBuilder text and in turn avoids re-allocation and speeds up response time.
        /// </summary>
        /// <param name="test">string.AsSpan() to check end of StringBuilder.</param>
        /// <returns></returns>
        public static bool EndsWith(this StringBuilder sb, ReadOnlySpan<char> value) => sb.EndsWith(value, StringComparison.CurrentCulture);

        /// <summary>
        /// The main EndsWith method using ReadOnlySpan for performance<br/>
        /// AsSpan() is used for matching StringBuilder text and in turn avoids re-allocation and speeds up response time.
        /// </summary>
        /// <param name="value">string.AsSpan() match performance.</param>
        /// <param name="comparison">(Optional) Default: StringComparison.CurrentCulture</param>
        /// <returns></returns>
        public static bool EndsWith(this StringBuilder sb, ReadOnlySpan<char> value, StringComparison comparison)
        {
            // Add a precondition check to catch invalid arguments early
            Contract.Requires(sb != null, "StringBuilder cannot be null.");

            if (value.Length > sb.Length)
                return false;

            // Get a span of the last 'test.Length' characters from the StringBuilder.
            // This is safe and avoids allocation.
            ReadOnlySpan<char> endOfSb = sb.ToString().AsSpan(sb.Length - value.Length, value.Length);

            return endOfSb.Equals(value, comparison);
        }

        /// <summary>
        /// Overload for convenience<br/>
        /// Will check end of StringBuilder and if string.AsSpan(), "value" is found,
        /// it will be removed, only from the end of the string builder object.
        /// </summary>
        /// <param name="value">string.AsSpan() match performance.</param>
        /// <returns></returns>
        public static bool RemoveEnding(this StringBuilder sb, ReadOnlySpan<char> value, bool checkValue = true) => RemoveEnding(sb, value, StringComparison.CurrentCulture, checkValue);

        /// <summary>
        /// Will check end of StringBuilder and if string.AsSpan(), "value" is found,
        /// it will be removed, only from the end of the string builder object.
        /// </summary>
        /// <param name="value">string.AsSpan() match performance.</param>
        /// <param name="comparison">(Optional) Default: StringComparison.CurrentCulture</param>
        /// <param name="checkValue">(Optional) Default: true<br/>
        /// If set to true, it will validate end of sb has the 'value'. NOTE: The search can slow down the process if the StringBuilder object has a lot of content.<br/>
        /// If set to false, it will assume caller has done the validation and remove 'value.Lengh' from the end of sb.</param>
        /// <returns>true, if remove occured. false, if remove didn't occure.</returns>
        public static bool RemoveEnding(this StringBuilder sb, ReadOnlySpan<char> value, StringComparison comparison, bool checkValue = true)
        {
            var removed = false;
            var removeEnding = true;

            if (checkValue)
                removeEnding = sb.EndsWith(value, comparison);

            if (removeEnding)
            {
                sb.Remove(sb.Length - value.Length, value.Length);
                removed = true;
            }

            return removed;
        }
    }    
}
