using System;
using System.IO;
using System.Drawing;
using Chizl.FileCompare;
using System.Diagnostics;
using Chizl.Applications;
using System.Windows.Forms;
using Chizl.FileComparer.utils;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Chizl.FileComparer
{
    public partial class StartForm : Form
    {
        delegate void NoParmDelegateEvent();

        private readonly Color LINE_COLOR = Color.Silver;
        private readonly Color ADD_COLOR = Color.FromArgb(128, 255, 128);
        private readonly Color DELETE_COLOR = Color.FromArgb(255, 190, 190);
        private readonly Color MODIFIED_COLOR = Color.FromArgb(255, 255, 128);
        private readonly Color BACK_COLOR = Color.Empty;
        private readonly Color OFFSET_COLOR = Color.FromArgb(255, 0, 0);
        private readonly Color HEX_COLOR = Color.FromArgb(0, 128, 0);
        private readonly Color PRINTABLE_COLOR = Color.FromArgb(0, 0, 255);
        
        private readonly static List<double> _addPerc = new List<double>();
        private readonly static List<double> _delPerc = new List<double>();
        private readonly static List<double> _modPerc = new List<double>();

        private string _oldFile = ".\\testfiles\\test_old.txt";
        private string _newFile = ".\\testfiles\\test_new.txt";

        public StartForm(string[] args)
        {
            InitializeComponent();

            if (args.Length>0)
                _oldFile = args[0];
            if (args.Length > 1)
                _newFile = args[1];
        }

        private void StartForm_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            this.OldAsciiFile.Text = _oldFile;
            this.NewAsciiFile.Text = _newFile;
            this.ScoreThresholdDropdown.Text = "30%";

            // hook up to the scroll handlers for the rich text boxes
            this.OldAsciiContent.VScroll += new EventHandler(this.RichText_VScroll);
            this.OldAsciiContent.HScroll += new EventHandler(this.RichText_HScroll);
            this.NewAsciiContent.VScroll += new EventHandler(this.RichText_VScroll);
            this.NewAsciiContent.HScroll += new EventHandler(this.RichText_HScroll);
            this.Text = About.TitleWithFileVersion;
        }
        private void RichText_VScroll(object sender, EventArgs e)
        {
            // vertical scroll handler
            if (sender is RichTextBox)
            {
                // get the right sender
                RichTextBox senderRtb = sender as RichTextBox;
                RichTextBox targetRtb = senderRtb == OldAsciiContent ? NewAsciiContent : OldAsciiContent;
                ApplyScroll(senderRtb, targetRtb, ScrollBarDirection.SB_VERT);
            }
        }
        private void RichText_HScroll(object sender, EventArgs e)
        {
            // horizontal scroll handler
            if (sender is RichTextBox)
            {
                // get the right sender
                RichTextBox senderRtb = sender as RichTextBox;
                RichTextBox targetRtb = senderRtb == OldAsciiContent ? NewAsciiContent : OldAsciiContent;
                ApplyScroll(senderRtb, targetRtb, ScrollBarDirection.SB_HORZ);
            }
        }
        private void ToolStripMenuItem2_Click(object sender, EventArgs e)=> this.Close();
        private void OldAsciiButton_Click(object sender, EventArgs e)
        {
            if (Common.GetFilePath(this, "Select Source/Older File", out string fileName))
                OldAsciiFile.Text = fileName;
        }
        private void NewAsciiButton_Click(object sender, EventArgs e)
        {
            if (Common.GetFilePath(this, "Select Target/Newier File", out string fileName))
                NewAsciiFile.Text = fileName;
        }
        private void CompareButtonToollbar_Click(object sender, EventArgs e) => CompareFiles();
        private void OldAsciiViewButton_Click(object sender, EventArgs e) => OpenExplorerAndSelectFile(OldAsciiFile.Text);
        private void NewAsciiViewButton_Click(object sender, EventArgs e) => OpenExplorerAndSelectFile(NewAsciiFile.Text);
        private void OldBinaryViewButton_Click(object sender, EventArgs e) => LoadBinaryHexView(OldAsciiFile.Text, OldAsciiContent);
        private void NewBinaryViewButton_Click(object sender, EventArgs e) => LoadBinaryHexView(NewAsciiFile.Text, NewAsciiContent);
        private void AsciiContent_Resize(object sender, EventArgs e)
        {
            //RichTextBox rtb = (RichTextBox)sender;
            //if (string.IsNullOrWhiteSpace(rtb.Text) || (!string.IsNullOrWhiteSpace(rtb.Text) && !rtb.Text.StartsWith("00000000:")))
            //{
            //    rtb.Font = new Font(rtb.Font.FontFamily, 8.25f);
            //    return;
            //}

            //rtb.Font = new Font(rtb.Font.FontFamily, 6.25f);
            //SetCurrentFontSize(rtb);
        }
        private void ChangeColor_Paint(object sender, PaintEventArgs e)
        {
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

        private void AddModifiedLine(RichTextBox rb, CharDiff[] charDiff, bool isNew)
        {
            foreach(var cd in charDiff)
            {
                switch(cd.DiffType)
                {
                    case DiffType.Added:
                        if (isNew)
                            AddText(rb, $"{cd.Char}", ADD_COLOR); 
                        break;
                    case DiffType.Deleted:
                        if (!isNew)
                            AddText(rb, $"{cd.Char}", DELETE_COLOR);
                        break;
                    case DiffType.Modified:
                    default:
                        AddText(rb, $"{cd.Char}", MODIFIED_COLOR);
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
                OldAsciiContent.Clear();
                NewAsciiContent.Clear();
                var score_threshold = .30;
                if (!string.IsNullOrWhiteSpace(ScoreThresholdDropdown.Text))
                {
                    var score = $".{ScoreThresholdDropdown.Text.Replace("%", "")}";
                    if (!double.TryParse(score, out score_threshold))
                        score_threshold = .30;
                }

                var prevLineSize = 30;
                var fileComparison = DiffTool.CompareFiles(OldAsciiFile.Text, NewAsciiFile.Text, score_threshold);

                if (fileComparison.HasException)
                {
                    StatusText.Text = $"Error:  {fileComparison.Exception.Message}";
                    StatusText.BackColor = Color.FromArgb(255, 192, 192);
                }
                else
                {
                    StatusText.Text = $"Line by Line Status:  Added( {fileComparison.Diffs.Added} ), Deleted( {fileComparison.Diffs.Deleted} ), Modified( {fileComparison.Diffs.Modified} ), No Change({fileComparison.Diffs.Identical} )";
                    StatusText.BackColor = SystemColors.Control;
                }

                var stringFiller = $"{new string(' ', prevLineSize)}\n";
                var maxPerc = fileComparison.LineComparison.Length;
                var line = 0;

                _addPerc.Clear();
                _delPerc.Clear();
                _modPerc.Clear();

                foreach (var cmpr in fileComparison.LineComparison)
                {
                    line++;

                    var lineNumber = $"{cmpr.LineNumber:000}: ";
                    var lineString = $"{cmpr.LineDiffStr}\n";
                    if (string.IsNullOrWhiteSpace(lineString))
                    {
                        stringFiller = $"{new string(' ', prevLineSize)}\n";
                        lineString = stringFiller;
                    }
                    else
                        stringFiller = $"{new string(' ', cmpr.LineDiffStr.Length)}\n";

                    AddText(OldAsciiContent, $"{lineNumber}", LINE_COLOR);
                    AddText(NewAsciiContent, $"{lineNumber}", LINE_COLOR);
                    AddText(OldAsciiContent, $" ");
                    AddText(NewAsciiContent, $" ");

                    switch (cmpr.DiffType)
                    {
                        case DiffType.Added:
                            _addPerc.Add((double)line / (double)maxPerc);
                            AddText(OldAsciiContent, stringFiller, ADD_COLOR);
                            AddText(NewAsciiContent, lineString, ADD_COLOR);
                            break;
                        case DiffType.Deleted:
                            _delPerc.Add((double)line / (double)maxPerc);
                            AddText(OldAsciiContent, lineString, DELETE_COLOR);
                            AddText(NewAsciiContent, stringFiller, DELETE_COLOR);
                            break;
                        case DiffType.Modified:
                            _modPerc.Add((double)line / (double)maxPerc);
                            AddModifiedLine(OldAsciiContent, cmpr.LineBreakDown, false);
                            AddModifiedLine(NewAsciiContent, cmpr.LineBreakDown, true);
                            break;
                        default:
                            AddText(OldAsciiContent, lineString);
                            AddText(NewAsciiContent, lineString);
                            break;
                    }
                    prevLineSize = lineString.Length;
                    if (prevLineSize == 0) prevLineSize = 30;
                }

                // if using project test files, show as CS so that coloring of keywords will work for CSharp.
                ColourRrbText(OldAsciiContent, OldAsciiFile.Text);
                ColourRrbText(NewAsciiContent, NewAsciiFile.Text);

                DiffColorScroll();
            }
        }
        private void DiffColorScroll()
        {
            this.OldChangeColor.Invalidate();
            this.NewChangeColor.Invalidate();
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

            var arrayList = DiffTool.ShowInHex(filePath);

            SetFontSize(arrayList[0], rtb);

            foreach (var line in arrayList)
            {
                AddText(rtb, $"{line.Offset}  ", BACK_COLOR, OFFSET_COLOR);
                AddText(rtb, $"{line.HexValues}  ", BACK_COLOR, HEX_COLOR);
                AddText(rtb, $"{line.PrintableChars}\n", BACK_COLOR, PRINTABLE_COLOR);
            }
        }
        private void SetFontSize(BinaryHexView bhv, RichTextBox rtb)
        {
            rtb.Clear();
            rtb.Font = new Font(rtb.Font.FontFamily, 6.25f);
            rtb.Clear();

            AddText(rtb, $"{bhv.Offset}  ", BACK_COLOR, OFFSET_COLOR);
            AddText(rtb, $"{bhv.HexValues}  ", BACK_COLOR, HEX_COLOR);
            AddText(rtb, $"{bhv.PrintableChars}\n", BACK_COLOR, PRINTABLE_COLOR);

            SetCurrentFontSize(rtb);

            rtb.Clear();
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
    }
}
