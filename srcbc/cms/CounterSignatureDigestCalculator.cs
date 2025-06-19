using System;

using iTextSharp.Org.BouncyCastle.Crypto;
using iTextSharp.Org.BouncyCastle.Security;

namespace iTextSharp.Org.BouncyCastle.Cms
{
	internal class CounterSignatureDigestCalculator
		: IDigestCalculator
	{
		private readonly string alg;
		private readonly byte[] data;

		internal CounterSignatureDigestCalculator(
			string	alg,
			byte[]	data)
		{
			this.alg = alg;
			this.data = data;
		}

		public byte[] GetDigest()
		{
			IDigest digest = CmsSignedHelper.Instance.GetDigestInstance(alg);
			digest.BlockUpdate(data, 0, data.Length);
			return DigestUtilities.DoFinal(digest);
		}
	}
}
