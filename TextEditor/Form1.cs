using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using FastColoredTextBoxNS;
namespace TextEditor
{
    public partial class Form1 : Form
    {
        // Space for "x" button.
        const int LEADING_SPACE = 12;
        const int CLOSE_AREA = 30;
        // Determine Tab width.
        List<int> TabWidthList = new List<int>() { 200 };
        // To record file, use in the TimeMachine.
        List<FileJournal> filejournal = FileJournal.LoadFileInfo();
        // Theme color.
        string ColorTheme = "Light";
        public Form1()
        {
            InitializeComponent();
            // Fix window size.
            this.MinimumSize = new Size(Screen.PrimaryScreen.WorkingArea.Size.Width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2);
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            // Set pre setting.
            SetSetting();
            // Open lsat time file.
            OpenOldFile();
            if (tabControl1.TabCount != 0)
                this.Text = tabControl1.SelectedTab.Text;
            else
                this.Text = "Create or open file... Anyway, just do something...";
            // Start timer to count autosave and TimeMachine.
            timer1.Start();
            timer2.Start();
            // Switch option between C# file and text file.
            SwitchStyleOption();
        }


        /// <summary>
        /// Draw "x" to close tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.Graphics.DrawString("x", e.Font, Brushes.Black, e.Bounds.Right - CLOSE_AREA, e.Bounds.Top + 4);
                e.Graphics.DrawString(this.tabControl1.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + LEADING_SPACE, e.Bounds.Top + 4);
                e.DrawFocusRectangle();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        /// <summary>
        /// Find "x", make close area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {

            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                Rectangle r = tabControl1.GetTabRect(i);

                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - CLOSE_AREA, r.Top + 4, 30, 30);
                if (closeButton.Contains(e.Location))
                {
                    if (!tabPages[i].SavedOrNot && tabControl1.TabCount != 0)
                    {
                        // Save info dialog.
                        this.Text = tabControl1.TabPages[i].Text;
                        var result = MessageBox.Show($"Would you like to Save {tabControl1.TabPages[i].Text} before close this Tab?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            TabWidthList.Remove(this.tabControl1.TabPages[i].Text.Length * 18);
                            SaveFile();
                            tabPages.RemoveAt(i);
                            this.tabControl1.TabPages.RemoveAt(i);
                            RefreshTabSize();
                            SwitchStyleOption();

                            break;
                        }
                        else if (result == DialogResult.No)
                        {
                            // No save.
                            TabWidthList.Remove(this.tabControl1.TabPages[i].Text.Length * 18);
                            this.tabControl1.TabPages.RemoveAt(i);
                            tabPages.RemoveAt(i);
                            RefreshTabSize();
                            SwitchStyleOption();
                            break;
                        }
                    }
                    else
                    {
                        // Saved. Worked as DialogResult.No
                        TabWidthList.Remove(this.tabControl1.TabPages[i].Text.Length * 18);
                        this.tabControl1.TabPages.RemoveAt(i);
                        tabPages.RemoveAt(i);
                        RefreshTabSize();
                        SwitchStyleOption();
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Create new file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // New File dialog.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text Files | *.txt| RTF Files | *.rtf";
            saveFileDialog1.Title = "Create an Text File";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Create new tab with text.
                string fileName = saveFileDialog1.FileName;
                File.CreateText(fileName).Close();
                this.tabControl1.TabPages.Add(CreateNewTab(fileName));
            }
            SaveAll();
            if (tabControl1.TabCount != 0)
                tabControl1.SelectTab(tabControl1.TabCount - 1);
        }
        /// <summary>
        /// Open file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }
        /// <summary>
        /// Save as.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Just like in save.
            if (tabControl1.TabCount != 0)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Text Files | *.txt|RTF Files | *.rtf|C# file | *.cs";
                saveFileDialog1.Title = $"Save {tabControl1.SelectedTab.Text}";
                SaveFile(saveFileDialog1);
            }
        }
        /// <summary>
        /// Just save. Simple.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check is saved or not.
            if (tabControl1.TabCount != 0)
            {
                SaveFile();
                tabPages[tabControl1.SelectedIndex].SavedOrNot = true;
                tabControl1.SelectedTab.Text = tabControl1.SelectedTab.Text.Replace("*", "");
                this.Text = tabControl1.SelectedTab.Text;
                tabControl1.Refresh();
            }
        }
        /// <summary>
        /// Exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Ask save file when closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveBeforeClose(out DialogResult result);
            if (result == DialogResult.Cancel)
                e.Cancel = true;
        }
        /// <summary>
        /// Open setting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SettingForm st = new SettingForm();
            st.ShowDialog();
            SetSetting();
            ReopenAll();
            this.Refresh();
        }
        /// <summary>
        /// AutoSave.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            SaveAll();
        }
        /// <summary>
        /// Bold font.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            BoldSelection();
        }
        /// <summary>
        /// Italic font.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            ItalicSelection();
        }
        /// <summary>
        /// UnderLine font.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Ubutton_Click(object sender, EventArgs e)
        {
            UnderlineSelection();
        }
        /// <summary>
        /// Strikeout font.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SButton_Click(object sender, EventArgs e)
        {
            StrikeoutSelection();
        }
        /// <summary>
        /// New window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newWinStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();
        }
        /// <summary>
        /// Save all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAllStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveAll();
        }
        /// <summary>
        /// Format font.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromatToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count != 0 && tabControl1.SelectedTab.Controls.OfType<RichTextBox>() != null)
            {
                FontDialog fd = new FontDialog();
                fd.Font = tabPages[tabControl1.SelectedIndex].tabFont;
                fd.ShowDialog();
                tabPages[tabControl1.SelectedIndex].tabFont = fd.Font;
                tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Font = fd.Font;
            }
        }
        /// <summary>
        /// Change form text to file name for elegant.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedTab != null)
                this.Text = tabControl1.SelectedTab.Text;
            else
                this.Text = "Create or open file... Anyway, just do something...";
        }
        /// <summary>
        /// Cut.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutAct();

        }
        /// <summary>
        /// Copy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyAct();
        }
        /// <summary>
        /// Paste.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteAct();
        }
        /// <summary>
        /// Select all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectAllAct();
        }
        /// <summary>
        /// Right mouse select all.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RselectAllStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectAllAct();
        }
        /// <summary>
        /// Right mouse paste.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void RPasteStripMenuItem1_Click(object sender, EventArgs e)
        {
            PasteAct();
        }
        /// <summary>
        /// Right mouse copy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RCopyStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopyAct();
        }
        /// <summary>
        /// Right mouse cut.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RCutStripMenuItem1_Click(object sender, EventArgs e)
        {
            CutAct();
        }
        /// <summary>
        /// Right mouse bold font.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RBoldStripMenuItem1_Click(object sender, EventArgs e)
        {
            BoldSelection();
        }
        /// <summary>
        /// Right mouse italic font.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RItalicStripMenuItem2_Click(object sender, EventArgs e)
        {
            ItalicSelection();
        }
        /// <summary>
        /// Right mouse strikeout font.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RStrikeStripMenuItem4_Click(object sender, EventArgs e)
        {
            StrikeoutSelection();
        }
        /// <summary>
        /// Right mouse underline font.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RUnderStripMenuItem3_Click(object sender, EventArgs e)
        {
            UnderlineSelection();
        }
        /// <summary>
        /// TimeMachine save.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            filejournal = FileJournal.LoadFileInfo();
            string PathToJournal = Path.GetRelativePath("TextEditor\\bin\\Debug\\netcoreapp3.1", "TextEditor\\Journal");
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                try
                {
                    string fileName = tabControl1.TabPages[i].Text;
                    if (filejournal.Select(e => e.filePosition).ToList().Contains(tabPages[i].PathToFile))
                    {
                        // Save in the existing folder.
                        // Folder name is random 10char string, so no problem with same filename or path.
                        FileJournal.AddRecord(tabPages[i].PathToFile);
                        string folderPath = Path.Combine(PathToJournal, filejournal.Find(e => e.filePosition == tabPages[i].PathToFile).folderName);
                        folderPath = Path.Combine(folderPath, filejournal.Find(e => e.filePosition == tabPages[i].PathToFile).ChangeTime.Count.ToString());
                        SaveFileByExtension(tabPages[i].PathToFile, folderPath + ".", tabControl1.TabPages[i]);
                    }
                    else
                    {
                        // Create new folder and node in the journal.xml.
                        string folderName = FileJournal.RandomString();
                        FileJournal.AddNewNode(tabPages[i].PathToFile, folderName, fileName);
                        Directory.CreateDirectory(Path.Combine(PathToJournal, folderName));
                        filejournal = FileJournal.LoadFileInfo();
                        FileJournal.AddRecord(tabPages[i].PathToFile);
                        string folderPath = Path.Combine(PathToJournal, filejournal.Find(e => e.filePosition == tabPages[i].PathToFile).folderName);
                        folderPath = Path.Combine(folderPath, filejournal.Find(e => e.filePosition == tabPages[i].PathToFile).ChangeTime.Count.ToString());
                        SaveFileByExtension(tabPages[i].PathToFile, folderPath + ".", tabControl1.TabPages[i]);

                    }
                }
                catch
                {
                    var result = MessageBox.Show($"File {tabPages[i].PathToFile} can not be saved for some reason, try other file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }
        /// <summary>
        /// TimeMachine menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeMachineStripMenuItem1_Click(object sender, EventArgs e)
        {
            TimeMachineForm tmf = new TimeMachineForm();
            SaveAll();
            tmf.ShowDialog();
            ReopenAll();
            tabControl1.Refresh();
        }
        /// <summary>
        /// Undo action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tabControl1.TabCount != 0)

                if (Path.GetExtension(tabPages[tabControl1.SelectedIndex].PathToFile) != ".cs")
                {
                    if (tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().CanUndo)
                        tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Undo();
                }
                else
                {
                    tabControl1.SelectedTab.Controls.OfType<FastColoredTextBox>().Last().Undo();
                }
        }
        /// <summary>
        /// Redo action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tabControl1.TabCount != 0)

                if (Path.GetExtension(tabPages[tabControl1.SelectedIndex].PathToFile) != ".cs")
                {
                    if (tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().CanRedo)
                        tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Redo();
                }
                else
                {
                    tabControl1.SelectedTab.Controls.OfType<FastColoredTextBox>().Last().Redo();
                }
        }
        /// <summary>
        /// Formatting code(C#)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void formattingStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.tabControl1.TabCount != 0)

                if (Path.GetExtension(tabPages[tabControl1.SelectedIndex].PathToFile) == ".cs")
                {
                    // FaseColoredTextBox.
                    FastColoredTextBox fctb = tabControl1.SelectedTab.Controls.OfType<FastColoredTextBox>().Last();
                    int position = fctb.SelectionStart;
                    // Set the font.
                    fctb.Font = new Font("Courier New", 12);
                    fctb.Text = FormattingCode.GetFormatCode(fctb.Text);
                    if (fctb.Text.Length > position)
                        fctb.SelectionStart = position;
                }
        }
        /// <summary>
        /// Saved or not with indicator "*".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedTabBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tabPages[tabControl1.SelectedIndex].SavedOrNot)
            {
                tabPages[tabControl1.SelectedIndex].SavedOrNot = false;
                tabControl1.SelectedTab.Text = tabPages[tabControl1.SelectedIndex].FileName + "*";
                this.Text = tabControl1.SelectedTab.Text;

            }
        }
        /// <summary>
        /// Saved or not with indicator "*".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedTabRichBox_TextChanged(object sender, EventArgs e)
        {
            if (tabPages[tabControl1.SelectedIndex].SavedOrNot)
            {
                tabPages[tabControl1.SelectedIndex].SavedOrNot = false;
                tabControl1.SelectedTab.Text = tabPages[tabControl1.SelectedIndex].FileName + "*";
                this.Text = tabControl1.SelectedTab.Text;
            }
        }
        /// <summary>
        /// Switch the formatting option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SwitchStyleOption();
        }
        /// <summary>
        /// Compile the code.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void compileStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> errors = new List<string>();
                if (tabControl1.SelectedTab != null && tabControl1.SelectedTab.Controls.OfType<FastColoredTextBox>().Last() != null)
                    errors = CSharpCompiler.ComplieCode(tabControl1.SelectedTab.Controls.OfType<FastColoredTextBox>().Last().Text);
                Form f = new Form();
                f.Width = 1000;
                f.Height = 500;
                f.Text = "Errors";
                TextBox textBox = new TextBox();
                ListBox listBox = new ListBox();
                listBox.Dock = DockStyle.Fill;
                listBox.Items.AddRange(errors.ToArray());
                f.Controls.Add(listBox);
                f.Show();
            }
            catch
            {
                MessageBox.Show("Error! Unknow Error! Can't compile this code!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void helpStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFile("Helper.rtf");
            }
            catch
            {
                MessageBox.Show("Something go wrong, plz restart the programm", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

