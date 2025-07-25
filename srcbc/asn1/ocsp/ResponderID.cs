using System;

using iTextSharp.Org.BouncyCastle.Asn1;
using iTextSharp.Org.BouncyCastle.Asn1.X509;

namespace iTextSharp.Org.BouncyCastle.Asn1.Ocsp
{
    public class ResponderID
        : Asn1Encodable, IAsn1Choice
    {
        private readonly Asn1Encodable id;

		public static ResponderID GetInstance(
			object obj)
		{
			if (obj == null || obj is ResponderID)
			{
				return (ResponderID)obj;
			}

			if (obj is DerOctetString)
			{
				return new ResponderID((DerOctetString)obj);
			}

			if (obj is Asn1TaggedObject)
			{
				Asn1TaggedObject o = (Asn1TaggedObject)obj;

				if (o.TagNo == 1)
				{
					return new ResponderID(X509Name.GetInstance(o, true));
				}

				return new ResponderID(Asn1OctetString.GetInstance(o, true));
			}

			return new ResponderID(X509Name.GetInstance(obj));
		}

		public ResponderID(
            Asn1OctetString id)
        {
			if (id == null)
				throw new ArgumentNullException("id");

			this.id = id;
        }

		public ResponderID(
            X509Name id)
        {
			if (id == null)
				throw new ArgumentNullException("id");

			this.id = id;
        }

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         * ResponderID ::= CHOICE {
         *      byName          [1] Name,
         *      byKey           [2] KeyHash }
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            if (id is Asn1OctetString)
            {
                return new DerTaggedObject(true, 2, id);
            }

			return new DerTaggedObject(true, 1, id);
        }
    }
}
