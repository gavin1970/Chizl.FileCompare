namespace Chizl.FileComparer
{
    partial class StartForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.OldAsciiContent = new System.Windows.Forms.RichTextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.OldChangeColor = new System.Windows.Forms.Panel();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.OldAsciiFile = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.OldBinaryViewButton = new System.Windows.Forms.Button();
            this.OldFileViewButton = new System.Windows.Forms.Button();
            this.OldAsciiButton = new System.Windows.Forms.Button();
            this.NewAsciiContent = new System.Windows.Forms.RichTextBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.NewChangeColor = new System.Windows.Forms.Panel();
            this.Panel3 = new System.Windows.Forms.Panel();
            this.NewAsciiFile = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Panel4 = new System.Windows.Forms.Panel();
            this.NewBinaryViewButton = new System.Windows.Forms.Button();
            this.NewFileViewButton = new System.Windows.Forms.Button();
            this.NewAsciiButton = new System.Windows.Forms.Button();
            this.StartFormMenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ScoreThresholdDropdown = new System.Windows.Forms.ToolStripComboBox();
            this.StartFormStatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusPadding = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.CompareButtonToollbar = new System.Windows.Forms.Button();
            this.ResetTimer = new System.Windows.Forms.Timer(this.components);
            this.ViewAsBinaryButtonToollbar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.Panel2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.Panel3.SuspendLayout();
            this.Panel4.SuspendLayout();
            this.StartFormMenuStrip.SuspendLayout();
            this.StartFormStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // SplitContainer1
            // 
            this.SplitContainer1.BackColor = System.Drawing.Color.Gray;
            this.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer1.Location = new System.Drawing.Point(0, 27);
            this.SplitContainer1.Name = "SplitContainer1";
            // 
            // SplitContainer1.Panel1
            // 
            this.SplitContainer1.Panel1.Controls.Add(this.OldAsciiContent);
            this.SplitContainer1.Panel1.Controls.Add(this.panel5);
            this.SplitContainer1.Panel1.Controls.Add(this.Panel1);
            this.SplitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(5);
            // 
            // SplitContainer1.Panel2
            // 
            this.SplitContainer1.Panel2.Controls.Add(this.NewAsciiContent);
            this.SplitContainer1.Panel2.Controls.Add(this.panel6);
            this.SplitContainer1.Panel2.Controls.Add(this.Panel3);
            this.SplitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(5);
            this.SplitContainer1.Size = new System.Drawing.Size(1344, 636);
            this.SplitContainer1.SplitterDistance = 667;
            this.SplitContainer1.SplitterWidth = 10;
            this.SplitContainer1.TabIndex = 1;
            // 
            // OldAsciiContent
            // 
            this.OldAsciiContent.DetectUrls = false;
            this.OldAsciiContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OldAsciiContent.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OldAsciiContent.Location = new System.Drawing.Point(5, 36);
            this.OldAsciiContent.Name = "OldAsciiContent";
            this.OldAsciiContent.ReadOnly = true;
            this.OldAsciiContent.ShortcutsEnabled = false;
            this.OldAsciiContent.ShowSelectionMargin = true;
            this.OldAsciiContent.Size = new System.Drawing.Size(642, 595);
            this.OldAsciiContent.TabIndex = 0;
            this.OldAsciiContent.Text = "";
            this.OldAsciiContent.WordWrap = false;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.OldChangeColor);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel5.Location = new System.Drawing.Point(647, 36);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(0, 20, 0, 20);
            this.panel5.Size = new System.Drawing.Size(15, 595);
            this.panel5.TabIndex = 4;
            // 
            // OldChangeColor
            // 
            this.OldChangeColor.BackColor = System.Drawing.Color.White;
            this.OldChangeColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OldChangeColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OldChangeColor.Location = new System.Drawing.Point(0, 20);
            this.OldChangeColor.Name = "OldChangeColor";
            this.OldChangeColor.Size = new System.Drawing.Size(15, 555);
            this.OldChangeColor.TabIndex = 3;
            this.OldChangeColor.Paint += new System.Windows.Forms.PaintEventHandler(this.ChangeColor_Paint);
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.OldAsciiFile);
            this.Panel1.Controls.Add(this.label1);
            this.Panel1.Controls.Add(this.Panel2);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel1.Location = new System.Drawing.Point(5, 5);
            this.Panel1.Name = "Panel1";
            this.Panel1.Padding = new System.Windows.Forms.Padding(5);
            this.Panel1.Size = new System.Drawing.Size(657, 31);
            this.Panel1.TabIndex = 2;
            // 
            // OldAsciiFile
            // 
            this.OldAsciiFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OldAsciiFile.FormattingEnabled = true;
            this.OldAsciiFile.Location = new System.Drawing.Point(105, 5);
            this.OldAsciiFile.Name = "OldAsciiFile";
            this.OldAsciiFile.Size = new System.Drawing.Size(481, 21);
            this.OldAsciiFile.TabIndex = 4;
            this.OldAsciiFile.SelectedIndexChanged += new System.EventHandler(this.Dropdown_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 21);
            this.label1.TabIndex = 5;
            this.label1.Text = "Older File:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Panel2
            // 
            this.Panel2.Controls.Add(this.OldBinaryViewButton);
            this.Panel2.Controls.Add(this.OldFileViewButton);
            this.Panel2.Controls.Add(this.OldAsciiButton);
            this.Panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.Panel2.Location = new System.Drawing.Point(586, 5);
            this.Panel2.Name = "Panel2";
            this.Panel2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.Panel2.Size = new System.Drawing.Size(66, 21);
            this.Panel2.TabIndex = 3;
            this.Panel2.Tag = "&Binary View";
            // 
            // OldBinaryViewButton
            // 
            this.OldBinaryViewButton.BackgroundImage = global::Chizl.FileComparer.Properties.Resources.binary_wheel;
            this.OldBinaryViewButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.OldBinaryViewButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.OldBinaryViewButton.Enabled = false;
            this.OldBinaryViewButton.Location = new System.Drawing.Point(3, 0);
            this.OldBinaryViewButton.Name = "OldBinaryViewButton";
            this.OldBinaryViewButton.Size = new System.Drawing.Size(21, 21);
            this.OldBinaryViewButton.TabIndex = 4;
            this.OldBinaryViewButton.Tag = "Binary View";
            this.OldBinaryViewButton.UseVisualStyleBackColor = true;
            this.OldBinaryViewButton.Click += new System.EventHandler(this.OldBinaryViewButton_Click);
            // 
            // OldFileViewButton
            // 
            this.OldFileViewButton.BackgroundImage = global::Chizl.FileComparer.Properties.Resources.view_on;
            this.OldFileViewButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.OldFileViewButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.OldFileViewButton.Enabled = false;
            this.OldFileViewButton.Location = new System.Drawing.Point(24, 0);
            this.OldFileViewButton.Name = "OldFileViewButton";
            this.OldFileViewButton.Size = new System.Drawing.Size(21, 21);
            this.OldFileViewButton.TabIndex = 3;
            this.OldFileViewButton.UseVisualStyleBackColor = true;
            this.OldFileViewButton.Click += new System.EventHandler(this.OldAsciiViewButton_Click);
            // 
            // OldAsciiButton
            // 
            this.OldAsciiButton.BackgroundImage = global::Chizl.FileComparer.Properties.Resources.AddButton_128x128;
            this.OldAsciiButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.OldAsciiButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.OldAsciiButton.Location = new System.Drawing.Point(45, 0);
            this.OldAsciiButton.Name = "OldAsciiButton";
            this.OldAsciiButton.Size = new System.Drawing.Size(21, 21);
            this.OldAsciiButton.TabIndex = 2;
            this.OldAsciiButton.UseVisualStyleBackColor = true;
            this.OldAsciiButton.Click += new System.EventHandler(this.OldAsciiButton_Click);
            // 
            // NewAsciiContent
            // 
            this.NewAsciiContent.DetectUrls = false;
            this.NewAsciiContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NewAsciiContent.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewAsciiContent.Location = new System.Drawing.Point(5, 36);
            this.NewAsciiContent.Name = "NewAsciiContent";
            this.NewAsciiContent.ReadOnly = true;
            this.NewAsciiContent.ShortcutsEnabled = false;
            this.NewAsciiContent.ShowSelectionMargin = true;
            this.NewAsciiContent.Size = new System.Drawing.Size(642, 595);
            this.NewAsciiContent.TabIndex = 1;
            this.NewAsciiContent.Text = "";
            this.NewAsciiContent.WordWrap = false;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.NewChangeColor);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel6.Location = new System.Drawing.Point(647, 36);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(0, 20, 0, 20);
            this.panel6.Size = new System.Drawing.Size(15, 595);
            this.panel6.TabIndex = 5;
            // 
            // NewChangeColor
            // 
            this.NewChangeColor.BackColor = System.Drawing.Color.White;
            this.NewChangeColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NewChangeColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NewChangeColor.Location = new System.Drawing.Point(0, 20);
            this.NewChangeColor.Name = "NewChangeColor";
            this.NewChangeColor.Size = new System.Drawing.Size(15, 555);
            this.NewChangeColor.TabIndex = 4;
            this.NewChangeColor.Paint += new System.Windows.Forms.PaintEventHandler(this.ChangeColor_Paint);
            // 
            // Panel3
            // 
            this.Panel3.Controls.Add(this.NewAsciiFile);
            this.Panel3.Controls.Add(this.label2);
            this.Panel3.Controls.Add(this.Panel4);
            this.Panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel3.Location = new System.Drawing.Point(5, 5);
            this.Panel3.Name = "Panel3";
            this.Panel3.Padding = new System.Windows.Forms.Padding(5);
            this.Panel3.Size = new System.Drawing.Size(657, 31);
            this.Panel3.TabIndex = 3;
            // 
            // NewAsciiFile
            // 
            this.NewAsciiFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NewAsciiFile.FormattingEnabled = true;
            this.NewAsciiFile.Location = new System.Drawing.Point(105, 5);
            this.NewAsciiFile.Name = "NewAsciiFile";
            this.NewAsciiFile.Size = new System.Drawing.Size(482, 21);
            this.NewAsciiFile.TabIndex = 5;
            this.NewAsciiFile.SelectedIndexChanged += new System.EventHandler(this.Dropdown_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(5, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 21);
            this.label2.TabIndex = 6;
            this.label2.Text = "Newer File:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Panel4
            // 
            this.Panel4.Controls.Add(this.NewBinaryViewButton);
            this.Panel4.Controls.Add(this.NewFileViewButton);
            this.Panel4.Controls.Add(this.NewAsciiButton);
            this.Panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.Panel4.Location = new System.Drawing.Point(587, 5);
            this.Panel4.Name = "Panel4";
            this.Panel4.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.Panel4.Size = new System.Drawing.Size(65, 21);
            this.Panel4.TabIndex = 3;
            // 
            // NewBinaryViewButton
            // 
            this.NewBinaryViewButton.BackgroundImage = global::Chizl.FileComparer.Properties.Resources.binary_wheel;
            this.NewBinaryViewButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.NewBinaryViewButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.NewBinaryViewButton.Enabled = false;
            this.NewBinaryViewButton.Location = new System.Drawing.Point(2, 0);
            this.NewBinaryViewButton.Name = "NewBinaryViewButton";
            this.NewBinaryViewButton.Size = new System.Drawing.Size(21, 21);
            this.NewBinaryViewButton.TabIndex = 4;
            this.NewBinaryViewButton.Tag = "Binary View";
            this.NewBinaryViewButton.UseVisualStyleBackColor = true;
            this.NewBinaryViewButton.Click += new System.EventHandler(this.NewBinaryViewButton_Click);
            // 
            // NewFileViewButton
            // 
            this.NewFileViewButton.BackgroundImage = global::Chizl.FileComparer.Properties.Resources.view_on;
            this.NewFileViewButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.NewFileViewButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.NewFileViewButton.Enabled = false;
            this.NewFileViewButton.Location = new System.Drawing.Point(23, 0);
            this.NewFileViewButton.Name = "NewFileViewButton";
            this.NewFileViewButton.Size = new System.Drawing.Size(21, 21);
            this.NewFileViewButton.TabIndex = 3;
            this.NewFileViewButton.UseVisualStyleBackColor = true;
            this.NewFileViewButton.Click += new System.EventHandler(this.NewAsciiViewButton_Click);
            // 
            // NewAsciiButton
            // 
            this.NewAsciiButton.BackgroundImage = global::Chizl.FileComparer.Properties.Resources.AddButton_128x128;
            this.NewAsciiButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.NewAsciiButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.NewAsciiButton.Location = new System.Drawing.Point(44, 0);
            this.NewAsciiButton.Name = "NewAsciiButton";
            this.NewAsciiButton.Size = new System.Drawing.Size(21, 21);
            this.NewAsciiButton.TabIndex = 2;
            this.NewAsciiButton.UseVisualStyleBackColor = true;
            this.NewAsciiButton.Click += new System.EventHandler(this.NewAsciiButton_Click);
            // 
            // StartFormMenuStrip
            // 
            this.StartFormMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.ScoreThresholdDropdown});
            this.StartFormMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.StartFormMenuStrip.Name = "StartFormMenuStrip";
            this.StartFormMenuStrip.Size = new System.Drawing.Size(1344, 27);
            this.StartFormMenuStrip.TabIndex = 2;
            this.StartFormMenuStrip.Text = "MenuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem2});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(37, 23);
            this.FileToolStripMenuItem.Text = "&File";
            // 
            // ToolStripMenuItem2
            // 
            this.ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            this.ToolStripMenuItem2.Size = new System.Drawing.Size(92, 22);
            this.ToolStripMenuItem2.Text = "E&xit";
            this.ToolStripMenuItem2.Click += new System.EventHandler(this.ToolStripMenuItem2_Click);
            // 
            // ScoreThresholdDropdown
            // 
            this.ScoreThresholdDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ScoreThresholdDropdown.Items.AddRange(new object[] {
            "15%",
            "20%",
            "25%",
            "30%",
            "35%",
            "40%",
            "45%",
            "50%",
            "55%",
            "60%",
            "65%",
            "70%",
            "75%"});
            this.ScoreThresholdDropdown.Name = "ScoreThresholdDropdown";
            this.ScoreThresholdDropdown.Size = new System.Drawing.Size(75, 23);
            // 
            // StartFormStatusStrip
            // 
            this.StartFormStatusStrip.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.StartFormStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusPadding,
            this.StatusText});
            this.StartFormStatusStrip.Location = new System.Drawing.Point(0, 663);
            this.StartFormStatusStrip.Name = "StartFormStatusStrip";
            this.StartFormStatusStrip.Size = new System.Drawing.Size(1344, 22);
            this.StartFormStatusStrip.SizingGrip = false;
            this.StartFormStatusStrip.TabIndex = 3;
            this.StartFormStatusStrip.Text = "StatusStrip1";
            // 
            // toolStripStatusPadding
            // 
            this.toolStripStatusPadding.Name = "toolStripStatusPadding";
            this.toolStripStatusPadding.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusPadding.Text = " ";
            // 
            // StatusText
            // 
            this.StatusText.BackColor = System.Drawing.SystemColors.ControlDark;
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(1319, 17);
            this.StatusText.Spring = true;
            this.StatusText.Text = "Ready...";
            this.StatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CompareButtonToollbar
            // 
            this.CompareButtonToollbar.Location = new System.Drawing.Point(131, 1);
            this.CompareButtonToollbar.Name = "CompareButtonToollbar";
            this.CompareButtonToollbar.Size = new System.Drawing.Size(75, 25);
            this.CompareButtonToollbar.TabIndex = 5;
            this.CompareButtonToollbar.Text = "&Compare";
            this.CompareButtonToollbar.UseVisualStyleBackColor = true;
            this.CompareButtonToollbar.Click += new System.EventHandler(this.CompareButtonToollbar_Click);
            // 
            // ResetTimer
            // 
            this.ResetTimer.Interval = 3000;
            this.ResetTimer.Tick += new System.EventHandler(this.ResetTimer_Tick);
            // 
            // ViewAsBinaryButtonToollbar
            // 
            this.ViewAsBinaryButtonToollbar.Location = new System.Drawing.Point(212, 1);
            this.ViewAsBinaryButtonToollbar.Name = "ViewAsBinaryButtonToollbar";
            this.ViewAsBinaryButtonToollbar.Size = new System.Drawing.Size(75, 25);
            this.ViewAsBinaryButtonToollbar.TabIndex = 6;
            this.ViewAsBinaryButtonToollbar.Text = "&Binary View";
            this.ViewAsBinaryButtonToollbar.UseVisualStyleBackColor = true;
            this.ViewAsBinaryButtonToollbar.Visible = false;
            this.ViewAsBinaryButtonToollbar.Click += new System.EventHandler(this.ViewAsBinaryButtonToollbar_Click);
            // 
            // StartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1344, 690);
            this.Controls.Add(this.ViewAsBinaryButtonToollbar);
            this.Controls.Add(this.CompareButtonToollbar);
            this.Controls.Add(this.SplitContainer1);
            this.Controls.Add(this.StartFormMenuStrip);
            this.Controls.Add(this.StartFormStatusStrip);
            this.Name = "StartForm";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.StartForm_Load);
            this.ResizeBegin += new System.EventHandler(this.StartForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.StartForm_ResizeEnd);
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.Panel1.ResumeLayout(false);
            this.Panel2.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.Panel3.ResumeLayout(false);
            this.Panel4.ResumeLayout(false);
            this.StartFormMenuStrip.ResumeLayout(false);
            this.StartFormMenuStrip.PerformLayout();
            this.StartFormStatusStrip.ResumeLayout(false);
            this.StartFormStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.RichTextBox OldAsciiContent;
        private System.Windows.Forms.RichTextBox NewAsciiContent;
        private System.Windows.Forms.SplitContainer SplitContainer1;
        private System.Windows.Forms.MenuStrip StartFormMenuStrip;
        private System.Windows.Forms.StatusStrip StartFormStatusStrip;
        private System.Windows.Forms.Panel Panel1;
        private System.Windows.Forms.Button OldAsciiButton;
        private System.Windows.Forms.Panel Panel2;
        private System.Windows.Forms.Panel Panel3;
        private System.Windows.Forms.Panel Panel4;
        private System.Windows.Forms.Button NewAsciiButton;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem2;
        private System.Windows.Forms.ToolStripComboBox ScoreThresholdDropdown;
        private System.Windows.Forms.Button CompareButtonToollbar;
        private System.Windows.Forms.Button OldFileViewButton;
        private System.Windows.Forms.Button NewFileViewButton;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
        private System.Windows.Forms.Panel OldChangeColor;
        private System.Windows.Forms.Panel NewChangeColor;
        private System.Windows.Forms.Button OldBinaryViewButton;
        private System.Windows.Forms.Button NewBinaryViewButton;
        private System.Windows.Forms.Timer ResetTimer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusPadding;
        private System.Windows.Forms.Button ViewAsBinaryButtonToollbar;
        private System.Windows.Forms.ComboBox OldAsciiFile;
        private System.Windows.Forms.ComboBox NewAsciiFile;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

