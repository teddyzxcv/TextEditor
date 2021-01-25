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
                if (FileDialog1.FileName.Substring(FileDialog1.FileName.LastIndexOf('.')) == ".rtf")
                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().LoadFile(FileDialog1.FileName, RichTextBoxStreamType.RichText);
                else
                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Text = File.ReadAllText(FileDialog1.FileName);
            }
        }
        private void OpenFile(string Path)
        {
            if (File.Exists(Path))
            {
                this.tabControl1.TabPages.Add(CreateNewTab(Path));
                tabControl1.SelectTab(tabControl1.TabCount - 1);
                if (Path.Substring(Path.LastIndexOf('.')) == ".rtf")

                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().LoadFile(Path, RichTextBoxStreamType.RichText);
                else
                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().Text = File.ReadAllText(Path);
            }
        }

        private void SaveFile(SaveFileDialog sfd)
        {

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (tabControl1.SelectedTab.Text.Substring(tabControl1.SelectedTab.Text.LastIndexOf('.')) == ".rtf")
                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SaveFile(sfd.FileName, RichTextBoxStreamType.RichText);
                else
                    tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);


            }
        }
        private void SaveFile()
        {
            if (tabControl1.SelectedTab.Text.Substring(tabControl1.SelectedTab.Text.LastIndexOf('.')) == ".rtf")
                tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SaveFile(tabPages[tabControl1.SelectedIndex].PathToFile, RichTextBoxStreamType.RichText);
            else
                tabControl1.SelectedTab.Controls.OfType<RichTextBox>().Last().SaveFile(tabPages[tabControl1.SelectedIndex].PathToFile, RichTextBoxStreamType.PlainText);
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
                using (StreamWriter sw = new StreamWriter(tabPages[tabControl1.SelectedIndex].PathToFile))
                {
                    sw.Write(tabControl1.TabPages[i].Controls.OfType<RichTextBox>().Last().Text);
                    sw.Close();
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
    }
}