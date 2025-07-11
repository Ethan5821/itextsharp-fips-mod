using System;



namespace iTextSharp.Org.BouncyCastle.Bcpg.Sig
{
    /**
    * packet giving signature creation time.
    */
    public class PreferredAlgorithms
        : SignatureSubpacket
    {
        private static byte[] IntToByteArray(
            int[]    v)
        {
            byte[]    data = new byte[v.Length];

            for (int i = 0; i != v.Length; i++)
            {
                data[i] = (byte)v[i];
            }

            return data;
        }

        public PreferredAlgorithms(
            SignatureSubpacketTag        type,
            bool    critical,
            byte[]     data)
            : base(type, critical, data)
        {
        }

        public PreferredAlgorithms(
            SignatureSubpacketTag        type,
            bool    critical,
            int[]      preferences)
            : base(type, critical, IntToByteArray(preferences))
        {
        }

        public int[] GetPreferences()
        {
            int[]    v = new int[data.Length];

            for (int i = 0; i != v.Length; i++)
            {
                v[i] = data[i] & 0xff;
            }

            return v;
        }
    }
}
