using System.Text;

namespace CrazyPanda.UnityCore.Utils
{
    public static class Utf8Utils
    {
        public static bool IsUtf8FirstByte( byte @byte )
        {
            if( (@byte & 0b10000000) == 0b00000000 )
            {
                return true;
            }

            if( (@byte & 0b11100000) == 0b11000000 )
            {
                return true;
            }

            if( (@byte & 0b11110000) == 0b11100000 )
            {
                return true;
            }

            if( (@byte & 0b11111000) == 0b11110000 )
            {
                return true;
            }

            if( (@byte & 0b11111100) == 0b11111000 )
            {
                return true;
            }

            if( (@byte & 0b11111110) == 0b11111100 )
            {
                return true;
            }

            return false;
        }

        public static bool IsEnoughBytes( string str, int bytes )
        {
            // https: //learn.microsoft.com/en-us/dotnet/api/system.string
            // https://stackoverflow.com/questions/55056322/maximum-utf-8-string-size-given-utf-16-size
            if( bytes >= Encoding.UTF8.GetMaxByteCount(str.Length) )
            {
                return true;
            }

            if( bytes >= Encoding.UTF8.GetByteCount( str ) )
            {
                return true;
            }

            return false;
        }
    }
}