using System;
using System.Diagnostics;

using iTextSharp.Org.BouncyCastle.Crypto.Generators;
using iTextSharp.Org.BouncyCastle.Crypto.Parameters;
using iTextSharp.Org.BouncyCastle.Math;
using iTextSharp.Org.BouncyCastle.Security;

namespace iTextSharp.Org.BouncyCastle.Crypto.Agreement
{
	/**
	 * a Diffie-Hellman key exchange engine.
	 * <p>
	 * note: This uses MTI/A0 key agreement in order to make the key agreement
	 * secure against passive attacks. If you're doing Diffie-Hellman and both
	 * parties have long term public keys you should look at using this. For
	 * further information have a look at RFC 2631.</p>
	 * <p>
	 * It's possible to extend this to more than two parties as well, for the moment
	 * that is left as an exercise for the reader.</p>
	 */
	public class DHAgreement
	{
		private DHPrivateKeyParameters  key;
		private DHParameters			dhParams;
		private BigInteger				privateValue;
		private SecureRandom			random;

		public void Init(
			ICipherParameters parameters)
		{
			AsymmetricKeyParameter kParam;
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom rParam = (ParametersWithRandom)parameters;

				this.random = rParam.Random;
				kParam = (AsymmetricKeyParameter)rParam.Parameters;
			}
			else
			{
				this.random = new SecureRandom();
				kParam = (AsymmetricKeyParameter)parameters;
			}

			if (!(kParam is DHPrivateKeyParameters))
			{
				throw new ArgumentException("DHEngine expects DHPrivateKeyParameters");
			}

			this.key = (DHPrivateKeyParameters)kParam;
			this.dhParams = key.Parameters;
		}

		/**
		 * calculate our initial message.
		 */
		public BigInteger CalculateMessage()
		{
			DHKeyPairGenerator dhGen = new DHKeyPairGenerator();
			dhGen.Init(new DHKeyGenerationParameters(random, dhParams));
			AsymmetricCipherKeyPair dhPair = dhGen.GenerateKeyPair();

			this.privateValue = ((DHPrivateKeyParameters)dhPair.Private).X;

			return ((DHPublicKeyParameters)dhPair.Public).Y;
		}

		/**
		 * given a message from a given party and the corresponding public key
		 * calculate the next message in the agreement sequence. In this case
		 * this will represent the shared secret.
		 */
		public BigInteger CalculateAgreement(
			DHPublicKeyParameters	pub,
			BigInteger				message)
		{
			if (pub == null)
				throw new ArgumentNullException("pub");
			if (message == null)
				throw new ArgumentNullException("message");

			if (!pub.Parameters.Equals(dhParams))
			{
				throw new ArgumentException("Diffie-Hellman public key has wrong parameters.");
			}

			BigInteger p = dhParams.P;

			return message.ModPow(key.X, p).Multiply(pub.Y.ModPow(privateValue, p)).Mod(p);
		}
	}
}
