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
    partial class Form1
    {


        List<TabF> tabPages = new List<TabF>();
        private void OpenFile()
        {
            OpenFileDialog FileDialog1 = new OpenFileDialog();
            FileDialog1.RestoreDirectory = true;
            FileDialog1.Filter = "Text Files | *.txt|RTF Files | *.rtf";
            FileDialog1.Title = "Open an Text File";
            if (FileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.tabControl1.TabPages.Add(CreateNewTab(FileDialog1.FileName));
                tabControl1.SelectTab(tabControl1.TabCount - 1);
                var sr = new StreamReader(FileDialog1.FileName);
                tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Text = sr.ReadToEnd();
                sr.Close();
            }
        }
        private void OpenFile(string Path)
        {


            this.tabControl1.TabPages.Add(CreateNewTab(Path));
            tabControl1.SelectTab(tabControl1.TabCount - 1);
            var sr = new StreamReader(Path);
            tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Text = sr.ReadToEnd();
            sr.Close();

        }

        private void SaveFile(SaveFileDialog sfd)
        {

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    sw.Write(tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Text);
                    sw.Close();
                }
            }
        }
        private void SaveFile()
        {
            using (StreamWriter sw = new StreamWriter(tabPages[tabControl1.SelectedIndex].PathToFile))
            {
                sw.Write(tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Text);
                sw.Close();
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
            newTab.Controls.Add(newTextBox);
            RefreshTabSize(fileName);
            TabF newT = new TabF();
            newT.PathToFile = FileName;
            newT.Page = newTab;
            newT.SaveOrNot = true;
            tabPages.Add(newT);
            return newTab;
        }
    }
}