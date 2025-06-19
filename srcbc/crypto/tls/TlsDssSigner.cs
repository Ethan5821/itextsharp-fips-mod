using System;

using iTextSharp.Org.BouncyCastle.Crypto.Digests;
using iTextSharp.Org.BouncyCastle.Crypto.Signers;

namespace iTextSharp.Org.BouncyCastle.Crypto.Tls
{
	internal class TlsDssSigner
		: DsaDigestSigner
	{
		internal TlsDssSigner()
        	: base(new DsaSigner(), new Sha1Digest())
		{
		}
	}
}
