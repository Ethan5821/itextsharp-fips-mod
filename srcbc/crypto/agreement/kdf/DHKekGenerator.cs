using System;

using iTextSharp.Org.BouncyCastle.Asn1;

namespace iTextSharp.Org.BouncyCastle.Crypto.Agreement.Kdf
{
	/**
	* RFC 2631 Diffie-hellman KEK derivation function.
	*/
	public class DHKekGenerator
		: IDerivationFunction
	{
		private readonly IDigest digest;

		private DerObjectIdentifier	algorithm;
		private int					keySize;
		private byte[]				z;
		private byte[]				partyAInfo;

		public DHKekGenerator(
			IDigest digest)
		{
			this.digest = digest;
		}

		public void Init(
			IDerivationParameters param)
		{
			DHKdfParameters parameters = (DHKdfParameters)param;

			this.algorithm = parameters.Algorithm;
			this.keySize = parameters.KeySize;
			this.z = parameters.GetZ(); // TODO Clone?
			this.partyAInfo = parameters.GetExtraInfo(); // TODO Clone?
		}

		public IDigest Digest
		{
			get { return digest; }
		}

		public int GenerateBytes(
			byte[]	outBytes,
			int		outOff,
			int		len)
		{
			if ((outBytes.Length - len) < outOff)
			{
				throw new DataLengthException("output buffer too small");
			}

			long oBytes = len;
			int outLen = digest.GetDigestSize();

			//
			// this is at odds with the standard implementation, the
			// maximum value should be hBits * (2^32 - 1) where hBits
			// is the digest output size in bits. We can't have an
			// array with a long index at the moment...
			//
			if (oBytes > ((2L << 32) - 1))
			{
				throw new ArgumentException("Output length too large");
			}

			int cThreshold = (int)((oBytes + outLen - 1) / outLen);

			byte[] dig = new byte[digest.GetDigestSize()];

			int counter = 1;

			for (int i = 0; i < cThreshold; i++)
			{
				digest.BlockUpdate(z, 0, z.Length);

				// KeySpecificInfo
				DerSequence keyInfo = new DerSequence(
					algorithm,
					new DerOctetString(integerToBytes(counter)));

				// OtherInfo
				Asn1EncodableVector v1 = new Asn1EncodableVector(keyInfo);

				if (partyAInfo != null)
				{
					v1.Add(new DerTaggedObject(true, 0, new DerOctetString(partyAInfo)));
				}

				v1.Add(new DerTaggedObject(true, 2, new DerOctetString(integerToBytes(keySize))));

				byte[] other = new DerSequence(v1).GetDerEncoded();

				digest.BlockUpdate(other, 0, other.Length);

				digest.DoFinal(dig, 0);

				if (len > outLen)
				{
					Array.Copy(dig, 0, outBytes, outOff, outLen);
					outOff += outLen;
					len -= outLen;
				}
				else
				{
					Array.Copy(dig, 0, outBytes, outOff, len);
				}

				counter++;
			}

			digest.Reset();

			return len;
		}

		private byte[] integerToBytes(
			int keySize)
		{
			byte[] val = new byte[4];

			val[0] = (byte)(keySize >> 24);
			val[1] = (byte)(keySize >> 16);
			val[2] = (byte)(keySize >> 8);
			val[3] = (byte)keySize;

			return val;
		}
	}
}
