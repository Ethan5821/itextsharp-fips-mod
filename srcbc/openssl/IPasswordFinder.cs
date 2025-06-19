using System;

namespace iTextSharp.Org.BouncyCastle.OpenSsl
{
	public interface IPasswordFinder
	{
		char[] GetPassword();
	}
}
