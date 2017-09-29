using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FON_show2
{
    class fontheader
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

        public byte numBytesPerRow;                                      // 02 possibly num of bytes per row
        public UInt16 numBytesPerChar;

        public byte codeStart;                                  // 20   = first code point (here the Blank)
        public byte codeEnd;                                    // FF   = last code point (here 255)
        public byte bReserved3;                                 // 00
        public byte UserVersionNumber;                          // 33 (51)
        public string UserDate; //8 chars                       // 04/26/06
        public string FontNameLong;//20 chars                   // CP1250_H27_W10_M  (25 chars?)
    }

    //inherit class
    class fontheader10:fontheader
    {
    }
    class fontheader13:fontheader
    {
    }
    class fontheader20 : fontheader
    {
    }

}
