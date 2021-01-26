using System;
using System.Configuration;
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
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfg.AppSettings.Settings["AutosaveTime"].Value = this.numericUpDown1.Value.ToString();
            cfg.AppSettings.Settings["ThemeColor"].Value = this.comboBox1.SelectedItem.ToString();
            cfg.AppSettings.Settings["TimeMachine"].Value = this.TimeMachineUpDown2.Value.ToString();
            cfg.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            this.Close();
        }
        private void SettingForm_Load(object sender, EventArgs e)
        {
            this.numericUpDown1.Value = int.Parse(ConfigurationManager.AppSettings["AutosaveTime"]);
            //this.comboBox1.SelectedItem = AllSetting[1].Split(' ')[2];
            this.comboBox1.SelectedItem = ConfigurationManager.AppSettings["ThemeColor"];

            this.TimeMachineUpDown2.Value = int.Parse(ConfigurationManager.AppSettings["TimeMachine"]);

        }


    }
}
