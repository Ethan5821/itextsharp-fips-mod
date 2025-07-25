using System.Collections;
using System.Globalization;

using iTextSharp.Org.BouncyCastle.Asn1;
using iTextSharp.Org.BouncyCastle.Asn1.Iana;
using iTextSharp.Org.BouncyCastle.Asn1.Pkcs;
using iTextSharp.Org.BouncyCastle.Crypto;
using iTextSharp.Org.BouncyCastle.Crypto.Engines;
using iTextSharp.Org.BouncyCastle.Crypto.Macs;
using iTextSharp.Org.BouncyCastle.Crypto.Paddings;

namespace iTextSharp.Org.BouncyCastle.Security
{
	/// <remarks>
	///  Utility class for creating HMac object from their names/Oids
	/// </remarks>
	public sealed class MacUtilities
	{
		private MacUtilities()
		{
		}

		private static readonly Hashtable algorithms = new Hashtable();
//		private static readonly Hashtable oids = new Hashtable();

		static MacUtilities()
		{
			algorithms[IanaObjectIdentifiers.HmacMD5.Id] = "HMAC-MD5";
			algorithms[IanaObjectIdentifiers.HmacRipeMD160.Id] = "HMAC-RIPEMD160";
			algorithms[IanaObjectIdentifiers.HmacSha1.Id] = "HMAC-SHA1";
			algorithms[IanaObjectIdentifiers.HmacTiger.Id] = "HMAC-TIGER";

			algorithms[PkcsObjectIdentifiers.IdHmacWithSha1.Id] = "HMAC-SHA1";
			algorithms[PkcsObjectIdentifiers.IdHmacWithSha224.Id] = "HMAC-SHA224";
			algorithms[PkcsObjectIdentifiers.IdHmacWithSha256.Id] = "HMAC-SHA256";
			algorithms[PkcsObjectIdentifiers.IdHmacWithSha384.Id] = "HMAC-SHA384";
			algorithms[PkcsObjectIdentifiers.IdHmacWithSha512.Id] = "HMAC-SHA512";

			algorithms["DES"] = "DESMAC";
			algorithms["DES/CFB8"] = "DESMAC/CFB8";
			algorithms["DESEDE"] = "DESEDEMAC";
			algorithms["DESEDE/CFB8"] = "DESEDEMAC/CFB8";
			algorithms["DESISO9797MAC"] = "DESWITHISO9797";
			algorithms["DESEDE64"] = "DESEDEMAC64";

			algorithms["DESEDE64WITHISO7816-4PADDING"] = "DESEDEMAC64WITHISO7816-4PADDING";
			algorithms["DESEDEISO9797ALG1MACWITHISO7816-4PADDING"] = "DESEDEMAC64WITHISO7816-4PADDING";
			algorithms["DESEDEISO9797ALG1WITHISO7816-4PADDING"] = "DESEDEMAC64WITHISO7816-4PADDING";

			algorithms["ISO9797ALG3"] = "ISO9797ALG3MAC";
			algorithms["ISO9797ALG3MACWITHISO7816-4PADDING"] = "ISO9797ALG3WITHISO7816-4PADDING";

			algorithms["SKIPJACK"] = "SKIPJACKMAC";
			algorithms["SKIPJACK/CFB8"] = "SKIPJACKMAC/CFB8";
			algorithms["IDEA"] = "IDEAMAC";
			algorithms["IDEA/CFB8"] = "IDEAMAC/CFB8";
			algorithms["RC2"] = "RC2MAC";
			algorithms["RC2/CFB8"] = "RC2MAC/CFB8";
			algorithms["RC5"] = "RC5MAC";
			algorithms["RC5/CFB8"] = "RC5MAC/CFB8";
			algorithms["GOST28147"] = "GOST28147MAC";
			algorithms["VMPC"] = "VMPCMAC";
			algorithms["VMPC-MAC"] = "VMPCMAC";

			algorithms["PBEWITHHMACSHA"] = "PBEWITHHMACSHA1";
			algorithms["1.3.14.3.2.26"] = "PBEWITHHMACSHA1";
		}

//		/// <summary>
//		/// Returns a ObjectIdentifier for a given digest mechanism.
//		/// </summary>
//		/// <param name="mechanism">A string representation of the digest meanism.</param>
//		/// <returns>A DerObjectIdentifier, null if the Oid is not available.</returns>
//		public static DerObjectIdentifier GetObjectIdentifier(
//			string mechanism)
//		{
//			mechanism = (string) algorithms[mechanism.ToUpper(CultureInfo.InvariantCulture)];
//
//			if (mechanism != null)
//			{
//				return (DerObjectIdentifier)oids[mechanism];
//			}
//
//			return null;
//		}

//		public static ICollection Algorithms
//		{
//			get { return oids.Keys; }
//		}

		public static IMac GetMac(
			DerObjectIdentifier id)
		{
			return GetMac(id.Id);
		}

		public static IMac GetMac(
			string algorithm)
		{
			string upper = algorithm.ToUpper(CultureInfo.InvariantCulture);

			string mechanism = (string) algorithms[upper];

			if (mechanism == null)
			{
				mechanism = upper;
			}

			if (mechanism.StartsWith("PBEWITH"))
			{
				mechanism = mechanism.Substring("PBEWITH".Length);
			}

			if (mechanism.StartsWith("HMAC"))
			{
				string digestName;
				if (mechanism.StartsWith("HMAC-") || mechanism.StartsWith("HMAC/"))
				{
					digestName = mechanism.Substring(5);
				}
				else
				{
					digestName = mechanism.Substring(4);
				}

				return new HMac(DigestUtilities.GetDigest(digestName));
			}

			if (mechanism == "DESMAC")
			{
				return new CbcBlockCipherMac(new DesEngine());
			}
			if (mechanism == "DESMAC/CFB8")
			{
				return new CfbBlockCipherMac(new DesEngine());
			}
			if (mechanism == "DESEDEMAC")
			{
				return new CbcBlockCipherMac(new DesEdeEngine());
			}
			if (mechanism == "DESEDEMAC/CFB8")
			{
				return new CfbBlockCipherMac(new DesEdeEngine());
			}
			if (mechanism == "DESEDEMAC64")
			{
				return new CbcBlockCipherMac(new DesEdeEngine(), 64);
			}
			if (mechanism == "DESEDEMAC64WITHISO7816-4PADDING")
			{
				return new CbcBlockCipherMac(new DesEdeEngine(), 64, new ISO7816d4Padding());
			}
			if (mechanism == "DESWITHISO9797"
				|| mechanism == "ISO9797ALG3MAC")
			{
				return new ISO9797Alg3Mac(new DesEngine());
			}
			if (mechanism == "ISO9797ALG3WITHISO7816-4PADDING")
			{
				return new ISO9797Alg3Mac(new DesEngine(), new ISO7816d4Padding());
			}
			if (mechanism == "SKIPJACKMAC")
			{
				return new CbcBlockCipherMac(new SkipjackEngine());
			}
			if (mechanism == "SKIPJACKMAC/CFB8")
			{
				return new CfbBlockCipherMac(new SkipjackEngine());
			}
#if INCLUDE_IDEA
			if (mechanism == "IDEAMAC")
			{
				return new CbcBlockCipherMac(new IdeaEngine());
			}
			if (mechanism == "IDEAMAC/CFB8")
			{
				return new CfbBlockCipherMac(new IdeaEngine());
			}
#endif
			if (mechanism == "RC2MAC")
			{
				return new CbcBlockCipherMac(new RC2Engine());
			}
			if (mechanism == "RC2MAC/CFB8")
			{
				return new CfbBlockCipherMac(new RC2Engine());
			}
			if (mechanism == "RC5MAC")
			{
				return new CbcBlockCipherMac(new RC532Engine());
			}
			if (mechanism == "RC5MAC/CFB8")
			{
				return new CfbBlockCipherMac(new RC532Engine());
			}
			if (mechanism == "GOST28147MAC")
			{
				return new Gost28147Mac();
			}
			if (mechanism == "VMPCMAC")
			{
				return new VmpcMac();
			}
			throw new SecurityUtilityException("Mac " + mechanism + " not recognised.");
		}

		public static string GetAlgorithmName(
			DerObjectIdentifier oid)
		{
			return (string) algorithms[oid.Id];
		}

		public static byte[] DoFinal(
			IMac mac)
		{
			byte[] b = new byte[mac.GetMacSize()];
			mac.DoFinal(b, 0);
			return b;
		}
	}
}
