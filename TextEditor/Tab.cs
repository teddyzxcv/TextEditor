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
    public class TabF
    {
        public string PathToFile { get; set; }

        public bool SaveOrNot { get; set; }

        public TabPage Page { get; set; }
        public Font tabFont { get; set; }


    }
}