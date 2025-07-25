using System;
using System.Globalization;

using iTextSharp.Org.BouncyCastle.Asn1;
using iTextSharp.Org.BouncyCastle.Math;

namespace iTextSharp.Org.BouncyCastle.Crypto.Parameters
{
    public class ECPrivateKeyParameters
		: ECKeyParameters
    {
		private readonly BigInteger d;

		public ECPrivateKeyParameters(
			BigInteger			d,
			ECDomainParameters	parameters)
			: this("EC", d, parameters)
		{
		}

		public ECPrivateKeyParameters(
			BigInteger			d,
			DerObjectIdentifier publicKeyParamSet)
			: base("ECGOST3410", true, publicKeyParamSet)
		{
			if (d == null)
				throw new ArgumentNullException("d");

			this.d = d;
		}

		public ECPrivateKeyParameters(
			string				algorithm,
			BigInteger			d,
			ECDomainParameters	parameters)
			: base(algorithm, true, parameters)
		{
			if (d == null)
				throw new ArgumentNullException("d");

			this.d = d;
		}

		public BigInteger D
		{
			get { return d; }
		}

		public override bool Equals(
			object obj)
        {
			if (obj == this)
				return true;

			ECPrivateKeyParameters other = obj as ECPrivateKeyParameters;

			if (other == null)
				return false;

			return Equals(other);
        }

		protected bool Equals(
			ECPrivateKeyParameters other)
		{
			return d.Equals(other.d) && base.Equals(other);
		}

		public override int GetHashCode()
        {
            return d.GetHashCode() ^ base.GetHashCode();
        }
	}
}
