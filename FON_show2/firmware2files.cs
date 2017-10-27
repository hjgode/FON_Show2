using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace FON_show2
{
    class firmware2files
    {
        const string marker = "@(#)ROMD";
        byte[] bMarker = Encoding.UTF8.GetBytes(marker);
        string m_sFilename = "";

        public firmware2files(string sFilename)
        {
            m_sFilename = sFilename;
        }

        public void split(System.IO.DirectoryInfo directory)
        {
            string dirname = directory.FullName;
            FileStream streamReader = new FileStream(m_sFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader br = new BinaryReader(streamReader);
            long fLength = br.BaseStream.Length;
            byte[] bytes = br.ReadBytes((int)br.BaseStream.Length); //read all bytes
            br.Close();
            streamReader.Close();
            //foreach (byte b in bytes)
            //{
            //}
            List<byte[]> parts = Separate(bytes, bMarker);
            System.Diagnostics.Debug.WriteLine("====================START====================");
            foreach (byte[] buf in parts)
            {
                if(Encoding.ASCII.GetString(buf).IndexOf(".fnt")>-1){
                    byte[] buf2 = new byte[0x60];
                    Array.Copy(buf, buf2, 0x60);
                    System.Diagnostics.Debug.WriteLine(Hex.Dump(buf2));
                    System.Diagnostics.Debug.WriteLine("\nLength: "+ buf.Length.ToString() + "(0x" + buf.Length.ToString("x08") + ")");
                    //System.Diagnostics.Debug.WriteLine("====================END====================");
                    //Array.Copy(buf, buf.Length-0x60, buf2,0, 0x60);
                    //System.Diagnostics.Debug.WriteLine(Hex.Dump(buf2));
                    //length stored at offset 04
                    // 00 00 2A 51
                    long len=buf[4]*0xFFFF + buf[5]*0xFFFF + buf[6]*0xFF + buf[7];
                    
                    //name starts at offset 0x1c and ends at 0x3f or earlier?!, 27bytes
                    buf2=new byte[23];
//                    Array.Copy(buf,0x1c, buf2,0, 23);
                    int c = 0; List<byte> bString=new List<byte>();
                    while (buf[0x1c + c] != 0 && c<=23)
                    {
                        bString.Add(buf[0x1c + c]);
                        c++;
                    }
                    buf2 = bString.ToArray();
                    //followed by 'EPF' and the name
                    string name = Encoding.ASCII.GetString(buf2);//.Replace("\0", ""); //replace does not work with single \0
                    //name = name.Replace("\0", "");
                    
                    //at offset 0x36 starts the font file data with the size of the file (4 bytes, reversed storage)
                    //these are also the first 4 bytes of the fontheader!
                    long filesize = len;// -0x1B;//buf[0x39] * 0xFFFF + buf[0x38] * 0xFFFF + buf[0x37] * 0xFF + buf[0x36];

                    System.Diagnostics.Debug.WriteLine("\nFileSize: "+filesize.ToString()+"/0x"+filesize.ToString("x"));
                    System.Diagnostics.Debug.WriteLine("buf len: "+buf.Length.ToString()+"/0x"+buf.Length.ToString("x"));
                    System.Diagnostics.Debug.WriteLine("buf len+0x37: " + (buf.Length + 0x37).ToString() + "/0x" + (buf.Length + 0x37).ToString("x"));

                    if (!dirname.EndsWith("\\"))
                        dirname += "\\";
                    int n = 0;
                    //while (System.IO.File.Exists(dirname + name))
                    //    name = name + "(" + (++n).ToString() + ")";
                    streamReader = new FileStream(dirname + name, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                    BinaryWriter bw = new BinaryWriter(streamReader);
                    if (buf.Length < filesize - 0x37)
                        bw.Write(buf, 0x37, (int)filesize);
                    else
                    {
                        bw.Write(buf, 0x37, buf.Length - 0x37);
                        System.Diagnostics.Debug.WriteLine("error");
                    }
                    bw.Flush();
                    bw.Close();
                    streamReader.Close();
                    System.Diagnostics.Debug.WriteLine("==================START======================");
                }
            }
        }

        public List<byte[]> Separate(byte[] source, byte[] separator)
        {
            var Parts = new List<byte[]>();// parts is a list of byte arrays
            var Index = 0;
            byte[] Part;
            for (var I = 0; I < source.Length; ++I)
            {
                if (Equals(source, separator, I))
                {
                    Part = new byte[I - Index];
                    Array.Copy(source, Index, Part, 0, Part.Length);
                    Parts.Add(Part);
                    Index = I + separator.Length;
                    I += separator.Length - 1;
                }
            }
            //Part = new byte[source.Length - Index];
            //Array.Copy(source, Index, Part, 0, Part.Length);
            //Parts.Add(Part);
            return Parts;// Parts.ToArray();
        }
        bool Equals(byte[] source, byte[] separator, int index)
        {
            for (int i = 0; i < separator.Length; ++i)
                if (index + i >= source.Length || source[index + i] != separator[i])
                    return false;
            return true;
        }
    }
}
