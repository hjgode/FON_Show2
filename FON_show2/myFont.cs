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
        
        */

        public byte[] FileMarker=new byte[4]; //4 bytes                  // 00 00 00 00 | FF FF FF FF=fixed width
                                                                         // 02 6A 00 00 = prop
                                                                         // 7C 6A 00 00 = prop
        // for prop fonts, first byte of byte array is width of character, for example
        // 0x0A 
        public string FileVersion;//3 chars                              // 1.0
        public byte bReserved1;   //1 byte                               // 0x18, possibly baseline?

        public string FontNameShort;//5 chars                            // CC020
        public byte FontID;//1 byte                                      // c
        public byte bReserved1b;                                         // 00
        public byte CharWidth;                                           // 0A possibly the width = 10
        // for prop fonts this is 0xff (255) and the width is stored in front of every byte block as two-byte (low first)

        public byte bReserved1c;                                         // 00
        public byte CharHeight;                                          // 1B possibly the height = 27 pixels
        public byte bReserved1d;                                         // 00 
        public byte numBytesPerRow;                                      // 02 possibly num of bytes per row
        public byte unknown1;                                        // 36 
        public byte bReserved2;                                          // 00
                                                                         // ?36 baseline = (h=27->b=22 for v1.3 font)
                                                                         
        public byte codeStart;                                  // 20   = first code point (here the Blank)
        public byte codeEnd;                                    // FF   = last code point (here 255)
        public byte bReserved3;                                 // 00
        public byte bReserved4;                                 // 33 (51)
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
            readHeader();
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
                bReserved1 = br.ReadByte();
                //offset 0x08
                FontNameShort = Encoding.ASCII.GetString(br.ReadBytes(5));
                FontID = br.ReadByte();                                     //offset 0x0d

                bReserved1b = br.ReadByte();
                CharWidth = br.ReadByte();                                  //offset 0x1F
                
                bReserved1c = br.ReadByte();
                CharHeight = br.ReadByte();                                 //offset 0x11
                
                bReserved1d = br.ReadByte();

                numBytesPerRow = br.ReadByte();                             //offset 0x13
                unknown1 = br.ReadByte();
                
                bReserved2 = br.ReadByte();
                codeStart = br.ReadByte();
                codeEnd = br.ReadByte();
                
                bReserved3 = br.ReadByte();
                bReserved4 = br.ReadByte();
                
                UserDate = Encoding.ASCII.GetString(br.ReadBytes(8));       //offset 0x1A
                byte[] buf = br.ReadBytes(20);                              //offset 0x22
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
                    if(thisCharWidth==0xFF){ //is proportional font?
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
            StringBuilder sb = new StringBuilder();
            sb.Append("FileVersion=" + this.FileVersion + "\r\n");
            sb.Append("Fontname short=" + this.FontNameShort + "\r\n");
            sb.Append("UserDate=" + this.UserDate + "\r\n");
            sb.Append("Fontname long='" + this.FontNameLong + "'\r\n");
            sb.Append("Font ID=0x" + this.FontID.ToString("x02") + "(" + Encoding.ASCII.GetString(new byte[] { this.FontID }, 0, 1) + ")\r\n");
            sb.Append("code start=" + this.codeStart.ToString() + "\r\n");
            sb.Append("code end=" + this.codeEnd.ToString() + "\r\n");
            sb.Append("Char width (bits)=" + this.CharWidth.ToString() + " ");
            if (CharWidth == 0xFF)
                sb.AppendLine("(prop.)");
            else
                sb.AppendLine("(fixed)");
            sb.Append("Char height (bytes)=" + this.CharHeight.ToString() + "\r\n");
            sb.Append("num bytes/row=" + this.numBytesPerRow.ToString() + "\r\n");
            System.Diagnostics.Debug.WriteLine(sb.ToString());
            return sb.ToString();
        }

        //byte array have to be aligned at multiples of 4 (DOWRD alignment)


        [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi,Pack=1)]
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
