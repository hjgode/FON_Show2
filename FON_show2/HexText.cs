using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FON_show2
{
    class Hex
    {
        private readonly byte[] _bytes;
        private readonly int _bytesPerLine;
        private readonly bool _showHeader;
        private readonly bool _showOffset;
        private readonly bool _showAscii;

        private readonly int _length;

        private int _index;
        private readonly StringBuilder _sb = new StringBuilder();

        private Hex(byte[] bytes, int bytesPerLine, bool showHeader, bool showOffset, bool showAscii)
        {
            _bytes = bytes;
            _bytesPerLine = bytesPerLine;
            _showHeader = showHeader;
            _showOffset = showOffset;
            _showAscii = showAscii;
            _length = bytes.Length;
        }

            public static string Dump(byte[] bytes){
                return Dump(bytes, 16,true,true,true);
            }
        public static string Dump(byte[] bytes, int bytesPerLine , bool showHeader , bool showOffset , bool showAscii )
        {
            if (bytes == null)
            {
                return "<null>";
            }
            return (new Hex(bytes, bytesPerLine, showHeader, showOffset, showAscii)).Dump();
        }

        private string Dump()
        {
            if (_showHeader)
            {
                WriteHeader();
            }
            WriteBody();
            return _sb.ToString();
        }

        private void WriteHeader()
        {
            if (_showOffset)
            {
                _sb.Append("Offset(h)   ");
            }
            for (int i = 0; i < _bytesPerLine; i++)
            {
                //_sb.Append($"{i & 0xFF:X2}");
                _sb.Append((i&0xff).ToString("X02")); 
                if (i + 1 < _bytesPerLine)
                {
                    _sb.Append(" ");
                    if (i==7)
                        _sb.Append("   ");
                }
            }
            _sb.AppendLine();
            _sb.AppendLine(new string('-',_sb.ToString().Length));
        }

        private void WriteBody()
        {
            while (_index < _length)
            {
                if (_index % _bytesPerLine == 0)
                {
                    if (_index > 0)
                    {
                        if (_showAscii)
                        {
                            WriteAscii();
                        }
                        _sb.AppendLine();
                    }

                    if (_showOffset)
                    {
                        WriteOffset();
                    }
                }

                WriteByte();
                if (_index % _bytesPerLine != 0 && _index < _length)
                {
                    _sb.Append(" ");
                    if(_index % 0x08==0)
                        _sb.Append("   ");
                }
            }

            //last line rest
            if (_showAscii)
            {
                _sb.Append("   ");
                WriteAscii();                
            }
        }

        private void WriteOffset()
        {
            //_sb.Append($"{_index:X8}   ");
            _sb.Append(_index.ToString("X08")+"    ");
        }

        private void WriteByte()
        {
            //_sb.Append($"{_bytes[_index]:X2}");
            _sb.Append(_bytes[_index].ToString("X02"));
            _index++;
        }

        private void WriteAscii()
        {
            int backtrack = ((_index-1)/_bytesPerLine)*_bytesPerLine;
            int length = _index - backtrack;

            // This is to fill up last string of the dump if it's shorter than _bytesPerLine
            _sb.Append(new string(' ', (_bytesPerLine - length) * 3));

            _sb.Append("   ");
            for (int i = 0; i < length; i++)
            {
                _sb.Append(Translate(_bytes[backtrack + i]));
                if(i==7)
                    _sb.Append("  ");
            }
        }

        private string Translate(byte b)
        {
            return b < 32 ? "." : Encoding.ASCII.GetString(new[] {b});
        }
    }
}
