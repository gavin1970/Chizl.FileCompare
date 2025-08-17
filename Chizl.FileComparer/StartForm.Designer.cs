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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartForm));
            this.OldAsciiContent = new System.Windows.Forms.RichTextBox();
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.OldAsciiFile = new System.Windows.Forms.TextBox();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.OldAsciiButton = new System.Windows.Forms.Button();
            this.NewAsciiContent = new System.Windows.Forms.RichTextBox();
            this.Panel3 = new System.Windows.Forms.Panel();
            this.NewAsciiFile = new System.Windows.Forms.TextBox();
            this.Panel4 = new System.Windows.Forms.Panel();
            this.NewAsciiButton = new System.Windows.Forms.Button();
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ScoreThresholdDropdown = new System.Windows.Forms.ToolStripComboBox();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.CompareButtonToollbar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.Panel3.SuspendLayout();
            this.MenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OldAsciiContent
            // 
            this.OldAsciiContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OldAsciiContent.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OldAsciiContent.Location = new System.Drawing.Point(5, 36);
            this.OldAsciiContent.Name = "OldAsciiContent";
            this.OldAsciiContent.ReadOnly = true;
            this.OldAsciiContent.ShowSelectionMargin = true;
            this.OldAsciiContent.Size = new System.Drawing.Size(637, 600);
            this.OldAsciiContent.TabIndex = 0;
            this.OldAsciiContent.Text = "";
            this.OldAsciiContent.WordWrap = false;
            // 
            // SplitContainer1
            // 
            this.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer1.Location = new System.Drawing.Point(0, 27);
            this.SplitContainer1.Name = "SplitContainer1";
            // 
            // SplitContainer1.Panel1
            // 
            this.SplitContainer1.Panel1.Controls.Add(this.OldAsciiContent);
            this.SplitContainer1.Panel1.Controls.Add(this.Panel1);
            this.SplitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(5);
            // 
            // SplitContainer1.Panel2
            // 
            this.SplitContainer1.Panel2.Controls.Add(this.NewAsciiContent);
            this.SplitContainer1.Panel2.Controls.Add(this.Panel3);
            this.SplitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(5);
            this.SplitContainer1.Size = new System.Drawing.Size(1276, 641);
            this.SplitContainer1.SplitterDistance = 647;
            this.SplitContainer1.SplitterWidth = 10;
            this.SplitContainer1.TabIndex = 1;
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.OldAsciiFile);
            this.Panel1.Controls.Add(this.Panel2);
            this.Panel1.Controls.Add(this.OldAsciiButton);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel1.Location = new System.Drawing.Point(5, 5);
            this.Panel1.Name = "Panel1";
            this.Panel1.Padding = new System.Windows.Forms.Padding(5);
            this.Panel1.Size = new System.Drawing.Size(637, 31);
            this.Panel1.TabIndex = 2;
            // 
            // OldAsciiFile
            // 
            this.OldAsciiFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OldAsciiFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OldAsciiFile.Location = new System.Drawing.Point(5, 5);
            this.OldAsciiFile.Name = "OldAsciiFile";
            this.OldAsciiFile.ReadOnly = true;
            this.OldAsciiFile.Size = new System.Drawing.Size(593, 20);
            this.OldAsciiFile.TabIndex = 1;
            // 
            // Panel2
            // 
            this.Panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.Panel2.Location = new System.Drawing.Point(598, 5);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(13, 21);
            this.Panel2.TabIndex = 3;
            // 
            // OldAsciiButton
            // 
            this.OldAsciiButton.BackgroundImage = global::Chizl.FileComparer.Properties.Resources.AddButton_128x128;
            this.OldAsciiButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.OldAsciiButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.OldAsciiButton.Location = new System.Drawing.Point(611, 5);
            this.OldAsciiButton.Name = "OldAsciiButton";
            this.OldAsciiButton.Size = new System.Drawing.Size(21, 21);
            this.OldAsciiButton.TabIndex = 2;
            this.OldAsciiButton.UseVisualStyleBackColor = true;
            this.OldAsciiButton.Click += new System.EventHandler(this.OldAsciiButton_Click);
            // 
            // NewAsciiContent
            // 
            this.NewAsciiContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NewAsciiContent.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewAsciiContent.Location = new System.Drawing.Point(5, 36);
            this.NewAsciiContent.Name = "NewAsciiContent";
            this.NewAsciiContent.ReadOnly = true;
            this.NewAsciiContent.ShowSelectionMargin = true;
            this.NewAsciiContent.Size = new System.Drawing.Size(609, 600);
            this.NewAsciiContent.TabIndex = 1;
            this.NewAsciiContent.Text = "";
            this.NewAsciiContent.WordWrap = false;
            // 
            // Panel3
            // 
            this.Panel3.Controls.Add(this.NewAsciiFile);
            this.Panel3.Controls.Add(this.Panel4);
            this.Panel3.Controls.Add(this.NewAsciiButton);
            this.Panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel3.Location = new System.Drawing.Point(5, 5);
            this.Panel3.Name = "Panel3";
            this.Panel3.Padding = new System.Windows.Forms.Padding(5);
            this.Panel3.Size = new System.Drawing.Size(609, 31);
            this.Panel3.TabIndex = 3;
            // 
            // NewAsciiFile
            // 
            this.NewAsciiFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NewAsciiFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NewAsciiFile.Location = new System.Drawing.Point(5, 5);
            this.NewAsciiFile.Name = "NewAsciiFile";
            this.NewAsciiFile.ReadOnly = true;
            this.NewAsciiFile.Size = new System.Drawing.Size(565, 20);
            this.NewAsciiFile.TabIndex = 1;
            // 
            // Panel4
            // 
            this.Panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.Panel4.Location = new System.Drawing.Point(570, 5);
            this.Panel4.Name = "Panel4";
            this.Panel4.Size = new System.Drawing.Size(13, 21);
            this.Panel4.TabIndex = 3;
            // 
            // NewAsciiButton
            // 
            this.NewAsciiButton.BackgroundImage = global::Chizl.FileComparer.Properties.Resources.AddButton_128x128;
            this.NewAsciiButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.NewAsciiButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.NewAsciiButton.Location = new System.Drawing.Point(583, 5);
            this.NewAsciiButton.Name = "NewAsciiButton";
            this.NewAsciiButton.Size = new System.Drawing.Size(21, 21);
            this.NewAsciiButton.TabIndex = 2;
            this.NewAsciiButton.UseVisualStyleBackColor = true;
            this.NewAsciiButton.Click += new System.EventHandler(this.NewAsciiButton_Click);
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.ScoreThresholdDropdown});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Size = new System.Drawing.Size(1276, 27);
            this.MenuStrip1.TabIndex = 2;
            this.MenuStrip1.Text = "MenuStrip1";
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
            // StatusStrip1
            // 
            this.StatusStrip1.Location = new System.Drawing.Point(0, 668);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Size = new System.Drawing.Size(1276, 22);
            this.StatusStrip1.TabIndex = 3;
            this.StatusStrip1.Text = "StatusStrip1";
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
            // StartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1276, 690);
            this.Controls.Add(this.CompareButtonToollbar);
            this.Controls.Add(this.SplitContainer1);
            this.Controls.Add(this.MenuStrip1);
            this.Controls.Add(this.StatusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip1;
            this.Name = "StartForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.StartForm_Load);
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.Panel3.ResumeLayout(false);
            this.Panel3.PerformLayout();
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox OldAsciiContent;
        private System.Windows.Forms.SplitContainer SplitContainer1;
        private System.Windows.Forms.RichTextBox NewAsciiContent;
        private System.Windows.Forms.TextBox OldAsciiFile;
        private System.Windows.Forms.MenuStrip MenuStrip1;
        private System.Windows.Forms.StatusStrip StatusStrip1;
        private System.Windows.Forms.Panel Panel1;
        private System.Windows.Forms.Button OldAsciiButton;
        private System.Windows.Forms.Panel Panel2;
        private System.Windows.Forms.Panel Panel3;
        private System.Windows.Forms.TextBox NewAsciiFile;
        private System.Windows.Forms.Panel Panel4;
        private System.Windows.Forms.Button NewAsciiButton;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem2;
        private System.Windows.Forms.ToolStripComboBox ScoreThresholdDropdown;
        private System.Windows.Forms.Button CompareButtonToollbar;
    }
}

