using System;
using System.Collections;

using iTextSharp.Org.BouncyCastle.Asn1;
using iTextSharp.Org.BouncyCastle.Asn1.Tsp;
using iTextSharp.Org.BouncyCastle.Asn1.X509;
using iTextSharp.Org.BouncyCastle.Math;

namespace iTextSharp.Org.BouncyCastle.Tsp
{
	/**
	 * Generator for RFC 3161 Time Stamp Request objects.
	 */
	public class TimeStampRequestGenerator
	{
		private DerObjectIdentifier reqPolicy;

		private DerBoolean certReq;

		private Hashtable	extensions = new Hashtable();
		private ArrayList	extOrdering = new ArrayList();

		public void SetReqPolicy(
			string reqPolicy)
		{
			this.reqPolicy = new DerObjectIdentifier(reqPolicy);
		}

		public void SetCertReq(
			bool certReq)
		{
			this.certReq = DerBoolean.GetInstance(certReq);
		}

		/**
		 * add a given extension field for the standard extensions tag (tag 3)
		 * @throws IOException
		 */
		public void AddExtension(
			string			oid,
			bool			critical,
			Asn1Encodable	value)
		{
			this.AddExtension(oid, critical, value.GetEncoded());
		}

		/**
		* add a given extension field for the standard extensions tag
		* The value parameter becomes the contents of the octet string associated
		* with the extension.
		*/
		public void AddExtension(
			string	oid,
			bool	critical,
			byte[]	value)
		{
			DerObjectIdentifier derOid = new DerObjectIdentifier(oid);
			extensions[derOid] = new X509Extension(critical, new DerOctetString(value));
			extOrdering.Add(derOid);
		}

		public TimeStampRequest Generate(
			string	digestAlgorithm,
			byte[]	digest)
		{
			return this.Generate(digestAlgorithm, digest, null);
		}

		public TimeStampRequest Generate(
			string		digestAlgorithmOid,
			byte[]		digest,
			BigInteger	nonce)
		{
			if (digestAlgorithmOid == null)
			{
				throw new ArgumentException("No digest algorithm specified");
			}

			DerObjectIdentifier digestAlgOid = new DerObjectIdentifier(digestAlgorithmOid);

			AlgorithmIdentifier algID = new AlgorithmIdentifier(digestAlgOid, DerNull.Instance);
			MessageImprint messageImprint = new MessageImprint(algID, digest);

			X509Extensions  ext = null;

			if (extOrdering.Count != 0)
			{
				ext = new X509Extensions(extOrdering, extensions);
			}

			DerInteger derNonce = nonce == null
				?	null
				:	new DerInteger(nonce);

			return new TimeStampRequest(
				new TimeStampReq(messageImprint, reqPolicy, derNonce, certReq, ext));
		}
	}
}
