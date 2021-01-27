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
    class FormattingCode
    {
        static int TabBeforeLine = 0;
        //Do formatting
        public static List<string> GetFormatLineCode(List<string> code)
        {
            bool InsideBigComment = false;
            for (int i = 0; i < code.Count; i++)
            {
                string codeline = code[i].Trim();
                string[] sym = { "{", "}", ";" };
                codeline = codeline.Replace("{", "\n{\n");
                codeline = codeline.Replace("}", "\n}\n");
                codeline = codeline.Replace(";", ";\n");
                List<char> codecharlist = new List<char>(codeline);
                bool insidecomma = false;
                bool insidecomment = false;
                for (int j = 0; j < codecharlist.Count; j++)
                {
                    if (j >= 1 && $"{codecharlist[j - 1]}{codecharlist[j]}" == "//")
                        insidecomment = true;
                    if (j >= 1 && $"{codecharlist[j - 1]}{codecharlist[j]}" == "/*")
                        InsideBigComment = true;
                    if (j >= 1 && $"{codecharlist[j - 1]}{codecharlist[j]}" == "*/")
                        InsideBigComment = false;
                    if (codecharlist[j] == '\"')
                        insidecomma = !insidecomma;
                    if ((insidecomma || insidecomment || InsideBigComment) && codecharlist[j] == '\n')
                    {
                        codecharlist.RemoveAt(j);
                        j--;
                    }
                }
                code[i] = new string(codecharlist.ToArray());
            }

            return code;
        }
        public static List<string> GetFormatTabCode(List<string> code)
        {
            for (int i = 0; i < code.Count; i++)
            {
                string codeline = code[i].Trim();
                if (codeline.Trim() == "}")
                    TabBeforeLine--;
                string indent = new string('\t', TabBeforeLine);
                codeline = indent + codeline;
                if (codeline.Trim() == "{")
                    TabBeforeLine++;

                code[i] = codeline;
            }
            TabBeforeLine = 0;
            return code;
        }
        public static List<int> AllIndexesOf(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }



    }
}
