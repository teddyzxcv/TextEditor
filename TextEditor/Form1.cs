using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Host.Mef;




namespace TextEditor
{
    public partial class Form1 : Form
    {
        const int LEADING_SPACE = 12;
        const int CLOSE_AREA = 30;
        List<int> TabWidthList = new List<int>() { 200 };
        List<FileJournal> filejournal = FileJournal.LoadFileInfo();

        string ColorTheme = "Light";
        public Form1()
        {
            InitializeComponent();
            this.MinimumSize = new Size(Screen.PrimaryScreen.WorkingArea.Size.Width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2);
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            SetSetting();
            OpenOldFile();
            if (tabControl1.TabCount != 0)
                this.Text = tabControl1.SelectedTab.Text;
            else
                this.Text = "Create or open file... Anyway, just do something...";
            timer1.Start();
            timer2.Start();
        }



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

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {

            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                Rectangle r = tabControl1.GetTabRect(i);

                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - CLOSE_AREA, r.Top + 4, 30, 30);
                if (closeButton.Contains(e.Location))
                {
                    var result = MessageBox.Show($"Would you like to Save {tabControl1.TabPages[i].Text} before close this Tab?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        TabWidthList.Remove(this.tabControl1.TabPages[i].Text.Length * 18);
                        SaveFile();
                        tabPages.RemoveAt(i);
                        this.tabControl1.TabPages.RemoveAt(i);
                        RefreshTabSize();
                        break;
                    }
                    else if (result == DialogResult.No)
                    {
                        TabWidthList.Remove(this.tabControl1.TabPages[i].Text.Length * 18);
                        this.tabControl1.TabPages.RemoveAt(i);
                        tabPages.RemoveAt(i);
                        RefreshTabSize();
                        break;
                    }
                }
            }
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text Files | *.txt | RTF Files | *.rtf";
            saveFileDialog1.Title = "Create an Text File";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
                File.CreateText(fileName).Close();
                this.tabControl1.TabPages.Add(CreateNewTab(fileName));
            }
            SaveAll();
            tabControl1.SelectTab(tabControl1.TabCount - 1);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text Files | *.txt|RTF Files | *.rtf|C# file | *.cs";
            saveFileDialog1.Title = $"Save {tabControl1.SelectedTab.Text}";
            SaveFile(saveFileDialog1);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveBeforeClose();
        }
        private void SettingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SettingForm st = new SettingForm();
            st.ShowDialog();
            SetSetting();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SaveAll();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            BoldSelection();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ItalicSelection();
        }

        private void Ubutton_Click(object sender, EventArgs e)
        {
            UnderlineSelection();
        }

        private void SButton_Click(object sender, EventArgs e)
        {
            StrikeoutSelection();
        }

        private void newWinStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();
        }

        private void saveAllStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        private void FromatToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count != 0)
            {
                FontDialog fd = new FontDialog();
                fd.Font = tabPages[tabControl1.SelectedIndex].tabFont;
                fd.ShowDialog();
                tabPages[tabControl1.SelectedIndex].tabFont = fd.Font;
                tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Font = fd.Font;
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedTab != null)
                this.Text = tabControl1.SelectedTab.Text;
            else
                this.Text = "Create or open file... Anyway, just do something...";
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutAct();

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyAct();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteAct();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectAllAct();
        }

        private void RselectAllStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectAllAct();
        }

        private void RPasteStripMenuItem1_Click(object sender, EventArgs e)
        {
            PasteAct();
        }

        private void RCopyStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopyAct();
        }

        private void RCutStripMenuItem1_Click(object sender, EventArgs e)
        {
            CutAct();
        }

        private void RBoldStripMenuItem1_Click(object sender, EventArgs e)
        {
            BoldSelection();
        }

        private void RItalicStripMenuItem2_Click(object sender, EventArgs e)
        {
            ItalicSelection();
        }

        private void RStrikeStripMenuItem4_Click(object sender, EventArgs e)
        {
            StrikeoutSelection();
        }

        private void RUnderStripMenuItem3_Click(object sender, EventArgs e)
        {
            UnderlineSelection();
        }

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
                        FileJournal.AddRecord(tabPages[i].PathToFile);
                        string folderPath = Path.Combine(PathToJournal, filejournal.Find(e => e.filePosition == tabPages[i].PathToFile).folderName);
                        folderPath = Path.Combine(folderPath, filejournal.Find(e => e.filePosition == tabPages[i].PathToFile).ChangeTime.Count.ToString());
                        SaveFileByExtension(fileName, folderPath + ".", tabControl1.TabPages[i].Controls.OfType<RichTextBox>().Last());
                    }
                    else
                    {
                        string folderName = FileJournal.RandomString();
                        FileJournal.AddNewNode(tabPages[i].PathToFile, folderName, fileName);
                        Directory.CreateDirectory(Path.Combine(PathToJournal, folderName));
                        filejournal = FileJournal.LoadFileInfo();
                        FileJournal.AddRecord(tabPages[i].PathToFile);
                        string folderPath = Path.Combine(PathToJournal, filejournal.Find(e => e.filePosition == tabPages[i].PathToFile).folderName);
                        folderPath = Path.Combine(folderPath, filejournal.Find(e => e.filePosition == tabPages[i].PathToFile).ChangeTime.Count.ToString());
                        SaveFileByExtension(fileName, folderPath + ".", tabControl1.TabPages[i].Controls.OfType<RichTextBox>().Last());

                    }
                }
                catch
                {
                    var result = MessageBox.Show($"File {tabPages[i].PathToFile} can not be saved for some reason, try other file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }

        private void TimeMachineStripMenuItem1_Click(object sender, EventArgs e)
        {
            TimeMachineForm tmf = new TimeMachineForm();
            SaveAll();
            tmf.ShowDialog();
            ReopenAll();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().CanUndo)
                tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().CanRedo)
                tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Redo();
        }

        private void formattingStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Path.GetExtension(tabControl1.SelectedTab.Text) == ".cs")
            {
                List<string> CodeLines = new List<string>(tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Text.Split("\n", StringSplitOptions.RemoveEmptyEntries));
                string formattedCode = String.Join("\n", new List<string>(FormattingCode.GetFormatLineCode(CodeLines)));
                CodeLines = FormattingCode.GetFormatTabCode(new List<string>(formattedCode.Split("\n", StringSplitOptions.RemoveEmptyEntries)));
                formattedCode = String.Join("\n", new List<string>(FormattingCode.GetFormatTabCode(CodeLines)));
                tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Text = formattedCode;
            }
        }
        public static void AppendText(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
        private void SelectedTabBox_TextChanged(object sender, EventArgs e)
        {
            if (Path.GetExtension(tabControl1.SelectedTab.Text) == ".cs")
            {
                var host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
                var workspace = new AdhocWorkspace(host);
                string code = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Text;
                var sourceText = SourceText.From(code);
                SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);
                var compilation = CSharpCompilation.Create("Dummy").AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location)).AddSyntaxTrees(tree);
                var semanticModel = compilation.GetSemanticModel(tree);
                var classifiedSpans = Classifier.GetClassifiedSpans(semanticModel, new TextSpan(0, code.Length), workspace);
                IDictionary<int, Color> positionColorMap = classifiedSpans.ToDictionary(c => c.TextSpan.Start, c => GetColorFor(c.ClassificationType));
                tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Text = "";
                for (int charPosition = 0; charPosition < code.Length; charPosition++)
                {
                    // Проверяем, нужно ли изменять цвет консоли
                    Color newColor;
                    if (positionColorMap.TryGetValue(charPosition, out newColor))
                    {
                        AppendText(tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last(), code[charPosition].ToString(), newColor);
                    }
                }
                tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Font = new Font("Courier New", 15);


            }
        }



    }
}

