using System;
using System.Collections;
using System.IO;

using iTextSharp.Org.BouncyCastle.Asn1;
using iTextSharp.Org.BouncyCastle.Asn1.Cms;
using iTextSharp.Org.BouncyCastle.Asn1.Kisa;
using iTextSharp.Org.BouncyCastle.Asn1.Nist;
using iTextSharp.Org.BouncyCastle.Asn1.Ntt;
using iTextSharp.Org.BouncyCastle.Asn1.Pkcs;
using iTextSharp.Org.BouncyCastle.Asn1.X509;
using iTextSharp.Org.BouncyCastle.Asn1.X9;
using iTextSharp.Org.BouncyCastle.Crypto;
using iTextSharp.Org.BouncyCastle.Crypto.Parameters;
using iTextSharp.Org.BouncyCastle.Math;
using iTextSharp.Org.BouncyCastle.Security;
using iTextSharp.Org.BouncyCastle.X509;

namespace iTextSharp.Org.BouncyCastle.Cms
{
	/**
	* General class for generating a CMS enveloped-data message.
	*
	* A simple example of usage.
	*
	* <pre>
	*      CMSEnvelopedDataGenerator  fact = new CMSEnvelopedDataGenerator();
	*
	*      fact.addKeyTransRecipient(cert);
	*
	*      CMSEnvelopedData         data = fact.generate(content, algorithm, "BC");
	* </pre>
	*/
	public class CmsEnvelopedGenerator
	{
		internal static readonly short[] rc2Table =
		{
			0xbd, 0x56, 0xea, 0xf2, 0xa2, 0xf1, 0xac, 0x2a, 0xb0, 0x93, 0xd1, 0x9c, 0x1b, 0x33, 0xfd, 0xd0,
			0x30, 0x04, 0xb6, 0xdc, 0x7d, 0xdf, 0x32, 0x4b, 0xf7, 0xcb, 0x45, 0x9b, 0x31, 0xbb, 0x21, 0x5a,
			0x41, 0x9f, 0xe1, 0xd9, 0x4a, 0x4d, 0x9e, 0xda, 0xa0, 0x68, 0x2c, 0xc3, 0x27, 0x5f, 0x80, 0x36,
			0x3e, 0xee, 0xfb, 0x95, 0x1a, 0xfe, 0xce, 0xa8, 0x34, 0xa9, 0x13, 0xf0, 0xa6, 0x3f, 0xd8, 0x0c,
			0x78, 0x24, 0xaf, 0x23, 0x52, 0xc1, 0x67, 0x17, 0xf5, 0x66, 0x90, 0xe7, 0xe8, 0x07, 0xb8, 0x60,
			0x48, 0xe6, 0x1e, 0x53, 0xf3, 0x92, 0xa4, 0x72, 0x8c, 0x08, 0x15, 0x6e, 0x86, 0x00, 0x84, 0xfa,
			0xf4, 0x7f, 0x8a, 0x42, 0x19, 0xf6, 0xdb, 0xcd, 0x14, 0x8d, 0x50, 0x12, 0xba, 0x3c, 0x06, 0x4e,
			0xec, 0xb3, 0x35, 0x11, 0xa1, 0x88, 0x8e, 0x2b, 0x94, 0x99, 0xb7, 0x71, 0x74, 0xd3, 0xe4, 0xbf,
			0x3a, 0xde, 0x96, 0x0e, 0xbc, 0x0a, 0xed, 0x77, 0xfc, 0x37, 0x6b, 0x03, 0x79, 0x89, 0x62, 0xc6,
			0xd7, 0xc0, 0xd2, 0x7c, 0x6a, 0x8b, 0x22, 0xa3, 0x5b, 0x05, 0x5d, 0x02, 0x75, 0xd5, 0x61, 0xe3,
			0x18, 0x8f, 0x55, 0x51, 0xad, 0x1f, 0x0b, 0x5e, 0x85, 0xe5, 0xc2, 0x57, 0x63, 0xca, 0x3d, 0x6c,
			0xb4, 0xc5, 0xcc, 0x70, 0xb2, 0x91, 0x59, 0x0d, 0x47, 0x20, 0xc8, 0x4f, 0x58, 0xe0, 0x01, 0xe2,
			0x16, 0x38, 0xc4, 0x6f, 0x3b, 0x0f, 0x65, 0x46, 0xbe, 0x7e, 0x2d, 0x7b, 0x82, 0xf9, 0x40, 0xb5,
			0x1d, 0x73, 0xf8, 0xeb, 0x26, 0xc7, 0x87, 0x97, 0x25, 0x54, 0xb1, 0x28, 0xaa, 0x98, 0x9d, 0xa5,
			0x64, 0x6d, 0x7a, 0xd4, 0x10, 0x81, 0x44, 0xef, 0x49, 0xd6, 0xae, 0x2e, 0xdd, 0x76, 0x5c, 0x2f,
			0xa7, 0x1c, 0xc9, 0x09, 0x69, 0x9a, 0x83, 0xcf, 0x29, 0x39, 0xb9, 0xe9, 0x4c, 0xff, 0x43, 0xab
		};

		internal static readonly short[] rc2Ekb =
		{
			0x5d, 0xbe, 0x9b, 0x8b, 0x11, 0x99, 0x6e, 0x4d, 0x59, 0xf3, 0x85, 0xa6, 0x3f, 0xb7, 0x83, 0xc5,
			0xe4, 0x73, 0x6b, 0x3a, 0x68, 0x5a, 0xc0, 0x47, 0xa0, 0x64, 0x34, 0x0c, 0xf1, 0xd0, 0x52, 0xa5,
			0xb9, 0x1e, 0x96, 0x43, 0x41, 0xd8, 0xd4, 0x2c, 0xdb, 0xf8, 0x07, 0x77, 0x2a, 0xca, 0xeb, 0xef,
			0x10, 0x1c, 0x16, 0x0d, 0x38, 0x72, 0x2f, 0x89, 0xc1, 0xf9, 0x80, 0xc4, 0x6d, 0xae, 0x30, 0x3d,
			0xce, 0x20, 0x63, 0xfe, 0xe6, 0x1a, 0xc7, 0xb8, 0x50, 0xe8, 0x24, 0x17, 0xfc, 0x25, 0x6f, 0xbb,
			0x6a, 0xa3, 0x44, 0x53, 0xd9, 0xa2, 0x01, 0xab, 0xbc, 0xb6, 0x1f, 0x98, 0xee, 0x9a, 0xa7, 0x2d,
			0x4f, 0x9e, 0x8e, 0xac, 0xe0, 0xc6, 0x49, 0x46, 0x29, 0xf4, 0x94, 0x8a, 0xaf, 0xe1, 0x5b, 0xc3,
			0xb3, 0x7b, 0x57, 0xd1, 0x7c, 0x9c, 0xed, 0x87, 0x40, 0x8c, 0xe2, 0xcb, 0x93, 0x14, 0xc9, 0x61,
			0x2e, 0xe5, 0xcc, 0xf6, 0x5e, 0xa8, 0x5c, 0xd6, 0x75, 0x8d, 0x62, 0x95, 0x58, 0x69, 0x76, 0xa1,
			0x4a, 0xb5, 0x55, 0x09, 0x78, 0x33, 0x82, 0xd7, 0xdd, 0x79, 0xf5, 0x1b, 0x0b, 0xde, 0x26, 0x21,
			0x28, 0x74, 0x04, 0x97, 0x56, 0xdf, 0x3c, 0xf0, 0x37, 0x39, 0xdc, 0xff, 0x06, 0xa4, 0xea, 0x42,
			0x08, 0xda, 0xb4, 0x71, 0xb0, 0xcf, 0x12, 0x7a, 0x4e, 0xfa, 0x6c, 0x1d, 0x84, 0x00, 0xc8, 0x7f,
			0x91, 0x45, 0xaa, 0x2b, 0xc2, 0xb1, 0x8f, 0xd5, 0xba, 0xf2, 0xad, 0x19, 0xb2, 0x67, 0x36, 0xf7,
			0x0f, 0x0a, 0x92, 0x7d, 0xe3, 0x9d, 0xe9, 0x90, 0x3e, 0x23, 0x27, 0x66, 0x13, 0xec, 0x81, 0x15,
			0xbd, 0x22, 0xbf, 0x9f, 0x7e, 0xa9, 0x51, 0x4b, 0x4c, 0xfb, 0x02, 0xd3, 0x70, 0x86, 0x31, 0xe7,
			0x3b, 0x05, 0x03, 0x54, 0x60, 0x48, 0x65, 0x18, 0xd2, 0xcd, 0x5f, 0x32, 0x88, 0x0e, 0x35, 0xfd
		};


		// TODO Create named constants for all of these
		public static readonly string DesEde3Cbc		= PkcsObjectIdentifiers.DesEde3Cbc.Id;
		public static readonly string RC2Cbc			= PkcsObjectIdentifiers.RC2Cbc.Id;
		public const string IdeaCbc						= "1.3.6.1.4.1.188.7.1.1.2";
		public const string Cast5Cbc					= "1.2.840.113533.7.66.10";
		public static readonly string Aes128Cbc			= NistObjectIdentifiers.IdAes128Cbc.Id;
		public static readonly string Aes192Cbc			= NistObjectIdentifiers.IdAes192Cbc.Id;
		public static readonly string Aes256Cbc			= NistObjectIdentifiers.IdAes256Cbc.Id;
		public static readonly string Camellia128Cbc	= NttObjectIdentifiers.IdCamellia128Cbc.Id;
		public static readonly string Camellia192Cbc	= NttObjectIdentifiers.IdCamellia192Cbc.Id;
		public static readonly string Camellia256Cbc	= NttObjectIdentifiers.IdCamellia256Cbc.Id;
		public static readonly string SeedCbc			= KisaObjectIdentifiers.IdSeedCbc.Id;

		public static readonly string DesEde3Wrap		= PkcsObjectIdentifiers.IdAlgCms3DesWrap.Id;
		public static readonly string Aes128Wrap		= NistObjectIdentifiers.IdAes128Wrap.Id;
		public static readonly string Aes192Wrap		= NistObjectIdentifiers.IdAes192Wrap.Id;
		public static readonly string Aes256Wrap		= NistObjectIdentifiers.IdAes256Wrap.Id;
		public static readonly string Camellia128Wrap	= NttObjectIdentifiers.IdCamellia128Wrap.Id;
		public static readonly string Camellia192Wrap	= NttObjectIdentifiers.IdCamellia192Wrap.Id;
		public static readonly string Camellia256Wrap	= NttObjectIdentifiers.IdCamellia256Wrap.Id;
		public static readonly string SeedWrap			= KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap.Id;

		public static readonly string ECDHSha1Kdf		= X9ObjectIdentifiers.DHSinglePassStdDHSha1KdfScheme.Id;

		internal static readonly CmsEnvelopedHelper Helper = CmsEnvelopedHelper.Instance;

		internal readonly IList recipientInfs = new ArrayList();
		internal readonly SecureRandom rand;

		protected class RecipientInf
		{
			private readonly X509Certificate		cert;
			private AlgorithmIdentifier	keyEncAlg;
			private readonly AsymmetricKeyParameter	pubKey;
			private readonly Asn1OctetString		subKeyId;

			private readonly string			secKeyAlgorithm;
			private readonly KeyParameter	secKey;
			private readonly KekIdentifier	secKeyId;

			private readonly OriginatorIdentifierOrKey	originator;
			private const Asn1OctetString ukm = null;

			private readonly AlgorithmIdentifier derivationAlg;

			internal RecipientInf(
				X509Certificate cert)
			{
				this.cert = cert;
				this.pubKey = cert.GetPublicKey();

				try
				{
					TbsCertificateStructure tbs = TbsCertificateStructure.GetInstance(
						Asn1Object.FromByteArray(cert.GetTbsCertificate()));

					keyEncAlg = tbs.SubjectPublicKeyInfo.AlgorithmID;
				}
//				catch (IOException e)
				catch (Exception)
				{
					throw new ArgumentException("can't extract key algorithm from this cert");
				}
//				catch (CertificateEncodingException)
//				{
//					throw new ArgumentException("can't extract tbs structure from this cert");
//				}
			}

			internal RecipientInf(
				AsymmetricKeyParameter	pubKey,
				Asn1OctetString			subKeyId)
			{
				this.pubKey = pubKey;
				this.subKeyId = subKeyId;

				try
				{
					SubjectPublicKeyInfo info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pubKey);

					keyEncAlg = info.AlgorithmID;
				}
				catch (IOException)
				{
					throw new ArgumentException("can't extract key algorithm from this key");
				}
			}

			internal RecipientInf(
				string			secKeyAlgorithm, // TODO Can get this from secKey?
				KeyParameter	secKey,
				KekIdentifier	secKeyId)
			{
				this.secKeyAlgorithm = secKeyAlgorithm;
				this.secKey = secKey;
				this.secKeyId = secKeyId;

				if (secKeyAlgorithm.StartsWith("DES"))
				{
					keyEncAlg = new AlgorithmIdentifier(
						PkcsObjectIdentifiers.IdAlgCms3DesWrap,
						DerNull.Instance);
				}
				else if (secKeyAlgorithm.StartsWith("RC2"))
				{
					keyEncAlg = new AlgorithmIdentifier(
						PkcsObjectIdentifiers.IdAlgCmsRC2Wrap,
						new DerInteger(58));
				}
				else if (secKeyAlgorithm.StartsWith("AES"))
				{
					int length = secKey.GetKey().Length * 8;
					DerObjectIdentifier wrapOid;

					if (length == 128)
					{
						wrapOid = NistObjectIdentifiers.IdAes128Wrap;
					}
					else if (length == 192)
					{
						wrapOid = NistObjectIdentifiers.IdAes192Wrap;
					}
					else if (length == 256)
					{
						wrapOid = NistObjectIdentifiers.IdAes256Wrap;
					}
					else
					{
						throw new ArgumentException("illegal keysize in AES");
					}

					keyEncAlg = new AlgorithmIdentifier(wrapOid);  // parameters absent
				}
				else if (secKeyAlgorithm.StartsWith("SEED"))
				{
					// parameters absent
					keyEncAlg = new AlgorithmIdentifier(KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap);
				}
				else if (secKeyAlgorithm.StartsWith("CAMELLIA"))
				{
					int length = secKey.GetKey().Length * 8;
					DerObjectIdentifier wrapOid;

					if (length == 128)
					{
						wrapOid = NttObjectIdentifiers.IdCamellia128Wrap;
					}
					else if (length == 192)
					{
						wrapOid = NttObjectIdentifiers.IdCamellia192Wrap;
					}
					else if (length == 256)
					{
						wrapOid = NttObjectIdentifiers.IdCamellia256Wrap;
					}
					else
					{
						throw new ArgumentException("illegal keysize in Camellia");
					}

					keyEncAlg = new AlgorithmIdentifier(wrapOid); // parameters must be absent
				}
				else
				{
					throw new ArgumentException("unknown algorithm");
				}
			}

			public RecipientInf(
				string						secKeyAlgorithm, // TODO Can get this from secKey?
				KeyParameter				secKey,
				string						algorithm,
				string						wrapOid,
				OriginatorIdentifierOrKey	originator,
				X509Certificate				cert)
			{
				DerSequence paramSeq = new DerSequence(
					new DerObjectIdentifier(wrapOid),
					DerNull.Instance);

				this.secKeyAlgorithm = secKeyAlgorithm;
				this.secKey = secKey;
				this.keyEncAlg = new AlgorithmIdentifier(new DerObjectIdentifier(algorithm), paramSeq);
				this.originator = originator;
				this.cert = cert;
			}

			public RecipientInf(
				string				secKeyAlgorithm, // TODO Can get this from secKey?
				KeyParameter		secKey,
				AlgorithmIdentifier	derivationAlg)
			{
				this.secKeyAlgorithm = secKeyAlgorithm;
				this.secKey = secKey;
				this.derivationAlg = derivationAlg;
			}

			internal RecipientInfo ToRecipientInfo(
				KeyParameter	key,
				SecureRandom	random)
			{
				byte[] keyBytes = key.GetKey();

				if (pubKey != null)
				{
					IWrapper keyWrapper = Helper.CreateWrapper(keyEncAlg.ObjectID.Id);

					keyWrapper.Init(true, new ParametersWithRandom(pubKey, random));

					Asn1OctetString encKey = new DerOctetString(
						keyWrapper.Wrap(keyBytes, 0, keyBytes.Length));

					RecipientIdentifier recipId;
					if (cert != null)
					{
						TbsCertificateStructure tbs = TbsCertificateStructure.GetInstance(
							Asn1Object.FromByteArray(cert.GetTbsCertificate()));

						Asn1.Cms.IssuerAndSerialNumber encSid = new Asn1.Cms.IssuerAndSerialNumber(
							tbs.Issuer, tbs.SerialNumber.Value);

						recipId = new RecipientIdentifier(encSid);
					}
					else
					{
						recipId = new RecipientIdentifier(subKeyId);
					}

					return new RecipientInfo(new KeyTransRecipientInfo(recipId, keyEncAlg, encKey));
				}
				else if (originator != null)
				{
					IWrapper keyWrapper = Helper.CreateWrapper(
						DerObjectIdentifier.GetInstance(
							Asn1Sequence.GetInstance(keyEncAlg.Parameters)[0]).Id);

					keyWrapper.Init(true, new ParametersWithRandom(secKey, random));

					Asn1OctetString encKey = new DerOctetString(
						keyWrapper.Wrap(keyBytes, 0, keyBytes.Length));

					RecipientEncryptedKey rKey = new RecipientEncryptedKey(
						new KeyAgreeRecipientIdentifier(
							new Asn1.Cms.IssuerAndSerialNumber(
								PrincipalUtilities.GetIssuerX509Principal(cert),
								cert.SerialNumber)),
						encKey);

					return new RecipientInfo(
						new KeyAgreeRecipientInfo(originator, ukm, keyEncAlg, new DerSequence(rKey)));
				}
				else if (derivationAlg != null)
				{
					string rfc3211WrapperName = Helper.GetRfc3211WrapperName(secKeyAlgorithm);
					IWrapper keyWrapper = Helper.CreateWrapper(rfc3211WrapperName);


					// Note: In Java build, the IV is automatically generated in JCE layer
					int ivLength = rfc3211WrapperName.StartsWith("DESEDE") ? 8 : 16;
					byte[] iv = new byte[ivLength];
					random.NextBytes(iv);


					ICipherParameters parameters = new ParametersWithIV(secKey, iv);
					keyWrapper.Init(true, new ParametersWithRandom(parameters, random));

					Asn1OctetString encKey = new DerOctetString(
						keyWrapper.Wrap(keyBytes, 0, keyBytes.Length));

//					byte[] iv = keyWrapper.GetIV();

					DerSequence seq = new DerSequence(
						new DerObjectIdentifier(secKeyAlgorithm),
						new DerOctetString(iv));

					keyEncAlg = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgPwriKek, seq);

					return new RecipientInfo(new PasswordRecipientInfo(derivationAlg, keyEncAlg, encKey));
				}
				else
				{
					IWrapper keyWrapper = Helper.CreateWrapper(keyEncAlg.ObjectID.Id);

					keyWrapper.Init(true, new ParametersWithRandom(secKey, random));

					Asn1OctetString encKey = new DerOctetString(
						keyWrapper.Wrap(keyBytes, 0, keyBytes.Length));

					return new RecipientInfo(new KekRecipientInfo(secKeyId, keyEncAlg, encKey));
				}
			}
		}

		public CmsEnvelopedGenerator()
			: this(new SecureRandom())
		{
		}

		/// <summary>Constructor allowing specific source of randomness</summary>
		/// <param name="rand">Instance of <c>SecureRandom</c> to use.</param>
		public CmsEnvelopedGenerator(
			SecureRandom rand)
		{
			this.rand = rand;
		}

		/**
		 * add a recipient.
		 *
		 * @param cert recipient's public key certificate
		 * @exception ArgumentException if there is a problem with the certificate
		 */
		public void AddKeyTransRecipient(
			X509Certificate cert)
		{
			recipientInfs.Add(new RecipientInf(cert));
		}

		/**
		* add a recipient
		*
		* @param key the public key used by the recipient
		* @param subKeyId the identifier for the recipient's public key
		* @exception ArgumentException if there is a problem with the key
		*/
		public void AddKeyTransRecipient(
			AsymmetricKeyParameter	pubKey,
			byte[]					subKeyId)
		{
			recipientInfs.Add(new CmsEnvelopedGenerator.RecipientInf(pubKey, new DerOctetString(subKeyId)));
		}

		/**
		* add a KEK recipient.
		* @param key the secret key to use for wrapping
		* @param keyIdentifier the byte string that identifies the key
		*/
		public void AddKekRecipient(
			string			keyAlgorithm, // TODO Remove need for this parameter
			KeyParameter	key,
			byte[]			keyIdentifier)
		{
			recipientInfs.Add(new RecipientInf(keyAlgorithm, key, new KekIdentifier(keyIdentifier, null, null)));
		}

		public void AddPasswordRecipient(
			CmsPbeKey	pbeKey,
			string		kekAlgorithmOid)
		{
			Pbkdf2Params p = new Pbkdf2Params(pbeKey.Salt, pbeKey.IterationCount);
			KeyParameter secretKey = pbeKey.GetEncoded(kekAlgorithmOid);
			recipientInfs.Add(new RecipientInf(kekAlgorithmOid, secretKey, new AlgorithmIdentifier(PkcsObjectIdentifiers.IdPbkdf2, p)));
		}

		/**
		* Add a key agreement based recipient.
		*
		* @param agreementAlgorithm key agreement algorithm to use.
		* @param senderPrivateKey private key to initialise sender side of agreement with.
		* @param senderPublicKey sender public key to include with message.
		* @param recipientCert recipient's public key certificate.
		* @param cekWrapAlgorithm OID for key wrapping algorithm to use.
		* @exception SecurityUtilityException if the algorithm requested cannot be found
		* @exception InvalidKeyException if the keys are inappropriate for the algorithm specified
		*/
		public void AddKeyAgreementRecipient(
			string					agreementAlgorithm,
			AsymmetricKeyParameter	senderPrivateKey,
			AsymmetricKeyParameter	senderPublicKey,
			X509Certificate			recipientCert,
			string					cekWrapAlgorithm)
		{
			if (!senderPrivateKey.IsPrivate)
				throw new ArgumentException("Expected private key", "senderPrivateKey");
			if (senderPublicKey.IsPrivate)
				throw new ArgumentException("Expected public key", "senderPublicKey");

			IBasicAgreement agreement = AgreementUtilities.GetBasicAgreementWithKdf(
				agreementAlgorithm, cekWrapAlgorithm);

			agreement.Init(new ParametersWithRandom(senderPrivateKey, rand));

			BigInteger secretNum = agreement.CalculateAgreement(recipientCert.GetPublicKey());

			try
			{
				SubjectPublicKeyInfo oPubKeyInfo =
					SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(senderPublicKey);

				OriginatorIdentifierOrKey originator = new OriginatorIdentifierOrKey(
					new OriginatorPublicKey(
						new AlgorithmIdentifier(oPubKeyInfo.AlgorithmID.ObjectID, DerNull.Instance),
						oPubKeyInfo.PublicKeyData.GetBytes()));

				// TODO Fix the way bytes are derived from the secret
				byte[] secretBytes = secretNum.ToByteArrayUnsigned();
				KeyParameter secret = ParameterUtilities.CreateKeyParameter(
					cekWrapAlgorithm, secretBytes);

				recipientInfs.Add(
					new RecipientInf(cekWrapAlgorithm, secret, agreementAlgorithm,
						cekWrapAlgorithm, originator, recipientCert));
			}
			catch (IOException e)
			{
				throw new InvalidKeyException("cannot extract originator public key: " + e);
			}
		}

		protected internal virtual AlgorithmIdentifier GetAlgorithmIdentifier(
			string					encryptionOid,
			KeyParameter			encKey,
			Asn1Encodable			asn1Params,
			out ICipherParameters	cipherParameters)
		{
			Asn1Object asn1Object;
			if (asn1Params != null)
			{
				asn1Object = asn1Params.ToAsn1Object();
				cipherParameters = ParameterUtilities.GetCipherParameters(
					encryptionOid, encKey, asn1Object);
			}
			else
			{
				asn1Object = DerNull.Instance;
				cipherParameters = encKey;
			}

			return new AlgorithmIdentifier(
				new DerObjectIdentifier(encryptionOid),
				asn1Object);
		}

		protected internal virtual Asn1Encodable GenerateAsn1Parameters(
			string	encryptionOid,
			byte[]	encKeyBytes)
		{
			Asn1Encodable asn1Params = null;

			try
			{
				if (encryptionOid.Equals(RC2Cbc))
				{
					byte[] iv = new byte[8];
					rand.NextBytes(iv);

					// TODO Is this detailed repeat of Java version really necessary?
					int effKeyBits = encKeyBytes.Length * 8;
					int parameterVersion;

					if (effKeyBits < 256)
					{
						parameterVersion = rc2Table[effKeyBits];
					}
					else
					{
						parameterVersion = effKeyBits;
					}

					asn1Params = new RC2CbcParameter(parameterVersion, iv);
				}
				else
				{
					asn1Params = ParameterUtilities.GenerateParameters(encryptionOid, rand);
				}
			}
			catch (SecurityUtilityException)
			{
				// No problem... no parameters generated
			}

			return asn1Params;
		}
	}
}
