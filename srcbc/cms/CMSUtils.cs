using System;
using System.Collections;
using System.IO;

using iTextSharp.Org.BouncyCastle.Asn1;
using iTextSharp.Org.BouncyCastle.Asn1.Cms;
using iTextSharp.Org.BouncyCastle.Asn1.X509;
using iTextSharp.Org.BouncyCastle.Security.Certificates;
using iTextSharp.Org.BouncyCastle.Utilities.IO;
using iTextSharp.Org.BouncyCastle.X509;
using iTextSharp.Org.BouncyCastle.X509.Store;

namespace iTextSharp.Org.BouncyCastle.Cms
{
    internal class CmsUtilities
    {
		// TODO Is there a .NET equivalent to this?
//		private static readonly Runtime RUNTIME = Runtime.getRuntime();

		internal static int MaximumMemory
		{
			get
			{
				// TODO Is there a .NET equivalent to this?
				long maxMem = int.MaxValue;//RUNTIME.maxMemory();

				if (maxMem > int.MaxValue)
				{
					return int.MaxValue;
				}

				return (int)maxMem;
			}
		}

		internal static ContentInfo ReadContentInfo(
			byte[] input)
		{
			// enforce limit checking as from a byte array
			return ReadContentInfo(new Asn1InputStream(input));
		}

		internal static ContentInfo ReadContentInfo(
			Stream input)
		{
			// enforce some limit checking
			return ReadContentInfo(new Asn1InputStream(input, MaximumMemory));
		}

		private static ContentInfo ReadContentInfo(
			Asn1InputStream aIn)
		{
			try
			{
				return ContentInfo.GetInstance(aIn.ReadObject());
			}
			catch (IOException e)
			{
				throw new CmsException("IOException reading content.", e);
			}
			catch (InvalidCastException e)
			{
				throw new CmsException("Malformed content.", e);
			}
			catch (ArgumentException e)
			{
				throw new CmsException("Malformed content.", e);
			}
		}

		public static byte[] StreamToByteArray(
            Stream inStream)
        {
			return Streams.ReadAll(inStream);
        }

		public static IList GetCertificatesFromStore(
			IX509Store certStore)
		{
			try
			{
				IList certs = new ArrayList();

				if (certStore != null)
				{
					foreach (X509Certificate c in certStore.GetMatches(null))
					{
						certs.Add(
							X509CertificateStructure.GetInstance(
								Asn1Object.FromByteArray(c.GetEncoded())));
					}
				}

				return certs;
			}
			catch (CertificateEncodingException e)
			{
				throw new CmsException("error encoding certs", e);
			}
			catch (Exception e)
			{
				throw new CmsException("error processing certs", e);
			}
		}

		public static IList GetCrlsFromStore(
			IX509Store crlStore)
		{
			try
			{
				IList crls = new ArrayList();

				if (crlStore != null)
				{
					foreach (X509Crl c in crlStore.GetMatches(null))
					{
						crls.Add(
							CertificateList.GetInstance(
								Asn1Object.FromByteArray(c.GetEncoded())));
					}
				}

				return crls;
			}
			catch (CrlException e)
			{
				throw new CmsException("error encoding crls", e);
			}
			catch (Exception e)
			{
				throw new CmsException("error processing crls", e);
			}
		}

		public static Asn1Set CreateBerSetFromList(
			IList berObjects)
		{
			Asn1EncodableVector v = new Asn1EncodableVector();

			foreach (Asn1Encodable ae in berObjects)
			{
				v.Add(ae);
			}

			return new BerSet(v);
		}

		public static Asn1Set CreateDerSetFromList(
			IList derObjects)
		{
			Asn1EncodableVector v = new Asn1EncodableVector();

			foreach (Asn1Encodable ae in derObjects)
			{
				v.Add(ae);
			}

			return new DerSet(v);
		}
    }
}
