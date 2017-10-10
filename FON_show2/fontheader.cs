using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FON_show2
{
    class Fontheader
    {
        const UInt16 ProportionalMark = 0xFF00;
        public byte[] FileMarker = new byte[4]; //4 bytes                  // 00 00 00 00 | FF FF FF FF=fixed width
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

        public UInt16 numBytesPerRow;                                      // 02 possibly num of bytes per row
        public UInt16 numBytesPerChar;

        public byte codeStart;                                  // 20   = first code point (here the Blank)
        public byte codeEnd;                                    // FF   = last code point (here 255)
        public byte UserVersionNumber;                          // 33 (51)
        public string UserDate; //8 chars                       // 04/26/06
        public string FontNameLong;//20 chars                   // CP1250_H27_W10_M  (25 chars?)

        public FontVersion fontVersion = FontVersion.Unknown;
        public UInt32 headerSize = 54;
        public byte displayCode = 0;
        public byte underLineRow = 0;

        public String fontDisplay = "";
        public myBitmap.myBitmapAll allChars;
        List<byte> myHeaderBytes=new List<byte>();
        public byte[] headerbytes
        {
            get { return myHeaderBytes.ToArray(); }
        }

        public Fontheader(String fileName)
        {
            FileStream streamReader;
            streamReader = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            long pos = 0;
            List<byte> myBytes=new List<byte>();

            BinaryReader br = new BinaryReader(streamReader);

            br.BaseStream.Seek(0, SeekOrigin.Begin);
            //start from 0
            FileMarker = br.ReadBytes(4);
            //offset 0x04
            FileVersion = Encoding.ASCII.GetString(br.ReadBytes(3));
            if (FileVersion == "1.0")
                fontVersion = FontVersion.Version10;
            else if (FileVersion == "1.3")
            {
                fontVersion = FontVersion.Version13;
                headerSize = 71;
            }
            else if (FileVersion == "2.0")
                fontVersion = FontVersion.Version20;

            //read header bytes
            pos = br.BaseStream.Position;
            myHeaderBytes.AddRange(br.ReadBytes((int)fontVersion));
            br.BaseStream.Seek(pos, SeekOrigin.Begin); //reset pos

            if (fontVersion == FontVersion.Version13 || fontVersion == FontVersion.Version20)
                br.ReadByte(); //advance 1 byte for NUL terminator, except for Version 1.0 font
            if (fontVersion == FontVersion.Version20)
            {
                byte[] bHeaderSize = br.ReadBytes(4);
                headerSize = (uint)(bHeaderSize[0]+0xff*bHeaderSize[1]+0xff00*bHeaderSize[2]+0xff0000*bHeaderSize[3]);// Convert.ToUInt32((bHeaderSize); //read 4 bytes of header size (normally 96), only with version 2.0 font
            }

            ModuloFontName = br.ReadByte();
            FontNameShort = Encoding.ASCII.GetString(br.ReadBytes(5));
            if (fontVersion == FontVersion.Version13 || fontVersion == FontVersion.Version20)
                br.ReadByte(); //advance 1 byte for NUL terminator, except for Version 1.0 font

            FontID = br.ReadByte();                                     //offset 0x0d
            if (fontVersion == FontVersion.Version13 || fontVersion == FontVersion.Version20)
                br.ReadBytes(4); //ignore PICA, Elite and there italic font IDs

            AllwaysZero = br.ReadByte();
            if (AllwaysZero != 0)
                System.Diagnostics.Debug.WriteLine("not a font?, allways zero byte is=" + AllwaysZero.ToString());
            if (fontVersion == FontVersion.Version13 || fontVersion == FontVersion.Version20)
                displayCode = br.ReadByte();

            byte[] bUint16=null;
            bUint16 = br.ReadBytes(2);
            CharWidth = getUint16(bUint16);                             //offset 0x1F and 0x20

            if (fontVersion == FontVersion.Version13 || fontVersion == FontVersion.Version20)
                for(int i=0; i<4; i++)
                    br.ReadBytes(2); //ignore PICA, Elite and there italic font width

            bUint16 = br.ReadBytes(2);
            CharHeight = getUint16(bUint16);                            //offset 0x21 and 0x22

            if (fontVersion == FontVersion.Version10 || fontVersion == FontVersion.Version13)
                numBytesPerRow = br.ReadByte();                             //offset 0x23
            else if(fontVersion == FontVersion.Version20)
                numBytesPerRow = getUint16(br.ReadBytes(2));                // version 2.0 font uses two bytes

            numBytesPerChar = (getUint16(br.ReadBytes(2))); //offset 0x24 and 0x25
            codeStart = br.ReadByte();                                  //offset 0x26
            codeEnd = br.ReadByte();                                    //offset 0x27

            underLineRow = br.ReadByte(); //always 0 for version 1.0 font

            UserVersionNumber = br.ReadByte();                          //offset 0x29  USER version number
            
            if (fontVersion == FontVersion.Version10)
                UserDate = Encoding.ASCII.GetString(br.ReadBytes(8));       //offset 0x2A
            else if (fontVersion == FontVersion.Version13){
                UserDate = Encoding.ASCII.GetString(br.ReadBytes(8));       //offset 0x2A
                br.ReadByte();  //read nul terminator for version 1.3 and 2.0
            }
            else if (fontVersion == FontVersion.Version20)
            {
                UserDate = Encoding.ASCII.GetString(br.ReadBytes(10));
                br.ReadByte();  //read nul terminator for version 1.3 and 2.0
            }

            byte[] buf=null;
            if (fontVersion == FontVersion.Version10)
            {
                buf = br.ReadBytes(20);                             //offset 0x32 to 0x46
            }
            else if (fontVersion == FontVersion.Version13 || fontVersion == FontVersion.Version20)
            {
                buf = br.ReadBytes(21);                             //offset 0x32 to 0x46
            }
            FontNameLong = Encoding.ASCII.GetString(buf);
            FontNameLong = FontNameLong.Replace('\0', ' ');

            if (fontVersion == FontVersion.Version20)
                br.ReadBytes(15);   //read fill bytes 15 bytes with 0xff

            // ############## end of header ######################

            //read all header bytes again
            pos = br.BaseStream.Position; //mark last position
            //read header bytes
            for (int c = 0; c < headerSize; c++)
                myBytes.Add(br.ReadByte());
            br.BaseStream.Seek(pos, SeekOrigin.Begin);

            //store all bitmap bytes
            List<myBitmap.myBitmapChar> allCharBitmaps = new List<myBitmap.myBitmapChar>();
            
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
            //just a test
            byte[] bTest = allBitmaps.get(codeStart);
            System.Drawing.Bitmap bmp = allChars.getBitmap(1);

        }

        /// <summary>
        /// the FontVersion, holds also header byte length
        /// </summary>
        public enum FontVersion
        {
            Unknown=0,
            Version10=0x36,
            Version13=71,
            Version20=96
        }
        /// <summary>
        /// return uint16 for two bytes with LSB first
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public UInt16 getUint16(byte[] bytes)
        {
            UInt16 i16 = 0;
            i16 = (UInt16)(bytes[1] * 0xff + bytes[0]);
            return i16;
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

    }
/*
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

}
