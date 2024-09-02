using System.Text;

namespace CrazyPanda.UnityCore.Utils
{
    public static class UnicodeUtils
    {
        /// <summary>
        /// Validates that string does not exceed byte count limit when encoded to UTF-8
        /// </summary>
        /// <returns>
        /// Correct UTF-16 string that does not exceed maxSizeInBytes when encoded to UTF-8
        /// </returns>
        /// <remarks>
        /// Because this method guarantees validity of an output string
        /// it has to iterate over entire string to make sure that input is a valid UTF-16 string,
        /// if it is not, the output string will contain only valid part of the input string
        /// </remarks>
        public static unsafe string ValidateStringForUtf8( string str, uint maxSizeInBytes )
        {
            // if str.Length * 3 <= byteCountLimit early exit could be made
            //
            // The maximum factor of size in bytes increase for UTF-16 string when encoded in UTF-8 is 3 / 2,
            // str.Length is count of System.Char used to represent UTF-16 string,
            // So if size in bytes for C# string is str.Length * sizeof(System.Char) == str.Length * 2
            // Then size in bytes of the same string encoded in UTF-8 does not exceed
            // (3 / 2) * str.Length * 2 == 3 * str.Length
            //
            // check remarks to learn why this is not used

            fixed( char* first = str )
            {
                int   totalBytes   = 0;
                char* ptr          = first;
                char* last         = first + str.Length - 1;
                char* codePointPtr = stackalloc char[ 2 ];

                while( ptr <= last )
                {
                    int  sequenceLength;
                    char ch = *ptr;

                    if( char.IsHighSurrogate( ch ) )
                    {
                        // Unexpected end of string, but expected low surrogate,
                        // because current surrogate pair is not correct it wont be included
                        if( ptr == last )
                        {
                            return str.Substring( 0, ( int )(ptr - first) );
                        }

                        char high = ch;
                        char low  = *(ptr + 1);

                        // This surrogate pair is not a valid one, hence incorrect sequence encountered and we exit
                        if( !char.IsSurrogatePair( high, low ) )
                        {
                            return str.Substring( 0, ( int )(ptr - first) );
                        }

                        sequenceLength  = 2;
                        codePointPtr[0] = high;
                        codePointPtr[1] = low;
                    }
                    // This is invalid string, Surrogate Pair should start with High Surrogate
                    else if( char.IsLowSurrogate( ch ) )
                    {
                        return str.Substring( 0, ( int )(ptr - first) );
                    }
                    // Code Point is encoded with one Code Unit (1 char, 2 bytes)
                    else
                    {
                        sequenceLength = 1;
                        codePointPtr[0]   = ch;
                    }

                    totalBytes += Encoding.UTF8.GetByteCount( codePointPtr, sequenceLength );
                    if( totalBytes > maxSizeInBytes )
                    {
                        return str.Substring( 0, ( int )(ptr - first) );
                    }

                    ptr += sequenceLength;
                }
            }

            return str;
        }
    }
}
