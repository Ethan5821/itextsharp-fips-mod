using System;

using iTextSharp.Org.BouncyCastle.Asn1;
using iTextSharp.Org.BouncyCastle.Asn1.CryptoPro;
using iTextSharp.Org.BouncyCastle.Asn1.Nist;
using iTextSharp.Org.BouncyCastle.Asn1.Oiw;
using iTextSharp.Org.BouncyCastle.Asn1.Pkcs;
using iTextSharp.Org.BouncyCastle.Asn1.TeleTrust;
using iTextSharp.Org.BouncyCastle.Asn1.X509;
using iTextSharp.Org.BouncyCastle.Asn1.X9;
using iTextSharp.Org.BouncyCastle.Crypto;

namespace iTextSharp.Org.BouncyCastle.X509
{
	internal class X509SignatureUtilities
	{
		private static readonly Asn1Null derNull = DerNull.Instance;

		internal static void SetSignatureParameters(
			ISigner			signature,
			Asn1Encodable	parameters)
		{
			if (parameters != null && !derNull.Equals(parameters))
			{
				// TODO Put back in
//				AlgorithmParameters sigParams = AlgorithmParameters.GetInstance(signature.getAlgorithm());
//
//				try
//				{
//					sigParams.Init(parameters.ToAsn1Object().GetDerEncoded());
//				}
//				catch (IOException e)
//				{
//					throw new SignatureException("IOException decoding parameters: " + e.Message);
//				}
//
//				if (signature.getAlgorithm().EndsWith("MGF1"))
//				{
//					try
//					{
//						signature.setParameter(sigParams.getParameterSpec(PSSParameterSpec.class));
//					}
//					catch (GeneralSecurityException e)
//					{
//						throw new SignatureException("Exception extracting parameters: " + e.Message);
//					}
//				}
			}
		}

		internal static string GetSignatureName(
			AlgorithmIdentifier sigAlgId)
		{
			Asn1Encodable parameters = sigAlgId.Parameters;

			if (parameters != null && !derNull.Equals(parameters))
			{
				if (sigAlgId.ObjectID.Equals(PkcsObjectIdentifiers.IdRsassaPss))
				{
					RsassaPssParameters rsaParams = RsassaPssParameters.GetInstance(parameters);

					return GetDigestAlgName(rsaParams.HashAlgorithm.ObjectID) + "withRSAandMGF1";
				}
				if (sigAlgId.ObjectID.Equals(X9ObjectIdentifiers.ECDsaWithSha2))
				{
					Asn1Sequence ecDsaParams = Asn1Sequence.GetInstance(parameters);

					return GetDigestAlgName((DerObjectIdentifier)ecDsaParams[0]) + "withECDSA";
				}
			}

			return sigAlgId.ObjectID.Id;
		}

		/**
		 * Return the digest algorithm using one of the standard JCA string
		 * representations rather than the algorithm identifier (if possible).
		 */
		private static string GetDigestAlgName(
			DerObjectIdentifier digestAlgOID)
		{
			if (PkcsObjectIdentifiers.MD5.Equals(digestAlgOID))
			{
				return "MD5";
			}
			else if (OiwObjectIdentifiers.IdSha1.Equals(digestAlgOID))
			{
				return "SHA1";
			}
			else if (NistObjectIdentifiers.IdSha224.Equals(digestAlgOID))
			{
				return "SHA224";
			}
			else if (NistObjectIdentifiers.IdSha256.Equals(digestAlgOID))
			{
				return "SHA256";
			}
			else if (NistObjectIdentifiers.IdSha384.Equals(digestAlgOID))
			{
				return "SHA384";
			}
			else if (NistObjectIdentifiers.IdSha512.Equals(digestAlgOID))
			{
				return "SHA512";
			}
			else if (TeleTrusTObjectIdentifiers.RipeMD128.Equals(digestAlgOID))
			{
				return "RIPEMD128";
			}
			else if (TeleTrusTObjectIdentifiers.RipeMD160.Equals(digestAlgOID))
			{
				return "RIPEMD160";
			}
			else if (TeleTrusTObjectIdentifiers.RipeMD256.Equals(digestAlgOID))
			{
				return "RIPEMD256";
			}
			else if (CryptoProObjectIdentifiers.GostR3411.Equals(digestAlgOID))
			{
				return "GOST3411";
			}
			else
			{
				return digestAlgOID.Id;
			}
		}
	}
}
