using iTextSharp.Org.BouncyCastle.Asn1;
using iTextSharp.Org.BouncyCastle.Asn1.X509;

namespace iTextSharp.Org.BouncyCastle.Asn1.Pkcs
{
    public class KeyDerivationFunc
        : AlgorithmIdentifier
    {
        internal KeyDerivationFunc(Asn1Sequence seq)
			: base(seq)
        {
        }

		internal KeyDerivationFunc(
			DerObjectIdentifier	id,
			Asn1Encodable		parameters)
			: base(id, parameters)
        {
        }
    }
}
