using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FON_show2
{
    public partial class Form1 : Form
    {
        myFont mFont = new myFont(@"D:\C-Source\Active\FON_show2\CC020 CP1250 v10.FON");
        public Form1()
        {
            InitializeComponent();
        }
        void start() {
            folderBrowserDialog1.SelectedPath = System.IO.Directory.GetCurrentDirectory();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                label2.Text = folderBrowserDialog1.SelectedPath;
                listBox1.Items.Clear();
                DirectoryInfo dinfo = new DirectoryInfo(label2.Text);
                FileInfo[] Files = dinfo.GetFiles("*.*");
                foreach (FileInfo file in Files)
                {
                    listBox1.Items.Add(file.Name);
                }

            }
        }
        void doUpdateFont(string sFile)
        {
            //myFont mFont = new myFont(@"D:\tmp\font\FontsForThermalVer4.xx\Mf025.fon");
            mFont = new myFont(sFile);
            textBox1.Text = mFont.dumpHeader();
            //System.Diagnostics.Debug.WriteLine( mFont.dumpHeader());
            textBox2.Text = mFont.fontDisplay;

            //block1.setByte(new byte[]{0x0c,0x00});

            hScrollBar1.Minimum = mFont.codeStart;
            hScrollBar1.Maximum = mFont.codeEnd;

            hScrollBar1.Value = mFont.codeStart;
        }
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            label1.Text = hScrollBar1.Value.ToString();
            pictureBox1.Image = mFont.allChars.getBitmap(hScrollBar1.Value-mFont.codeStart);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            start();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            string s = label2.Text +"\\"+ listBox1.SelectedItem.ToString();
            doUpdateFont(s);
        }
    }
}
