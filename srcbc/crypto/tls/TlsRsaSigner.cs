using System;

using iTextSharp.Org.BouncyCastle.Crypto.Encodings;
using iTextSharp.Org.BouncyCastle.Crypto.Engines;
using iTextSharp.Org.BouncyCastle.Crypto.Signers;

namespace iTextSharp.Org.BouncyCastle.Crypto.Tls
{
	internal class TlsRsaSigner
		: GenericSigner
	{
		internal TlsRsaSigner()
        	: base(new Pkcs1Encoding(new RsaBlindedEngine()), new CombinedHash())
		{
		}
	}
}
