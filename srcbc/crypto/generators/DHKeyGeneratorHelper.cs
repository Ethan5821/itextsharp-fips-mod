using System;

using iTextSharp.Org.BouncyCastle.Crypto.Parameters;
using iTextSharp.Org.BouncyCastle.Math;
using iTextSharp.Org.BouncyCastle.Security;
using iTextSharp.Org.BouncyCastle.Utilities;

namespace iTextSharp.Org.BouncyCastle.Crypto.Generators
{
	class DHKeyGeneratorHelper
	{
		internal static readonly DHKeyGeneratorHelper Instance = new DHKeyGeneratorHelper();

		private DHKeyGeneratorHelper()
		{
		}

		internal BigInteger CalculatePrivate(
			DHParameters	dhParams,
			SecureRandom	random)
		{
			int limit = dhParams.L;

			if (limit != 0)
			{
				return new BigInteger(limit, random).SetBit(limit - 1);
			}

			BigInteger min = BigInteger.Two;
			int m = dhParams.M;
			if (m != 0)
			{
				min = BigInteger.One.ShiftLeft(m - 1);
			}

			BigInteger max = dhParams.P.Subtract(BigInteger.Two);
			BigInteger q = dhParams.Q;
			if (q != null)
			{
				max = q.Subtract(BigInteger.Two);
			}

			return BigIntegers.CreateRandomInRange(min, max, random);
		}

		internal BigInteger CalculatePublic(
			DHParameters	dhParams,
			BigInteger		x)
		{
			return dhParams.G.ModPow(x, dhParams.P);
		}
	}
}
