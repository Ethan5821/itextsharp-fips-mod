using System;

namespace iTextSharp.Org.BouncyCastle.Pkix
{
    public class PkixNameConstraintValidatorException : Exception
    {
        public PkixNameConstraintValidatorException(String msg)
            : base(msg)
        {
        }
    }
}
