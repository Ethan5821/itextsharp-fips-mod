using System;
using System.Collections;

using iTextSharp.Org.BouncyCastle.Asn1;
using iTextSharp.Org.BouncyCastle.Asn1.X509;
using iTextSharp.Org.BouncyCastle.Security;
using iTextSharp.Org.BouncyCastle.X509.Store;

namespace iTextSharp.Org.BouncyCastle.X509
{
	/**
	 * Carrying class for an attribute certificate issuer.
	 */
	public class AttributeCertificateIssuer
		//: CertSelector, Selector
		: IX509Selector
	{
		internal readonly Asn1Encodable form;

		/**
		 * Set the issuer directly with the ASN.1 structure.
		 * 
		 * @param issuer The issuer
		 */
		public AttributeCertificateIssuer(
			AttCertIssuer issuer)
		{
			form = issuer.Issuer;
		}

		public AttributeCertificateIssuer(
			X509Name principal)
		{
//			form = new V2Form(GeneralNames.GetInstance(new DerSequence(new GeneralName(principal))));
			form = new V2Form(new GeneralNames(new GeneralName(principal)));
		}

		private object[] GetNames()
		{
			GeneralNames name;
			if (form is V2Form)
			{
				name = ((V2Form)form).IssuerName;
			}
			else
			{
				name = (GeneralNames)form;
			}

			GeneralName[] names = name.GetNames();

			ArrayList l = new ArrayList(names.Length);

			for (int i = 0; i != names.Length; i++)
			{
				if (names[i].TagNo == GeneralName.DirectoryName)
				{
					l.Add(X509Name.GetInstance(names[i].Name));
				}
			}

			return l.ToArray();
		}

		/// <summary>Return any principal objects inside the attribute certificate issuer object.</summary>
		/// <returns>An array of IPrincipal objects (usually X509Principal).</returns>
		public X509Name[] GetPrincipals()
		{
			object[] p = this.GetNames();
			ArrayList l = new ArrayList(p.Length);

			for (int i = 0; i != p.Length; i++)
			{
				if (p[i] is X509Name)
				{
					l.Add(p[i]);
				}
			}

			return (X509Name[]) l.ToArray(typeof(X509Name));
		}

		private bool MatchesDN(
			X509Name		subject,
			GeneralNames	targets)
		{
			GeneralName[] names = targets.GetNames();

			for (int i = 0; i != names.Length; i++)
			{
				GeneralName gn = names[i];

				if (gn.TagNo == GeneralName.DirectoryName)
				{
					try
					{
						if (X509Name.GetInstance(gn.Name).Equivalent(subject))
						{
							return true;
						}
					}
					catch (Exception)
					{
					}
				}
			}

			return false;
		}

		public object Clone()
		{
			return new AttributeCertificateIssuer(AttCertIssuer.GetInstance(form));
		}

		public bool Match(
//			Certificate cert)
			X509Certificate x509Cert)
		{
//			if (!(cert is X509Certificate))
//			{
//				return false;
//			}
//
//			X509Certificate x509Cert = (X509Certificate)cert;

			if (form is V2Form)
			{
				V2Form issuer = (V2Form) form;
				if (issuer.BaseCertificateID != null)
				{
					return issuer.BaseCertificateID.Serial.Value.Equals(x509Cert.SerialNumber)
						&& MatchesDN(x509Cert.IssuerDN, issuer.BaseCertificateID.Issuer);
				}

				return MatchesDN(x509Cert.SubjectDN, issuer.IssuerName);
			}

			return MatchesDN(x509Cert.SubjectDN, (GeneralNames) form);
		}

		public override bool Equals(
			object obj)
		{
			if (obj == this)
			{
				return true;
			}

			if (!(obj is AttributeCertificateIssuer))
			{
				return false;
			}

			AttributeCertificateIssuer other = (AttributeCertificateIssuer)obj;

			return this.form.Equals(other.form);
		}

		public override int GetHashCode()
		{
			return this.form.GetHashCode();
		}

		public bool Match(
			object obj)
		{
			if (!(obj is X509Certificate))
			{
				return false;
			}

			//return Match((Certificate)obj);
			return Match((X509Certificate)obj);
		}
	}
}
