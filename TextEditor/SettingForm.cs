using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;



namespace TextEditor
{
    public partial class SettingForm : Form
    {
        /// <summary>
        ///  Initialize setting form.
        /// </summary>
        public SettingForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        /// <summary>
        /// Save all when click save.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Save_Click(object sender, EventArgs e)
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfg.AppSettings.Settings["AutosaveTime"].Value = this.numericUpDown1.Value.ToString();
            cfg.AppSettings.Settings["ThemeColor"].Value = this.comboBox1.SelectedItem.ToString();
            cfg.AppSettings.Settings["TimeMachine"].Value = this.TimeMachineUpDown2.Value.ToString();
            cfg.AppSettings.Settings["CSharpCommentStyle"].Value = this.button1.BackColor.ToArgb().ToString();
            cfg.AppSettings.Settings["CSharpKeywordStyle"].Value = this.button2.BackColor.ToArgb().ToString();
            cfg.AppSettings.Settings["CSharpAttributeStyle"].Value = this.button3.BackColor.ToArgb().ToString();
            cfg.AppSettings.Settings["CSharpClassNameStyle"].Value = this.button4.BackColor.ToArgb().ToString();
            cfg.AppSettings.Settings["CSharpCommentTagStyle"].Value = this.button5.BackColor.ToArgb().ToString();
            cfg.AppSettings.Settings["CSharpNumberStyle"].Value = this.button6.BackColor.ToArgb().ToString();
            cfg.AppSettings.Settings["CSharpStringStyle"].Value = this.button7.BackColor.ToArgb().ToString();
            cfg.AppSettings.Settings["CSharpVariableStyle"].Value = this.button8.BackColor.ToArgb().ToString();
            cfg.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            FormattingCode.SetSyntaxColor();
            this.Close();
        }
        /// <summary>
        /// Setting setting form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingForm_Load(object sender, EventArgs e)
        {
            this.numericUpDown1.Value = int.Parse(ConfigurationManager.AppSettings["AutosaveTime"]);
            //this.comboBox1.SelectedItem = AllSetting[1].Split(' ')[2];
            this.comboBox1.SelectedItem = ConfigurationManager.AppSettings["ThemeColor"];
            this.TimeMachineUpDown2.Value = int.Parse(ConfigurationManager.AppSettings["TimeMachine"]);
            this.button1.BackColor = Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpCommentStyle"]));
            this.button2.BackColor = Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpKeywordStyle"]));
            this.button3.BackColor = Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpAttributeStyle"]));
            this.button4.BackColor = Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpClassNameStyle"]));
            this.button5.BackColor = Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpCommentTagStyle"]));
            this.button6.BackColor = Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpNumberStyle"]));
            this.button7.BackColor = Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpStringStyle"]));
            this.button8.BackColor = Color.FromArgb(int.Parse(ConfigurationManager.AppSettings["CSharpVariableStyle"]));
        }
        /// <summary>
        /// Clean all file journal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Cleanbutton1_Click(object sender, EventArgs e)
        {
            FileJournal.DeleteAllRecord();
        }
        /// <summary>
        /// Comment color set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = button1.BackColor;
            cd.ShowDialog();
            button1.BackColor = cd.Color;
        }
        /// <summary>
        /// Keyword color set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = button2.BackColor;
            cd.ShowDialog();
            button2.BackColor = cd.Color;
        }
        /// <summary>
        /// Attribute color set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = button3.BackColor;
            cd.ShowDialog();
            button3.BackColor = cd.Color;
        }
        /// <summary>
        /// Class name color set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void button4_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = button4.BackColor;
            cd.ShowDialog();
            button4.BackColor = cd.Color;
        }
        /// <summary>
        /// Comment tag color set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void button5_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = button5.BackColor;
            cd.ShowDialog();
            button5.BackColor = cd.Color;
        }
        /// <summary>
        /// Number color set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void button6_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = button6.BackColor;
            cd.ShowDialog();
            button6.BackColor = cd.Color;
        }
        /// <summary>
        /// String color set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void button7_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = button7.BackColor;
            cd.ShowDialog();
            button7.BackColor = cd.Color;
        }
        /// <summary>
        /// Variable color set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = button8.BackColor;
            cd.ShowDialog();
            button8.BackColor = cd.Color;
        }
    }
}
