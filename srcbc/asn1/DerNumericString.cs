using System;
using System.Text;

namespace iTextSharp.Org.BouncyCastle.Asn1
{
    /**
     * Der NumericString object - this is an ascii string of characters {0,1,2,3,4,5,6,7,8,9, }.
     */
    public class DerNumericString
        : DerStringBase
    {
        private readonly string str;

        /**
         * return a Numeric string from the passed in object
         *
         * @exception ArgumentException if the object cannot be converted.
         */
        public static DerNumericString GetInstance(
            object obj)
        {
            if (obj == null || obj is DerNumericString)
            {
                return (DerNumericString)obj;
            }

            if (obj is Asn1OctetString)
            {
                return new DerNumericString(((Asn1OctetString)obj).GetOctets());
            }

            if (obj is Asn1TaggedObject)
            {
                return GetInstance(((Asn1TaggedObject)obj).GetObject());
            }

            throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
        }

        /**
         * return an Numeric string from a tagged object.
         *
         * @param obj the tagged object holding the object we want
         * @param explicitly true if the object is meant to be explicitly
         *              tagged false otherwise.
         * @exception ArgumentException if the tagged object cannot
         *               be converted.
         */
        public static DerNumericString GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(obj.GetObject());
        }

		/**
		 * basic constructor - with bytes.
		 */
		public DerNumericString(
			byte[] str)
			: this(Encoding.ASCII.GetString(str, 0, str.Length), false)
		{
		}

		/**
		 * basic constructor -  without validation..
		 */
		public DerNumericString(
			string str)
			: this(str, false)
		{
		}

		/**
		* Constructor with optional validation.
		*
		* @param string the base string to wrap.
		* @param validate whether or not to check the string.
		* @throws ArgumentException if validate is true and the string
		* contains characters that should not be in a NumericString.
		*/
		public DerNumericString(
			string	str,
			bool	validate)
		{
			if (str == null)
				throw new ArgumentNullException("str");
			if (validate && !IsNumericString(str))
				throw new ArgumentException("string contains illegal characters", "str");

			this.str = str;
		}

		public override string GetString()
        {
            return str;
        }

		public byte[] GetOctets()
        {
			return Encoding.ASCII.GetBytes(str);
        }

		internal override void Encode(
            DerOutputStream derOut)
        {
            derOut.WriteEncoded(Asn1Tags.NumericString, GetOctets());
        }

		protected override bool Asn1Equals(
			Asn1Object asn1Object)
		{
			DerNumericString other = asn1Object as DerNumericString;

			if (other == null)
				return false;

			return this.str.Equals(other.str);
        }

		/**
		 * Return true if the string can be represented as a NumericString ('0'..'9', ' ')
		 *
		 * @param str string to validate.
		 * @return true if numeric, fale otherwise.
		 */
		public static bool IsNumericString(
			string str)
		{
			foreach (char ch in str)
			{
				if (ch > 0x007f || (ch != ' ' && !char.IsDigit(ch)))
					return false;
			}

			return true;
		}
	}
}
