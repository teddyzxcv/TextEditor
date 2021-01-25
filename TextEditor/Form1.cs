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
            this.Text = "Window1";
            this.MinimumSize = new Size(Screen.PrimaryScreen.WorkingArea.Size.Width / 2, Screen.PrimaryScreen.WorkingArea.Size.Height / 2);
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            SetSetting();
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
        private void SaveBeforeClose()
        {
            File.AppendAllLines("Setting.conf", tabPages.Select(e => e.PathToFile));
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

        private void SettingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SettingForm st = new SettingForm();
            st.ShowDialog();
            SetSetting();
        }
        private void SetSetting()
        {
            List<string> AllSetting = new List<string>(File.ReadAllLines("Setting.conf"));
            try
            {
                this.timer1.Interval = int.Parse(AllSetting[0].Split(' ')[2]) * 60000;
                ColorTheme = AllSetting[1].Split(' ')[2];
            }
            catch
            {
                var result = MessageBox.Show("Incorrect setting! plz don't change file Setting.conf!!, Press OK to fix setting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (result == DialogResult.OK)
                {
                    File.CreateText("Setting.conf").Close();
                    this.Close();
                }
            }
            switch (ColorTheme)
            {
                case ("Light"):
                    menuStrip1.BackColor = Color.White;
                    splitContainer1.Panel1.BackColor = Color.White;
                    break;
                case ("Dark"):
                    menuStrip1.BackColor = Color.Gray;
                    splitContainer1.Panel1.BackColor = Color.Gray;
                    break;
                case ("Red"):
                    menuStrip1.BackColor = Color.OrangeRed;
                    splitContainer1.Panel1.BackColor = Color.OrangeRed;
                    break;
            }
            if (AllSetting.Count == 2 && tabControl1.TabCount == 0)
                OpenFile("Intro.rtf");
            else
                AllSetting.GetRange(2, AllSetting.Count - 2).ForEach(OpenFile);
            File.WriteAllLines("Setting.conf", AllSetting.GetRange(0, 2));
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            SaveAll();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                Font f = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont;
                FontStyle fs = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont.Style;
                if (f.Bold)
                {
                    button1.BackColor = Color.White;
                    fs &= ~FontStyle.Bold;
                }
                else
                {
                    button1.BackColor = Color.Gray;
                    fs |= FontStyle.Bold;
                }
                if (tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont != null)
                {
                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont = new Font(f, fs);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                Font f = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont;
                FontStyle fs = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont.Style;
                if (f.Italic)
                {
                    button2.BackColor = Color.White;
                    fs &= ~FontStyle.Italic;
                }
                else
                {
                    button2.BackColor = Color.Gray;
                    fs |= FontStyle.Italic;
                }
                if (tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont != null)
                {
                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont = new Font(f, fs);
                }

            }
        }

        private void Ubutton_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                Font f = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont;
                FontStyle fs = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont.Style;
                if (f.Underline)
                {
                    Ubutton.BackColor = Color.White;
                    fs &= ~FontStyle.Underline;

                }
                else
                {
                    Ubutton.BackColor = Color.Gray;
                    fs |= FontStyle.Underline;

                }
                if (tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont != null)
                {
                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont = new Font(f, fs);
                }
            }
        }

        private void SButton_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                Font f = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont;
                FontStyle fs = tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont.Style;
                if (f.Strikeout)
                {
                    Sbutton.BackColor = Color.White;
                    fs &= ~FontStyle.Strikeout;

                }
                else
                {
                    Sbutton.BackColor = Color.Gray;
                    fs |= FontStyle.Strikeout;

                }
                if (tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont != null)
                {
                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SelectionFont = new Font(f, fs);
                }
            }
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
    }
}

