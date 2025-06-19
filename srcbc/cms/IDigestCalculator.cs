using System;

namespace iTextSharp.Org.BouncyCastle.Cms
{
	internal interface IDigestCalculator
	{
		byte[] GetDigest();
	}
}
