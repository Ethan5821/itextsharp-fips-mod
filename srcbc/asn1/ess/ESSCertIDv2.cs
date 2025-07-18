using System;

using iTextSharp.Org.BouncyCastle.Asn1.Nist;
using iTextSharp.Org.BouncyCastle.Asn1.X509;
using iTextSharp.Org.BouncyCastle.Utilities;

namespace iTextSharp.Org.BouncyCastle.Asn1.Ess
{
	public class EssCertIDv2
		: Asn1Encodable
	{
		private readonly AlgorithmIdentifier hashAlgorithm;
		private readonly byte[]              certHash;
		private readonly IssuerSerial        issuerSerial;

		private static readonly AlgorithmIdentifier DefaultAlgID = new AlgorithmIdentifier(
			NistObjectIdentifiers.IdSha256);

		public static EssCertIDv2 GetInstance(
			object o)
		{
			if (o == null || o is EssCertIDv2)
				return (EssCertIDv2) o;

			if (o is Asn1Sequence)
				return new EssCertIDv2((Asn1Sequence) o);

			throw new ArgumentException(
				"unknown object in 'EssCertIDv2' factory : "
				+ o.GetType().Name + ".");
		}

		private EssCertIDv2(
			Asn1Sequence seq)
		{
			if (seq.Count != 2 && seq.Count != 3)
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");

			int count = 0;

			if (seq[0] is Asn1OctetString)
			{
				// Default value
				this.hashAlgorithm = DefaultAlgID;
			}
			else
			{
				this.hashAlgorithm = AlgorithmIdentifier.GetInstance(seq[count++].ToAsn1Object());
			}

			this.certHash = Asn1OctetString.GetInstance(seq[count++].ToAsn1Object()).GetOctets();

			if (seq.Count > count)
			{
				this.issuerSerial = IssuerSerial.GetInstance(
					Asn1Sequence.GetInstance(seq[count].ToAsn1Object()));
			}
		}

		public EssCertIDv2(
			AlgorithmIdentifier	algId,
			byte[]				certHash)
			: this(algId, certHash, null)
		{
		}

		public EssCertIDv2(
			AlgorithmIdentifier	algId,
			byte[]				certHash,
			IssuerSerial		issuerSerial)
		{
			if (algId == null)
			{
				// Default value
				this.hashAlgorithm = DefaultAlgID;
			}
			else
			{
				this.hashAlgorithm = algId;
			}

			this.certHash = certHash;
			this.issuerSerial = issuerSerial;
		}

		public AlgorithmIdentifier HashAlgorithm
		{
			get { return this.hashAlgorithm; }
		}

		public byte[] GetCertHash()
		{
			return Arrays.Clone(certHash);
		}

		public IssuerSerial IssuerSerial
		{
			get { return issuerSerial; }
		}

		/**
		 * <pre>
		 * EssCertIDv2 ::=  SEQUENCE {
		 *     hashAlgorithm     AlgorithmIdentifier
		 *              DEFAULT {algorithm id-sha256},
		 *     certHash          Hash,
		 *     issuerSerial      IssuerSerial OPTIONAL
		 * }
		 *
		 * Hash ::= OCTET STRING
		 *
		 * IssuerSerial ::= SEQUENCE {
		 *     issuer         GeneralNames,
		 *     serialNumber   CertificateSerialNumber
		 * }
		 * </pre>
		 */
		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector();

			if (!hashAlgorithm.Equals(DefaultAlgID))
			{
				v.Add(hashAlgorithm);
			}

			v.Add(new DerOctetString(certHash).ToAsn1Object());

			if (issuerSerial != null)
			{
				v.Add(issuerSerial);
			}

			return new DerSequence(v);
		}

	}
}
