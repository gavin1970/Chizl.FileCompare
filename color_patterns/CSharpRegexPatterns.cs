using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Chizl.FileCompare
{
    /// <summary>
    /// Example of use:
    /// <code>
    /// private void ColourRrbText(RichTextBox rtb)
    /// {
    ///     foreach (Match match in CSharpRegexPatterns.Pattern.Matches(rtb.Text))
    ///     {
    ///         foreach (var groupName in CSharpRegexPatterns.Pattern.GetGroupNames())
    ///         {
    ///             if (match.Groups[groupName].Success && CSharpRegexPatterns.Colors.ContainsKey(groupName))
    ///             {
    ///                 rtb.Select(match.Groups[groupName].Index, match.Groups[groupName].Length);
    ///                 rtb.SelectionColor = CSharpRegexPatterns.Colors[groupName];
    ///                 break; // only one group should match
    ///             }
    ///         }
    ///     }
    /// }
    /// </code>>
    /// </summary>
    public static class CSharpRegexPatterns
    {
        public static Regex Pattern { get { return CSharpCombinedRegex.Keywords; } }
        public static Dictionary<string, Color> Colors { get { return CSharpCombinedRegex.GroupColors; } }
    }
    internal static class CSharpCombinedRegex
    {
        public static readonly Dictionary<string, Color> GroupColors = new Dictionary<string, Color>()
        {
            ["AccessModifiers"] = Color.FromArgb(0, 0, 190),   // Dark Blue
            ["BuiltInTypes"] = Color.Blue,
            ["ControlFlow"] = Color.Blue,
            ["ExceptionHandling"] = Color.FromArgb(255, 128, 0), // Dark Orange
            ["DeclarationKeywords"] = Color.FromArgb(0, 0, 190), // Dark Blue
            ["Modifiers"] = Color.FromArgb(0, 0, 190), // Dark Blue
            ["Operators"] = Color.Blue,
            ["Contextual"] = Color.Blue,
            ["Linq"] = Color.FromArgb(190, 0, 0), // Dark Red
            ["Literals"] = Color.Blue,
            ["Special"] = Color.FromArgb(190, 0, 0), // Dark Red
            ["ParameterModifiers"] = Color.FromArgb(255, 128, 0), // Dark Orange
            ["Additional"] = Color.FromArgb(255, 128, 0), // Dark Orange
            ["Pragma"] = Color.FromArgb(132, 78, 41), // Brown
        };

        public static readonly Regex Keywords = new Regex(
            string.Join("|", new[]
            {
                @"(?<AccessModifiers>\b(public|private|protected|internal)\b)",
                @"(?<BuiltInTypes>\b(bool|byte|sbyte|char|decimal|double|float|int|uint|long|ulong|object|short|ushort|string|void)\b)",
                @"(?<ControlFlow>\b(if|else|switch|case|default|do|while|for|foreach|in|break|continue|goto|return|yield)\b)",
                @"(?<ExceptionHandling>\b(try|catch|finally|throw)\b)",
                @"(?<DeclarationKeywords>\b(class|struct|interface|enum|delegate|namespace|using|record)\b)",
                @"(?<Modifiers>\b(abstract|async|const|event|extern|new|override|partial|readonly|sealed|static|unsafe|virtual|volatile)\b)",
                @"(?<Operators>\b(as|await|checked|unchecked|is|sizeof|stackalloc|typeof)\b)",
                @"(?<Contextual>\b(add|alias|dynamic|get|global|init|managed|nameof|nint|not|notnull|nuint|or|remove|required|scoped|set|unmanaged|value|var|when|with)\b)",
                @"(?<Linq>\b(from|where|select|group|into|orderby|join|let|ascending|descending|on|equals|by)\b)",
                @"(?<Literals>\b(null|true|false)\b)",
                @"(?<Special>\b(base|this)\b)",
                @"(?<ParameterModifiers>\b(params|ref|out)\b)",
                @"(?<Additional>\b(file|lock|fixed|region|endregion)\b)",
                @"(?<Pragma>(#region|#endregion|#if|#else|#endif))"
            }), RegexOptions.Compiled | RegexOptions.CultureInvariant
        );
    }
}
