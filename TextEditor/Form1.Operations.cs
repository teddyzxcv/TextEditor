using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis;
namespace TextEditor
{
    partial class Form1
    {


        List<TabF> tabPages = new List<TabF>();

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
                    OpenFileByExtension(FileDialog1.FileName, tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last());
                    /*if (FileDialog1.FileName.Substring(FileDialog1.FileName.LastIndexOf('.')) == ".rtf")
                        tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().LoadFile(FileDialog1.FileName, RichTextBoxStreamType.RichText);
                    else
                        tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Text = File.ReadAllText(FileDialog1.FileName);*/
                }
            }
            catch
            {
                var result = MessageBox.Show($"File {FileDialog1.FileName} can not be opened for some reason, try other file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tabPages.RemoveAt(tabControl1.TabCount - 1);
                tabControl1.TabPages.RemoveAt(tabControl1.TabCount - 1);

            }
        }
        private void OpenFile(string Path)
        {
            try
            {
                if (File.Exists(Path))
                {
                    this.tabControl1.TabPages.Add(CreateNewTab(Path));
                    tabControl1.SelectTab(tabControl1.TabCount - 1);
                    //Open file
                    OpenFileByExtension(Path, tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last());
                    /*if (Path.Substring(Path.LastIndexOf('.')) == ".rtf")
                        tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().LoadFile(Path, RichTextBoxStreamType.RichText);
                    else
                        tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Text = File.ReadAllText(Path);*/
                }
            }
            catch
            {
                var result = MessageBox.Show($"File {Path} can not be opened for some reason, try other file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tabPages.RemoveAt(tabControl1.TabCount - 1);
                tabControl1.TabPages.RemoveAt(tabControl1.TabCount - 1);
            }
        }

        private void SaveFile(SaveFileDialog sfd)
        {
            try
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    //Save File
                    SaveFileByExtension(tabControl1.SelectedTab.Text, Path.ChangeExtension(sfd.FileName, ""), tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last());
                    /*if (tabControl1.SelectedTab.Text.Substring(tabControl1.SelectedTab.Text.LastIndexOf('.')) == ".rtf")
                        tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SaveFile(sfd.FileName, RichTextBoxStreamType.RichText);
                    else
                        tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);*/


                }
            }
            catch
            {
                var result = MessageBox.Show($"File {sfd.FileName} can not be opened for some reason, try other file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveBeforeClose()
        {
            //tabPages.Select(e => e.PathToFile)
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfg.AppSettings.Settings["OldPaths"].Value = String.Join('|', tabPages.Select(e => e.PathToFile));
            cfg.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            while (tabControl1.TabPages.Count != 0)
            {
                var result = MessageBox.Show($"Would you like to Save {tabControl1.TabPages[0].Text} before close this Tab?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    TabWidthList.Remove(this.tabControl1.TabPages[0].Text.Length * 18);
                    SaveFile();
                    tabPages.RemoveAt(0);
                    this.tabControl1.TabPages.RemoveAt(0);
                    RefreshTabSize();
                }
                else if (result == DialogResult.No)
                {
                    TabWidthList.Remove(this.tabControl1.TabPages[0].Text.Length * 18);
                    this.tabControl1.TabPages.RemoveAt(0);
                    tabPages.RemoveAt(0);
                    RefreshTabSize();
                }
            }

        }
        private void SaveFile()
        {
            try
            {
                //Save file
                SaveFileByExtension(tabControl1.SelectedTab.Text, Path.ChangeExtension(tabPages[tabControl1.SelectedIndex].PathToFile, ""), tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last());
                /* if (tabControl1.SelectedTab.Text.Substring(tabControl1.SelectedTab.Text.LastIndexOf('.')) == ".rtf")
                     tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SaveFile(tabPages[tabControl1.SelectedIndex].PathToFile, RichTextBoxStreamType.RichText);
                 else
                     tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SaveFile(tabPages[tabControl1.SelectedIndex].PathToFile, RichTextBoxStreamType.PlainText);*/
            }
            catch
            {
                var result = MessageBox.Show($"File {tabPages[tabControl1.SelectedIndex].PathToFile} can not be saved for some reason, try other file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        private void RefreshTabSize(string FileName)
        {
            TabWidthList.Add(FileName.Length * 18);
            int MaxTabWidth = TabWidthList.Max();
            this.tabControl1.ItemSize = new Size(MaxTabWidth, 36);
        }
        private void RefreshTabSize()
        {
            int MaxTabWidth = TabWidthList.Max();
            this.tabControl1.ItemSize = new Size(MaxTabWidth, 36);
        }
        private void SaveAll()
        {
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                try
                {
                    //Save file
                    SaveFileByExtension(tabControl1.TabPages[i].Text, Path.ChangeExtension(tabPages[i].PathToFile, ""), tabControl1.TabPages[i].Controls.OfType<RichTextBox>().Last());
                    /* if (tabControl1.TabPages[i].Text.Substring(tabControl1.TabPages[i].Text.LastIndexOf('.')) == ".rtf")
                         tabControl1.TabPages[i].Controls.OfType<RichTextBox>().Last().SaveFile(tabPages[i].PathToFile, RichTextBoxStreamType.RichText);
                     else
                         tabControl1.TabPages[i].Controls.OfType<RichTextBox>().Last().SaveFile(tabPages[i].PathToFile, RichTextBoxStreamType.PlainText);*/
                }
                catch
                {
                    var result = MessageBox.Show($"File {tabPages[i].PathToFile} can not be saved for some reason, try other file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private TabPage CreateNewTab(string FileName)
        {
            string fileName = FileName.Substring(FileName.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            TabPage newTab = new TabPage(fileName);
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
            newTextBox.TextChanged += new System.EventHandler(this.SelectedTabBox_TextChanged);
            newTab.Controls.Add(newTextBox);
            RefreshTabSize(fileName);
            TabF newT = new TabF();
            newT.PathToFile = FileName;
            newT.Page = newTab;
            newT.SaveOrNot = true;
            tabPages.Add(newT);
            this.Text = fileName;
            return newTab;
        }
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
        private void SetSetting()
        {
            try
            {
                this.timer1.Interval = int.Parse(ConfigurationManager.AppSettings["AutosaveTime"]) * 60000;
                this.timer2.Interval = int.Parse(ConfigurationManager.AppSettings["TimeMachine"]) * 60000;
                ColorTheme = ConfigurationManager.AppSettings["ThemeColor"];
            }
            catch
            {
                var result = MessageBox.Show("Incorrect setting! plz don't change file App.config!!, Press OK to fix setting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        }
        private void CutAct()
        {
            RichTextBox rb = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last();
            if (rb.SelectedText != string.Empty)
                Clipboard.SetData(DataFormats.Text, rb.SelectedText);
            rb.SelectedText = string.Empty;
        }
        private void CopyAct()
        {
            RichTextBox rb = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last();
            if (rb.SelectedText != string.Empty)
                Clipboard.SetData(DataFormats.Text, rb.SelectedText);

        }
        private void PasteAct()
        {
            RichTextBox rb = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last();
            int position = rb.SelectionStart;
            rb.Text = rb.Text.Insert(position, Clipboard.GetText());
            rb.SelectionStart = position + Clipboard.GetText().Length;
        }
        private void SelectAllAct()
        {
            RichTextBox rb = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last();
            rb.SelectAll();
        }
        private void BoldSelection()
        {
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
        private void ItalicSelection()
        {
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
        private void UnderlineSelection()
        {
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
        private void SaveFileByExtension(string filepath, string savepath, RichTextBox rb)
        {
            switch (Path.GetExtension(filepath))
            {
                case (".rtf"):
                    rb.SaveFile(savepath + "rtf", RichTextBoxStreamType.RichText);
                    break;
                case (".txt"):
                    rb.SaveFile(savepath + "txt", RichTextBoxStreamType.PlainText);
                    break;
                case (".cs"):
                    rb.SaveFile(savepath + "cs", RichTextBoxStreamType.PlainText);
                    break;

            }
        }

        private void OpenFileByExtension(string filepath, RichTextBox rb)
        {
            switch (Path.GetExtension(filepath))
            {
                case (".rtf"):
                    rb.LoadFile(filepath, RichTextBoxStreamType.RichText);
                    break;
                case (".txt"):
                    rb.LoadFile(filepath, RichTextBoxStreamType.PlainText);
                    break;
                case (".cs"):
                    rb.LoadFile(filepath, RichTextBoxStreamType.PlainText);
                    break;
            }

        }

        private void ReopenAll()
        {
            int indexSelected = tabControl1.SelectedIndex;
            List<TabF> listtab = new List<TabF>(tabPages);
            tabControl1.TabPages.Clear();
            tabPages.Clear();
            listtab.Select(e => e.PathToFile).ToList().ForEach(OpenFile);
            tabControl1.SelectedIndex = indexSelected;

        }

        private void StrikeoutSelection()
        {
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


        private static Color GetColorFor(string classificatioName)
        {
            switch (classificatioName)
            {
                case ClassificationTypeNames.InterfaceName:
                case ClassificationTypeNames.EnumName:
                case ClassificationTypeNames.Keyword:
                    return Color.DarkCyan;

                case ClassificationTypeNames.ClassName:
                case ClassificationTypeNames.StructName:
                    return Color.Yellow;

                case ClassificationTypeNames.Identifier:
                    return Color.DarkGray;

                case ClassificationTypeNames.Comment:
                    return Color.DarkGreen;

                case ClassificationTypeNames.StringLiteral:
                case ClassificationTypeNames.VerbatimStringLiteral:
                    return Color.DarkRed;

                case ClassificationTypeNames.Punctuation:
                    return Color.Gray;

                case ClassificationTypeNames.WhiteSpace:
                    return Color.Black;

                case ClassificationTypeNames.NumericLiteral:
                    return Color.Yellow;

                case ClassificationTypeNames.PreprocessorKeyword:
                    return Color.DarkMagenta;
                case ClassificationTypeNames.PreprocessorText:
                    return Color.DarkGreen;

                default:
                    return Color.Gray;
            }

        }

    }
}