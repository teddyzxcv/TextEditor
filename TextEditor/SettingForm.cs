using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace TextEditor
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }



        private void Save_Click(object sender, EventArgs e)
        {
            List<string> AllSetting = new List<string>();
            AllSetting.Add($"AutoSaveTime = {numericUpDown1.Value}");
            AllSetting.Add($"ColorTheme = {comboBox1.SelectedItem.ToString()}");
            using (StreamWriter sw = new StreamWriter("Setting.conf"))
            {
                AllSetting.ForEach(sw.WriteLine);
                sw.Close();
            }
            this.Close();
        }



        private void SettingForm_Load(object sender, EventArgs e)
        {
            if (!File.Exists("Setting.conf"))
            {
                var result = MessageBox.Show("Incorrect setting! plz don't change or delete file Setting.conf!!, Press OK to fix setting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (result == DialogResult.OK)
                {
                    File.CreateText("Setting.conf").Close();
                    this.Close();
                }
            }
            List<string> AllSetting = new List<string>(File.ReadAllLines("Setting.conf"));
            if (AllSetting.Count != 0)
                try
                {
                    this.numericUpDown1.Value = int.Parse(AllSetting[0].Split(' ')[2]);
                    this.comboBox1.SelectedItem = AllSetting[1].Split(' ')[2];
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
        }


    }
}
