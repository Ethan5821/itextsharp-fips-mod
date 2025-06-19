using System;
using iTextSharp.Org.BouncyCastle.Math;
using iTextSharp.Org.BouncyCastle.Security;
using iTextSharp.Org.BouncyCastle.Crypto;
using iTextSharp.Org.BouncyCastle.Crypto.Parameters;

namespace iTextSharp.Org.BouncyCastle.Crypto.Generators
{
    /**
     * a DSA key pair generator.
     *
     * This Generates DSA keys in line with the method described
     * in FIPS 186-2.
     */
    public class DsaKeyPairGenerator
		: IAsymmetricCipherKeyPairGenerator
    {
        private DsaKeyGenerationParameters param;

		public void Init(
			KeyGenerationParameters parameters)
        {
			if (parameters == null)
				throw new ArgumentNullException("parameters");

			// Note: If we start accepting instances of KeyGenerationParameters,
			// must apply constraint checking on strength (see DsaParametersGenerator.Init)

			this.param = (DsaKeyGenerationParameters) parameters;
        }

		public AsymmetricCipherKeyPair GenerateKeyPair()
        {
            DsaParameters dsaParams = param.Parameters;
            SecureRandom random = param.Random;

			BigInteger q = dsaParams.Q;
			BigInteger x;

			do
            {
                x = new BigInteger(160, random);
            }
            while (x.SignValue == 0 || x.CompareTo(q) >= 0);

            //
            // calculate the public key.
            //
			BigInteger y = dsaParams.G.ModPow(x, dsaParams.P);

			return new AsymmetricCipherKeyPair(
                new DsaPublicKeyParameters(y, dsaParams),
                new DsaPrivateKeyParameters(x, dsaParams));
        }
	}
}
