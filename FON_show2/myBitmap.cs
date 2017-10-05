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
    public partial class myBitmap : UserControl
    {
        public class myBitmapRow
        {
            public byte[] _bytesRow;
            public myBitmapRow(byte[] bytes)
            {
                _bytesRow = bytes;
            }
        }
        public class myBitmapChar
        {
            public myBitmapRow[] _bitmapRows;
            public int _charWidth = 0;
            public myBitmapChar(myBitmapRow[] rows, int charWidth)
            {
                _charWidth = charWidth;
                _bitmapRows = rows;
            }
        }
        public class myBitmapAll
        {
            myBitmapChar[] _bitmapChars;
            int byteWidth;
            int rowCount;
            int factor = 10;
            public myBitmapAll(myBitmapChar[] chars)
            {
                _bitmapChars = chars;
                byteWidth = _bitmapChars[0]._bitmapRows[0]._bytesRow.Length;
                rowCount = _bitmapChars[0]._bitmapRows.Length;
            }
            public myBitmapChar get(int idx)
            {
                if (idx < 0 || idx > _bitmapChars.Length)
                    return null;
                myBitmapChar c = _bitmapChars[idx];
                return c;
            }

            public Bitmap getBitmap(int idx)
            {
                //create a new bitmap with 
                Bitmap bmp = new Bitmap(byteWidth * 8 * factor, rowCount*factor); //ie 160x270
                Graphics g = Graphics.FromImage(bmp);
                //how many blocks per row?
                int blockW = bmp.Width / (byteWidth * 8);
                int blockH = bmp.Height / rowCount;

                System.Drawing.Pen myPen = new Pen(Color.Red);
                System.Drawing.Brush myBrush = new System.Drawing.SolidBrush(Color.Black);
                System.Drawing.Brush myBrushWhite = new System.Drawing.SolidBrush(Color.Gold);// Color.White);
                //erase
                g.FillRectangle(myBrushWhite, new Rectangle(new Point(0, 0), new Size(bmp.Width, bmp.Height)));
                g.DrawLine(myPen, new Point(_bitmapChars[idx]._charWidth * factor, 0), new Point(_bitmapChars[idx]._charWidth * factor, bmp.Height));
                for (int r = 0; r < rowCount; r++)
                {
                    byte[] _b = _bitmapChars[idx]._bitmapRows[r]._bytesRow;
                    string sBin = "";
                    for (int segment = 0; segment < _b.Length; segment++)
                    {
                        sBin = sBin + Convert.ToString(_b[segment], 2).PadLeft(8, '0');
                    }
                    for (int pos = 0; pos < 8 * _b.Length; pos++)
                    {
                        if (sBin.Substring(pos, 1) == "1") // ((_b & (1 << pos)) != 0)
                            g.FillEllipse(myBrush, pos * blockW, r*blockH, blockW, blockH);
                        else
                            g.FillEllipse(myBrushWhite, pos * blockW, r*blockH, blockW, blockH);
                    }
                }
                return bmp;
            }
        }

        public class myAllBitmaps
        {
            List<byte> _allBytes;
            int _numBytesPerRow = 0;
            int _linesPerChar = 0;
            int _codeStart = 0;
            public myAllBitmaps(List<byte> bytes, int numBytesPerRow, int linesPerChar, int codeStart)
            {
                _allBytes = bytes;
                _numBytesPerRow = numBytesPerRow;
                _linesPerChar = linesPerChar;
                _codeStart = codeStart;
            }
            public byte[] get(int codePoint)
            {
                byte[] bytes= _allBytes.ToArray();
                byte[] copy = new byte[_linesPerChar * _numBytesPerRow];
                Array.Copy(bytes, (codePoint-_codeStart) *(_linesPerChar * _numBytesPerRow) ,copy,  0, _linesPerChar*_numBytesPerRow);
                return copy;
            }
        }

        public myBitmap()
        {
            InitializeComponent();
        }
    }
}
