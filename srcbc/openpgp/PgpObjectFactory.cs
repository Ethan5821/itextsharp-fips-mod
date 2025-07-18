using System;
using System.Collections;
using System.IO;

namespace iTextSharp.Org.BouncyCastle.Bcpg.OpenPgp
{
	/// <remarks>
    /// General class for reading a PGP object stream.
    /// <p>
    /// Note: if this class finds a PgpPublicKey or a PgpSecretKey it
    /// will create a PgpPublicKeyRing, or a PgpSecretKeyRing for each
    /// key found. If all you are trying to do is read a key ring file use
    /// either PgpPublicKeyRingBundle or PgpSecretKeyRingBundle.</p>
	/// </remarks>
	public class PgpObjectFactory
    {
        private readonly BcpgInputStream bcpgIn;

		public PgpObjectFactory(
            Stream inputStream)
        {
            this.bcpgIn = BcpgInputStream.Wrap(inputStream);
        }

        public PgpObjectFactory(
            byte[] bytes)
            : this(new MemoryStream(bytes, false))
        {
        }

		/// <summary>Return the next object in the stream, or null if the end is reached.</summary>
		/// <exception cref="IOException">On a parse error</exception>
        public PgpObject NextPgpObject()
        {
            PacketTag tag = bcpgIn.NextPacketTag();

            if ((int) tag == -1) return null;

            switch (tag)
            {
                case PacketTag.Signature:
                {
                    ArrayList l = new ArrayList();

                    while (bcpgIn.NextPacketTag() == PacketTag.Signature)
                    {
                        try
                        {
                            l.Add(new PgpSignature(bcpgIn));
                        }
                        catch (PgpException e)
                        {
                            throw new IOException("can't create signature object: " + e);
                        }
                    }

					return new PgpSignatureList(
						(PgpSignature[]) l.ToArray(typeof(PgpSignature)));
                }
                case PacketTag.SecretKey:
                    try
                    {
                        return new PgpSecretKeyRing(bcpgIn);
                    }
                    catch (PgpException e)
                    {
                        throw new IOException("can't create secret key object: " + e);
                    }
                case PacketTag.PublicKey:
                    return new PgpPublicKeyRing(bcpgIn);
                case PacketTag.CompressedData:
                    return new PgpCompressedData(bcpgIn);
                case PacketTag.LiteralData:
                    return new PgpLiteralData(bcpgIn);
                case PacketTag.PublicKeyEncryptedSession:
                case PacketTag.SymmetricKeyEncryptedSessionKey:
                    return new PgpEncryptedDataList(bcpgIn);
                case PacketTag.OnePassSignature:
                {
                    ArrayList l = new ArrayList();

                    while (bcpgIn.NextPacketTag() == PacketTag.OnePassSignature)
                    {
                        try
                        {
                            l.Add(new PgpOnePassSignature(bcpgIn));
                        }
                        catch (PgpException e)
                        {
							throw new IOException("can't create one pass signature object: " + e);
						}
                    }

					return new PgpOnePassSignatureList(
						(PgpOnePassSignature[]) l.ToArray(typeof(PgpOnePassSignature)));
                }
                case PacketTag.Marker:
                    return new PgpMarker(bcpgIn);
                case PacketTag.Experimental1:
                case PacketTag.Experimental2:
                case PacketTag.Experimental3:
                case PacketTag.Experimental4:
					return new PgpExperimental(bcpgIn);
            }

            throw new IOException("unknown object in stream " + bcpgIn.NextPacketTag());
        }

		[Obsolete("Use NextPgpObject() instead")]
		public object NextObject()
		{
			return NextPgpObject();
		}

		/// <summary>
		/// Return all available objects in a list.
		/// </summary>
		/// <returns>An <c>IList</c> containing all objects from this factory, in order.</returns>
		public IList AllPgpObjects()
		{
			ArrayList result = new ArrayList();
			PgpObject pgpObject;
			while ((pgpObject = NextPgpObject()) != null)
			{
				result.Add(pgpObject);
			}
			return result;
		}
	}
}
