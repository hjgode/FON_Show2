using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Reflection;

namespace FON_show2
{
    class myFont2
    {
        public byte[] FileMarker { get; set; }
        public string FileVersion { get; set; } public myFontHeader.FontVersion fontVersion = myFontHeader.FontVersion.Version10;
        public byte ModuloFontName { get; set; }
        public string FontNameShort { get; set; }
        public byte FontID { get; set; }
        public byte AllwaysZero { get; set; }
        public UInt16 CharWidth { get; set; }
        public UInt16 CharHeight { get; set; }
        public UInt16 numBytesPerRow { get; set; }
        public UInt16 numBytesPerChar { get; set; }
        public byte codeStart { get; set; }
        public byte codeEnd { get; set; }
        public byte Reserved { get; set; }
        public byte[] Reserved2 { get; set; }
        public byte UserVersionNumber { get; set; }
        public string UserDate { get; set; }
        public string FontNameLong { get; set; }

        public bool bIsProportinalFont = false;

        const UInt16 ProportionalMark = 0xFF00;
        public String fontDisplay = "";
        public myBitmap.myBitmapAll allChars;

        public byte[] headerbytes;

        public myFont2(string fileName)
        {
            FileStream streamReader;
            myFontHeader[] theFontHeader=null;
            streamReader = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader br = new BinaryReader(streamReader);
            byte[] bytes = br.ReadBytes((int)myFontHeader.FontVersion.Version10); //read 54 bytes
            //get fileversion
            myFontHeader _fontheader = myFontHeader.getName("FileVersion", myFontHeader._fontHeaderEntries);
            string fileversion = getString(bytes, (int)_fontheader._offset, (int)_fontheader._size);
            if (fileversion == "1.0")
            {
                fontVersion = myFontHeader.FontVersion.Version10;
                theFontHeader=myFontHeader._fontHeaderEntries;
            }
            else if (fileversion == "1.3")
            {
                fontVersion = myFontHeader.FontVersion.Version13;
                theFontHeader=myFontHeader._fontHeaderEntries13;
            }
            else if (fileversion == "2.0")
            {
                fontVersion = myFontHeader.FontVersion.Version20;
                theFontHeader=myFontHeader._fontHeaderEntries20;
            }
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            bytes = br.ReadBytes((int)fontVersion);
            headerbytes = new byte[bytes.Length];
            Array.Copy(bytes,headerbytes,bytes.Length);
            //now assign all properties
            foreach (myFontHeader FH in theFontHeader)
            {
                //get property for name
                PropertyInfo property = this.GetType().GetProperty(FH._name);
                var type = FH._type;
                var size = FH._size;
                var name = FH._name;
                //
                if(type==TypeCode.Byte){
                    if (size == 1)
                    {
                        property.SetValue(this, bytes[FH._offset], null);
                        System.Diagnostics.Debug.WriteLine("Set "+property.Name+" to "+ property.GetValue(this,null));
                    }
                    else
                    {
                        byte[] bValue = new byte[size];
                        Array.Copy(bytes, FH._offset, bValue, 0, FH._size);
                        property.SetValue(this, bValue, null);
                        System.Diagnostics.Debug.WriteLine("Set " + property.Name + " to " + property.GetValue(this, null));
                    }
                }
                else if (type == TypeCode.String) { 
                    String sValue = getString(bytes, (int)FH._offset, (int)FH._size);
                    property.SetValue(this, sValue, null);
                    System.Diagnostics.Debug.WriteLine("Set " + property.Name + " to " + property.GetValue(this, null));
                }
                else if (type == TypeCode.UInt16)
                {
                    byte[] bValue = new byte[size];
                    Array.Copy(bytes, FH._offset, bValue, 0, FH._size);
                    UInt16 u16=getUint16(bValue);
                    property.SetValue(this, u16, null);
                    System.Diagnostics.Debug.WriteLine("Set " + property.Name + " to " + property.GetValue(this, null));
                }
            }

            if (CharWidth == ProportionalMark)
                bIsProportinalFont = true;

            //Now go on and read the bitmaps...
            br.BaseStream.Seek((int)fontVersion, SeekOrigin.Begin);
            //store all bitmap bytes
            List<myBitmap.myBitmapChar> allCharBitmaps = new List<myBitmap.myBitmapChar>();
            List<byte> myBytes = new List<byte>();
            //store the bitmap pixels in a string
            StringBuilder sbFont = new StringBuilder();

            for (int i = codeStart; i <= codeEnd; i++)
            {
                System.Diagnostics.Debug.WriteLine("################################");
                //start a new charBitmap
                List<myBitmap.myBitmapRow> charBitmapRow = new List<myBitmap.myBitmapRow>();
                System.Diagnostics.Debug.WriteLine("reading code point: " + i.ToString());
                List<byte> bitmapRow = new List<byte>();

                int thisCharWidth = this.CharWidth;
                if (thisCharWidth == ProportionalMark)
                { //is proportional font?
                    byte[] bcw = br.ReadBytes(2);
                    thisCharWidth = bcw[1] * 0xff + bcw[0]; //read char width
                }
                //start a new char bitmap row
                for (int y = 0; y < CharHeight; y++)
                {
                    //System.Diagnostics.Debug.WriteLine("reading byte lines: " + y.ToString());
                    //read num bytes per row
                    for (int iBytesPerRow = 0; iBytesPerRow < this.numBytesPerRow; iBytesPerRow++)
                    {
                        byte currByte = br.ReadByte();
                        //add current byte to list of bytes per row
                        bitmapRow.Add(currByte);
                        myBytes.Add(currByte);
                        //add pixel to string representation
                        sbFont.Append(Convert.ToString(currByte, 2).PadLeft(8, '0').Replace("0", " ").Replace("1", "█"));
                    }
                    //add row to bitamp
                    myBitmap.myBitmapRow rowX = new myBitmap.myBitmapRow(bitmapRow.ToArray());
                    charBitmapRow.Add(rowX);
                    bitmapRow.Clear();
                    //start a new line in the string representation
                    sbFont.Append("\r\n");
                    //add one row of bytes to list of rows
                }//iterate thru char rows
                fontDisplay = sbFont.ToString();
                //add bitmap matrix as char bitmap
                myBitmap.myBitmapChar charX = new myBitmap.myBitmapChar(charBitmapRow.ToArray(), thisCharWidth);
                allCharBitmaps.Add(charX);
                charBitmapRow.Clear();
            }//iterate thru chars
            allChars = new myBitmap.myBitmapAll(allCharBitmaps.ToArray());
            myBitmap.myAllBitmaps allBitmaps = new myBitmap.myAllBitmaps(myBytes, numBytesPerRow, CharHeight, codeStart);
            //TEST ONLY
            byte[] bTest = allBitmaps.get(codeStart);
            System.Drawing.Bitmap bmp = allChars.getBitmap(1);

        }
        public string dumpHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("FileVersion=" + this.FileVersion + "\r\n");
            sb.Append("Fontname short=" + this.FontNameShort + " (");
            if (ModuloFontName != getModuloForFontName(FontNameShort))
                sb.AppendLine("Modulo does not match Fontname)");
            else
                sb.AppendLine("Modulo matches Fontname)");
            sb.Append("UserDate=" + this.UserDate + "\r\n");
            sb.Append("UserVersion=" + Encoding.ASCII.GetString(new byte[] { this.UserVersionNumber }) + "\r\n");
            sb.Append("Fontname long='" + this.FontNameLong + "'\r\n");
            sb.Append("Font ID=0x" + this.FontID.ToString("x02") + "(" + Encoding.ASCII.GetString(new byte[] { this.FontID }, 0, 1) + ")\r\n");
            sb.Append("code start=" + this.codeStart.ToString() + "\r\n");
            sb.Append("code end=" + this.codeEnd.ToString() + "\r\n");
            sb.Append("Char width (bits)=" + this.CharWidth.ToString() + " ");
            if (CharWidth == ProportionalMark)
                sb.AppendLine("(prop.)");
            else
                sb.AppendLine("(fixed)");
            sb.Append("Char height (bytes)=" + this.CharHeight.ToString() + "\r\n");
            sb.Append("num bytes/row=" + this.numBytesPerRow.ToString() + "\r\n");
            System.Diagnostics.Debug.WriteLine(sb.ToString());
            return sb.ToString();
        }
        byte getModuloForFontName(string fontname)
        {
            if (fontname.Length != 5)
                return 0x00;
            uint bSum = 0;
            byte[] bChars = Encoding.ASCII.GetBytes(fontname);
            for (int i = 0; i < bChars.Length; i++)
            {
                bSum += bChars[i];
            }
            //use low part only
            bSum = bSum & 0xFF;
            return (byte)bSum;
        }
        string getString(byte[] buf, int offset, int length)
        {
            string s = "";
            s = Encoding.ASCII.GetString(buf, offset, length);
            s = s.Replace("\0", "");
            return s;
        }
        /// <summary>
        /// return uint16 for two bytes with LSB first
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public UInt16 getUint16(byte[] bytes)
        {
            UInt16 i16 = 0;
            if (bytes.Length == 2)
                i16 = (UInt16)(bytes[0] + bytes[1] * 0xff);
            else
                i16 = bytes[0];
            return i16;
        }
    }
    class myFontHeader
    {
        public string _name { get; set; }
        public long _offset { get; set; }
        public long _size { get; set; }
        public TypeCode _type { get; set; }

        public myFontHeader(string name, long offset, long size, TypeCode type)
        {
            _name = name;
            _offset = offset;
            _size = size;
            _type = type;
        }

        //get the name, offset, length of an font attribute
        public static myFontHeader getName(string name, myFontHeader[] fontheader)
        {
            myFontHeader fh = new myFontHeader("", 0, 0, TypeCode.Byte);
            foreach (myFontHeader FH in fontheader)
            {
                if (FH._name == name)
                {
                    fh = FH;
                    continue;
                }
            }
            return fh;
        }

        /// <summary>
        /// the FontVersion, holds also header byte length
        /// </summary>
        public enum FontVersion
        {
            Unknown = 0,
            Version10 = 0x36,
            Version13 = 71,
            Version20 = 96
        }
        public static myFontHeader[] _fontHeaderEntries=new myFontHeader[]{
            new myFontHeader("FileMarker", 0, 4, TypeCode.Byte),
            new myFontHeader("FileVersion", 4, 3, TypeCode.String),
            new myFontHeader("ModuloFontName", 7, 1, TypeCode.Byte),
            new myFontHeader("FontNameShort", 8, 5, TypeCode.String),
            new myFontHeader("FontID", 13, 1, TypeCode.Byte),
            new myFontHeader("AllwaysZero", 14, 1, TypeCode.Byte),
            new myFontHeader("CharWidth", 15, 2, TypeCode.UInt16),
            new myFontHeader("CharHeight", 17, 2, TypeCode.UInt16),
            new myFontHeader("numBytesPerRow", 19, 1, TypeCode.Byte),
            new myFontHeader("numBytesPerChar", 20, 2, TypeCode.UInt16),
            new myFontHeader("codeStart", 22, 1, TypeCode.Byte),
            new myFontHeader("codeEnd", 23, 1, TypeCode.Byte),
            new myFontHeader("Reserved", 24, 1, TypeCode.Byte),
            new myFontHeader("UserVersionNumber", 25, 1, TypeCode.Byte),
            new myFontHeader("UserDate", 26, 8, TypeCode.String),
            new myFontHeader("FontNameLong", 34, 20, TypeCode.String),
        };
        public static myFontHeader[] _fontHeaderEntries13 = new myFontHeader[]{
            new myFontHeader("FileMarker", 0, 4, TypeCode.Byte),
            new myFontHeader("FileVersion", 4, 4, TypeCode.String),
            new myFontHeader("ModuloFontName", 8, 1, TypeCode.Byte),
            new myFontHeader("FontNameShort", 9, 6, TypeCode.String),
            new myFontHeader("FontID", 15, 1, TypeCode.Byte),
            //16 to 19 for PICA, Elite, Italic
            new myFontHeader("AllwaysZero", 20, 1, TypeCode.Byte),
            //21 = display code 1 byte
            new myFontHeader("CharWidth", 22, 2, TypeCode.UInt16),
            //24 to 30 for PICA, Elite, Italic
            new myFontHeader("CharHeight", 32, 2, TypeCode.UInt16),
            new myFontHeader("numBytesPerRow", 34, 1, TypeCode.Byte),
            new myFontHeader("numBytesPerChar", 35, 2, TypeCode.UInt16),
            new myFontHeader("codeStart", 37, 1, TypeCode.Byte),
            new myFontHeader("codeEnd", 38, 1, TypeCode.Byte),
            new myFontHeader("Reserved", 39, 1, TypeCode.Byte),
            new myFontHeader("UserVersionNumber", 40, 1, TypeCode.Byte),
            new myFontHeader("UserDate", 41, 9, TypeCode.String),
            new myFontHeader("FontNameLong", 50, 21, TypeCode.String),
        };
        public static myFontHeader[] _fontHeaderEntries20 = new myFontHeader[]{
            new myFontHeader("FileMarker", 0, 4, TypeCode.Byte),
            new myFontHeader("FileVersion", 4, 4, TypeCode.String),
            //8 header size 4 bytes (always 96)
            new myFontHeader("ModuloFontName", 12, 1, TypeCode.Byte),
            new myFontHeader("FontNameShort", 13, 6, TypeCode.String),
            new myFontHeader("FontID", 19, 1, TypeCode.Byte),
            //20 to 23 for PICA, Elite, Italic
            new myFontHeader("AllwaysZero", 24, 1, TypeCode.Byte),
            //25 = display code 1 byte
            new myFontHeader("CharWidth", 26, 2, TypeCode.UInt16),
            //24 to 30 for PICA, Elite, Italic
            new myFontHeader("CharHeight", 36, 2, TypeCode.UInt16),
            new myFontHeader("numBytesPerRow", 38, 2, TypeCode.UInt16), //this two bytes with v 2.0
            new myFontHeader("numBytesPerChar", 40, 2, TypeCode.UInt16),
            new myFontHeader("codeStart", 42, 1, TypeCode.Byte),
            new myFontHeader("codeEnd", 43, 1, TypeCode.Byte),
            new myFontHeader("Reserved2", 44, 2, TypeCode.Byte), //undeline pos
            //underline is 44 to 45 = 2 bytes
            //baseline is 46 to 47 = 2 bytes
            new myFontHeader("UserVersionNumber", 48, 1, TypeCode.Byte),
            new myFontHeader("UserDate", 49, 11, TypeCode.String),
            new myFontHeader("FontNameLong", 60, 21, TypeCode.String),
            //plus 15 bytes of 0xFF to fill 96 bytes 81 to 96
        };
    }
}
