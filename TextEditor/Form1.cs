using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace TextEditor
{
    public partial class Form1 : Form
    {
        const int LEADING_SPACE = 12;
        const int CLOSE_AREA = 30;
        List<int> TabWidthList = new List<int>() { 200 };

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
                this.Text = string.Empty;
            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {

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
            tabControl1.SelectTab(tabControl1.TabCount - 1);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text Files | *.txt | RTF Files | *.rtf";
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
            FontDialog fd = new FontDialog();
            fd.Font = tabPages[tabControl1.SelectedIndex].tabFont;
            fd.ShowDialog();
            tabPages[tabControl1.SelectedIndex].tabFont = fd.Font;
            tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Font = fd.Font;
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedTab != null)
                this.Text = tabControl1.SelectedTab.Text;
            else
                this.Text = "";
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
    }
}

