using System.Collections;

using iTextSharp.Org.BouncyCastle.Asn1.CryptoPro;
using iTextSharp.Org.BouncyCastle.Asn1.Nist;
using iTextSharp.Org.BouncyCastle.Asn1.Oiw;
using iTextSharp.Org.BouncyCastle.Asn1.Pkcs;
using iTextSharp.Org.BouncyCastle.Asn1.TeleTrust;

namespace iTextSharp.Org.BouncyCastle.Tsp
{
	/**
	 * Recognised hash algorithms for the time stamp protocol.
	 */
	public abstract class TspAlgorithms
	{
		public static readonly string MD5 = PkcsObjectIdentifiers.MD5.Id;

		public static readonly string Sha1 = OiwObjectIdentifiers.IdSha1.Id;

		public static readonly string Sha224 = NistObjectIdentifiers.IdSha224.Id;
		public static readonly string Sha256 = NistObjectIdentifiers.IdSha256.Id;
		public static readonly string Sha384 = NistObjectIdentifiers.IdSha384.Id;
		public static readonly string Sha512 = NistObjectIdentifiers.IdSha512.Id;

		public static readonly string RipeMD128 = TeleTrusTObjectIdentifiers.RipeMD128.Id;
		public static readonly string RipeMD160 = TeleTrusTObjectIdentifiers.RipeMD160.Id;
		public static readonly string RipeMD256 = TeleTrusTObjectIdentifiers.RipeMD256.Id;

		public static readonly string Gost3411 = CryptoProObjectIdentifiers.GostR3411.Id;

		public static readonly ArrayList Allowed;

		static TspAlgorithms()
		{
			string[] algs = new string[]
			{
				Gost3411, MD5, Sha1, Sha224, Sha256, Sha384, Sha512, RipeMD128, RipeMD160, RipeMD256
			};

			Allowed = new ArrayList();
			foreach (string alg in algs)
			{
				Allowed.Add(alg);
			}
		}
	}
}
