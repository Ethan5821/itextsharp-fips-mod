using System;

using iTextSharp.Org.BouncyCastle.Asn1.Pkcs;
using iTextSharp.Org.BouncyCastle.Asn1.X509;
using iTextSharp.Org.BouncyCastle.Crypto;
using iTextSharp.Org.BouncyCastle.Crypto.Generators;
using iTextSharp.Org.BouncyCastle.Crypto.Parameters;

namespace iTextSharp.Org.BouncyCastle.Cms
{
	/// <summary>
	/// PKCS5 scheme-2 - password converted to bytes assuming ASCII.
	/// </summary>
	public class Pkcs5Scheme2PbeKey
		: CmsPbeKey
	{
		public Pkcs5Scheme2PbeKey(
			string	password,
			byte[]	salt,
			int		iterationCount)
			: base(password, salt, iterationCount)
		{
		}

		public Pkcs5Scheme2PbeKey(
			string				password,
			AlgorithmIdentifier keyDerivationAlgorithm)
			: base(password, keyDerivationAlgorithm)
		{
		}

		internal override KeyParameter GetEncoded(
			string algorithmOid)
		{
			Pkcs5S2ParametersGenerator gen = new Pkcs5S2ParametersGenerator();

			gen.Init(
				PbeParametersGenerator.Pkcs5PasswordToBytes(this.Password),
				this.Salt,
				this.IterationCount);

			return (KeyParameter) gen.GenerateDerivedParameters(
				algorithmOid,
				CmsEnvelopedHelper.Instance.GetKeySize(algorithmOid));
		}
	}
}
