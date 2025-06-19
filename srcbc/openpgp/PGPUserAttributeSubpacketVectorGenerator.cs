using System;
using System.Collections;

using iTextSharp.Org.BouncyCastle.Bcpg.Attr;

namespace iTextSharp.Org.BouncyCastle.Bcpg.OpenPgp
{
	public class PgpUserAttributeSubpacketVectorGenerator
	{
		private ArrayList list = new ArrayList();

		public virtual void SetImageAttribute(
			ImageAttrib.Format	imageType,
			byte[]				imageData)
		{
			if (imageData == null)
				throw new ArgumentException("attempt to set null image", "imageData");

			list.Add(new ImageAttrib(imageType, imageData));
		}

		public virtual PgpUserAttributeSubpacketVector Generate()
		{
			return new PgpUserAttributeSubpacketVector(
				(UserAttributeSubpacket[]) list.ToArray(typeof(UserAttributeSubpacket)));
		}
	}
}
