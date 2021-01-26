using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
namespace TextEditor
{
    public partial class TimeMachineForm : Form
    {
        List<FileJournal> journal = FileJournal.LoadFileInfo();
        string PathToJournal = Path.GetRelativePath("TextEditor\\bin\\Debug\\netcoreapp3.1", "TextEditor\\Journal");


        public TimeMachineForm()
        {
            InitializeComponent();
            List<string> strjournal = journal.Select(e => e.ToString()).ToList();
            listBox1.Items.AddRange(strjournal.ToArray());
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                listBox2.Items.Clear();
                listBox2.Items.Add(journal[listBox1.SelectedIndex].filePosition);
                listBox2.Items.AddRange(journal[listBox1.SelectedIndex].ChangeTime.ToArray());
            }
        }

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
                }
            }
            catch
            {
                var result = MessageBox.Show($"Please close {journal[listBox1.SelectedIndex].filePosition} to rollback this file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }

    public class FileJournal
    {
        private static Random random = new Random();
        public static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static List<FileJournal> LoadFileInfo()
        {
            string PathToJournal = Path.GetRelativePath("TextEditor\\bin\\Debug\\netcoreapp3.1", "TextEditor\\Journal\\Journal.xml");
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
        public static void AddNewNode(string path, string folderName, string name)
        {
            string PathToJournal = Path.GetRelativePath("TextEditor\\bin\\Debug\\netcoreapp3.1", "TextEditor\\Journal\\Journal.xml");
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
        public string folderName { get; set; }
        public string fileName { get; set; }
        public string filePosition { get; set; }
        private List<string> changetime = new List<string>();

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

        public static void AddRecord(string Address)
        {
            string PathToJournal = Path.GetRelativePath("TextEditor\\bin\\Debug\\netcoreapp3.1", "TextEditor\\Journal\\Journal.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(PathToJournal);
            XmlNode root = doc.SelectSingleNode("/Journal");
            string strPath = string.Format($"/Journal/File[@fileposition = \"{Address}\"]");
            XmlElement selectedFile = (XmlElement)root.SelectSingleNode(strPath);
            XmlElement newRecrod = doc.CreateElement("Record");
            newRecrod.InnerText = DateTime.Now.ToString("G");
            selectedFile.SelectSingleNode("ChangeTime").AppendChild(newRecrod);
            doc.Save(PathToJournal);
        }


        public void RemoveRecord(string record)
        {
            changetime.Remove(record);
        }
        public override string ToString()
        {
            return $"{fileName}";
        }
        public string[] GetRecord()
        {
            return changetime.ToArray();
        }


    }
}
