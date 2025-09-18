using System;
using System.IO;
using Chizl.Rtf;
using System.Text;
using System.Linq;
using System.Drawing;
using Chizl.FileCompare;
using Chizl.Applications;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Chizl.FileComparer
{
    public partial class StartForm : Form
    {
        delegate void NoParmDelegateEvent();
        delegate void RtbDelegateEvent(RichTextBox srcRtb, RichTextBox trgRtb);

        const int OLD_DROPDOWN = 1;
        const int NEW_DROPDOWN = 2;
        const int OLD_NEW_DROPDOWN = 3;
        const string _fileHistoryName = "./cfc_file.history";
        const string _fileIconName = "./compare.ico";

        private readonly Color LINE_COLOR = Color.Silver;
        private readonly Color ADD_COLOR = Color.FromArgb(128, 255, 128);
        private readonly Color DELETE_COLOR = Color.FromArgb(255, 190, 190);
        private readonly Color MODIFIED_COLOR = Color.FromArgb(255, 255, 0);
        private readonly Color BACK_COLOR = Color.Empty;
        private readonly Color OFFSET_COLOR = Color.FromArgb(255, 0, 0);
        private readonly Color HEX_COLOR = Color.FromArgb(0, 128, 0);
        private readonly Color PRINTABLE_COLOR = Color.FromArgb(0, 0, 255);

        private readonly static List<double> _addPerc = new List<double>();
        private readonly static List<double> _delPerc = new List<double>();
        private readonly static List<double> _modPerc = new List<double>();

        private static readonly ConcurrentDictionary<string, int> _fileHistory = new ConcurrentDictionary<string, int>();
        private static ComparisonResults _lastFileComparison = ComparisonResults.Empty;
        private static bool _formResizing = false;
        private static bool _isSideBySide = true;
        private static RichTextBox _srcRtb;
        private static RichTextBox _trgRtb;

        private string _oldFile = ".\\testfiles\\test_old.txt";
        private string _newFile = ".\\testfiles\\test_new.txt";
        //private string _oldFile = "C:\\Program Files\\TechSmith\\Snagit 2024\\de-DE\\TechSmith Snagit EULA.pdf";
        //private string _newFile = "C:\\Program Files\\TechSmith\\Snagit 2024\\en-US\\TechSmith Snagit EULA.pdf";

        public StartForm(string[] args)
        {
            InitializeComponent();

            if (args.Length > 0)
                _oldFile = args[0];
            if (args.Length > 1)
                _newFile = args[1];
        }
        private void StartForm_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            this.OldAsciiFile.Text = File.Exists(_oldFile) ? _oldFile : "";
            this.NewAsciiFile.Text = File.Exists(_newFile) ? _newFile : "";
            this.ScoreThresholdDropdown.Text = "30%";
            this.OverlayDropdown.SelectedIndex = 0;

            // hook up to the scroll handlers for the rich text boxes
            this.OldAsciiContent.VScroll += new EventHandler(this.RichText_VScroll);
            this.OldAsciiContent.HScroll += new EventHandler(this.RichText_HScroll);
            this.NewAsciiContent.VScroll += new EventHandler(this.RichText_VScroll);
            this.NewAsciiContent.HScroll += new EventHandler(this.RichText_HScroll);
            this.Text = About.TitleWithFileVersion;

            ResetContent();
            LoadHistory();

            if (File.Exists(_fileIconName))
                this.Icon = Common.GetIcon(_fileIconName);

            // LoadTestRtf();
        }
        private void LoadTestRtf()
        {
            var myText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor " +
                         "incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud " +
                         "exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute " +
                         "irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla " +
                         "pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia " +
                         "deserunt mollit anim id est laborum.";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{\\rtf1\\ansi\\ansicpg1250\\deff0\\deflang1050{\\fonttbl{\\f0\\fnil\\fcharset238 Courier New;}}");
            sb.AppendLine("{\\colortbl ;\\red0\\green190\\blue0;\\red190\\green0\\blue190;}");
            sb.AppendLine("\\viewkind4\\uc1\\pard\\f0\\fs17");

            string[] bgCol = new string[] { "\\chcbpat1 ", "\\chcbpat2 " };
            string[] fgCol = new string[] { "\\cf2 ", "\\cf1 " };

            int ndx = 0;
            for (int i = 0; i < myText.Length; i++)
            {
                if (i != 0)
                {
                    if (i % 100 == 0)
                        sb.Append("\\par ");    // carriage return

                    if (i % 10 == 0)
                    {
                        sb.Append("\\chcbpat0 ");   // turn BG color off
                        sb.Append("\\cf0 ");        // turn FG color off
                    }
                    else if (i % 5 == 0)
                    {
                        ndx = ndx == 1 ? 0 : 1;
                        sb.Append(bgCol[ndx]);
                        sb.Append(fgCol[ndx]);
                    }
                }

                sb.Append(myText[i]);
            }
            sb.Append("\\par\r\n}\r\n");

            OldAsciiContent.Rtf = sb.ToString();
        }
        private void StartForm_ResizeBegin(object sender, EventArgs e) => _formResizing = true;
        private void StartForm_ResizeEnd(object sender, EventArgs e)
        {
            _formResizing = false;
            InvalidateAll();
        }
        private void LoadHistory()
        {
            const int PATH = 0;
            const int DOWNDOWN_FLAG = 1;

            if (File.Exists(_fileHistoryName))
            {
                foreach (var line in File.ReadAllLines(_fileHistoryName))
                {
                    var sLine = line.Split('|');
                    if (sLine.Length == 2)
                    {
                        if (int.TryParse(sLine[DOWNDOWN_FLAG], out int val) && val >= OLD_DROPDOWN && val <= OLD_NEW_DROPDOWN)
                        {
                            if (!File.Exists(sLine[PATH]))
                                continue;

                            if (_fileHistory.TryAdd(sLine[PATH], val))
                            {
                                if ((val & OLD_DROPDOWN) == OLD_DROPDOWN)
                                    this.OldAsciiFile.Items.Add(sLine[PATH]);

                                if ((val & NEW_DROPDOWN) == NEW_DROPDOWN)
                                    this.NewAsciiFile.Items.Add(sLine[PATH]);
                            }
                        }
                    }
                }
            }
        }
        private void RichText_VScroll(object sender, EventArgs e)
        {
            if (_formResizing)
                return;

            // vertical scroll handler
            if (sender is RichTextBox)
            {
                // get the right sender
                RichTextBox senderRtb = sender as RichTextBox;
                RichTextBox targetRtb = senderRtb == this.OldAsciiContent ? this.NewAsciiContent : this.OldAsciiContent;
                ApplyScroll(senderRtb, targetRtb, ScrollBarDirection.SB_VERT);
            }
        }
        private void RichText_HScroll(object sender, EventArgs e)
        {
            if (_formResizing)
                return;

            // horizontal scroll handler
            if (sender is RichTextBox)
            {
                // get the right sender
                RichTextBox senderRtb = sender as RichTextBox;
                RichTextBox targetRtb = senderRtb == this.OldAsciiContent ? this.NewAsciiContent : this.OldAsciiContent;
                ApplyScroll(senderRtb, targetRtb, ScrollBarDirection.SB_HORZ);
            }
        }
        private void ToolStripMenuItem2_Click(object sender, EventArgs e) => this.Close();
        private void OldAsciiButton_Click(object sender, EventArgs e)
        {
            if (Common.GetFilePath(this, "Select Source/Older File", out string fileName))
            {
                OldAsciiFile.Text = fileName;
                ResetContent();
            }
        }
        private void NewAsciiButton_Click(object sender, EventArgs e)
        {
            if (Common.GetFilePath(this, "Select Target/Newier File", out string fileName))
            {
                NewAsciiFile.Text = fileName;
                ResetContent();
            }
        }
        private void CompareButtonToollbar_Click(object sender, EventArgs e) => CompareFiles();
        private void OldAsciiViewButton_Click(object sender, EventArgs e) => OpenExplorerAndSelectFile(OldAsciiFile.Text);
        private void NewAsciiViewButton_Click(object sender, EventArgs e) => OpenExplorerAndSelectFile(NewAsciiFile.Text);
        private void OldBinaryViewButton_Click(object sender, EventArgs e) => LoadBinaryHexView(OldAsciiFile.Text, this.OldAsciiContent);
        private void NewBinaryViewButton_Click(object sender, EventArgs e) => LoadBinaryHexView(NewAsciiFile.Text, this.NewAsciiContent);
        private void ChangeColor_Paint(object sender, PaintEventArgs e)
        {
            if (_formResizing)
                return;

            var g = e.Graphics;
            var maxPerc = (double)e.ClipRectangle.Height;
            var r = e.ClipRectangle.Width;

            Pen addPen = new Pen(Color.Green, 1);
            Pen delPen = new Pen(Color.Red, 1);
            Pen modPen = new Pen(Color.Orange, 1);

            foreach (var a in _addPerc)
            {
                var t = (float)(maxPerc * a);
                g.DrawLine(addPen, 0, t, r, t);
            }
            foreach (var a in _delPerc)
            {
                var t = (float)(maxPerc * a);
                g.DrawLine(delPen, 0, t, r, t);
            }
            foreach (var a in _modPerc)
            {
                var t = (float)(maxPerc * a);
                g.DrawLine(modPen, 0, t, r, t);
            }
        }

        private void AddModifiedLine(RichTextBox rb, ByteLevel[] charDiff, bool isNew, bool asHex = false)
        {
            foreach (var cd in charDiff)
            {
                switch (cd.DiffType)
                {
                    case DiffType.Added:
                        if (isNew)
                            AddText(rb, $"{(asHex ? $"{cd.Hex} " : $"{cd.Char}")}", ADD_COLOR);
                        break;
                    case DiffType.Deleted:
                        if (!isNew)
                            AddText(rb, $"{(asHex ? $"{cd.Hex} " : $"{cd.Char}")}", DELETE_COLOR);
                        break;
                    case DiffType.Modified:
                    default:
                        AddText(rb, $"{(asHex ? $"{cd.Hex} " : $"{cd.Char}")}", MODIFIED_COLOR);
                        break;
                }
            }
            AddText(rb, $"\n", MODIFIED_COLOR);
        }
        private void AddText(RichTextBox rb, string text) => AddText(rb, text, Color.Empty, Color.Empty);
        private void AddText(RichTextBox rb, string text, Color bgColor) => AddText(rb, text, bgColor, Color.Empty);
        private void AddText(RichTextBox rb, string text, Color bgColor, Color fgColor)
        {
            rb.SelectionStart = rb.TextLength;
            rb.SelectionBackColor = bgColor.IsEmpty ? rb.BackColor : bgColor;
            rb.SelectionColor = fgColor.IsEmpty ? rb.ForeColor : fgColor;
            rb.AppendText(text);
        }

        private void ColourRrbText(RichTextBox rtb, string fullFilePath)
        {
            var ext = fullFilePath.ToLower().StartsWith(".\\testfiles\\test_") ? ".cs" : Path.GetExtension(OldAsciiFile.Text).ToLower();

            switch (ext)
            {
                case ".cs":
                    ColorCSharpKeywords(rtb);
                    break;
            }
        }
        private void ColorCSharpKeywords(RichTextBox rtb)
        {
            foreach (Match match in CSharpRegexPatterns.Pattern.Matches(rtb.Text))
            {
                foreach (var groupName in CSharpRegexPatterns.Pattern.GetGroupNames())
                {
                    if (match.Groups[groupName].Success && CSharpRegexPatterns.Colors.ContainsKey(groupName))
                    {
                        rtb.Select(match.Groups[groupName].Index, match.Groups[groupName].Length);
                        rtb.SelectionColor = CSharpRegexPatterns.Colors[groupName];
                        break; // only one group should match
                    }
                }
            }
        }

        private void CompareFiles()
        {
            if (InvokeRequired)
            {
                var d = new NoParmDelegateEvent(CompareFiles);
                if (!Disposing && !IsDisposed)
                {
                    try { Invoke(d); }
                    catch (ObjectDisposedException ex) { Debug.WriteLine(ex.Message); }
                    catch { /* Ingore, shutting down. */ }
                }
            }
            else if (!Disposing && !IsDisposed)
            {
                // this.OldAsciiContent.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                // this.NewAsciiContent.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

                this.SplitContainer1.Panel1Collapsed = false;
                this.SplitContainer1.Panel2Collapsed = false;

                ClearRichText();

                var score_threshold = .30;
                if (!string.IsNullOrWhiteSpace(ScoreThresholdDropdown.Text))
                {
                    var score = $".{ScoreThresholdDropdown.Text.Replace("%", "")}";
                    if (!double.TryParse(score, out score_threshold))
                        score_threshold = .30;
                }

                // remove any quotes 
                var oldFile = this.OldAsciiFile.Text.Replace("\"", "").Replace("'", "");
                var newFile = this.NewAsciiFile.Text.Replace("\"", "").Replace("'", "");

                _lastFileComparison = DiffTool.CompareFiles(oldFile, newFile, score_threshold, 3);
                //Task.Run(() => { _lastFileComparison = DiffTool.CompareFiles(oldFile, newFile, score_threshold, 3); }).Wait();

                // Add OLD_DROPDOWN as the value, to represent Old File downdown component.  If Key already exists,
                // check existingvalue and if New File dropdown (2), make it 3, to represent both Old and New
                // should have it.  If existing value isn't 2, leave it as it, because 1 or 3 already exists.
                _fileHistory.AddOrUpdate(oldFile.Trim(), OLD_DROPDOWN,
                    (key, existingValue) => { return existingValue == NEW_DROPDOWN ? OLD_NEW_DROPDOWN : existingValue; });

                // Add 2 as the value, to represent New File downdown component.  If Key already exists, check existing
                // value and if Old File dropdown (1), make it 3, to represent both Old and New should have it.
                // If existing value isn't 1, leave it as it, because 2 or 3 already exists.
                _fileHistory.AddOrUpdate(newFile.Trim(), NEW_DROPDOWN,
                    (key, existingValue) => { return existingValue == OLD_DROPDOWN ? OLD_NEW_DROPDOWN : existingValue; });

                this.ViewAsBinaryButtonToollbar.Visible = !_lastFileComparison.IsBinary;

                if (_lastFileComparison.HasException)
                    MessageBox.Show(this, _lastFileComparison.Exception.Message, About.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    ShowComparison(false, _isSideBySide);
                    UpdateHistoryFile();
                }
            }
        }
        private void UpdateHistoryFile()
        {
            StringBuilder sb = new StringBuilder();
            var fHistory = _fileHistory.Select(k => $"{k.Key}|{k.Value}").ToArray();
            File.WriteAllLines(_fileHistoryName, fHistory);
        }
        private void InvalidateAll()
        {
            this.OldChangeColor.Invalidate();
            this.NewChangeColor.Invalidate();
            this.Invalidate();
        }
        private void OpenExplorerAndSelectFile(string filePath)
        {
            if (File.Exists(filePath))
                // The /select parameter tells explorer.exe to open to the file's directory and select the specified file.
                Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            else
                // Handle the case where the file does not exist (e.g., log an error, show a message).
                MessageBox.Show($"Error: File not found at '{filePath}'", About.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void LoadBinaryHexView(string filePath, RichTextBox rtb)
        {
            if (!File.Exists(filePath))
                return;

            ResetContent(true);

            rtb.Clear();

            if (rtb.Name.StartsWith("Old"))
            {
                if (SplitContainer1.Panel2Collapsed)
                {
                    SplitContainer1.Panel2Collapsed = false;
                    return;
                }
                SplitContainer1.Panel2Collapsed = true;
                SplitContainer1.Panel1Collapsed = false;
            }
            else
            {
                if (SplitContainer1.Panel1Collapsed)
                {
                    SplitContainer1.Panel1Collapsed = false;
                    return;
                }
                SplitContainer1.Panel1Collapsed = true;
                SplitContainer1.Panel2Collapsed = false;
            }

            // var arrayList = DiffTool.ShowInHex(filePath);
            foreach (var line in DiffTool.ShowInHex(filePath))
            {
                AddText(rtb, $"{line.Offset}  ", BACK_COLOR, OFFSET_COLOR);
                AddText(rtb, $"{line.HexValues}  ", BACK_COLOR, HEX_COLOR);
                AddText(rtb, $"{line.PrintableChars}\n", BACK_COLOR, PRINTABLE_COLOR);
            }

            StatusText.Text = $"HexView auto zooms text based on window.  Use Control-MouseWheel to Zoom In/Out.  Click HexView again to exit HexView.";
            StatusText.BackColor = Color.Yellow;
            ResetTimer.Enabled = true;
        }
        private void ResetContent(bool binaryButtonOnly = false)
        {
            this.ViewAsBinaryButtonToollbar.Visible = false;
            this.ViewAsBinaryButtonToollbar.Text = _binaryViewBtnTxt;

            if (binaryButtonOnly)
                return;

            if (this.SplitContainer1.Panel1Collapsed)
                this.SplitContainer1.Panel1Collapsed = false;
            if (this.SplitContainer1.Panel2Collapsed)
                this.SplitContainer1.Panel2Collapsed = false;

            ClearRichText();

            this.OldAsciiContent.ScrollBars = RichTextBoxScrollBars.Both;
            this.NewAsciiContent.ScrollBars = RichTextBoxScrollBars.Both;

            EnableButtons();
            InvalidateAll();
        }
        private void ClearRichText()
        {
            this.OldAsciiContent.Clear();
            this.NewAsciiContent.Clear();

            // seem to be a odd bug where the scrollbars are not showing up, until the
            // form is resized.  The RichTextBoxes are both using Dock = Fill, so setting
            // Width=0 doesn't change the size, but it forces the scrollbars to show up.
            this.NewAsciiContent.Width = 0;
            this.OldAsciiContent.Width = 0;
        }
        private void EnableButtons()
        {
            var oldEnabled = !string.IsNullOrWhiteSpace(this.OldAsciiFile.Text) && File.Exists(this.OldAsciiFile.Text) && this.OldAsciiContent.Text.Length > 0;
            this.OldBinaryViewButton.Enabled = oldEnabled;
            this.OldFileViewButton.Enabled = oldEnabled;

            var newEnabled = !string.IsNullOrWhiteSpace(this.NewAsciiFile.Text) && File.Exists(this.NewAsciiFile.Text) && this.NewAsciiContent.Text.Length > 0;
            this.NewBinaryViewButton.Enabled = newEnabled;
            this.NewFileViewButton.Enabled = newEnabled;
        }
        private void SetCurrentFontSize(RichTextBox rtb)
        {
            if (string.IsNullOrWhiteSpace(rtb.Text))
                return;

            SizeF textSize = TextRenderer.MeasureText(rtb.Text, rtb.Font, rtb.Size, TextFormatFlags.WordBreak);
            float currentFontSize = rtb.Font.Size;
            float newFontSize = currentFontSize * (rtb.ClientRectangle.Width / textSize.Width);
            rtb.Font = new Font(rtb.Font.FontFamily, newFontSize);
        }
        private void ApplyScroll(RichTextBox Source, RichTextBox Target, ScrollBarDirection Direction)
        {
            // unhook target from relevant event, otherwise we end up in an infinite loop!
            switch (Direction)
            {
                case ScrollBarDirection.SB_VERT:
                    Target.VScroll -= RichText_VScroll;
                    break;
                case ScrollBarDirection.SB_HORZ:
                    Target.HScroll -= RichText_HScroll;
                    break;
            }

            IntPtr ptrLparam = new IntPtr(0);
            IntPtr ptrWparam;

            // Prepare scroll info struct
            SCROLLINFO si = new SCROLLINFO();
            si.cbSize = (uint)Marshal.SizeOf(si);
            si.fMask = (uint)ScrollInfoMask.SIF_ALL;

            // Get current scroller posion
            ExternApi.GetScrollInfo(Source.Handle, (int)Direction, ref si);

            // if we're tracking, set target to current track position
            if ((si.nTrackPos > 0) || ((si.nTrackPos == 0) && (si.nPos != 0)))
                si.nPos = si.nTrackPos;

            // Reposition scroller
            ExternApi.SetScrollInfo(Target.Handle, (int)Direction, ref si, true);
            ptrWparam = new IntPtr(ExternApi.SB_THUMBTRACK + 0x10000 * si.nPos);

            // send the relevant message to the target control, and rehook the event
            switch (Direction)
            {
                case ScrollBarDirection.SB_VERT:
                    ExternApi.SendMessage(Target.Handle, ExternApi.WM_VSCROLL, ptrWparam, ptrLparam);
                    Target.VScroll += new EventHandler(this.RichText_VScroll);
                    break;
                case ScrollBarDirection.SB_HORZ:
                    ExternApi.SendMessage(Target.Handle, ExternApi.WM_HSCROLL, ptrWparam, ptrLparam);
                    Target.HScroll += new EventHandler(this.RichText_HScroll);
                    break;
            }
        }
        private void ViewAsBinaryButtonToollbar_Click(object sender, EventArgs e)
        {
            if (ViewAsBinaryButtonToollbar.Text == _binaryViewBtnTxt)
            {
                ViewAsBinaryButtonToollbar.Text = _asciiViewBtnTxt;
                ShowComparison(true, _isSideBySide);
            }
            else
            {
                ViewAsBinaryButtonToollbar.Text = _binaryViewBtnTxt;
                ShowComparison(false, true);    // Ascii is always Side By Side, 
            }
        }
        private void Dropdown_SelectedIndexChanged(object sender, EventArgs e) => ViewAsBinaryButtonToollbar.Visible = false;
        private void ResetTimer_Tick(object sender, EventArgs e)
        {
            ResetTimer.Enabled = false;
            StatusText.BackColor = SystemColors.ControlDark;
        }
        private void OverlayDropdown_SelectedIndexChanged(object sender, EventArgs e) => _isSideBySide = OverlayDropdown.SelectedIndex.Equals(0);
        /// <summary>
        /// This method was seperated so it can be used to call, without doing a full compare on the files again.
        /// This will load the RichText components with information, both ASCII or Binary (Hex) View, and show color differences.
        /// </summary>
        /// <param name="forceBinary">True to show Ascii in Binary view.</param>
        /// <param name="sideBySide">False will do an overlay only for Binary view.</param>
        private void ShowComparison(bool forceBinary = false, bool sideBySide = true)
        {
            ClearRichText();

            var prevLineSize = 30;
            var useBinary = forceBinary || _lastFileComparison.IsBinary;

            if (_lastFileComparison.HasException)
            {
                StatusText.Text = $"Error:  {_lastFileComparison.Exception.Message}";
                StatusText.BackColor = Color.FromArgb(255, 192, 192);
                StatusText.Invalidate();
                return;
            }
            else
            {
                StatusText.Text = $"Line by Line Status:  Added( {_lastFileComparison.Diffs.Added} ), Deleted( {_lastFileComparison.Diffs.Deleted} ), Modified( {_lastFileComparison.Diffs.Modified} ), No Change({_lastFileComparison.Diffs.Identical} )";
                StatusText.BackColor = Color.Yellow;
                StatusText.Invalidate();
            }

            ResetTimer.Enabled = true;

            var lines = _lastFileComparison.LineComparison.OrderBy(o => o.LineNumber).ToArray();
            var maxPerc = lines.Length;
            var scrollLineMarker = 0;
            StatusText.Tag = $"Lines: {lines.Length} / {lines.Count()}";

            _addPerc.Clear();
            _delPerc.Clear();
            _modPerc.Clear();

            var oldRtfBuilder = new RtfBuilder(new Color[4] { LINE_COLOR, ADD_COLOR, DELETE_COLOR, MODIFIED_COLOR });
            var newRtfBuilder = new RtfBuilder(new Color[4] { LINE_COLOR, ADD_COLOR, DELETE_COLOR, MODIFIED_COLOR });

            if (!useBinary)
            {
                var stringFiller = $"{new string(' ', prevLineSize)}\n";
                foreach (var cmpr in lines)
                {
                    scrollLineMarker++;

                    var lineNumber = $"{cmpr.LineNumber:0000}: ";
                    var lineString = $"{cmpr.LineDiffStr}";

                    // removing any extra that might be residing..
                    while (lineString.EndsWith("\n") || lineString.EndsWith("\r") || lineString.EndsWith("?"))
                        lineString = lineString.Substring(0, lineString.Length - 1);

                    if (string.IsNullOrWhiteSpace(lineString))
                    {
                        stringFiller = $"{new string(' ', prevLineSize)}\n";
                        lineString = stringFiller;
                    }
                    else
                    {
                        stringFiller = $"{new string(' ', lineString.Length)}\n";
                        lineString += "\n";
                    }

                    oldRtfBuilder.Append($"{lineNumber}", LINE_COLOR);
                    oldRtfBuilder.Append($" ");
                    newRtfBuilder.Append($"{lineNumber}", LINE_COLOR);
                    newRtfBuilder.Append($" ");

                    switch (cmpr.DiffType)
                    {
                        case DiffType.Added:
                            _addPerc.Add((double)scrollLineMarker / (double)maxPerc);
                            oldRtfBuilder.AppendLine($"{stringFiller}", ADD_COLOR);
                            newRtfBuilder.AppendLine($"{lineString}", ADD_COLOR);
                            break;
                        case DiffType.Deleted:
                            _delPerc.Add((double)scrollLineMarker / (double)maxPerc);
                            oldRtfBuilder.AppendLine($"{lineString}", DELETE_COLOR);
                            newRtfBuilder.AppendLine($"{stringFiller}", DELETE_COLOR);
                            break;
                        case DiffType.Modified:
                            _modPerc.Add((double)scrollLineMarker / (double)maxPerc);
                            // windows
                            var ignoreLast2 = cmpr.ByteByByteDiff[cmpr.ByteByByteDiff.Length - 2].Byte == 13;
                            // linux
                            var ignoreLast1 = cmpr.ByteByByteDiff[cmpr.ByteByByteDiff.Length - 1].Byte == 10;
                            // if /r or /n then we want to stop before it's displayed.
                            var forLength = ignoreLast2 ? cmpr.ByteByByteDiff.Length - 2 : ignoreLast1 ? cmpr.ByteByByteDiff.Length - 1 : cmpr.ByteByByteDiff.Length;

                            for (int i = 0; i < forLength; i++)
                            {
                                var b = cmpr.ByteByByteDiff[i];
                                var color = DELETE_COLOR;
                                switch (b.DiffType)
                                {
                                    case DiffType.Added:
                                        newRtfBuilder.Append($"{b.Str}", ADD_COLOR);
                                        break;
                                    case DiffType.Deleted:
                                        oldRtfBuilder.Append($"{b.Str}", DELETE_COLOR);
                                        break;
                                    case DiffType.Modified:
                                    case DiffType.None:
                                        oldRtfBuilder.Append($"{b.Str}", MODIFIED_COLOR);
                                        newRtfBuilder.Append($"{b.Str}", MODIFIED_COLOR);
                                        break;
                                }
                            }
                            oldRtfBuilder.AppendLine("");   // add carriage return
                            newRtfBuilder.AppendLine("");   // add carriage return
                            break;
                        default:
                            oldRtfBuilder.AppendLine($"{lineString}");
                            newRtfBuilder.AppendLine($"{lineString}");
                            break;
                    }
                    prevLineSize = lineString.Length;
                    if (prevLineSize == 0) prevLineSize = 30;
                }
            }
            else
            {
                scrollLineMarker = 0;

                var oldPlainText = $"";
                var newPlainText = $"";
                var hexSize = 0;

                var hexCountLineMarker = scrollLineMarker;
                maxPerc = lines.Length;

                foreach (var cmpr in lines)
                {
                    scrollLineMarker++;
                    var byteCounter = 0;

                    foreach (var byteDff in cmpr.ByteByByteDiff)
                    {
                        if (hexSize.Equals(0))
                        {
                            var lineNumber = $"{hexCountLineMarker:X8}";
                            hexCountLineMarker += 16;

                            oldRtfBuilder.Append($" {lineNumber}h ", LINE_COLOR);
                            oldRtfBuilder.Append(" ");
                            if (sideBySide)
                            {
                                newRtfBuilder.Append($" {lineNumber}h ", LINE_COLOR);
                                newRtfBuilder.Append(" ");
                            }
                        }

                        hexSize++;

                        var stringFiller = "  ";
                        var hexString = byteDff.Hex;

                        hexString += hexSize < 16 ? " " : "";
                        stringFiller += hexSize < 16 ? " " : "";

                        switch (byteDff.DiffType)
                        {
                            case DiffType.Added:
                                _addPerc.Add((double)scrollLineMarker / (double)maxPerc);

                                if (sideBySide)
                                {
                                    newRtfBuilder.Append(hexString, ADD_COLOR);
                                    newPlainText += byteDff.Str;

                                    oldRtfBuilder.Append(stringFiller, ADD_COLOR);
                                    oldPlainText += "+";
                                }
                                else
                                {
                                    oldRtfBuilder.Append(hexString, ADD_COLOR);
                                    oldPlainText += byteDff.Str;
                                }

                                break;
                            case DiffType.Deleted:
                                _delPerc.Add((double)scrollLineMarker / (double)maxPerc);

                                if (sideBySide)
                                {
                                    newRtfBuilder.Append(stringFiller, DELETE_COLOR);
                                    newPlainText += "-";
                                }

                                oldRtfBuilder.Append(hexString, DELETE_COLOR);
                                oldPlainText += byteDff.Str;

                                break;
                            case DiffType.Modified:
                                _modPerc.Add((double)scrollLineMarker / (double)maxPerc);

                                if (sideBySide)
                                {
                                    newRtfBuilder.Append(hexString, MODIFIED_COLOR);
                                    newPlainText += byteDff.Str;
                                }

                                oldRtfBuilder.Append(hexString, MODIFIED_COLOR);
                                oldPlainText += byteDff.Str;

                                break;
                            default:
                                if (sideBySide)
                                {
                                    newRtfBuilder.Append(hexString);
                                    newPlainText += byteDff.Str;
                                }

                                oldRtfBuilder.Append(hexString);
                                oldPlainText += byteDff.Str;

                                break;
                        }

                        if (hexSize >= 16)
                        {
                            // the ending of a color in RTF forces a space. We need to remove 1 space to counter it.
                            oldRtfBuilder.AppendLine($"  {oldPlainText}");
                            newRtfBuilder.AppendLine($"  {newPlainText}");

                            hexSize = 0;
                            newPlainText = "";
                            oldPlainText = "";
                        }

                        byteCounter++;
                    }
                }

                // "AF " = 3 bytes.
                var padString = hexSize < 16 ? new string(' ', (16 * 3) - (hexSize * 3)) : "";
                oldRtfBuilder.Append(padString);
                newRtfBuilder.Append(padString);

                oldRtfBuilder.AppendLine($"  {oldPlainText}");
                newRtfBuilder.AppendLine($"  {newPlainText}");
            }

            this.OldAsciiContent.Rtf = oldRtfBuilder.GetDocument();

            if (sideBySide || !useBinary)
                this.NewAsciiContent.Rtf = newRtfBuilder.GetDocument();

            this.SplitContainer1.Panel2Collapsed = !sideBySide && useBinary;

            EnableButtons();
            InvalidateAll();
        }

        /// <summary>
        /// Use to sync up ZoomFactor between RichTextBoxes.
        /// </summary>
        private void ZoomCheck_KeyDown(object sender,  KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                _srcRtb = (RichTextBox)sender;
                _trgRtb = _srcRtb.Name.Equals("OldAsciiContent") ? NewAsciiContent : OldAsciiContent;
            }
        }
        private void ZoomCheck_KeyUp(object sender, KeyEventArgs e)
        {
            if (_trgRtb == null || _srcRtb == null)
                return;

            if (e.KeyCode.Equals(Keys.ControlKey))
                _trgRtb.ZoomFactor = _srcRtb.ZoomFactor;
        }
        private void Content_Leave(object sender, EventArgs e) => ZoomCheck_KeyUp(sender, new KeyEventArgs(Keys.ControlKey));
    }
}
