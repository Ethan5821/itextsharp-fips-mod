using System;
using System.IO;

using iTextSharp.Org.BouncyCastle.Utilities.IO;

namespace iTextSharp.Org.BouncyCastle.Asn1
{
    class DefiniteLengthInputStream
        : LimitedInputStream
    {
		private static readonly byte[] EmptyBytes = new byte[0];

		private readonly int _originalLength;
		private int _remaining;

        internal DefiniteLengthInputStream(
            Stream	inStream,
            int		length)
            : base(inStream)
        {
			if (length < 0)
				throw new ArgumentException("negative lengths not allowed", "length");

			this._originalLength = length;
			this._remaining = length;

			if (length == 0)
			{
				SetParentEofDetect(true);
			}
        }

		public override int ReadByte()
        {
			if (_remaining == 0)
				return -1;

			int b = _in.ReadByte();

			if (b < 0)
				throw new EndOfStreamException("DEF length " + _originalLength + " object truncated by " + _remaining);

			if (--_remaining == 0)
			{
				SetParentEofDetect(true);
			}

			return b;
        }

		public override int Read(
			byte[]	buf,
			int		off,
			int		len)
		{
			if (_remaining == 0)
				return 0;

			int toRead = System.Math.Min(len, _remaining);
			int numRead = _in.Read(buf, off, toRead);

			if (numRead < 1)
				throw new EndOfStreamException("DEF length " + _originalLength + " object truncated by " + _remaining);

			if ((_remaining -= numRead) == 0)
			{
				SetParentEofDetect(true);
			}

			return numRead;
		}

		internal byte[] ToArray()
		{
			if (_remaining == 0)
				return EmptyBytes;

			byte[] bytes = new byte[_remaining];
			if ((_remaining -= Streams.ReadFully(_in, bytes)) != 0)
				throw new EndOfStreamException("DEF length " + _originalLength + " object truncated by " + _remaining);
			SetParentEofDetect(true);
			return bytes;
		}
    }
}
