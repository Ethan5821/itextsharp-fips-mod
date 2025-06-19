using System;

using iTextSharp.Org.BouncyCastle.Security;

namespace iTextSharp.Org.BouncyCastle.Pkix
{
	/// <summary>
	/// Summary description for PkixCertPathBuilderException.
	/// </summary>
	public class PkixCertPathBuilderException : GeneralSecurityException
	{
		public PkixCertPathBuilderException() : base() { }
		
		public PkixCertPathBuilderException(string message) : base(message)	{ }  

		public PkixCertPathBuilderException(string message, Exception exception) : base(message, exception) { }
		
	}
}
