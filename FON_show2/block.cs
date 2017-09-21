using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FON_show2
{
    public partial class block : UserControl
    {
        byte[] _b=new byte[]{0x0c,0x00};
        public block()
        {
            InitializeComponent();
        }
        public void setByte(byte[] b)
        {
            _b = b;
            this.Invalidate();
            //this.Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {

            int width = this.Width;
            int height = this.Height;
            int blockW = width / (_b.Length*8); //how many bits need to be drawn per row

            System.Drawing.Brush myBrush = new System.Drawing.SolidBrush(Color.Black);
            System.Drawing.Brush myBrushWhite = new System.Drawing.SolidBrush(Color.White);
            //erase
            e.Graphics.FillRectangle(myBrushWhite, new Rectangle(new Point(0,0), new Size(width,height)));
            string sBin = "";
            for (int segment = 0; segment < _b.Length; segment++)
            {
                sBin = sBin + Convert.ToString(_b[segment], 2).PadLeft(8, '0');
            }
                for (int pos = 0; pos < 8*_b.Length; pos++)
                {
                    if (sBin.Substring(pos, 1) == "1") // ((_b & (1 << pos)) != 0)
                        e.Graphics.FillEllipse(myBrush, pos * blockW, 0, blockW, height);
                    else
                        e.Graphics.FillEllipse(myBrushWhite, pos * blockW, 0, blockW, height);
                }
            base.OnPaint(e);
            //this.Update();
        }
    }
}
