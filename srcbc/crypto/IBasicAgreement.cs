using System;
using iTextSharp.Org.BouncyCastle.Math;

namespace iTextSharp.Org.BouncyCastle.Crypto
{
    /**
     * The basic interface that basic Diffie-Hellman implementations
     * conforms to.
     */
    public interface IBasicAgreement
    {
        /**
         * initialise the agreement engine.
         */
        void Init(ICipherParameters parameters);

        /**
         * given a public key from a given party calculate the next
         * message in the agreement sequence.
         */
        BigInteger CalculateAgreement(ICipherParameters pubKey);
    }

}
