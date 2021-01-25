using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        const int LEADING_SPACE = 12;
        const int CLOSE_AREA = 30;
        public Form1()
        {
            InitializeComponent();
            this.Text = "Window1";

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.Graphics.DrawString("x", e.Font, Brushes.Black, e.Bounds.Right - CLOSE_AREA, e.Bounds.Top + 4);
                e.Graphics.DrawString(this.tabControl1.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + LEADING_SPACE, e.Bounds.Top + 4);
                e.DrawFocusRectangle();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
            {
                Rectangle r = tabControl1.GetTabRect(i);

                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - CLOSE_AREA, r.Top + 4, 30, 30);
                if (closeButton.Contains(e.Location))
                {
                    if (MessageBox.Show("Would you like to Save before close this Tab?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.tabControl1.TabPages.RemoveAt(i);
                        break;

                    }
                }
            }
        }

    }
}
