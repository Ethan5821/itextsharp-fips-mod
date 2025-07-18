﻿using System;
using System.Collections;
using System.Globalization;

using iTextSharp.Org.BouncyCastle.Asn1;
using iTextSharp.Org.BouncyCastle.Asn1.Nist;
using iTextSharp.Org.BouncyCastle.Asn1.Oiw;
using iTextSharp.Org.BouncyCastle.Asn1.Pkcs;
using iTextSharp.Org.BouncyCastle.Asn1.TeleTrust;
using iTextSharp.Org.BouncyCastle.Asn1.X509;
using iTextSharp.Org.BouncyCastle.Crypto;
using iTextSharp.Org.BouncyCastle.Crypto.Digests;
using iTextSharp.Org.BouncyCastle.Crypto.Engines;
using iTextSharp.Org.BouncyCastle.Crypto.Generators;
using iTextSharp.Org.BouncyCastle.Crypto.Macs;
using iTextSharp.Org.BouncyCastle.Crypto.Modes;
using iTextSharp.Org.BouncyCastle.Crypto.Paddings;
using iTextSharp.Org.BouncyCastle.Crypto.Parameters;

namespace iTextSharp.Org.BouncyCastle.Security
{
	/// <summary>
	///
	/// </summary>
	public sealed class PbeUtilities
	{
		private PbeUtilities()
		{
		}

		const string Pkcs5S1 = "Pkcs5S1";
		const string Pkcs5S2 = "Pkcs5S2";
		const string Pkcs12 = "Pkcs12";
		const string OpenSsl = "OpenSsl";

		private static readonly Hashtable algorithms = new Hashtable();
		private static readonly Hashtable algorithmType = new Hashtable();
		private static readonly Hashtable oids = new Hashtable();

		static PbeUtilities()
		{
			algorithms["PKCS5SCHEME1"] = "Pkcs5scheme1";
			algorithms["PKCS5SCHEME2"] = "Pkcs5scheme2";
			algorithms[PkcsObjectIdentifiers.IdPbeS2.Id] = "Pkcs5scheme2";
//			algorithms[PkcsObjectIdentifiers.IdPbkdf2.Id] = "Pkcs5scheme2";
			algorithms["PBEWITHMD2ANDDES-CBC"] = "PBEwithMD2andDES-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithMD2AndDesCbc.Id] = "PBEwithMD2andDES-CBC";
			algorithms["PBEWITHMD2ANDRC2-CBC"] = "PBEwithMD2andRC2-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithMD2AndRC2Cbc.Id] = "PBEwithMD2andRC2-CBC";
			algorithms["PBEWITHMD5ANDDES-CBC"] = "PBEwithMD5andDES-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithMD5AndDesCbc.Id] = "PBEwithMD5andDES-CBC";
			algorithms["PBEWITHMD5ANDRC2-CBC"] = "PBEwithMD5andRC2-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithMD5AndRC2Cbc.Id] = "PBEwithMD5andRC2-CBC";
			algorithms["PBEWITHSHA1ANDDES"] = "PBEwithSHA-1andDES-CBC";
			algorithms["PBEWITHSHA-1ANDDES"] = "PBEwithSHA-1andDES-CBC";
			algorithms["PBEWITHSHA1ANDDES-CBC"] = "PBEwithSHA-1andDES-CBC";
			algorithms["PBEWITHSHA-1ANDDES-CBC"] = "PBEwithSHA-1andDES-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithSha1AndDesCbc.Id] = "PBEwithSHA-1andDES-CBC";
			algorithms["PBEWITHSHA1ANDRC2"] = "PBEwithSHA-1andRC2-CBC";
			algorithms["PBEWITHSHA-1ANDRC2"] = "PBEwithSHA-1andRC2-CBC";
			algorithms["PBEWITHSHA1ANDRC2-CBC"] = "PBEwithSHA-1andRC2-CBC";
			algorithms["PBEWITHSHA-1ANDRC2-CBC"] = "PBEwithSHA-1andRC2-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithSha1AndRC2Cbc.Id] = "PBEwithSHA-1andRC2-CBC";
			algorithms["PKCS12"] = "Pkcs12";
			algorithms["PBEWITHSHAAND128BITRC4"] = "PBEwithSHA-1and128bitRC4";
			algorithms["PBEWITHSHA1AND128BITRC4"] = "PBEwithSHA-1and128bitRC4";
			algorithms["PBEWITHSHA-1AND128BITRC4"] = "PBEwithSHA-1and128bitRC4";
			algorithms[PkcsObjectIdentifiers.PbeWithShaAnd128BitRC4.Id] = "PBEwithSHA-1and128bitRC4";
			algorithms["PBEWITHSHAAND40BITRC4"] = "PBEwithSHA-1and40bitRC4";
			algorithms["PBEWITHSHA1AND40BITRC4"] = "PBEwithSHA-1and40bitRC4";
			algorithms["PBEWITHSHA-1AND40BITRC4"] = "PBEwithSHA-1and40bitRC4";
			algorithms[PkcsObjectIdentifiers.PbeWithShaAnd40BitRC4.Id] = "PBEwithSHA-1and40bitRC4";
			algorithms["PBEWITHSHAAND3-KEYDESEDE-CBC"] = "PBEwithSHA-1and3-keyDESEDE-CBC";
			algorithms["PBEWITHSHAAND3-KEYTRIPLEDES-CBC"] = "PBEwithSHA-1and3-keyDESEDE-CBC";
			algorithms["PBEWITHSHA1AND3-KEYDESEDE-CBC"] = "PBEwithSHA-1and3-keyDESEDE-CBC";
			algorithms["PBEWITHSHA1AND3-KEYTRIPLEDES-CBC"] = "PBEwithSHA-1and3-keyDESEDE-CBC";
			algorithms["PBEWITHSHA-1AND3-KEYDESEDE-CBC"] = "PBEwithSHA-1and3-keyDESEDE-CBC";
			algorithms["PBEWITHSHA-1AND3-KEYTRIPLEDES-CBC"] = "PBEwithSHA-1and3-keyDESEDE-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithShaAnd3KeyTripleDesCbc.Id] = "PBEwithSHA-1and3-keyDESEDE-CBC";
			algorithms["PBEWITHSHAAND2-KEYDESEDE-CBC"] = "PBEwithSHA-1and2-keyDESEDE-CBC";
			algorithms["PBEWITHSHAAND2-KEYTRIPLEDES-CBC"] = "PBEwithSHA-1and2-keyDESEDE-CBC";
			algorithms["PBEWITHSHA1AND2-KEYDESEDE-CBC"] = "PBEwithSHA-1and2-keyDESEDE-CBC";
			algorithms["PBEWITHSHA1AND2-KEYTRIPLEDES-CBC"] = "PBEwithSHA-1and2-keyDESEDE-CBC";
			algorithms["PBEWITHSHA-1AND2-KEYDESEDE-CBC"] = "PBEwithSHA-1and2-keyDESEDE-CBC";
			algorithms["PBEWITHSHA-1AND2-KEYTRIPLEDES-CBC"] = "PBEwithSHA-1and2-keyDESEDE-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithShaAnd2KeyTripleDesCbc.Id] = "PBEwithSHA-1and2-keyDESEDE-CBC";
			algorithms["PBEWITHSHAAND128BITRC2-CBC"] = "PBEwithSHA-1and128bitRC2-CBC";
			algorithms["PBEWITHSHA1AND128BITRC2-CBC"] = "PBEwithSHA-1and128bitRC2-CBC";
			algorithms["PBEWITHSHA-1AND128BITRC2-CBC"] = "PBEwithSHA-1and128bitRC2-CBC";
			algorithms[PkcsObjectIdentifiers.PbeWithShaAnd128BitRC2Cbc.Id] = "PBEwithSHA-1and128bitRC2-CBC";
			algorithms["PBEWITHSHAAND40BITRC2-CBC"] = "PBEwithSHA-1and40bitRC2-CBC";
			algorithms["PBEWITHSHA1AND40BITRC2-CBC"] = "PBEwithSHA-1and40bitRC2-CBC";
			algorithms["PBEWITHSHA-1AND40BITRC2-CBC"] = "PBEwithSHA-1and40bitRC2-CBC";
			algorithms[PkcsObjectIdentifiers.PbewithShaAnd40BitRC2Cbc.Id] = "PBEwithSHA-1and40bitRC2-CBC";
			algorithms["PBEWITHSHAAND128BITAES-CBC-BC"] = "PBEwithSHA-1and128bitAES-CBC-BC";
			algorithms["PBEWITHSHA1AND128BITAES-CBC-BC"] = "PBEwithSHA-1and128bitAES-CBC-BC";
			algorithms["PBEWITHSHA-1AND128BITAES-CBC-BC"] = "PBEwithSHA-1and128bitAES-CBC-BC";
			algorithms["PBEWITHSHAAND192BITAES-CBC-BC"] = "PBEwithSHA-1and192bitAES-CBC-BC";
			algorithms["PBEWITHSHA1AND192BITAES-CBC-BC"] = "PBEwithSHA-1and192bitAES-CBC-BC";
			algorithms["PBEWITHSHA-1AND192BITAES-CBC-BC"] = "PBEwithSHA-1and192bitAES-CBC-BC";
			algorithms["PBEWITHSHAAND256BITAES-CBC-BC"] = "PBEwithSHA-1and256bitAES-CBC-BC";
			algorithms["PBEWITHSHA1AND256BITAES-CBC-BC"] = "PBEwithSHA-1and256bitAES-CBC-BC";
			algorithms["PBEWITHSHA-1AND256BITAES-CBC-BC"] = "PBEwithSHA-1and256bitAES-CBC-BC";
			algorithms["PBEWITHSHA256AND128BITAES-CBC-BC"] = "PBEwithSHA-256and128bitAES-CBC-BC";
			algorithms["PBEWITHSHA-256AND128BITAES-CBC-BC"] = "PBEwithSHA-256and128bitAES-CBC-BC";
			algorithms["PBEWITHSHA256AND192BITAES-CBC-BC"] = "PBEwithSHA-256and192bitAES-CBC-BC";
			algorithms["PBEWITHSHA-256AND192BITAES-CBC-BC"] = "PBEwithSHA-256and192bitAES-CBC-BC";
			algorithms["PBEWITHSHA256AND256BITAES-CBC-BC"] = "PBEwithSHA-256and256bitAES-CBC-BC";
			algorithms["PBEWITHSHA-256AND256BITAES-CBC-BC"] = "PBEwithSHA-256and256bitAES-CBC-BC";
			algorithms["PBEWITHSHAANDIDEA"] = "PBEwithSHA-1andIDEA-CBC";
			algorithms["PBEWITHSHAANDIDEA-CBC"] = "PBEwithSHA-1andIDEA-CBC";
			algorithms["PBEWITHSHAANDTWOFISH"] = "PBEwithSHA-1andTWOFISH-CBC";
			algorithms["PBEWITHSHAANDTWOFISH-CBC"] = "PBEwithSHA-1andTWOFISH-CBC";
			algorithms["PBEWITHHMACSHA1"] = "PBEwithHmacSHA-1";
			algorithms["PBEWITHHMACSHA-1"] = "PBEwithHmacSHA-1";
			algorithms[OiwObjectIdentifiers.IdSha1.Id] = "PBEwithHmacSHA-1";
			algorithms["PBEWITHHMACSHA224"] = "PBEwithHmacSHA-224";
			algorithms["PBEWITHHMACSHA-224"] = "PBEwithHmacSHA-224";
			algorithms[NistObjectIdentifiers.IdSha224.Id] = "PBEwithHmacSHA-224";
			algorithms["PBEWITHHMACSHA256"] = "PBEwithHmacSHA-256";
			algorithms["PBEWITHHMACSHA-256"] = "PBEwithHmacSHA-256";
			algorithms[NistObjectIdentifiers.IdSha256.Id] = "PBEwithHmacSHA-256";
			algorithms["PBEWITHHMACRIPEMD128"] = "PBEwithHmacRipeMD128";
			algorithms[TeleTrusTObjectIdentifiers.RipeMD128.Id] = "PBEwithHmacRipeMD128";
			algorithms["PBEWITHHMACRIPEMD160"] = "PBEwithHmacRipeMD160";
			algorithms[TeleTrusTObjectIdentifiers.RipeMD160.Id] = "PBEwithHmacRipeMD160";
			algorithms["PBEWITHHMACRIPEMD256"] = "PBEwithHmacRipeMD256";
			algorithms[TeleTrusTObjectIdentifiers.RipeMD256.Id] = "PBEwithHmacRipeMD256";
			algorithms["PBEWITHHMACTIGER"] = "PBEwithHmacTiger";

			algorithms["PBEWITHMD5AND128BITAES-CBC-OPENSSL"] = "PBEwithMD5and128bitAES-CBC-OpenSSL";
			algorithms["PBEWITHMD5AND192BITAES-CBC-OPENSSL"] = "PBEwithMD5and192bitAES-CBC-OpenSSL";
			algorithms["PBEWITHMD5AND256BITAES-CBC-OPENSSL"] = "PBEwithMD5and256bitAES-CBC-OpenSSL";

			algorithmType["Pkcs5scheme1"] = Pkcs5S1;
			algorithmType["Pkcs5scheme2"] = Pkcs5S2;
			algorithmType["PBEwithMD2andDES-CBC"] = Pkcs5S1;
			algorithmType["PBEwithMD2andRC2-CBC"] = Pkcs5S1;
			algorithmType["PBEwithMD5andDES-CBC"] = Pkcs5S1;
			algorithmType["PBEwithMD5andRC2-CBC"] = Pkcs5S1;
			algorithmType["PBEwithSHA-1andDES-CBC"] = Pkcs5S1;
			algorithmType["PBEwithSHA-1andRC2-CBC"] = Pkcs5S1;
			algorithmType["Pkcs12"] = Pkcs12;
			algorithmType["PBEwithSHA-1and128bitRC4"] = Pkcs12;
			algorithmType["PBEwithSHA-1and40bitRC4"] = Pkcs12;
			algorithmType["PBEwithSHA-1and3-keyDESEDE-CBC"] = Pkcs12;
			algorithmType["PBEwithSHA-1and2-keyDESEDE-CBC"] = Pkcs12;
			algorithmType["PBEwithSHA-1and128bitRC2-CBC"] = Pkcs12;
			algorithmType["PBEwithSHA-1and40bitRC2-CBC"] = Pkcs12;
			algorithmType["PBEwithSHA-1and128bitAES-CBC-BC"] = Pkcs12;
			algorithmType["PBEwithSHA-1and192bitAES-CBC-BC"] = Pkcs12;
			algorithmType["PBEwithSHA-1and256bitAES-CBC-BC"] = Pkcs12;
			algorithmType["PBEwithSHA-256and128bitAES-CBC-BC"] = Pkcs12;
			algorithmType["PBEwithSHA-256and192bitAES-CBC-BC"] = Pkcs12;
			algorithmType["PBEwithSHA-256and256bitAES-CBC-BC"] = Pkcs12;
			algorithmType["PBEwithSHA-1andIDEA-CBC"] = "Pkcs12";
			algorithmType["PBEwithSHA-1andTWOFISH-CBC"] = "Pkcs12";
			algorithmType["PBEwithHmacSHA-1"] = Pkcs12;
			algorithmType["PBEwithHmacSHA-224"] = Pkcs12;
			algorithmType["PBEwithHmacSHA-256"] = Pkcs12;
			algorithmType["PBEwithHmacRipeMD128"] = Pkcs12;
			algorithmType["PBEwithHmacRipeMD160"] = Pkcs12;
			algorithmType["PBEwithHmacRipeMD256"] = Pkcs12;
			algorithmType["PBEwithHmacTiger"] = Pkcs12;

			algorithmType["PBEwithMD5and128bitAES-CBC-OpenSSL"] = OpenSsl;
			algorithmType["PBEwithMD5and192bitAES-CBC-OpenSSL"] = OpenSsl;
			algorithmType["PBEwithMD5and256bitAES-CBC-OpenSSL"] = OpenSsl;

			oids["PBEwithMD2andDES-CBC"] = PkcsObjectIdentifiers.PbeWithMD2AndDesCbc;
			oids["PBEwithMD2andRC2-CBC"] = PkcsObjectIdentifiers.PbeWithMD2AndRC2Cbc;
			oids["PBEwithMD5andDES-CBC"] = PkcsObjectIdentifiers.PbeWithMD5AndDesCbc;
			oids["PBEwithMD5andRC2-CBC"] = PkcsObjectIdentifiers.PbeWithMD5AndRC2Cbc;
			oids["PBEwithSHA-1andDES-CBC"] = PkcsObjectIdentifiers.PbeWithSha1AndDesCbc;
			oids["PBEwithSHA-1andRC2-CBC"] = PkcsObjectIdentifiers.PbeWithSha1AndRC2Cbc;
			oids["PBEwithSHA-1and128bitRC4"] = PkcsObjectIdentifiers.PbeWithShaAnd128BitRC4;
			oids["PBEwithSHA-1and40bitRC4"] = PkcsObjectIdentifiers.PbeWithShaAnd40BitRC4;
			oids["PBEwithSHA-1and3-keyDESEDE-CBC"] = PkcsObjectIdentifiers.PbeWithShaAnd3KeyTripleDesCbc;
			oids["PBEwithSHA-1and2-keyDESEDE-CBC"] = PkcsObjectIdentifiers.PbeWithShaAnd2KeyTripleDesCbc;
			oids["PBEwithSHA-1and128bitRC2-CBC"] = PkcsObjectIdentifiers.PbeWithShaAnd128BitRC2Cbc;
			oids["PBEwithSHA-1and40bitRC2-CBC"] = PkcsObjectIdentifiers.PbewithShaAnd40BitRC2Cbc;
			oids["PBEwithHmacSHA-1"] = OiwObjectIdentifiers.IdSha1;
			oids["PBEwithHmacSHA-224"] = NistObjectIdentifiers.IdSha224;
			oids["PBEwithHmacSHA-256"] = NistObjectIdentifiers.IdSha256;
			oids["PBEwithHmacRipeMD128"] = TeleTrusTObjectIdentifiers.RipeMD128;
			oids["PBEwithHmacRipeMD160"] = TeleTrusTObjectIdentifiers.RipeMD160;
			oids["PBEwithHmacRipeMD256"] = TeleTrusTObjectIdentifiers.RipeMD256;
		}

		static PbeParametersGenerator MakePbeGenerator(
			string	type,
			IDigest	digest,
			byte[]	key,
			byte[]	salt,
			int		iterationCount)
		{
			PbeParametersGenerator generator;

			if (type.Equals(Pkcs5S1))
			{
				generator = new Pkcs5S1ParametersGenerator(digest);
			}
			else if (type.Equals(Pkcs5S2))
			{
				generator = new Pkcs5S2ParametersGenerator();
			}
			else if (type.Equals(Pkcs12))
			{
				generator = new Pkcs12ParametersGenerator(digest);
			}
			else if (type.Equals(OpenSsl))
			{
				generator = new OpenSslPbeParametersGenerator();
			}
			else
			{
				throw new ArgumentException("Unknown PBE type: " + type, "type");
			}

			generator.Init(key, salt, iterationCount);
			return generator;
		}

		/// <summary>
		/// Returns a ObjectIdentifier for a give encoding.
		/// </summary>
		/// <param name="mechanism">A string representation of the encoding.</param>
		/// <returns>A DerObjectIdentifier, null if the Oid is not available.</returns>
		public static DerObjectIdentifier GetObjectIdentifier(
			string mechanism)
		{
			mechanism = (string) algorithms[mechanism.ToUpper(CultureInfo.InvariantCulture)];
			if (mechanism != null)
			{
				return (DerObjectIdentifier)oids[mechanism];
			}
			return null;
		}

		public static ICollection Algorithms
		{
			get { return oids.Keys; }
		}

		public static bool IsPkcs12(
			string algorithm)
		{
			string mechanism = (string) algorithms[algorithm.ToUpper(CultureInfo.InvariantCulture)];

			return mechanism != null && Pkcs12.Equals(algorithmType[mechanism]);
		}

		public static bool IsPkcs5Scheme1(
			string algorithm)
		{
			string mechanism = (string)algorithms[algorithm.ToUpper(CultureInfo.InvariantCulture)];

			return mechanism != null && Pkcs5S1.Equals(algorithmType[mechanism]);
		}

		public static bool IsPkcs5Scheme2(
			string algorithm)
		{
			string mechanism = (string)algorithms[algorithm.ToUpper(CultureInfo.InvariantCulture)];

			return mechanism != null && Pkcs5S2.Equals(algorithmType[mechanism]);
		}

		public static bool IsOpenSsl(
			string algorithm)
		{
			string mechanism = (string)algorithms[algorithm.ToUpper(CultureInfo.InvariantCulture)];

			return mechanism != null && OpenSsl.Equals(algorithmType[mechanism]);
		}

		public static bool IsPbeAlgorithm(
			string algorithm)
		{
			string mechanism = (string)algorithms[algorithm.ToUpper(CultureInfo.InvariantCulture)];

			return mechanism != null && algorithmType[mechanism] != null;
		}

		public static Asn1Encodable GenerateAlgorithmParameters(
			DerObjectIdentifier algorithmOid,
			byte[]              salt,
			int                 iterationCount)
		{
			return GenerateAlgorithmParameters(algorithmOid.Id, salt, iterationCount);
		}

		public static Asn1Encodable GenerateAlgorithmParameters(
			string  algorithm,
			byte[]  salt,
			int     iterationCount)
		{
			if (IsPkcs12(algorithm))
			{
				return new Pkcs12PbeParams(salt, iterationCount);
			}
			else if (IsPkcs5Scheme2(algorithm))
			{
				return new Pbkdf2Params(salt, iterationCount);
			}
			else
			{
				return new PbeParameter(salt, iterationCount);
			}
		}

		public static ICipherParameters GenerateCipherParameters(
			DerObjectIdentifier algorithmOid,
			char[]              password,
			Asn1Encodable       pbeParameters)
		{
			return GenerateCipherParameters(algorithmOid.Id, password, false, pbeParameters);
		}

		public static ICipherParameters GenerateCipherParameters(
			DerObjectIdentifier algorithmOid,
			char[]              password,
			bool				wrongPkcs12Zero,
			Asn1Encodable       pbeParameters)
		{
			return GenerateCipherParameters(algorithmOid.Id, password, wrongPkcs12Zero, pbeParameters);
		}

		public static ICipherParameters GenerateCipherParameters(
			AlgorithmIdentifier algID,
			char[]              password,
			bool				wrongPkcs12Zero)
		{
			return GenerateCipherParameters(algID.ObjectID.Id, password, wrongPkcs12Zero, algID.Parameters);
		}

		public static ICipherParameters GenerateCipherParameters(
			string          algorithm,
			char[]          password,
			Asn1Encodable   pbeParameters)
		{
			return GenerateCipherParameters(algorithm, password, false, pbeParameters);
		}

		public static ICipherParameters GenerateCipherParameters(
			string          algorithm,
			char[]          password,
			bool			wrongPkcs12Zero,
			Asn1Encodable   pbeParameters)
		{
			string	mechanism = (string) algorithms[algorithm.ToUpper(CultureInfo.InvariantCulture)];
			byte[]	keyBytes;
			//string	type = (string)algorithmType[mechanism];
			byte[]	salt;
			int		iterationCount;

			if (IsPkcs12(mechanism))
			{
				Pkcs12PbeParams pbeParams = Pkcs12PbeParams.GetInstance(pbeParameters);
				salt = pbeParams.GetIV();
				iterationCount = pbeParams.Iterations.IntValue;
				keyBytes = PbeParametersGenerator.Pkcs12PasswordToBytes(password, wrongPkcs12Zero);
			}
			else if (IsPkcs5Scheme2(mechanism))
			{
				Pbkdf2Params pbeParams = Pbkdf2Params.GetInstance(pbeParameters);
				salt = pbeParams.GetSalt();
				iterationCount = pbeParams.IterationCount.IntValue;
				keyBytes = PbeParametersGenerator.Pkcs5PasswordToBytes(password);
			}
			else
			{
				PbeParameter pbeParams = PbeParameter.GetInstance(pbeParameters);
				salt = pbeParams.GetSalt();
				iterationCount = pbeParams.IterationCount.IntValue;
				keyBytes = PbeParametersGenerator.Pkcs5PasswordToBytes(password);
			}

			ICipherParameters parameters = null;

			if (mechanism.StartsWith("PBEwithSHA-1"))
			{
				PbeParametersGenerator generator = MakePbeGenerator(
					(string) algorithmType[mechanism], new Sha1Digest(), keyBytes, salt, iterationCount);

				if (mechanism.Equals("PBEwithSHA-1and128bitRC4"))
				{
					parameters = generator.GenerateDerivedParameters("RC4", 128);
				}
				else if (mechanism.Equals("PBEwithSHA-1and40bitRC4"))
				{
					parameters = generator.GenerateDerivedParameters("RC4", 40);
				}
				else if (mechanism.Equals("PBEwithSHA-1and3-keyDESEDE-CBC"))
				{
					parameters = generator.GenerateDerivedParameters("DESEDE", 192, 64);
				}
				else if (mechanism.Equals("PBEwithSHA-1and2-keyDESEDE-CBC"))
				{
					parameters = generator.GenerateDerivedParameters("DESEDE", 128, 64);
				}
				else if (mechanism.Equals("PBEwithSHA-1and128bitRC2-CBC"))
				{
					parameters = generator.GenerateDerivedParameters("RC2", 128, 64);
				}
				else if (mechanism.Equals("PBEwithSHA-1and40bitRC2-CBC"))
				{
					parameters = generator.GenerateDerivedParameters("RC2", 40, 64);
				}
				else if (mechanism.Equals("PBEwithSHA-1andDES-CBC"))
				{
					parameters = generator.GenerateDerivedParameters("DES", 64, 64);
				}
				else if (mechanism.Equals("PBEwithSHA-1andRC2-CBC"))
				{
					parameters = generator.GenerateDerivedParameters("RC2", 64, 64);
				}
				else if (mechanism.Equals("PBEwithSHA-1and128bitAES-CBC-BC"))
				{
					parameters = generator.GenerateDerivedParameters("AES", 128, 128);
				}
				else if (mechanism.Equals("PBEwithSHA-1and192bitAES-CBC-BC"))
				{
					parameters = generator.GenerateDerivedParameters("AES", 192, 128);
				}
				else if (mechanism.Equals("PBEwithSHA-1and256bitAES-CBC-BC"))
				{
					parameters = generator.GenerateDerivedParameters("AES", 256, 128);
				}
			}
			else if (mechanism.StartsWith("PBEwithSHA-256"))
			{
				PbeParametersGenerator generator = MakePbeGenerator(
					(string) algorithmType[mechanism], new Sha256Digest(), keyBytes, salt, iterationCount);

				if (mechanism.Equals("PBEwithSHA-256and128bitAES-CBC-BC"))
				{
					parameters = generator.GenerateDerivedParameters("AES", 128, 128);
				}
				else if (mechanism.Equals("PBEwithSHA-256and192bitAES-CBC-BC"))
				{
					parameters = generator.GenerateDerivedParameters("AES", 192, 128);
				}
				else if (mechanism.Equals("PBEwithSHA-256and256bitAES-CBC-BC"))
				{
					parameters = generator.GenerateDerivedParameters("AES", 256, 128);
				}
			}
			else if (mechanism.StartsWith("PBEwithMD5"))
			{
				PbeParametersGenerator generator = MakePbeGenerator(
					(string)algorithmType[mechanism], new MD5Digest(), keyBytes, salt, iterationCount);

				if (mechanism.Equals("PBEwithMD5andDES-CBC"))
				{
					parameters = generator.GenerateDerivedParameters("DES", 64, 64);
				}
				else if (mechanism.Equals("PBEwithMD5andRC2-CBC"))
				{
					parameters = generator.GenerateDerivedParameters("RC2", 64, 64);
				}
				else if (mechanism.Equals("PBEwithMD5and128bitAES-CBC-OpenSSL"))
				{
					parameters = generator.GenerateDerivedParameters("AES", 128, 128);
				}
				else if (mechanism.Equals("PBEwithMD5and192bitAES-CBC-OpenSSL"))
				{
					parameters = generator.GenerateDerivedParameters("AES", 192, 128);
				}
				else if (mechanism.Equals("PBEwithMD5and256bitAES-CBC-OpenSSL"))
				{
					parameters = generator.GenerateDerivedParameters("AES", 256, 128);
				}
			}
			else if (mechanism.StartsWith("PBEwithMD2"))
			{
				PbeParametersGenerator generator = MakePbeGenerator(
					(string)algorithmType[mechanism], new MD2Digest(), keyBytes, salt, iterationCount);
				if (mechanism.Equals("PBEwithMD2andDES-CBC"))
				{
					parameters = generator.GenerateDerivedParameters("DES", 64, 64);
				}
				else if (mechanism.Equals("PBEwithMD2andRC2-CBC"))
				{
					parameters = generator.GenerateDerivedParameters("RC2", 64, 64);
				}
			}
			else if (mechanism.StartsWith("PBEwithHmac"))
			{
				string digestName = mechanism.Substring("PBEwithHmac".Length);
				IDigest digest = DigestUtilities.GetDigest(digestName);

				PbeParametersGenerator generator = MakePbeGenerator(
					(string) algorithmType[mechanism], digest, keyBytes, salt, iterationCount);

				int bitLen = digest.GetDigestSize() * 8;
				parameters = generator.GenerateDerivedMacParameters(bitLen);
			}

			Array.Clear(keyBytes, 0, keyBytes.Length);

			return parameters;
		}

		public static object CreateEngine(
			DerObjectIdentifier algorithmOid)
		{
			return CreateEngine(algorithmOid.Id);
		}

		public static object CreateEngine(
			AlgorithmIdentifier algID)
		{
			return CreateEngine(algID.ObjectID.Id);
		}

		private static bool EndsWith(
			string	s,
			string	ending)
		{
			int pos = s.Length - ending.Length;

			return pos >= 0 && s.Substring(pos) == ending;
		}

		public static object CreateEngine(
			string algorithm)
		{
			string mechanism = (string)algorithms[algorithm.ToUpper(CultureInfo.InvariantCulture)];

			if (mechanism.StartsWith("PBEwithHmac"))
			{
				string digestName = mechanism.Substring("PBEwithHmac".Length);

				return MacUtilities.GetMac("HMAC/" + digestName);
			}

			if (mechanism.StartsWith("PBEwithMD2")
				||	mechanism.StartsWith("PBEwithMD5")
				||	mechanism.StartsWith("PBEwithSHA-1"))
			{
				if (EndsWith(mechanism, "RC2-CBC"))
				{
					return CipherUtilities.GetCipher("RC2/CBC");
				}

				if (EndsWith(mechanism, "RC4"))
				{
					return CipherUtilities.GetCipher("RC4");
				}

				if (EndsWith(mechanism, "DES-CBC"))
				{
					return CipherUtilities.GetCipher("DES/CBC");
				}

				if (EndsWith(mechanism, "DESEDE-CBC"))
				{
					return CipherUtilities.GetCipher("DESEDE/CBC");
				}
			}

			return null;
		}

		public static string GetEncodingName(
			DerObjectIdentifier oid)
		{
			return (string) algorithms[oid.Id];
		}
	}
}
