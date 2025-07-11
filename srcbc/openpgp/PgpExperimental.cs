using System;

namespace iTextSharp.Org.BouncyCastle.Bcpg.OpenPgp
{
	public class PgpExperimental
		: PgpObject
	{
		private readonly ExperimentalPacket p;

		public PgpExperimental(
			BcpgInputStream bcpgIn)
		{
			p = (ExperimentalPacket) bcpgIn.ReadPacket();
		}
	}
}
