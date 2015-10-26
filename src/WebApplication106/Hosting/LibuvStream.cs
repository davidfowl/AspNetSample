using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Http
{
    public class LibuvStream : Stream
    {
        private byte[] _buffer = new byte[1024];
        private int _written;

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                return _written;
            }
        }

        public override long Position
        {
            get
            {
                return _written;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        public override void Flush()
        {

        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if ((_written + count) >= _buffer.Length)
            {
                Array.Resize(ref _buffer, _buffer.Length * 2);
            }

            Array.Copy(buffer, offset, _buffer, _written, count);
            _written += count;
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Write(buffer, offset, count);
            return null;
        }

        public ArraySegment<byte> GetBuffer()
        {
            return new ArraySegment<byte>(_buffer, 0, _written);
        }
    }
}
