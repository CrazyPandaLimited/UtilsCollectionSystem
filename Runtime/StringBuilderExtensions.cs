using System;
using System.Text;

namespace CrazyPanda.UnityCore.Utils
{
    public static class StringBuilderExtensions
    {
        private static readonly char[ ] _twoCharDigits;
        private static readonly char[] _buffer = new char[ 1024 ];
        static StringBuilderExtensions()
        {
            _twoCharDigits = new char[200];
            for( int i = 0; i < 10; i++ )
            {
                for( int j = 0; j < 10; j++ )
                {
                    int index = 2 * ( i * 10 + j );
                    _twoCharDigits[ index ] = ( char ) ( i + 48 );
                    _twoCharDigits[ index + 1 ] = ( char ) ( j + 48 );
                }
            }
        }

        /// <summary>
        /// Threadsafe fast analog of Append(dateTime.toString("yyyy-MM-dd HH:mm:ss.fff"))
        /// 0 allocs, 15 times faster than Append(dateTime).
        /// Regular Append(dateTime.toString("yyyy-MM-dd HH:mm:ss.fff")) causes 5 allocs
        /// </summary>
        ///
        public static unsafe StringBuilder AppendDate( this StringBuilder stringBuilder, 
                                                       DateTime dateTime, 
                                                       bool showDate = true, 
                                                       bool showTime = true, 
                                                       bool showMilliseconds = true,
                                                       char dateDelimiter = '-',
                                                       char dateTimeDelimiter = ' ',
                                                       char timeDelimiter = ':',
                                                       char timeMillisecondsDelimiter = '.')
        {
            int pos = 0;
            char* buffer = stackalloc char[23];

            if( showDate )
            {
                int year;
                int month;
                int day;
                
                dateTime.GetDateParts( out year, out month, out day);

                int yearFirstPart2 = (year / 100) * 2;
                int yearSecondPart2 = (year % 100) * 2;
                buffer[ pos++ ] = _twoCharDigits[ yearFirstPart2 ];
                buffer[ pos++ ] = _twoCharDigits[ yearFirstPart2 + 1 ];
                buffer[ pos++ ] = _twoCharDigits[ yearSecondPart2 ];
                buffer[ pos++ ] = _twoCharDigits[ yearSecondPart2 + 1 ];

                buffer[ pos++ ] = dateDelimiter;

                int month2 = month * 2;
                buffer[ pos++ ] = _twoCharDigits[ month2 ];
                buffer[ pos++ ] = _twoCharDigits[ month2 + 1 ];
                
                buffer[ pos++ ] = dateDelimiter;

                int day2 = day * 2;
                buffer[ pos++ ] = _twoCharDigits[ day2 ];
                buffer[ pos++ ] = _twoCharDigits[ day2 + 1 ];
            
                if( showTime || showMilliseconds )
                {
                    buffer[ pos++ ] = dateTimeDelimiter;
                }
            }
            
            if( showTime )
            {
                int hour2 = dateTime.Hour * 2;
                buffer[ pos++ ] = _twoCharDigits[ hour2 ];
                buffer[ pos++ ] = _twoCharDigits[ hour2 + 1 ];
                
                buffer[ pos++ ] = timeDelimiter;

                int minute2 = dateTime.Minute * 2;
                buffer[ pos++ ] = _twoCharDigits[ minute2 ];
                buffer[ pos++ ] = _twoCharDigits[ minute2 + 1 ];
                
                buffer[ pos++ ] = timeDelimiter;

                int second2 = dateTime.Second * 2;
                buffer[ pos++ ] = _twoCharDigits[ second2 ];
                buffer[ pos++ ] = _twoCharDigits[ second2 + 1 ];
            
                if( showMilliseconds )
                {
                    buffer[ pos++ ] = timeMillisecondsDelimiter;
                }
            }
            
            if( showMilliseconds )
            {
                int ms = dateTime.Millisecond;
                buffer[ pos++ ] = ( char ) ( ms / 100 + 48 );
                int msSecondPart2 = ( ms % 100 ) * 2;
                buffer[ pos++ ] = _twoCharDigits[ msSecondPart2 ];
                buffer[ pos++ ] = _twoCharDigits[ msSecondPart2 + 1 ];
            }
            
            stringBuilder.Append( buffer, pos );

            return stringBuilder;
        }

        public static unsafe StringBuilder AppendInt( this StringBuilder stringBuilder, int value )
        {
            if( value == int.MinValue ) // because only -int.MinValue is not valid int value
            {
                stringBuilder.Append( "-2147483648" );
                return stringBuilder;
            }

            if( value == 0 )
            {
                stringBuilder.Append( '0' );
                return stringBuilder;
            }
            
            bool isNegative = false;
            if ( value < 0 )
            {
                isNegative = true;
                value = -value;
            }
            
            int bufferLength = 11;
            int pos = bufferLength;
            char* buffer = stackalloc char[bufferLength];

            while( value >= 10 )
            {
                int twoCharPos = (value % 100) * 2;
                buffer[ --pos ] = _twoCharDigits[ twoCharPos + 1 ];
                buffer[ --pos ] = _twoCharDigits[ twoCharPos];
                value /= 100;
            }

            if( value > 0 )
            {
                buffer[ --pos ] = ( char ) ( value % 10 + 48 );
            }

            if( isNegative )
            {
                buffer[ --pos ] = '-';
            }

            stringBuilder.Append(buffer + pos, bufferLength - pos);

            return stringBuilder;
        }
        
        public static unsafe StringBuilder AppendUInt( this StringBuilder stringBuilder, uint value )
        {
            if( value == 0 )
            {
                stringBuilder.Append( '0' );
                return stringBuilder;
            }
            
            int bufferLength = 10;
            int pos = bufferLength;
            char* buffer = stackalloc char[bufferLength];

            while( value >= 10 )
            {
                uint twoCharPos = (value % 100) * 2;
                buffer[ --pos ] = _twoCharDigits[ twoCharPos + 1 ];
                buffer[ --pos ] = _twoCharDigits[ twoCharPos];
                value /= 100;
            }

            if( value > 0 )
            {
                buffer[ --pos ] = ( char ) ( value % 10 + 48 );
            }

            stringBuilder.Append(buffer + pos, bufferLength - pos);

            return stringBuilder;
        }
        
        public static unsafe StringBuilder AppendLong( this StringBuilder stringBuilder, long value )
        {
            if( value == long.MinValue ) // because only -long.MinValue is not valid long value
            {
                stringBuilder.Append( "-9223372036854775808" );
                return stringBuilder;
            }

            if( value == 0 )
            {
                stringBuilder.Append( '0' );
                return stringBuilder;
            }
            
            bool isNegative = false;
            if ( value < 0 )
            {
                isNegative = true;
                value = -value;
            }
            
            int bufferLength = 20;
            int pos = bufferLength;
            char* buffer = stackalloc char[bufferLength];

            while( value >= 10 )
            {
                int twoCharPos = (int)(value % 100) * 2;
                buffer[ --pos ] = _twoCharDigits[ twoCharPos + 1 ];
                buffer[ --pos ] = _twoCharDigits[ twoCharPos];
                value /= 100;
            }

            if( value > 0 )
            {
                buffer[ --pos ] = ( char ) ( value % 10 + 48 );
            }

            if( isNegative )
            {
                buffer[ --pos ] = '-';
            }

            stringBuilder.Append(buffer + pos, bufferLength - pos);

            return stringBuilder;
        }
        
        public static unsafe StringBuilder AppendULong( this StringBuilder stringBuilder, ulong value )
        {
            if( value == 0 )
            {
                stringBuilder.Append( '0' );
                return stringBuilder;
            }

            int bufferLength = 20;
            int pos = bufferLength;
            char* buffer = stackalloc char[bufferLength];

            while( value >= 10 )
            {
                int twoCharPos = (int)(value % 100) * 2;
                // long twoCharPos = (value % 100) * 2;
                buffer[ --pos ] = _twoCharDigits[ twoCharPos + 1 ];
                buffer[ --pos ] = _twoCharDigits[ twoCharPos];
                value /= 100;
            }

            if( value > 0 )
            {
                buffer[ --pos ] = ( char ) ( value % 10 + 48 );
            }

            stringBuilder.Append(buffer + pos, bufferLength - pos);

            return stringBuilder;
        }

        public static void AppendUtf8Bytes( this StringBuilder sb, byte[] bytes )
        {
            AppendUtf8BytesInternal( sb, bytes, bufferSize:256 );
        }

        internal static void AppendUtf8BytesInternal( this StringBuilder sb, byte[] bytes, int bufferSize )
        {
            AppendUtf8BytesWithBufferSizeUnsafeInternal( sb, bytes, bufferSize );
        }
        
        internal static void AppendUtf8BytesWithoutBufferInternal( this StringBuilder sb, byte[] bytes )
        {
            var encoding = Encoding.UTF8;
            var result = new char[ encoding.GetCharCount( bytes ) ];
            encoding.GetChars( bytes, 0, bytes.Length, result, 0 );
            sb.Append( result );
        }

        internal static void AppendUtf8BytesWithBufferSizeInternal( this StringBuilder sb, byte[] bytes, int bufferSize )
        {
            const int minimumBufferSize = 4;

            if( bufferSize < minimumBufferSize )
            {
                bufferSize = minimumBufferSize;
            }

            var encoding = Encoding.UTF8;
            var buffer = new char[ bufferSize ];
            int currentIndex = 0;

            while( currentIndex < bytes.Length )
            {
                int byteCount = 0;
                var endPosition = currentIndex + bufferSize;
              
                if( endPosition >= bytes.Length )
                {
                    byteCount = bytes.Length - currentIndex;
                }
                else
                {
                    for( int i = endPosition; i >= currentIndex; i-- )
                    {
                        if( Utf8Utils.IsUtf8FirstByte( bytes[ i ] ) )
                        {
                            byteCount = i - currentIndex;
                            break;
                        }
                    }
                }

                var charsReaded = encoding.GetChars( bytes, currentIndex, byteCount, buffer, 0 );

                for( int i = 0; i < charsReaded; i++ )
                {
                    sb.Append( buffer[ i ] );
                }

                currentIndex += byteCount;
            }
        }

        internal static unsafe void AppendUtf8BytesWithBufferSizeUnsafeInternal( this StringBuilder sb, byte[] bytes, int bufferSize )
        {
            const int minimumBufferSize = 4;

            if( bufferSize < minimumBufferSize )
            {
                bufferSize = minimumBufferSize;
            }

            var encoding = Encoding.UTF8;
            char* buffer = stackalloc char[ bufferSize ];
            int currentIndex = 0;

            fixed( byte* fixedBytes = bytes )
            {
                byte* bytesArray = fixedBytes;
                
                while( currentIndex < bytes.Length )
                {
                    int byteCount = 0;
                    var endPosition = currentIndex + bufferSize;

                    if( endPosition >= bytes.Length )
                    {
                        byteCount = bytes.Length - currentIndex;
                    }
                    else
                    {
                        for( int i = endPosition; i >= currentIndex; i-- )
                        {
                            if( Utf8Utils.IsUtf8FirstByte( bytes[ i ] ) )
                            {
                                byteCount = i - currentIndex;
                                break;
                            }
                        }
                    }

                    var charsReaded = encoding.GetChars( bytesArray, byteCount, buffer, byteCount );

                    for( int i = 0; i < charsReaded; i++ )
                    {
                        sb.Append( buffer[ i ] );
                    }

                    currentIndex += byteCount;
                    bytesArray += byteCount;
                }
            }
        }
        internal static void AppendBytes( this StringBuilder builder, byte[] bytes )
        {
            Encoding encoding = Encoding.UTF8;
            int currentIndex = 0;

            while( currentIndex < bytes.Length )
            {
                int byteCount = Math.Min( _buffer.Length, bytes.Length - currentIndex );
                int charsReader = encoding.GetChars( bytes, currentIndex, byteCount, _buffer, 0 );
                currentIndex += byteCount;
        
                for( int i = 0; i < charsReader; i++ )
                {
                    builder.Append( _buffer[ i ] );
                }
            }
        }
    }
}