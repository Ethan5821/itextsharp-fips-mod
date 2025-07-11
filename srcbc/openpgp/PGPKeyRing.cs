using System.Collections;
using System.IO;

namespace iTextSharp.Org.BouncyCastle.Bcpg.OpenPgp
{
	public abstract class PgpKeyRing
		: PgpObject
	{
		internal PgpKeyRing()
		{
		}

		internal static TrustPacket ReadOptionalTrustPacket(
			BcpgInputStream bcpgInput)
		{
			return (bcpgInput.NextPacketTag() == PacketTag.Trust)
				?	(TrustPacket) bcpgInput.ReadPacket()
				:	null;
		}

		internal static ArrayList ReadSignaturesAndTrust(
			BcpgInputStream	bcpgInput)
		{
			try
			{
				ArrayList sigList = new ArrayList();

				while (bcpgInput.NextPacketTag() == PacketTag.Signature)
				{
					SignaturePacket signaturePacket = (SignaturePacket) bcpgInput.ReadPacket();
					TrustPacket trustPacket = ReadOptionalTrustPacket(bcpgInput);

					sigList.Add(new PgpSignature(signaturePacket, trustPacket));
				}

				return sigList;
			}
			catch (PgpException e)
			{
				throw new IOException("can't create signature object: " + e.Message, e);
			}
		}

		internal static void ReadUserIDs(
			BcpgInputStream	bcpgInput,
			out ArrayList	ids,
			out ArrayList	idTrusts,
			out ArrayList	idSigs)
		{
			ids = new ArrayList();
			idTrusts = new ArrayList();
			idSigs = new ArrayList();

			while (bcpgInput.NextPacketTag() == PacketTag.UserId
				|| bcpgInput.NextPacketTag() == PacketTag.UserAttribute)
			{
				Packet obj = bcpgInput.ReadPacket();
				if (obj is UserIdPacket)
				{
					UserIdPacket id = (UserIdPacket)obj;
					ids.Add(id.GetId());
				}
				else
				{
					UserAttributePacket user = (UserAttributePacket) obj;
					ids.Add(new PgpUserAttributeSubpacketVector(user.GetSubpackets()));
				}

				idTrusts.Add(
					ReadOptionalTrustPacket(bcpgInput));

				idSigs.Add(
					ReadSignaturesAndTrust(bcpgInput));
			}
		}
	}
}
