using System;

using iTextSharp.Org.BouncyCastle.Crypto;
using iTextSharp.Org.BouncyCastle.Math;

namespace iTextSharp.Org.BouncyCastle.Crypto.Parameters
{
    public class RsaPrivateCrtKeyParameters
		: RsaKeyParameters
    {
        private readonly BigInteger e, p, q, dP, dQ, qInv;

		public RsaPrivateCrtKeyParameters(
            BigInteger	modulus,
            BigInteger	publicExponent,
            BigInteger	privateExponent,
            BigInteger	p,
            BigInteger	q,
            BigInteger	dP,
            BigInteger	dQ,
            BigInteger	qInv)
			: base(true, modulus, privateExponent)
        {
            this.e = publicExponent;
            this.p = p;
            this.q = q;
            this.dP = dP;
            this.dQ = dQ;
            this.qInv = qInv;
        }

		public BigInteger PublicExponent
        {
            get { return e; }
		}

		public BigInteger P
		{
			get { return p; }
		}

		public BigInteger Q
		{
			get { return q; }
		}

		public BigInteger DP
		{
			get { return dP; }
		}

		public BigInteger DQ
		{
			get { return dQ; }
		}

		public BigInteger QInv
		{
			get { return qInv; }
		}

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			RsaPrivateCrtKeyParameters kp = obj as RsaPrivateCrtKeyParameters;

			if (kp == null)
				return false;

			return kp.DP.Equals(dP)
				&& kp.DQ.Equals(dQ)
				&& kp.Exponent.Equals(this.Exponent)
				&& kp.Modulus.Equals(this.Modulus)
				&& kp.P.Equals(p)
				&& kp.Q.Equals(q)
				&& kp.PublicExponent.Equals(e)
				&& kp.QInv.Equals(qInv);
		}

		public override int GetHashCode()
		{
			return DP.GetHashCode() ^ DQ.GetHashCode() ^ Exponent.GetHashCode() ^ Modulus.GetHashCode()
				^ P.GetHashCode() ^ Q.GetHashCode() ^ PublicExponent.GetHashCode() ^ QInv.GetHashCode();
		}
	}
}
