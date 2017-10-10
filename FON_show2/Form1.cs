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
        //myFont mFont = new myFont(@"D:\svn\git\FON_Show2\FontsForThermalVer4.xx\CC020 CP1250 v10.FON");
        //Fontheader mFont = new Fontheader(@"D:\svn\git\FON_Show2\FontsForThermalVer4.xx\CC020 CP1250 v10.FON");
        myFont2 mFont = new myFont2(@"D:\svn\git\FON_Show2\FontsForThermalVer7.xx\ASN-Bv20.fon");
        public Form1()
        {
            InitializeComponent();
            //doUpdateFont(@"D:\svn\git\FON_Show2\FontsForThermalVer4.xx\CC020 CP1250 v10.FON");
            doUpdateFont(@"D:\svn\git\FON_Show2\FontsForThermalVer7.xx\ASN-Bv20.fon");
        }
        void start() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true; dlg.CheckPathExists = true;
            dlg.RestoreDirectory = true;
            dlg.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                label2.Text = System.IO.Path.GetDirectoryName(dlg.FileName);
                listBox1.Items.Clear();
                DirectoryInfo dinfo = new DirectoryInfo(label2.Text);
                FileInfo[] Files = dinfo.GetFiles("*.*");
                foreach (FileInfo file in Files)
                {
                    listBox1.Items.Add(file.Name);
                }
                doUpdateFont(dlg.FileName);
            }
        }
        void doUpdateFont(string sFile)
        {
            myFont2 mFont = new myFont2(sFile);

            //mFont = new myFont(@"D:\tmp\font\FontsForThermalVer4.xx\Mf025.fon");
            //mFont = new myFont(sFile);
            //mFont = new Fontheader(sFile);

            textBox1.Text = mFont.dumpHeader();
            System.Diagnostics.Debug.WriteLine( mFont.dumpHeader());
            textBox2.Text = mFont.fontDisplay;

            //block1.setByte(new byte[]{0x0c,0x00});

            hScrollBar1.Minimum = mFont.codeStart;
            hScrollBar1.Maximum = mFont.codeEnd;

            hScrollBar1.Value = mFont.codeStart;

            byte[] bTest = new byte[mFont.headerbytes.Length];
            Array.Copy(mFont.headerbytes, bTest, mFont.headerbytes.Length);
            System.Diagnostics.Debug.WriteLine(Hex.Dump(bTest));
            txtHex.Text = Hex.Dump(bTest);

            hScrollBar1.Value = mFont.codeStart+1;
            label1.Text = hScrollBar1.Value.ToString();
            pictureBox1.Image = mFont.allChars.getBitmap(hScrollBar1.Value - mFont.codeStart);
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
