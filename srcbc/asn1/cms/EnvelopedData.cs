using System;
using System.Collections;

using iTextSharp.Org.BouncyCastle.Asn1;

namespace iTextSharp.Org.BouncyCastle.Asn1.Cms
{
    public class EnvelopedData
        : Asn1Encodable
    {
        private DerInteger				version;
        private OriginatorInfo			originatorInfo;
        private Asn1Set					recipientInfos;
        private EncryptedContentInfo	encryptedContentInfo;
        private Asn1Set					unprotectedAttrs;

		public EnvelopedData(
            OriginatorInfo			originatorInfo,
            Asn1Set					recipientInfos,
            EncryptedContentInfo	encryptedContentInfo,
            Asn1Set					unprotectedAttrs)
        {
            if (originatorInfo != null || unprotectedAttrs != null)
            {
                version = new DerInteger(2);
            }
            else
            {
                version = new DerInteger(0);

				foreach (object o in recipientInfos)
				{
                    RecipientInfo ri = RecipientInfo.GetInstance(o);

					if (!ri.Version.Equals(version))
                    {
                        version = new DerInteger(2);
                        break;
                    }
                }
            }

			this.originatorInfo = originatorInfo;
            this.recipientInfos = recipientInfos;
            this.encryptedContentInfo = encryptedContentInfo;
            this.unprotectedAttrs = unprotectedAttrs;
        }

		public EnvelopedData(
            Asn1Sequence seq)
        {
            int index = 0;

			version = (DerInteger) seq[index++];

			object tmp = seq[index++];

			if (tmp is Asn1TaggedObject)
            {
                originatorInfo = OriginatorInfo.GetInstance((Asn1TaggedObject) tmp, false);
                tmp = seq[index++];
            }

			recipientInfos = Asn1Set.GetInstance(tmp);
            encryptedContentInfo = EncryptedContentInfo.GetInstance(seq[index++]);

			if (seq.Count > index)
            {
				unprotectedAttrs = Asn1Set.GetInstance((Asn1TaggedObject) seq[index], false);
            }
        }

		/**
         * return an EnvelopedData object from a tagged object.
         *
         * @param obj the tagged object holding the object we want.
         * @param explicitly true if the object is meant to be explicitly
         *              tagged false otherwise.
         * @exception ArgumentException if the object held by the
         *          tagged object cannot be converted.
         */
        public static EnvelopedData GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

		/**
         * return an EnvelopedData object from the given object.
         *
         * @param obj the object we want converted.
         * @exception ArgumentException if the object cannot be converted.
         */
        public static EnvelopedData GetInstance(
            object obj)
        {
            if (obj == null || obj is EnvelopedData)
            {
                return (EnvelopedData)obj;
            }

			if (obj is Asn1Sequence)
            {
                return new EnvelopedData((Asn1Sequence)obj);
            }

			throw new ArgumentException("Invalid EnvelopedData: " + obj.GetType().Name);
        }

		public DerInteger Version { get { return version; } }

		public OriginatorInfo OriginatorInfo { get { return originatorInfo; } }

		public Asn1Set RecipientInfos { get { return recipientInfos; } }

		public EncryptedContentInfo EncryptedContentInfo { get { return encryptedContentInfo; } }

		public Asn1Set UnprotectedAttrs { get { return unprotectedAttrs; } }

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         * EnvelopedData ::= Sequence {
         *     version CMSVersion,
         *     originatorInfo [0] IMPLICIT OriginatorInfo OPTIONAL,
         *     recipientInfos RecipientInfos,
         *     encryptedContentInfo EncryptedContentInfo,
         *     unprotectedAttrs [1] IMPLICIT UnprotectedAttributes OPTIONAL
         * }
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(version);

			if (originatorInfo != null)
            {
                v.Add(new DerTaggedObject(false, 0, originatorInfo));
            }

			v.Add(recipientInfos, encryptedContentInfo);

			if (unprotectedAttrs != null)
            {
                v.Add(new DerTaggedObject(false, 1, unprotectedAttrs));
            }

			return new BerSequence(v);
        }
    }
}
