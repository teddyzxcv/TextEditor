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

        public TimeMachineForm()
        {
            InitializeComponent();
            List<string> strjournal = journal.Select(e => e.ToString()).ToList();
            listBox1.Items.Add(strjournal[0].ToString());
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
                    fileinfo.filePosition = item.Attributes["filepositoin"].Value;
                    fileinfo.fileName = item.Attributes["Name"].Value;
                    fileinfo.changetime = item.SelectNodes("//ChangeTime/Record").Cast<XmlNode>().Select(e => e.InnerText).ToList();
                    journal.Add(fileinfo);
                }
            }
            return journal;

        }
        public static void AddNewNode()
        {
            string PathToJournal = Path.GetRelativePath("TextEditor\\bin\\Debug\\netcoreapp3.1", "TextEditor\\Journal\\Journal.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(PathToJournal);
            XmlNode root = doc.SelectSingleNode("/Journal");
            XmlElement newFile = doc.CreateElement("File");
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

        public void Add(string record)
        {
            changetime.Add(record);
        }
        public void Remove(string record)
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
