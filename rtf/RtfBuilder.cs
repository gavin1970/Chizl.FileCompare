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

        const string FONT_FAMILY_SIZE = @"{\rtf1\ansi\ansicpg1250\deff0\deflang1050{\fonttbl{\f0\fnil\fcharset238 Courier New;}}\fs17\r\n"; // \r\n
        const string START_COLORS = @"{\colortbl;";
        const string EMPTY_FGCOLOR = @"\cf0";       // \~
        const string EMPTY_BGCOLOR = @"\chcbpat0 ";

        private readonly RtfColorManager _colorTableManager;
        private readonly StringBuilder _contentTextRtf = new StringBuilder();
        private readonly StringBuilder _contentTextAscii = new StringBuilder();
        private Color _bgPrevColor = Color.Empty;
        private Color _fgPrevColor = Color.Empty;
        private bool _pageSetup = true;
        private int _lastFontSize = 17;
        private int _originalFontSize = 0;     // 17/2 = 8.5pt
        private string _fontFamilyWithSize = "{\\rtf1\\ansi\\ansicpg1250\\deff0\\deflang1050{\\fonttbl{\\f0\\fnil\\fcharset238 _FONTNAME_;}}\\fs_FONTSIZE_\r\n";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorIndexList">Colors to be used</param>
        /// <param name="fontName">(Optional) Default: Courier New - Suggested to use general font names as all systems might not have the font your settings.</param>
        /// <param name="fontPtSize"><code>
        /// (Optional) Default: 8.5pt - Size Range: (1.0/100.0 - Min/Max).
        /// NOTE: Font sizes can't be outside of 1/2 points.
        ///       8.0 or 8.5 for example are valid, 8.7 is not and will be rounded to 9.0.</code>
        /// </param>
        /// <param name="pageSetup">
        /// (Optional) Default: true<br/>
        /// When set to false, it's for line inserts with color into other RtfBuilder objects that already have the page setup.
        /// colorIndexList should match between the two for the correct results.
        /// </param>
        /// <exception cref="ArgumentException"></exception>
        public RtfBuilder(Color[] colorIndexList, bool pageSetup = true, string fontName = "Courier New", double fontPtSize = 8.5) 
        {
            if (!ValidFontSize(fontPtSize, out string msg))
                throw new ArgumentException(msg);

            if (string.IsNullOrWhiteSpace(fontName))
                throw new ArgumentException("Font name cannot be null or empty");

            // 1st Font Index, (17/2 = 8.5pt)
            _lastFontSize = (int)Math.Round(fontPtSize * 2, 0);

            _fontFamilyWithSize = _fontFamilyWithSize.Replace("_FONTNAME_", fontName).Replace("_FONTSIZE_", _lastFontSize.ToString());
            _colorTableManager = new RtfColorManager(colorIndexList);
            _pageSetup = pageSetup;
            RtfSetupPage();
        }

        public void Colors(Color bgColor, Color fgColor) => Append(string.Empty, bgColor, fgColor);
        public void FgColor(Color fgColor) => Append(string.Empty, Color.Empty, fgColor);
        public void BgColor(Color bgColor) => Append(string.Empty, bgColor, Color.Empty);
        public void ClearColor(bool bgColor = true, bool fgColor = true) => AddText(string.Empty, Color.Transparent, Color.Transparent, false, 0.0f);

        /// <summary>
        /// Append RTF string to RTF control with no return. This assumes all RTF codes being passed in rtfString are valid.
        /// </summary>
        /// <param name="rtfString">RTF string to append to RTF string.</param>
        public void AppendRtf(string rtfString) => AddRTF(rtfString, false);
        /// <summary>
        /// Append text to RTF string without a return.
        /// </summary>
        /// <param name="text">text to add to RTF string.</param>
        /// <param name="fontPtSize">(Optional) Default: 0.0 = use last font size. When changed, default will be the new size during an Append or AppendLine.</param>
        public void Append(string text, double fontPtSize = 0.0f) => AddText(text, Color.Empty, Color.Empty, false, fontPtSize);
        /// <summary>
        /// Append text to RTF string without a return.
        /// </summary>
        /// <param name="text">text to add to RTF string.</param>
        /// <param name="bg">(Optional) Default: RTF background color.</param>
        /// <param name="fontPtSize">(Optional) Default: 0.0 = use last font size. When changed, default will be the new size during an Append or AppendLine.</param>
        public void Append(string text, Color bg, double fontPtSize = 0.0f) => AddText(text, bg, Color.Empty, false, fontPtSize);
        /// <summary>
        /// Append text to RTF string without a return.
        /// </summary>
        /// <param name="text">text to add to RTF string.</param>
        /// <param name="bg">(Optional) Default: RTF background color.</param>
        /// <param name="fg">(Optional) Default: RTF text color.</param>
        /// <param name="fontPtSize">(Optional) Default: 0.0 = use last font size. When changed, default will be the new size during an Append or AppendLine.</param>
        public void Append(string text, Color bg, Color fg, double fontPtSize = 0.0f) => AddText(text, bg, fg, false, fontPtSize);

        /// <summary>
        /// Append RTF string to RTF control with return. This assumes all RTF codes being passed in rtfString are valid.
        /// </summary>
        /// <param name="rtfString">RTF string to append to RTF string.</param>
        public void AppendLineRtf(string rtfString) => AddRTF(rtfString, true);
        /// <summary>
                                                                   /// Append text to RTF string with a return at the end.
                                                                   /// </summary>
                                                                   /// <param name="text">text to add to RTF string.</param>
                                                                   /// <param name="fontPtSize">(Optional) Default: 0.0 = use last font size. When changed, default will be the new size during an Append or AppendLine.</param>
        public void AppendLine(string text, double fontPtSize = 0.0f) => AddText(text, Color.Empty, Color.Empty, true, fontPtSize);
        /// <summary>
        /// Append text to RTF string with a return at the end.
        /// </summary>
        /// <param name="text">text to add to RTF string.</param>
        /// <param name="bg">(Optional) Default: RTF background color.</param>
        /// <param name="fontPtSize">(Optional) Default: 0.0 = use last font size. When changed, default will be the new size during an Append or AppendLine.</param>
        public void AppendLine(string text, Color bg, double fontPtSize = 0.0f) => AddText(text, bg, Color.Empty, true, fontPtSize);
        /// <summary>
        /// Append text to RTF string with a return at the end.
        /// </summary>
        /// <param name="text">text to add to RTF string.</param>
        /// <param name="bg">(Optional) Default: RTF background color.</param>
        /// <param name="fg">(Optional) Default: RTF text color.</param>
        /// <param name="fontPtSize">(Optional) Default: 0.0 = use last font size. When changed, default will be the new size during an Append or AppendLine.</param>
        public void AppendLine(string text, Color bg, Color fg, double fontPtSize = 0.0f) => AddText(text, bg, fg, true, fontPtSize);

        /// <summary>
        /// Gets Content for RTF as a page or lines, based on initialization, then clears the RTF document if 'clear' argument is set to true.
        /// </summary>
        /// <param name="clear">Resets RTF document to empty and initializes based on original constructor params.</param>
        /// <returns>string value with all RTF codes.</returns>
        public string GetDocument(bool clear = true) 
        {
            var retVal = _pageSetup
                ? _contentTextRtf.AppendLine("}").ToString()
                : _contentTextRtf.ToString(); // close document

            if (clear)
                this.Clear();

            return retVal;
        }
        /// <summary>
        /// View existing RTF context data, which may or may not be complete.<br/>
        /// GetDocument(), will finalize the RTF document with any following RTF requirements.
        /// </summary>
        public string ViewRTFDocument() => _contentTextRtf.ToString();
        /// <summary>
        /// View the existing RTF content data in Ascii text form without any RTF codes.<br/>
        /// This is current state of the information.
        /// </summary>
        public string ViewAsciiDocument() => _contentTextAscii.ToString(); // close document
        /// <summary>
        /// Clears and sets RTF Document up as first setup.<br/>
        /// If set with PageSetup, then colors, font and size will reset back to those settings.<br/>
        /// If set without PageSetup, then colors and font size will be restored.  Font is based on the PageSetup only.
        /// </summary>
        public void Clear() => RtfSetupPage();

        internal void AddRTF(string text, bool endLine)
        {
            _contentTextAscii.Append(text);
            _contentTextRtf.Append(text);
            if (endLine)
            {
                _contentTextAscii.Append("\n");
                _contentTextRtf.Append("\\par");    // \\par = RTF Line Feed, same as "\n"
            }
        }
        internal void AddText(string text, Color bg, Color fg, bool endLine, double fontPtSize)
        {
            if (!string.IsNullOrEmpty(text))
            {
                // if previous background color wasn't empty and now it is, we want to reset color to default.
                if (!_bgPrevColor.IsEmpty && bg.IsEmpty)
                {
                    _contentTextRtf.Append(EMPTY_BGCOLOR);
                    _bgPrevColor = bg;
                }
                // if previous foreground color wasn't empty and now it is, we want to reset color to default.
                if (!_fgPrevColor.IsEmpty && fg.IsEmpty)
                {
                    _contentTextRtf.Append(EMPTY_FGCOLOR);
                    _fgPrevColor = fg;
                }
                // if background color isn't empty and previous background is different than new color, lets set color.
                if (!bg.IsEmpty && !_bgPrevColor.Equals(bg))
                {
                    _contentTextRtf.Append(_colorTableManager.GetIndex(bg, Color_Appearance.Background));
                    _bgPrevColor = bg;
                }
                // if foregroundcolor isn't empty and previous foregroundis different than new color, lets set color.
                if (!fg.IsEmpty && !_fgPrevColor.Equals(fg))
                {
                    _contentTextRtf.Append(_colorTableManager.GetIndex(fg, Color_Appearance.Foreground));
                    _fgPrevColor = fg;
                }

                if (_contentTextAscii.Length.Equals(0) && !_pageSetup)
                {
                    if (bg.IsEmpty && !_bgPrevColor.IsEmpty)
                        _contentTextRtf.Append(_colorTableManager.GetIndex(bg, Color_Appearance.Background));
                    if (fg.IsEmpty && !_fgPrevColor.IsEmpty)
                        _contentTextRtf.Append(_colorTableManager.GetIndex(fg, Color_Appearance.Foreground));
                }


                SetSize(fontPtSize);

                _contentTextAscii.Append(text);
                _contentTextRtf.Append(
                    text.Replace(@"\", @"\\")
                        .Replace("{", @"\{")
                        .Replace("}", @"\}"));
            }

            if (endLine)
            {
                _contentTextAscii.Append("\n");
                _contentTextRtf.Append("\\par");    // \\par = RTF Line Feed, same as "\n"
            }
        }
        private int GetSize(double fontPtSize) => (int)Math.Round(fontPtSize * 2, 0);
        private void SetSize(double fontPtSize, bool append = true)
        {
            if (fontPtSize != 0.0f)// && (int)Math.Round(fontPtSize * 2, 0) != _lastFontSize)
            {
                if (!ValidFontSize(fontPtSize, out string _))
                    return;

                var getRtfSize = GetSize(fontPtSize);

                if (_originalFontSize == 0)
                    _originalFontSize = getRtfSize;

                if (getRtfSize != _lastFontSize)
                {
                    _lastFontSize = getRtfSize;
                    if (append)
                        _contentTextAscii.Append($"\\fs{_lastFontSize} ");
                }
            }
        }
        private void RtfSetupPage()
        {
            _lastFontSize = _originalFontSize;
            // this is done, so that one next Append/AppendLine color passes in,
            // if it was the same as when this document was cleared, will pick back up.
            _fgPrevColor = Color.Transparent;
            _bgPrevColor = Color.Transparent;

            _contentTextAscii.Clear();
            _contentTextRtf.Clear();

            if (_pageSetup)
            {
                _contentTextRtf.Append(_fontFamilyWithSize);        // {\rtf1\ansi\deff0 and font table + default font
                _contentTextRtf.Append(START_COLORS);               // {\colortbl;

                foreach (var colorIndex in _colorTableManager.ColorArray)
                {
                    if (colorIndex.Equals(Color.Transparent))
                        continue;
                    _contentTextRtf.Append($"\\red{colorIndex.R}\\green{colorIndex.G}\\blue{colorIndex.B};");
                }

                _contentTextRtf.Append("}");
            }
        }
        private bool ValidFontSize(double fontPtSize, out string msg)
        {
            var retVal = true;
            if (fontPtSize < 1f || fontPtSize > 100f)
            {
                msg = "Font size must be between 1.0pt and 100.0pt";
                retVal = false;
            }
            else
            {
                if (_originalFontSize.Equals(0))
                    _originalFontSize = GetSize(fontPtSize);

                msg = string.Empty;
            }

            return retVal;
        }

    }
}
