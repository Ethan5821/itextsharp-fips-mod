using System;
using System.Collections;
using System.IO;
using System.Text;

using iTextSharp.Org.BouncyCastle.Asn1;
using iTextSharp.Org.BouncyCastle.Asn1.CryptoPro;
using iTextSharp.Org.BouncyCastle.Asn1.Nist;
using iTextSharp.Org.BouncyCastle.Asn1.Oiw;
using iTextSharp.Org.BouncyCastle.Asn1.Pkcs;
using iTextSharp.Org.BouncyCastle.Asn1.Sec;
using iTextSharp.Org.BouncyCastle.Asn1.TeleTrust;
using iTextSharp.Org.BouncyCastle.Asn1.X509;
using iTextSharp.Org.BouncyCastle.Asn1.X9;
using iTextSharp.Org.BouncyCastle.Crypto;
using iTextSharp.Org.BouncyCastle.Crypto.Parameters;
using iTextSharp.Org.BouncyCastle.Math;
using iTextSharp.Org.BouncyCastle.Math.EC;

namespace iTextSharp.Org.BouncyCastle.Security
{
    public sealed class PublicKeyFactory
    {
        private PublicKeyFactory()
        {
        }

		public static AsymmetricKeyParameter CreateKey(
			byte[] keyInfoData)
		{
			return CreateKey(
				SubjectPublicKeyInfo.GetInstance(
					Asn1Object.FromByteArray(keyInfoData)));
		}

		public static AsymmetricKeyParameter CreateKey(
			Stream inStr)
		{
			return CreateKey(
				SubjectPublicKeyInfo.GetInstance(
					Asn1Object.FromStream(inStr)));
		}

		public static AsymmetricKeyParameter CreateKey(
			SubjectPublicKeyInfo keyInfo)
        {
            AlgorithmIdentifier algID = keyInfo.AlgorithmID;
			DerObjectIdentifier algOid = algID.ObjectID;

			// TODO See RSAUtil.isRsaOid in Java build
			if (algOid.Equals(PkcsObjectIdentifiers.RsaEncryption)
				|| algOid.Equals(X509ObjectIdentifiers.IdEARsa)
				|| algOid.Equals(PkcsObjectIdentifiers.IdRsassaPss)
				|| algOid.Equals(PkcsObjectIdentifiers.IdRsaesOaep))
			{
				RsaPublicKeyStructure pubKey = RsaPublicKeyStructure.GetInstance(
					keyInfo.GetPublicKey());

				return new RsaKeyParameters(false, pubKey.Modulus, pubKey.PublicExponent);
			}
			else if (algOid.Equals(PkcsObjectIdentifiers.DhKeyAgreement)
				|| algOid.Equals(X9ObjectIdentifiers.DHPublicNumber))
			{
				DHParameter para = new DHParameter(
					Asn1Sequence.GetInstance(algID.Parameters.ToAsn1Object()));
				DerInteger derY = (DerInteger) keyInfo.GetPublicKey();

				BigInteger lVal = para.L;
				int l = lVal == null ? 0 : lVal.IntValue;
				DHParameters dhParams = new DHParameters(para.P, para.G, null, l);

				return new DHPublicKeyParameters(derY.Value, dhParams);
			}
			else if (algOid.Equals(OiwObjectIdentifiers.ElGamalAlgorithm))
			{
				ElGamalParameter para = new ElGamalParameter(
					Asn1Sequence.GetInstance(algID.Parameters.ToAsn1Object()));
				DerInteger derY = (DerInteger) keyInfo.GetPublicKey();

				return new ElGamalPublicKeyParameters(
					derY.Value,
					new ElGamalParameters(para.P, para.G));
			}
			else if (algOid.Equals(X9ObjectIdentifiers.IdDsa)
				|| algOid.Equals(OiwObjectIdentifiers.DsaWithSha1))
			{
				DerInteger derY = (DerInteger) keyInfo.GetPublicKey();
				Asn1Encodable ae = algID.Parameters;

				DsaParameters parameters = null;
				if (ae != null)
				{
					DsaParameter para = DsaParameter.GetInstance(ae.ToAsn1Object());
					parameters = new DsaParameters(para.P, para.Q, para.G);
				}

				return new DsaPublicKeyParameters(derY.Value, parameters);
			}
			else if (algOid.Equals(X9ObjectIdentifiers.IdECPublicKey))
			{
				X962Parameters para = new X962Parameters(
					algID.Parameters.ToAsn1Object());
				X9ECParameters ecP;

				if (para.IsNamedCurve)
				{
					// TODO ECGost3410NamedCurves support (returns ECDomainParameters though)

					DerObjectIdentifier oid = (DerObjectIdentifier)para.Parameters;
					ecP = X962NamedCurves.GetByOid(oid);

					if (ecP == null)
					{
						ecP = SecNamedCurves.GetByOid(oid);

						if (ecP == null)
						{
							ecP = NistNamedCurves.GetByOid(oid);

							if (ecP == null)
							{
								ecP = TeleTrusTNamedCurves.GetByOid(oid);
							}
						}
					}
				}
				else
				{
					ecP = new X9ECParameters((Asn1Sequence)para.Parameters);
				}

				ECDomainParameters dParams = new ECDomainParameters(
					ecP.Curve,
					ecP.G,
					ecP.N,
					ecP.H,
					ecP.GetSeed());

				DerBitString bits = keyInfo.PublicKeyData;
				byte[] data = bits.GetBytes();
				Asn1OctetString key = new DerOctetString(data);

				X9ECPoint derQ = new X9ECPoint(dParams.Curve, key);

				return new ECPublicKeyParameters(derQ.Point, dParams);
			}
			else if (algOid.Equals(CryptoProObjectIdentifiers.GostR3410x2001))
			{
				Gost3410PublicKeyAlgParameters gostParams = new Gost3410PublicKeyAlgParameters(
					(Asn1Sequence) algID.Parameters);

				Asn1OctetString key;
				try
				{
					key = (Asn1OctetString) keyInfo.GetPublicKey();
				}
				catch (IOException)
				{
					throw new ArgumentException("invalid info structure in GOST3410 public key");
				}

				byte[] keyEnc = key.GetOctets();
				byte[] x = new byte[32];
				byte[] y = new byte[32];

				for (int i = 0; i != y.Length; i++)
				{
					x[i] = keyEnc[32 - 1 - i];
				}

				for (int i = 0; i != x.Length; i++)
				{
					y[i] = keyEnc[64 - 1 - i];
				}

				ECDomainParameters ecP = ECGost3410NamedCurves.GetByOid(gostParams.PublicKeyParamSet);

				if (ecP == null)
					return null;

				ECPoint q = ecP.Curve.CreatePoint(new BigInteger(1, x), new BigInteger(1, y), false);

				return new ECPublicKeyParameters(q, gostParams.PublicKeyParamSet);
			}
			else if (algOid.Equals(CryptoProObjectIdentifiers.GostR3410x94))
			{
				Gost3410PublicKeyAlgParameters algParams = new Gost3410PublicKeyAlgParameters(
					(Asn1Sequence) algID.Parameters);

				DerOctetString derY;
				try
				{
					derY = (DerOctetString) keyInfo.GetPublicKey();
				}
				catch (IOException)
				{
					throw new ArgumentException("invalid info structure in GOST3410 public key");
				}

				byte[] keyEnc = derY.GetOctets();
				byte[] keyBytes = new byte[keyEnc.Length];

				for (int i = 0; i != keyEnc.Length; i++)
				{
					keyBytes[i] = keyEnc[keyEnc.Length - 1 - i]; // was little endian
				}

				BigInteger y = new BigInteger(1, keyBytes);

				return new Gost3410PublicKeyParameters(y, algParams.PublicKeyParamSet);
			}
            else
            {
                throw new SecurityUtilityException("algorithm identifier in key not recognised: " + algOid);
            }
        }
    }
}
