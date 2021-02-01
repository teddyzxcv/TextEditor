using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Xml;
namespace TextEditor
{
    public partial class TimeMachineForm : Form
    {
        List<FileJournal> journal = FileJournal.LoadFileInfo();
        string PathToJournal = Path.GetRelativePath("TextEditor\\bin\\Debug\\netcoreapp3.1", "TextEditor\\Journal");

        /// <summary>
        /// Initialize the form.
        /// </summary>
        public TimeMachineForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            List<string> strjournal = journal.Select(e => e.ToString()).ToList();
            listBox1.Items.AddRange(strjournal.ToArray());
            if (listBox1.Items.Count != 0)
                listBox1.SelectedIndex = 0;
        }
        /// <summary>
        /// Select one file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                listBox2.Items.Clear();
                listBox2.Items.Add(journal[listBox1.SelectedIndex].filePosition);
                listBox2.Items.AddRange(journal[listBox1.SelectedIndex].ChangeTime.ToArray());
            }
        }
        /// <summary>
        /// Double click to restore the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            journal = FileJournal.LoadFileInfo();
            try
            {
                if (listBox2.SelectedItem != null && listBox2.SelectedIndex != 0)
                {
                    string PathToTMFile = Path.Combine(PathToJournal, journal[listBox1.SelectedIndex].folderName);
                    PathToTMFile = Path.Combine(PathToTMFile, $"{listBox2.SelectedIndex - 1}" + Path.GetExtension(journal[listBox1.SelectedIndex].filePosition));
                    File.Copy(PathToTMFile, journal[listBox1.SelectedIndex].filePosition, true);
                    this.Close();
                    MessageBox.Show("Save successfully restored, (Rollback baby!)", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            catch
            {
                var result = MessageBox.Show($"Please close {journal[listBox1.SelectedIndex].filePosition} to rollback this file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        /// <summary>
        /// Change list2 when change selection in the list1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                listBox2.Items.Clear();
                listBox2.Items.Add(journal[listBox1.SelectedIndex].filePosition);
                listBox2.Items.AddRange(journal[listBox1.SelectedIndex].ChangeTime.ToArray());
            }

        }
    }

    /// <summary>
    /// Save and change file to do rollback.
    /// </summary>
    public class FileJournal
    {
        private static Random random = new Random();
        static string PathToJournal = Path.GetRelativePath("TextEditor\\bin\\Debug\\netcoreapp3.1", "TextEditor\\Journal\\Journal.xml");
        /// <summary>
        /// Get random 10 char string to get folder name.
        /// </summary>
        /// <returns></returns>
        public static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        /// <summary>
        /// Get file info from Xml document (/Journal/Journal.xml).
        /// </summary>
        /// <returns></returns>
        public static List<FileJournal> LoadFileInfo()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PathToJournal);
            XmlNodeList FileList = doc.SelectNodes("//Journal/File");
            List<FileJournal> journal = new List<FileJournal>();
            if (FileList != null)
            {
                foreach (XmlNode item in FileList)
                {
                    FileJournal fileinfo = new FileJournal();
                    fileinfo.folderName = item.Attributes["folderName"].Value;
                    fileinfo.filePosition = item.Attributes["fileposition"].Value;
                    fileinfo.fileName = item.Attributes["Name"].Value;
                    fileinfo.changetime = item.SelectSingleNode("ChangeTime").SelectNodes("Record").Cast<XmlNode>().Select(e => e.InnerText).ToList();
                    journal.Add(fileinfo);
                }
            }
            return journal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="folderName"></param>
        /// <param name="name"></param>
        public static void AddNewNode(string path, string folderName, string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PathToJournal);
            XmlNode root = doc.SelectSingleNode("/Journal");
            XmlElement newFile = doc.CreateElement("File");
            XmlAttribute fileAttrfolderName = doc.CreateAttribute("folderName");
            XmlAttribute fileAttrName = doc.CreateAttribute("Name");
            XmlAttribute fileAttrfileposition = doc.CreateAttribute("fileposition");
            fileAttrfileposition.Value = path;
            fileAttrfolderName.Value = folderName;
            fileAttrName.Value = name;
            newFile.SetAttributeNode(fileAttrfileposition);
            newFile.SetAttributeNode(fileAttrfolderName);
            newFile.SetAttributeNode(fileAttrName);
            XmlElement newChangeTime = doc.CreateElement("ChangeTime");
            newFile.AppendChild(newChangeTime);
            root.AppendChild(newFile);
            doc.Save(PathToJournal);
        }
        /// <summary>
        /// Parameter in the class.
        /// </summary>
        /// <value></value>
        public string folderName { get; set; }
        public string fileName { get; set; }
        public string filePosition { get; set; }
        private List<string> changetime = new List<string>();

        /// <summary>
        /// Change time record.
        /// </summary>
        /// <value></value>
        public List<string> ChangeTime
        {
            get
            {
                return changetime;
            }
            set
            {
                changetime = value;
            }
        }
        /// <summary>
        /// Add record to journal.
        /// </summary>
        /// <param name="Address"></param>
        public static void AddRecord(string Address)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PathToJournal);
            XmlNode root = doc.SelectSingleNode("/Journal");
            string strPath = string.Format($"/Journal/File[@fileposition = \"{Address}\"]");
            XmlElement selectedFile = (XmlElement)root.SelectSingleNode(strPath);
            XmlElement newRecrod = doc.CreateElement("Record");
            newRecrod.InnerText = DateTime.Now.ToString("G") + "<----------------";
            selectedFile.SelectSingleNode("ChangeTime").AppendChild(newRecrod);
            doc.Save(PathToJournal);
        }
        /// <summary>
        /// Delete all record in the journal.xml.
        /// </summary>

        public static void DeleteAllRecord()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PathToJournal);
            doc.SelectSingleNode("/Journal").RemoveAll();
            doc.Save(PathToJournal);
            List<string> dirlist = new List<string>(Directory.GetDirectories(Directory.GetParent(PathToJournal).ToString()));
            foreach (string item in dirlist)
            {
                Directory.Delete(item, true);
            }
            MessageBox.Show("All save deleted!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public override string ToString()
        {
            return $"{fileName}";
        }
    }
}
