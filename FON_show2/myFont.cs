using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace FON_show2
{
    public class myFont
    {
        string _sFile = "CC020 CP1250 v10.FON";
        /*
        <bh:00><bh:00><bh:00><bh:00>1.0<bh:18>CC020c<bh:00>
        <bh:00><bh:1b><bh:00><bh:02>6<bh:00> <bh:ff><bh:00>304/26/06CP1250_H27_W10_M
         * 
        00 00 00 00 31 2E 30 18  43 43 30 32 30 63 00 0A     ....1.0. CC020c.. 
        00 1B 00 02 36 00 20 FF  00 33 30 34 2F 32 36 2F     ....6... .304/26/
        30 36 43 50 31 32 35 30  5F 48 32 37 5F 57 31 30     06CP1250 _H27_W10
        5F 4D                                                _M
        
THE V1.0 HEADER:

    The 54 bytes (ALL bytes must be present) within the header are as follows:
	4 BYTES			May be anything, rewritten internally
	3 BYTES			Font version number (must be "1.0")
	1 BYTE			Mod 256 summation of the five character name
         * To speed finding fonts, each five character name has a MOD 256 summation of that name in the header.  To calculate this value, ADD the ASCII value of each character (in HEX) and use the lower order byte.  For example, the characters in the name PT10B have ASCII values 50H, 54H, 31H, 30H, and 42H which add to 147H.  Using just the lower order byte, we would enter 47H in the table where it says "MOD 256 summation
	5 BYTES			Five character name for this font
	1 BYTE			One character name for this font
	1 BYTE			Must be 00 (says this table is a FONT)
	2 BYTES, LSB 1st		Number of dots wide
	2 BYTES, LSB 1st		Number of dots high
	1 BYTE			Number of bytes in each row
	2 BYTES			Number of bytes in each character
	1 BYTE			First ASCII character represented in this font
	1 BYTE			Last ASCII character represented in this font
	1 BYTE			Reserved
	1 BYTE			USER version number
	8 BYTES			USER creation date
	20 BYTES			USER description

THE V1.3 HEADER:

	The 71 bytes (ALL bytes must be present) within the header are as follows:
	4 BYTES			May be anything, rewritten internally
	4 BYTES			Font version number (must be "1.3") plus NUL terminator
	1 BYTE			Mod 256 summation of the five character name
	6 BYTES			Five character name for this font plus NUL terminator
	1 BYTE			One character name for this font
	1 BYTE			One character name for this font (impact only - PICA)
	1 BYTE			One character name for this font (impact only - ELITE)
	1 BYTE			One character name for this font (impact only – italic PICA)
	1 BYTE			One character name for this font (impact only – italic ELITE)
	1 BYTE			Must be 00 (says this table is a FONT)
	1 BYTE			Display Code (0=do not display on self test; 1=display)
	2 BYTES, LSB 1st		Number of dots wide
	2 BYTES, LSB 1st		Number of dots wide (impact only – PICA)
	2 BYTES, LSB 1st		Number of dots wide (impact only – ELITE)
	2 BYTES, LSB 1st		Number of dots wide (impact only – italic PICA)
	2 BYTES, LSB 1st		Number of dots wide (impact only – italic ELITE)
	2 BYTES, LSB 1st		Number of dots high
	1 BYTE			Number of bytes in each row
	2 BYTES			Number of bytes in each character
	1 BYTE			First ASCII character represented in this font
	1 BYTE			Last ASCII character represented in this font
	1 BYTE			Dot row to place underline
	1 BYTE			USER version number
	9 BYTES			USER creation date plus NUL terminator
	21 BYTES			USER description plus NUL terminator

THE V2.0 HEADER:

	The 96 bytes (ALL bytes must be present) within the header are as follows:
	4 BYTES			May be anything, rewritten internally
	4 BYTES			Font version number (must be "2.0") plus NUL terminator
	4 BYTES			Must be 96 (size of the header)
	1 BYTE			Mod 256 summation of the five character name	
	6 BYTES			Five character name for this font plus NUL terminator
	1 BYTE			One character name for this font
	1 BYTE			One character name for this font (impact only - PICA)
	1 BYTE			One character name for this font (impact only – PICA cond)
	1 BYTE			One character name for this font (impact only – ELITE)
	1 BYTE			One character name for this font (impact only – ELITE cond)
	1 BYTE			Must be 00 (says this table is a FONT)
	1 BYTE			Display Code (0=do not display on self test; 1=display)
	2 BYTES, LSB 1st		Number of dots wide
	2 BYTES, LSB 1st		Number of dots wide (impact only – PICA)
	2 BYTES, LSB 1st		Number of dots wide (impact only – PICA cond)
	2 BYTES, LSB 1st		Number of dots wide (impact only – ELITE)
	2 BYTES, LSB 1st		Number of dots wide (impact only – ELITE cond)
	2 BYTES, LSB 1st		Number of dots high
	2 BYTES			Number of bytes in each row
	2 BYTES			Number of bytes in each character
	1 BYTE			First ASCII character represented in this font
	1 BYTE			Last ASCII character represented in this font
	2 BYTES			Dot row to place underline
	2 BYTES			Baseline Position
	1 BYTE			USER version number
	11 BYTES			USER creation date plus NUL terminator
	21 BYTES			USER description plus NUL terminator
	15 BYTES			Must be 0xFF – reserved bytes

        */

        const UInt16 ProportionalMark = 0xFF00;
        public byte[] FileMarker=new byte[4]; //4 bytes                  // 00 00 00 00 | FF FF FF FF=fixed width
                                                                         // 02 6A 00 00 = prop
                                                                         // 7C 6A 00 00 = prop
        // for prop fonts, first byte of byte array is width of character, for example
        // 0x0A 
        public string FileVersion;//3 chars                              // 1.0
        public byte ModuloFontName;   //1 byte                               // 0x18, possibly baseline?

        public string FontNameShort;//5 chars                            // CC020
        public byte FontID;//1 byte                                      // c
        public byte AllwaysZero;                                         // 00 for all fonts
        public UInt16 CharWidth;                                         // for prop fonts this is 0xff00 (255) and the width is stored in front of every byte block as two-byte (low first)
        public UInt16 CharHeight;                                          // 1B possibly the height = 27 pixels

        public byte numBytesPerRow;                                      // 02 possibly num of bytes per row
        public UInt16 numBytesPerChar;
                                                                         
        public byte codeStart;                                  // 20   = first code point (here the Blank)
        public byte codeEnd;                                    // FF   = last code point (here 255)
        public byte bReserved3;                                 // 00
        public byte UserVersionNumber;                          // 33 (51)
        public string UserDate; //8 chars                       // 04/26/06
        public string FontNameLong;//20 chars                   // CP1250_H27_W10_M  (25 chars?)

        public String fontDisplay = "";
        public myBitmap.myBitmapAll allChars;
        List<byte> myBytes;
        List<byte> myHeaderBytes;
        public byte[] headerbytes
        {
            get { return myHeaderBytes.ToArray(); }
        }
        public myFont(string sFile)
        {
            _sFile = sFile;
            myBytes = new List<byte>();
            myHeaderBytes = new List<byte>();
            readFont();
            //readHeader();
        }

        Fontheader font = null;
        void readFont()
        {
            try
            {
                font = new Fontheader(_sFile);
                allChars = font.allChars;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Exception: " + ex.Message);
            }
        }

        public void readHeader()
        {
            try
            {
                FileStream streamReader;
                streamReader = new FileStream(_sFile, FileMode.Open,FileAccess.Read,FileShare.Read);
                BinaryReader br = new BinaryReader(streamReader);
                
                //read header bytes
                for(int c=0;c<0x36;c++)
                    myHeaderBytes.Add (br.ReadByte());
                br.BaseStream.Seek(0, SeekOrigin.Begin);

                //start from 0
                FileMarker = br.ReadBytes(4);
                //offset 0x04
                FileVersion = Encoding.ASCII.GetString(br.ReadBytes(3));
                ModuloFontName = br.ReadByte();
                //offset 0x08
                FontNameShort = Encoding.ASCII.GetString(br.ReadBytes(5));
                FontID = br.ReadByte();                                     //offset 0x0d

                AllwaysZero = br.ReadByte();
                if (AllwaysZero != 0)
                    System.Diagnostics.Debug.WriteLine("not a font?, allways zero byte is="+AllwaysZero.ToString());
                byte[] bUint16;
                bUint16 = br.ReadBytes(2);
                CharWidth = getUint16(bUint16);                             //offset 0x1F and 0x20

                bUint16 = br.ReadBytes(2);
                CharHeight = getUint16(bUint16);                            //offset 0x21 and 0x22
                
                numBytesPerRow = br.ReadByte();                             //offset 0x23
                
                bUint16 = br.ReadBytes(2);
                numBytesPerChar = (getUint16(bUint16)); //offset 0x24 and 0x25

                codeStart = br.ReadByte();                                  //offset 0x26
                codeEnd = br.ReadByte();                                    //offset 0x27

                bReserved3 = br.ReadByte();                                 //offset 0x28
                UserVersionNumber = br.ReadByte();                          //offset 0x29  USER version number
                
                UserDate = Encoding.ASCII.GetString(br.ReadBytes(8));       //offset 0x2A
                byte[] buf = br.ReadBytes(20);                              //offset 0x32 to 0x46
                FontNameLong = Encoding.ASCII.GetString(buf);
                FontNameLong = FontNameLong.Replace('\0',' ');
                //now we have 224 or so chars to read...
                //assume we have CharHeight lines for one char
                StringBuilder sbFont = new StringBuilder();
                    
                //store all bitmap bytes
                List<myBitmap.myBitmapChar> allCharBitmaps = new List<myBitmap.myBitmapChar>();

                for (int i = codeStart; i <= codeEnd; i++)
                {
                    System.Diagnostics.Debug.WriteLine("################################");
                    //start a new charBitmap
                    List<myBitmap.myBitmapRow> charBitmapRow = new List<myBitmap.myBitmapRow>();
                    System.Diagnostics.Debug.WriteLine("reading code point: " + i.ToString());
                    List<byte> bitmapRow = new List<byte>();
                    
                    int thisCharWidth=this.CharWidth;
                    if (thisCharWidth == ProportionalMark)
                    { //is proportional font?
                        byte[] bcw = br.ReadBytes(2);
                        thisCharWidth=bcw[1]*0xff + bcw[0]; //read char width
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
                            sbFont.Append(Convert.ToString(currByte, 2).PadLeft(8, '0').Replace("0", " ").Replace("1", "█"));
                        }
                        //add row to bitamp
                        myBitmap.myBitmapRow rowX = new myBitmap.myBitmapRow(bitmapRow.ToArray());
                        charBitmapRow.Add(rowX);
                        bitmapRow.Clear();
                        
                        sbFont.Append("\r\n");
                        //add one row of bytes to list of rows
                    }//iterate thru char rows
                    //add bitmap matrix as char bitmap
                    myBitmap.myBitmapChar charX = new myBitmap.myBitmapChar(charBitmapRow.ToArray(), thisCharWidth);
                    allCharBitmaps.Add(charX);
                    charBitmapRow.Clear();
                }//iterate thru chars
                allChars = new myBitmap.myBitmapAll(allCharBitmaps.ToArray());
                fontDisplay=sbFont.ToString();
                myBitmap.myAllBitmaps allBitmaps = new myBitmap.myAllBitmaps(myBytes, numBytesPerRow, CharHeight, codeStart);
                byte[] bTest = allBitmaps.get(33);
                System.Drawing.Bitmap bmp = allChars.getBitmap(1);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
            }
        }

        public string dumpHeader()
        {
            return font.dumpHeader();

            StringBuilder sb = new StringBuilder();
            sb.Append("FileVersion=" + this.FileVersion + "\r\n");
            sb.Append("Fontname short=" + this.FontNameShort + " (");
            if (ModuloFontName != getModuloForFontName(FontNameShort))
                sb.AppendLine("Modulo does not match Fontname)");
            else
                sb.AppendLine("Modulo matches Fontname)");
            sb.Append("UserDate=" + this.UserDate + "\r\n");
            sb.Append("UserVersion=" + Encoding.ASCII.GetString(new byte[]{this.UserVersionNumber}) + "\r\n");
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
            byte[] bChars=Encoding.ASCII.GetBytes(fontname);
            for (int i = 0; i < bChars.Length; i++)
            {
                bSum += bChars[i];
            }
            //use low part only
            bSum = bSum & 0xFF;
            return (byte)bSum;
        }

        /// <summary>
        /// return uint16 for two bytes with LSB first
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        UInt16 getUint16(byte[] bytes){
            UInt16 i16=0;
            i16 = (UInt16)(bytes[1] * 0xff + bytes[0]);
            return i16;
        }

        //byte array have to be aligned at multiples of 4 (DOWRD alignment)
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        unsafe struct fontheader
        {
//            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] FileMarker;                               // 00 00 00 00

//            [FieldOffset(4)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
            public string FileVersion;                              // 1.0

//            [FieldOffset(7)]
            public byte bReserved1;                                 // 18

//            [FieldOffset(8)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]     //is only 5 but will be cutoff when used
            public string FontNameShort;                            // CC020

//            [FieldOffset(0x0D)]
            public byte FontID;                                     // c

//            [FieldOffset(0x0E)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)] // 00 0A 00 1B 00 02 36 00 
            public byte[] bReserved2;

            public byte codeStart;                                  // 20   = first code point (here the Blank)
            public byte codeEnd;                                    // FF   = last code point (here 255)
//            [FieldOffset(0x18)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x1A)]  // is only 0x19 but will be cutoff
            public string FontNameLong;                             // 304/26/06CP1250_H27_W10_M  (25 chars?)
        }

        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
