using System;
using System.Collections;

using iTextSharp.Org.BouncyCastle.Asn1;
using iTextSharp.Org.BouncyCastle.Asn1.X509;

namespace iTextSharp.Org.BouncyCastle.Asn1.Cms
{
    public class SignerInfo
        : Asn1Encodable
    {
        private DerInteger              version;
        private SignerIdentifier        sid;
        private AlgorithmIdentifier     digAlgorithm;
        private Asn1Set                 authenticatedAttributes;
        private AlgorithmIdentifier     digEncryptionAlgorithm;
        private Asn1OctetString         encryptedDigest;
        private Asn1Set                 unauthenticatedAttributes;

        public static SignerInfo GetInstance(
            object obj)
        {
            if (obj == null || obj is SignerInfo)
            {
                return (SignerInfo) obj;
            }

			if (obj is Asn1Sequence)
            {
                return new SignerInfo((Asn1Sequence) obj);
            }

			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		public SignerInfo(
            SignerIdentifier        sid,
            AlgorithmIdentifier     digAlgorithm,
            Asn1Set                 authenticatedAttributes,
            AlgorithmIdentifier     digEncryptionAlgorithm,
            Asn1OctetString         encryptedDigest,
            Asn1Set                 unauthenticatedAttributes)
        {
            if (sid.IsTagged)
            {
                this.version = new DerInteger(3);
            }
            else
            {
                this.version = new DerInteger(1);
            }

			this.sid = sid;
            this.digAlgorithm = digAlgorithm;
            this.authenticatedAttributes = authenticatedAttributes;
            this.digEncryptionAlgorithm = digEncryptionAlgorithm;
            this.encryptedDigest = encryptedDigest;
            this.unauthenticatedAttributes = unauthenticatedAttributes;
        }

		public SignerInfo(
            Asn1Sequence seq)
        {
            IEnumerator e = seq.GetEnumerator();

            e.MoveNext();
            version = (DerInteger) e.Current;

			e.MoveNext();
            sid = SignerIdentifier.GetInstance(e.Current);

			e.MoveNext();
            digAlgorithm = AlgorithmIdentifier.GetInstance(e.Current);

			e.MoveNext();
            object obj = e.Current;

			if (obj is Asn1TaggedObject)
            {
                authenticatedAttributes = Asn1Set.GetInstance((Asn1TaggedObject) obj, false);

				e.MoveNext();
                digEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(e.Current);
            }
            else
            {
                authenticatedAttributes = null;
                digEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(obj);
            }

			e.MoveNext();
            encryptedDigest = DerOctetString.GetInstance(e.Current);

			if (e.MoveNext())
            {
                unauthenticatedAttributes = Asn1Set.GetInstance((Asn1TaggedObject) e.Current, false);
            }
            else
            {
                unauthenticatedAttributes = null;
            }
        }

		public DerInteger Version { get { return version; } }

		public SignerIdentifier SignerID { get { return sid; } }

		public Asn1Set AuthenticatedAttributes { get { return authenticatedAttributes; } }

		public AlgorithmIdentifier DigestAlgorithm { get { return digAlgorithm; } }

		public Asn1OctetString EncryptedDigest { get { return encryptedDigest; } }

		public AlgorithmIdentifier DigestEncryptionAlgorithm { get { return digEncryptionAlgorithm; } }

		public Asn1Set UnauthenticatedAttributes { get { return unauthenticatedAttributes; } }

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         *  SignerInfo ::= Sequence {
         *      version Version,
         *      SignerIdentifier sid,
         *      digestAlgorithm DigestAlgorithmIdentifier,
         *      authenticatedAttributes [0] IMPLICIT Attributes OPTIONAL,
         *      digestEncryptionAlgorithm DigestEncryptionAlgorithmIdentifier,
         *      encryptedDigest EncryptedDigest,
         *      unauthenticatedAttributes [1] IMPLICIT Attributes OPTIONAL
         *  }
         *
         *  EncryptedDigest ::= OCTET STRING
         *
         *  DigestAlgorithmIdentifier ::= AlgorithmIdentifier
         *
         *  DigestEncryptionAlgorithmIdentifier ::= AlgorithmIdentifier
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(
				version, sid, digAlgorithm);

			if (authenticatedAttributes != null)
            {
                v.Add(new DerTaggedObject(false, 0, authenticatedAttributes));
            }

			v.Add(digEncryptionAlgorithm, encryptedDigest);

			if (unauthenticatedAttributes != null)
            {
                v.Add(new DerTaggedObject(false, 1, unauthenticatedAttributes));
            }

			return new DerSequence(v);
        }
    }
}
