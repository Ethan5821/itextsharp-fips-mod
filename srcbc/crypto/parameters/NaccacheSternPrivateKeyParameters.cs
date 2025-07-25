using System;
using System.Collections;

using iTextSharp.Org.BouncyCastle.Math;

namespace iTextSharp.Org.BouncyCastle.Crypto.Parameters
{
	/**
	 * Private key parameters for NaccacheStern cipher. For details on this cipher,
	 * please see
	 *
	 * http://www.gemplus.com/smart/rd/publications/pdf/NS98pkcs.pdf
	 */
	public class NaccacheSternPrivateKeyParameters : NaccacheSternKeyParameters
	{
		private readonly BigInteger phiN;
		private readonly ArrayList smallPrimes;

		/**
		 * Constructs a NaccacheSternPrivateKey
		 *
		 * @param g
		 *            the public enryption parameter g
		 * @param n
		 *            the public modulus n = p*q
		 * @param lowerSigmaBound
		 *            the public lower sigma bound up to which data can be encrypted
		 * @param smallPrimes
		 *            the small primes, of which sigma is constructed in the right
		 *            order
		 * @param phi_n
		 *            the private modulus phi(n) = (p-1)(q-1)
		 */
		public NaccacheSternPrivateKeyParameters(
			BigInteger	g,
			BigInteger	n,
			int			lowerSigmaBound,
			ArrayList	smallPrimes,
			BigInteger	phiN)
			: base(true, g, n, lowerSigmaBound)
		{
			this.smallPrimes = smallPrimes;
			this.phiN = phiN;
		}

		public BigInteger PhiN
		{
			get { return phiN; }
		}

		public ArrayList SmallPrimes
		{
			get { return smallPrimes; }
		}
	}
}
