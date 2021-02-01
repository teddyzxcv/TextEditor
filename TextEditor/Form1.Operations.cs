using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FastColoredTextBoxNS;
namespace TextEditor
{
    partial class Form1
    {


        /// <summary>
        /// Save the all information about opening tab.
        /// </summary>
        /// <typeparam name="TabF"></typeparam>
        /// <returns></returns>
        List<TabF> tabPages = new List<TabF>();
        /// <summary>
        /// Open file by dialog.
        /// </summary>
        private void OpenFile()
        {
            OpenFileDialog FileDialog1 = new OpenFileDialog();
            FileDialog1.RestoreDirectory = true;
            FileDialog1.Filter = "Text Files | *.txt|RTF Files | *.rtf|C# file | *.cs";
            FileDialog1.Title = "Open an Text File";
            try
            {
                if (FileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.tabControl1.TabPages.Add(CreateNewTab(FileDialog1.FileName));
                    tabControl1.SelectTab(tabControl1.TabCount - 1);
                    //Open file
                    OpenFileByExtension(FileDialog1.FileName);
                }
            }
            catch
            {
                var result = MessageBox.Show($"File {FileDialog1.FileName} can not be opened for some reason, try other file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tabPages.RemoveAt(tabControl1.TabCount - 1);
                tabControl1.TabPages.RemoveAt(tabControl1.TabCount - 1);

            }
        }
        /// <summary>
        /// Open file by path.
        /// </summary>
        /// <param name="Path"></param>
        private void OpenFile(string Path)
        {
            try
            {
                if (File.Exists(Path))
                {
                    this.tabControl1.TabPages.Add(CreateNewTab(Path));
                    tabControl1.SelectTab(tabControl1.TabCount - 1);
                    //Open file
                    OpenFileByExtension(Path);

                }
            }
            catch
            {
                var result = MessageBox.Show($"File {Path} can not be opened for some reason, try other file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tabPages.RemoveAt(tabControl1.TabCount - 1);
                tabControl1.TabPages.RemoveAt(tabControl1.TabCount - 1);
            }
        }
        /// <summary>
        /// Save file by dialog.
        /// </summary>
        /// <param name="sfd"></param>

        private void SaveFile(SaveFileDialog sfd)
        {
            try
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    //Save File
                    SaveFileByExtension(tabPages[tabControl1.SelectedIndex].PathToFile, Path.ChangeExtension(sfd.FileName, ""), tabControl1.SelectedTab);
                }
            }
            catch
            {
                var result = MessageBox.Show($"File {sfd.FileName} can not be opened for some reason, try other file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Save all file before close.
        /// </summary>
        /// <param name="result"></param>
        private void SaveBeforeClose(out DialogResult result)
        {
            //tabPages.Select(e => e.PathToFile)
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfg.AppSettings.Settings["OldPaths"].Value = String.Join('|', tabPages.Select(e => e.PathToFile));
            cfg.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            result = DialogResult.OK;
            while (tabControl1.TabPages.Count != 0)
            {
                if (!tabPages[tabControl1.SelectedIndex].SavedOrNot && tabControl1.TabCount != 0)
                {

                    result = MessageBox.Show($"Would you like to Save {tabControl1.TabPages[0].Text} before close this Tab?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        // Save the file and close it.
                        TabWidthList.Remove(this.tabControl1.TabPages[0].Text.Length * 18);
                        SaveFile();
                        tabPages[0].SavedOrNot = true;
                        tabControl1.TabPages[0].Text = tabControl1.TabPages[0].Text.Replace("*", String.Empty);
                        tabPages.RemoveAt(0);
                        this.tabControl1.TabPages.RemoveAt(0);
                        RefreshTabSize();
                        SwitchStyleOption();
                    }
                    else if (result == DialogResult.No)
                    {
                        // Close the taab with out saving it.
                        TabWidthList.Remove(this.tabControl1.TabPages[0].Text.Length * 18);
                        this.tabControl1.TabPages.RemoveAt(0);
                        tabPages.RemoveAt(0);
                        RefreshTabSize();
                        SwitchStyleOption();

                    }
                    else if (result == DialogResult.Cancel)
                    {
                        // Just cancel.
                        SwitchStyleOption();
                        break;
                    }
                }
                else
                {
                    //  Just remove.
                    TabWidthList.Remove(this.tabControl1.TabPages[0].Text.Length * 18);
                    this.tabControl1.TabPages.RemoveAt(0);
                    tabPages.RemoveAt(0);
                    RefreshTabSize();
                    SwitchStyleOption();

                }
            }

        }
        /// <summary>
        /// Save the opened tab file.
        /// </summary>
        private void SaveFile()
        {
            try
            {
                //Save file
                SaveFileByExtension(tabPages[tabControl1.SelectedIndex].PathToFile, Path.ChangeExtension(tabPages[tabControl1.SelectedIndex].PathToFile, ""), tabControl1.SelectedTab);

            }
            catch
            {
                var result = MessageBox.Show($"File {tabPages[tabControl1.SelectedIndex].PathToFile} can not be saved for some reason, try other file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        /// <summary>
        /// Refresh tab size by name of the file.
        /// </summary>
        /// <param name="FileName"></param>
        private void RefreshTabSize(string FileName)
        {
            TabWidthList.Add(FileName.Length * 18);
            int MaxTabWidth = TabWidthList.Max();
            this.tabControl1.ItemSize = new Size(MaxTabWidth, 36);
        }
        /// <summary>
        /// Refresh tab size.
        /// </summary>
        private void RefreshTabSize()
        {
            int MaxTabWidth = TabWidthList.Max();
            this.tabControl1.ItemSize = new Size(MaxTabWidth, 36);
        }
        /// <summary>
        /// Save all opened file.
        /// </summary>
        private void SaveAll()
        {
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                try
                {
                    //Save file
                    if ( tabControl1.TabCount != 0)
                    {
                        SaveFileByExtension(tabPages[i].PathToFile, Path.ChangeExtension(tabPages[i].PathToFile, ""), tabControl1.TabPages[i]);
                        tabPages[i].SavedOrNot = true;
                        tabControl1.TabPages[i].Text = tabControl1.TabPages[i].Text.Replace("*", "");
                        tabControl1.Refresh();


                    }
                }
                catch
                {
                    var result = MessageBox.Show($"File {tabPages[i].PathToFile} can not be saved for some reason, try other file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (tabControl1.SelectedTab != null)
                this.Text = tabControl1.SelectedTab.Text;
        }

        /// <summary>
        /// Create new tab.
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private TabPage CreateNewTab(string FileName)
        {
            string fileName = FileName.Substring(FileName.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            TabPage newTab = new TabPage(fileName);
            if (Path.GetExtension(fileName) == ".cs")
            {
                // Create new fast colored text box.
                FastColoredTextBox newTextBox = new FastColoredTextBox();
                newTextBox.Location = new Point(4, 4);
                newTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
                newTextBox.Location = new System.Drawing.Point(3, 3);
                newTextBox.Name = "richTextBox1";
                newTextBox.Size = new System.Drawing.Size(946, 593);
                newTextBox.TabIndex = 0;
                newTextBox.Text = "";
                newTextBox.Font = new Font("Courier New", 12, FontStyle.Regular);
                newTextBox.ContextMenuStrip = contextMenuStrip1;
                newTextBox.AcceptsTab = true;
                newTextBox.Language = Language.CSharp;
                FormattingCode.CSharpSyntaxHighlighter = newTextBox.SyntaxHighlighter;
                FormattingCode.SetSyntaxColor();
                newTextBox.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.SelectedTabBox_TextChanged);
                newTab.Controls.Add(newTextBox);
            }
            else
            {
                // Create new rich text box.
                RichTextBox newTextBox = new RichTextBox();
                newTextBox.Location = new Point(4, 4);
                newTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
                newTextBox.Location = new System.Drawing.Point(3, 3);
                newTextBox.Name = "richTextBox1";
                newTextBox.Size = new System.Drawing.Size(946, 593);
                newTextBox.TabIndex = 0;
                newTextBox.Text = "";
                newTextBox.Font = new Font("Arial", 15, FontStyle.Regular);
                newTextBox.ContextMenuStrip = contextMenuStrip1;
                newTextBox.AcceptsTab = true;
                newTextBox.TextChanged += new System.EventHandler(this.SelectedTabRichBox_TextChanged);
                newTab.Controls.Add(newTextBox);
            }
            // Add to the tabcontrol.
            RefreshTabSize(fileName);
            TabF newT = new TabF();
            newT.PathToFile = FileName;
            newT.Page = newTab;
            newT.SavedOrNot = true;
            tabPages.Add(newT);
            this.Text = fileName;
            return newTab;
        }
        /// <summary>
        /// Open all old file from last opening which are saved in the App.config.
        /// </summary>
        private void OpenOldFile()
        {
            try
            {
                if (ConfigurationManager.AppSettings["OldPaths"].Length != 0)
                    foreach (var item in ConfigurationManager.AppSettings["OldPaths"].Split('|').ToList<string>())
                    {
                        if (File.Exists(item))
                            OpenFile(item);
                    }
                // ConfigurationManager.AppSettings["OldPaths"].Split('|').ToList<string>().ForEach(OpenFile);
                else
                    OpenFile("Helper.rtf");
            }
            catch
            {

            }
        }
        /// <summary>
        /// Set all setting.
        /// </summary>
        private void SetSetting()
        {
            try
            {
                this.timer1.Interval = int.Parse(ConfigurationManager.AppSettings["AutosaveTime"]) * 60000;
                this.timer2.Interval = int.Parse(ConfigurationManager.AppSettings["TimeMachine"]) * 60000;
                ColorTheme = ConfigurationManager.AppSettings["ThemeColor"];
                switch (ColorTheme)
                {
                    case ("Light"):
                        menuStrip1.BackColor = Color.White;
                        splitContainer1.Panel1.BackColor = Color.White;
                        break;
                    case ("Gray"):
                        menuStrip1.BackColor = Color.Gray;
                        splitContainer1.Panel1.BackColor = Color.Gray;
                        break;
                    case ("Red"):
                        menuStrip1.BackColor = Color.OrangeRed;
                        splitContainer1.Panel1.BackColor = Color.OrangeRed;
                        break;
                }
                FormattingCode.CSharpCommentStyle = new TextStyle(new SolidBrush(Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpCommentStyle"]))), null, FontStyle.Italic);
                FormattingCode.CSharpKeywordStyle = new TextStyle(new SolidBrush(Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpKeywordStyle"]))), null, FontStyle.Regular);
                FormattingCode.CSharpAttributeStyle = new TextStyle(new SolidBrush(Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpAttributeStyle"]))), null, FontStyle.Regular);
                FormattingCode.CSharpClassNameStyle = new TextStyle(new SolidBrush(Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpClassNameStyle"]))), null, FontStyle.Bold | FontStyle.Underline);
                FormattingCode.CSharpCommentTagStyle = new TextStyle(new SolidBrush(Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpCommentTagStyle"]))), null, FontStyle.Regular);
                FormattingCode.CSharpNumberStyle = new TextStyle(new SolidBrush(Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpNumberStyle"]))), null, FontStyle.Regular);
                FormattingCode.CSharpStringStyle = new TextStyle(new SolidBrush(Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpStringStyle"]))), null, FontStyle.Regular);
                FormattingCode.CSharpVariableStyle = new TextStyle(new SolidBrush(Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpVariableStyle"]))), null, FontStyle.Regular);
                FormattingCode.SetSyntaxColor();
            }
            catch
            {
                var result = MessageBox.Show("Incorrect setting! plz don't change file App.config!!, Press OK to fix setting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        /// <summary>
        /// Do cut.
        /// </summary>
        private void CutAct()
        {
            if (this.tabControl1.TabCount != 0)

                if (Path.GetExtension(tabPages[tabControl1.SelectedIndex].PathToFile) != ".cs")

                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Cut();
                else
                    tabControl1.SelectedTab.Controls.OfType<FastColoredTextBox>().Last().Cut();
        }
        /// <summary>
        /// Do copy.
        /// </summary>
        private void CopyAct()
        {
            if (this.tabControl1.TabCount != 0)
                if (Path.GetExtension(tabPages[tabControl1.SelectedIndex].PathToFile) != ".cs")

                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Copy();
                else
                    tabControl1.SelectedTab.Controls.OfType<FastColoredTextBox>().Last().Copy();

        }
        /// <summary>
        /// Do paste.
        /// </summary>
        private void PasteAct()
        {
            if (this.tabControl1.TabCount != 0)

                if (Path.GetExtension(tabPages[tabControl1.SelectedIndex].PathToFile) != ".cs")

                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Paste();
                else
                    tabControl1.SelectedTab.Controls.OfType<FastColoredTextBox>().Last().Paste();

        }
        /// <summary>
        /// Select all.
        /// </summary>
        private void SelectAllAct()
        {
            if (this.tabControl1.TabCount != 0)

                if (Path.GetExtension(tabPages[tabControl1.SelectedIndex].PathToFile) != ".cs")
                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectAll();
                else
                    tabControl1.SelectedTab.Controls.OfType<FastColoredTextBox>().Last().SelectAll();

        }
        /// <summary>
        /// Make selection bold.
        /// </summary>
        private void BoldSelection()
        {
            if (this.tabControl1.TabCount != 0)

                if (tabControl1.SelectedTab != null)
                {
                    Font f = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont;
                    FontStyle fs = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont.Style;
                    if (f.Bold)
                    {
                        button1.BackColor = Color.White;
                        RBoldStripMenuItem1.BackColor = Color.White;
                        fs &= ~FontStyle.Bold;
                    }
                    else
                    {
                        button1.BackColor = Color.Gray;
                        RBoldStripMenuItem1.BackColor = Color.Gray;
                        fs |= FontStyle.Bold;
                    }
                    if (tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont != null)
                    {
                        tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont = new Font(f, fs);
                    }
                }
        }
        /// <summary>
        /// Make selection italic.
        /// </summary>
        private void ItalicSelection()
        {
            if (this.tabControl1.TabCount != 0)

                if (tabControl1.SelectedTab != null)
                {
                    Font f = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont;
                    FontStyle fs = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont.Style;
                    if (f.Italic)
                    {
                        button2.BackColor = Color.White;
                        RItalicStripMenuItem2.BackColor = Color.White;
                        fs &= ~FontStyle.Italic;
                    }
                    else
                    {
                        button2.BackColor = Color.Gray;
                        RItalicStripMenuItem2.BackColor = Color.Gray;

                        fs |= FontStyle.Italic;
                    }
                    if (tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont != null)
                    {
                        tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont = new Font(f, fs);
                    }

                }
        }
        /// <summary>
        /// Make selection underline.
        /// </summary>
        private void UnderlineSelection()
        {
            if (this.tabControl1.TabCount != 0)

                if (tabControl1.SelectedTab != null)
                {
                    Font f = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont;
                    FontStyle fs = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont.Style;
                    if (f.Underline)
                    {
                        Ubutton.BackColor = Color.White;
                        RUnderStripMenuItem3.BackColor = Color.White;
                        fs &= ~FontStyle.Underline;
                    }
                    else
                    {
                        Ubutton.BackColor = Color.Gray;
                        RUnderStripMenuItem3.BackColor = Color.Gray;
                        fs |= FontStyle.Underline;
                    }
                    if (tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont != null)
                    {
                        tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont = new Font(f, fs);
                    }
                }
        }
        /// <summary>
        /// savepath only path and file name + '.' :"/--/--/text."
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="savepath"></param>
        /// <param name="rb"></param>
        private void SaveFileByExtension(string filepath, string savepath, TabPage tb)
        {
            RichTextBox rb = new RichTextBox();
            FastColoredTextBox fctb = new FastColoredTextBox();


            switch (Path.GetExtension(filepath).Trim())
            {
                case (".rtf"):
                    rb = tb.Controls.OfType<RichTextBox>().Last();
                    rb.SaveFile(savepath + "rtf", RichTextBoxStreamType.RichText);
                    break;
                case (".txt"):
                    rb = tb.Controls.OfType<RichTextBox>().Last();
                    rb.SaveFile(savepath + "txt", RichTextBoxStreamType.PlainText);
                    break;
                case (".cs"):
                    fctb = tb.Controls.OfType<FastColoredTextBox>().Last();
                    var enc = EncodingDetector.DetectTextFileEncoding(filepath);
                    if (enc != null)
                        fctb.SaveToFile(savepath + "cs", enc);
                    else
                        fctb.SaveToFile(savepath + "cs", Encoding.Default);
                    break;

            }
        }
        /// <summary>
        /// Open file by checking the extention.
        /// </summary>
        /// <param name="filepath"></param>
        private void OpenFileByExtension(string filepath)
        {

            RichTextBox rb = new RichTextBox();
            FastColoredTextBox fctb = new FastColoredTextBox();
            switch (Path.GetExtension(filepath))
            {
                case (".rtf"):
                    rb = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last();
                    rb.LoadFile(filepath, RichTextBoxStreamType.RichText);
                    SwitchStyleOption();
                    break;
                case (".txt"):
                    rb = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last();
                    rb.LoadFile(filepath, RichTextBoxStreamType.PlainText);
                    SwitchStyleOption();
                    break;
                case (".cs"):
                    var enc = EncodingDetector.DetectTextFileEncoding(filepath);
                    fctb = tabControl1.SelectedTab.Controls.OfType<FastColoredTextBox>().Last();
                    SwitchStyleOption();
                    if (enc != null)
                        fctb.OpenFile(filepath, enc);
                    else
                        fctb.OpenFile(filepath, Encoding.Default);
                    break;
            }

        }

        /// <summary>
        /// Reopen all file.
        /// </summary>
        private void ReopenAll()
        {
            int indexSelected = tabControl1.SelectedIndex;
            List<TabF> listtab = new List<TabF>(tabPages);
            tabControl1.TabPages.Clear();
            tabPages.Clear();
            listtab.Select(e => e.PathToFile).ToList().ForEach(OpenFile);
            tabControl1.SelectedIndex = indexSelected;

        }
        /// <summary>
        /// Make selection strikeout.
        /// </summary>

        private void StrikeoutSelection()
        {
            if (this.tabControl1.TabCount != 0)

                if (tabControl1.SelectedTab != null)
                {
                    Font f = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont;
                    FontStyle fs = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont.Style;
                    if (f.Strikeout)
                    {
                        Sbutton.BackColor = Color.White;
                        RStrikeStripMenuItem4.BackColor = Color.White;
                        fs &= ~FontStyle.Strikeout;
                    }
                    else
                    {
                        Sbutton.BackColor = Color.Gray;
                        RStrikeStripMenuItem4.BackColor = Color.Gray;
                        fs |= FontStyle.Strikeout;
                    }
                    if (tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont != null)
                    {
                        tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont = new Font(f, fs);
                    }
                }
        }
        /// <summary>
        /// Switch style .cs and just text file.
        /// </summary>

        private void SwitchStyleOption()
        {
            if (tabControl1.SelectedTab != null && Path.GetExtension(tabPages[tabControl1.SelectedIndex].PathToFile) == ".cs")
            {
                button1.Visible = false;
                button2.Visible = false;
                Ubutton.Visible = false;
                Sbutton.Visible = false;
                RBoldStripMenuItem1.Visible = false;
                RUnderStripMenuItem3.Visible = false;
                RStrikeStripMenuItem4.Visible = false;
                RItalicStripMenuItem2.Visible = false;
                FromatToolStripMenuItem1.Visible = false;
            }
            else
            {
                button1.Visible = true;
                button2.Visible = true;
                Ubutton.Visible = true;
                Sbutton.Visible = true;
                RBoldStripMenuItem1.Visible = true;
                RUnderStripMenuItem3.Visible = true;
                RStrikeStripMenuItem4.Visible = true;
                RItalicStripMenuItem2.Visible = true;
                FromatToolStripMenuItem1.Visible = true;
            }
        }
    }
}