using System.IO;

using iTextSharp.Org.BouncyCastle.Utilities.IO;

namespace iTextSharp.Org.BouncyCastle.Asn1
{
    internal abstract class LimitedInputStream
        : BaseInputStream
    {
        protected readonly Stream _in;

        internal LimitedInputStream(
            Stream inStream)
        {
            this._in = inStream;
        }

		protected virtual void SetParentEofDetect(bool on)
        {
            if (_in is IndefiniteLengthInputStream)
            {
                ((IndefiniteLengthInputStream)_in).SetEofOn00(on);
            }
        }
    }
}
