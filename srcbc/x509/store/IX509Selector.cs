using System;

namespace iTextSharp.Org.BouncyCastle.X509.Store
{
	public interface IX509Selector
		: ICloneable
	{
		bool Match(object obj);
	}
}
