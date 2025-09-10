using System;
using System.Text;
using System.Drawing;

namespace Chizl.Rtf
{
    public class RtfBuilder
    {
        /*
         {\rtf1\ansi\deff0 is a standard header.
         {\colortbl; ...} defines the colors to be used.

         NUMBER on the end of each is the index, based everything betweenthe simicolors after: \\colortbl;
         e.g.   \\colortbl;\\red255\\green0\\blue0;\\red0\\green128\\blue0
         e.g. \cf1<------[Foreground Index 1]
         e.g. \cf2<------------------------------[Foreground Index 2]
         e.g. \chcbpat1<-[Background Index 1]
         e.g. \chcbpat2<-------------------------[Background Index 2]

         \chcbpat0 = defaults background color to original
         \cf0 = defaults foreground color to original
         \chshdng10000: Sets the shading percentage to 100%, ensuring a solid background fill.
         rtfBuilder.Append($"\\chshdng10000\\chcbpat3\\cf1 My Info");

         \chcbpat1 = Red Background for deleted bytes
         \cf1 = Red Foreground for deleted bytes
         \chcbpat2 = Green Background for added bytes
         \cf2 = Green for added bytes
        /**/

        const string START_RTF_ANSI_PAGE = @"{\rtf1\ansi\deff0";
        // 1st Font Index, (17/2 = 8.5pt)
        const string FONT_FAMILY_SIZE = @"{\fonttbl{\f0\fnil\fcharset0 Courier New;}}\f0\fs17";

        const string START_COLORS = @"{\colortbl;";
        const string EMPTY_FGCOLOR = @"\cf0\~";
        const string EMPTY_BGCOLOR = @"\chcbpat0\~";
        const string FGCOLOR = @"\cf";
        const string BGCOLOR = @"\chcbpat";

        private readonly RtfColorManager _colorTableManager;
        private readonly StringBuilder _contentTextRtf = new StringBuilder();
        private readonly StringBuilder _contentTextAscii = new StringBuilder();

        public RtfBuilder(Color[] colorIndexList) 
        {
            _colorTableManager = new RtfColorManager(colorIndexList);
            RtfSetupPage();
        }

        public void Colors(Color bgColor, Color fgColor) => Append(string.Empty, bgColor, fgColor);
        public void FgColor(Color fgColor) => Append(string.Empty, Color.Empty, fgColor);
        public void BgColor(Color bgColor) => Append(string.Empty, bgColor, Color.Empty);
        public void ClearColor(bool bgColor = true, bool fgColor = true) => AddText(string.Empty, Color.Transparent, Color.Transparent, false);

        public void Append(string text) => AddText(text, Color.Empty, Color.Empty, false);
        public void Append(string text, Color bg) => AddText(text, bg, Color.Empty, false);
        public void Append(string text, Color bg, Color fg) => AddText(text, bg, fg, false);

        public void AppendLine(string text) => AddText(text, Color.Empty, Color.Empty, true);
        public void AppendLine(string text, Color bg) => AddText(text, bg, Color.Empty, true);
        public void AppendLine(string text, Color bg, Color fg) => AddText(text, bg, fg, true);

        public string GetDocument() => _contentTextRtf.AppendLine("}").ToString(); // close document
        /// <summary>
        /// View existing RTF context data, which may or may not be complete.<br/>
        /// GetDocument(), will finalize the RTF document with a final curly bracket, '}'.
        /// </summary>
        public string ViewRTFDocument() => _contentTextRtf.ToString(); // close document
        /// <summary>
        /// View the existing RTF content data in Ascii text form without any RTF codes.<br/>
        /// This is current, which may or may not be complete.
        /// </summary>
        public string ViewAsciiDocument() => _contentTextAscii.ToString(); // close document

        public void Clear() => RtfSetupPage();
        private string _removeSpace = "";
        internal void AddText(string text, Color bg, Color fg, bool endLine)
        {
            var rtfSpace = "\\~";
            if (!string.IsNullOrEmpty(text))
            {
                if ((!bg.IsEmpty || !fg.IsEmpty) && _removeSpace.Length != 0)
                {
                    //because \~ creates a space and space is required after a
                    //RTF command, commands can be confused with following text
                    //without a space between them.  If another command is next,
                    //this allows command to override the forced space, so
                    //extra spaces are not showing up on the UI.
                    var removed = _contentTextRtf.RemoveEnding(_removeSpace.AsSpan(), false);
                }

                if (!bg.IsEmpty)
                    _contentTextRtf.Append($"{BGCOLOR}{_colorTableManager.GetIndex(bg)}{rtfSpace}");
                if (!fg.IsEmpty)
                    _contentTextRtf.Append($"{FGCOLOR}{_colorTableManager.GetIndex(fg)}{rtfSpace}");

                _contentTextAscii.Append(text);
                _contentTextRtf.Append(
                    text.Replace(@"\", @"\\")
                        .Replace("{", @"\{")
                        .Replace("}", @"\}"));

                if (!bg.IsEmpty)
                    _contentTextRtf.Append(EMPTY_BGCOLOR);
                if (!fg.IsEmpty)
                    _contentTextRtf.Append(EMPTY_FGCOLOR);

                if (endLine)
                {
                    _contentTextAscii.Append("\n");
                    _contentTextRtf.Append("\\par");    // \\par = RTF Line Feed, same as "\n"
                    _removeSpace = "";
                }
                else
                    _removeSpace = !bg.IsEmpty || !fg.IsEmpty ? $"{rtfSpace}" : text.EndsWith(" ") ? " " : "";
            }
        }

        private void RtfSetupPage()
        {
            _contentTextAscii.Clear();
            _contentTextRtf.Clear();
            _contentTextRtf.Append(START_RTF_ANSI_PAGE);      // {\rtf1\ansi\deff0
            _contentTextRtf.Append(FONT_FAMILY_SIZE);        // font table + default font
            _contentTextRtf.Append(START_COLORS);            // {\colortbl;

            foreach (var colorIndex in _colorTableManager.ColorArray)
            {
                if (colorIndex.Equals(Color.Transparent))
                    continue;
                _contentTextRtf.Append($"\\red{colorIndex.R}\\green{colorIndex.G}\\blue{colorIndex.B};");
            }

            _contentTextRtf.Append("}");
        }
    }
}
