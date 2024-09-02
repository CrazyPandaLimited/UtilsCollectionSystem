using System;
using System.IO;

namespace CrazyPanda.UnityCore.Utils
{
    public class TailMemoryStream : Stream
    {
        private readonly int _maxLength;
        private int _length;
        private readonly byte[ ] _array;
        private int _pos;
        private int _totalBytesWritten;

        public TailMemoryStream( int maxLength )
        {
            _maxLength = maxLength;
            _length = 0;
            _array = new byte[ maxLength ];
            _pos = 0;
            _totalBytesWritten = 0;
        }

        public override void Write( byte[ ] buffer, int offset, int count )
        {
            _totalBytesWritten += count;

            if( _maxLength == 0 )
            {
                return;
            }

            _length = Math.Min( _maxLength, _length + count );
            
            int tailLength = _length - _pos;
            if( count <= tailLength )
            {
                Array.Copy( buffer, offset, _array, _pos, count );
                _pos += count;
            }
            else if (count >= _maxLength)
            {
                _pos = 0;
                Array.Copy( buffer, offset + count - _maxLength, _array, _pos, _maxLength );
            }
            else
            {
                int toTail = _maxLength - _pos;
                Array.Copy( buffer, offset, _array, _pos, toTail );
                Array.Copy( buffer, offset + toTail, _array, 0, count - toTail );
                _pos = count - toTail;
            }
            
            if( _pos >= _maxLength )
            {
                _pos = 0;
            }

            // int fromPos = offset;
            // for( int i = 0; i < bytesToWrite; i++ )
            // {
            //     byte value = buffer[ fromPos ];
            //     _array[ _pos ] = value;
            //     _pos++;
            //     if( _pos >= _maxLength )
            //     {
            //         _pos = 0;
            //     }
            //     fromPos++;
            // }
        }

        public override int Read( byte[ ] buffer, int offset, int count )
        {
            int bytesToRead = Math.Min( _length, count );
            
            if( _length < _maxLength )
            {
                Array.Copy( _array, 0, buffer, offset, _pos );
            }
            else
            {
                int tailLength = _length - _pos;
                Array.Copy( _array, _pos, buffer, offset, tailLength );
                Array.Copy( _array, 0, buffer, offset + tailLength, _pos );
            }
            
            // int myPos;
            // if( _length < _maxLength )
            // {
            //     myPos = 0;
            // }
            // else
            // {
            //     myPos = _pos;
            // }
            // for( int i = 0; i < bytesToRead; i++ )
            // {
            //     if( myPos == _maxLength )
            //     {
            //         myPos = 0;
            //     }
            //
            //     buffer[ targetPos ] = _array[ myPos ];
            //     targetPos++;
            //     myPos++;
            // }

            return bytesToRead;
        }

        public override void Flush()
        {
        }

        public override long Seek( long offset, SeekOrigin origin )
        {
            return 0;
        }

        public override void SetLength( long value )
        {
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => _length;

        public int TotalBytesWritten { get { return _totalBytesWritten; } }

        public int BytesSkipped { get { return _totalBytesWritten - _length; } }

        public override long Position { get { return 0; } set { } }
    }
}
