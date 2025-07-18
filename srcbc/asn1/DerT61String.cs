using System;

using iTextSharp.Org.BouncyCastle.Utilities;

namespace iTextSharp.Org.BouncyCastle.Asn1
{
    /**
     * Der T61String (also the teletex string) - 8-bit characters
     */
    public class DerT61String
        : DerStringBase
    {
		private readonly string str;

		/**
         * return a T61 string from the passed in object.
         *
         * @exception ArgumentException if the object cannot be converted.
         */
        public static DerT61String GetInstance(
            object obj)
        {
            if (obj == null || obj is DerT61String)
            {
                return (DerT61String)obj;
            }

            if (obj is Asn1OctetString)
            {
                return new DerT61String(((Asn1OctetString)obj).GetOctets());
            }

            if (obj is Asn1TaggedObject)
            {
                return GetInstance(((Asn1TaggedObject)obj).GetObject());
            }

            throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
        }

        /**
         * return an T61 string from a tagged object.
         *
         * @param obj the tagged object holding the object we want
         * @param explicitly true if the object is meant to be explicitly
         *              tagged false otherwise.
         * @exception ArgumentException if the tagged object cannot
         *               be converted.
         */
        public static DerT61String GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(obj.GetObject());
        }

        /**
         * basic constructor - with bytes.
         */
        public DerT61String(
            byte[] str)
			: this(Strings.FromByteArray(str))
		{
        }

		/**
         * basic constructor - with string.
         */
        public DerT61String(
            string str)
        {
			if (str == null)
				throw new ArgumentNullException("str");

			this.str = str;
        }

		public override string GetString()
        {
            return str;
        }

        internal override void Encode(
            DerOutputStream derOut)
        {
            derOut.WriteEncoded(Asn1Tags.T61String, GetOctets());
        }

        public byte[] GetOctets()
        {
			return Strings.ToByteArray(str);
        }

		protected override bool Asn1Equals(
			Asn1Object asn1Object)
		{
			DerT61String other = asn1Object as DerT61String;

			if (other == null)
				return false;

            return this.str.Equals(other.str);
        }
	}
}
