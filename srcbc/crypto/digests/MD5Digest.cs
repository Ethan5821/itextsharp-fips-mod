using System;

namespace iTextSharp.Org.BouncyCastle.Crypto.Digests
{
    /**
    * implementation of MD5 as outlined in "Handbook of Applied Cryptography", pages 346 - 347.
    */
    public class MD5Digest
		: GeneralDigest
    {
        private const int DigestLength = 16;

        private int H1, H2, H3, H4;         // IV's

        private int[]	X = new int[16];
        private int		xOff;

        public MD5Digest()
        {
            Reset();
        }

        /**
        * Copy constructor.  This will copy the state of the provided
        * message digest.
        */
        public MD5Digest(MD5Digest t)
			: base(t)
        {
            H1 = t.H1;
            H2 = t.H2;
            H3 = t.H3;
            H4 = t.H4;

            Array.Copy(t.X, 0, X, 0, t.X.Length);
            xOff = t.xOff;
        }

		public override string AlgorithmName
		{
			get { return "MD5"; }
		}

		public override int GetDigestSize()
		{
			return DigestLength;
		}

		internal override void ProcessWord(
            byte[]  input,
            int     inOff)
        {
            X[xOff++] = (input[inOff] & 0xff) | ((input[inOff + 1] & 0xff) << 8)
                | ((input[inOff + 2] & 0xff) << 16) | ((input[inOff + 3] & 0xff) << 24);

            if (xOff == 16)
            {
                ProcessBlock();
            }
        }

        internal override void ProcessLength(
            long    bitLength)
        {
            if (xOff > 14)
            {
                ProcessBlock();
            }

            X[14] = (int)(bitLength & 0xffffffff);
            X[15] = (int)((ulong) bitLength >> 32);
        }

        private void UnpackWord(
            int     word,
            byte[]  outBytes,
            int     outOff)
        {
            outBytes[outOff]     = (byte)word;
            outBytes[outOff + 1] = (byte)((uint) word >> 8);
            outBytes[outOff + 2] = (byte)((uint) word >> 16);
            outBytes[outOff + 3] = (byte)((uint) word >> 24);
        }

        public override int DoFinal(
            byte[]  output,
            int     outOff)
        {
            Finish();

            UnpackWord(H1, output, outOff);
            UnpackWord(H2, output, outOff + 4);
            UnpackWord(H3, output, outOff + 8);
            UnpackWord(H4, output, outOff + 12);

            Reset();

            return DigestLength;
        }

        /**
        * reset the chaining variables to the IV values.
        */
        public override void Reset()
        {
            base.Reset();

            H1 = unchecked((int) 0x67452301);
            H2 = unchecked((int) 0xefcdab89);
            H3 = unchecked((int) 0x98badcfe);
            H4 = unchecked((int) 0x10325476);

            xOff = 0;

            for (int i = 0; i != X.Length; i++)
            {
                X[i] = 0;
            }
        }

        //
        // round 1 left rotates
        //
        private static readonly int S11 = 7;
        private static readonly int S12 = 12;
        private static readonly int S13 = 17;
        private static readonly int S14 = 22;

        //
        // round 2 left rotates
        //
        private static readonly int S21 = 5;
        private static readonly int S22 = 9;
        private static readonly int S23 = 14;
        private static readonly int S24 = 20;

        //
        // round 3 left rotates
        //
        private static readonly int S31 = 4;
        private static readonly int S32 = 11;
        private static readonly int S33 = 16;
        private static readonly int S34 = 23;

        //
        // round 4 left rotates
        //
        private static readonly int S41 = 6;
        private static readonly int S42 = 10;
        private static readonly int S43 = 15;
        private static readonly int S44 = 21;

        /*
        * rotate int x left n bits.
        */
        private int RotateLeft(
            int x,
            int n)
        {
            return (x << n) | (int) ((uint) x >> (32 - n));
        }

        /*
        * F, G, H and I are the basic MD5 functions.
        */
        private int F(
            int u,
            int v,
            int w)
        {
            return (u & v) | (~u & w);
        }

        private int G(
            int u,
            int v,
            int w)
        {
            return (u & w) | (v & ~w);
        }

        private int H(
            int u,
            int v,
            int w)
        {
            return u ^ v ^ w;
        }

        private int K(
            int u,
            int v,
            int w)
        {
            return v ^ (u | ~w);
        }

        internal override void ProcessBlock()
        {
            int a = H1;
            int b = H2;
            int c = H3;
            int d = H4;

            //
            // Round 1 - F cycle, 16 times.
            //
            a = RotateLeft((a + F(b, c, d) + X[ 0] + unchecked((int) 0xd76aa478)), S11) + b;
            d = RotateLeft((d + F(a, b, c) + X[ 1] + unchecked((int) 0xe8c7b756)), S12) + a;
            c = RotateLeft((c + F(d, a, b) + X[ 2] + unchecked((int) 0x242070db)), S13) + d;
            b = RotateLeft((b + F(c, d, a) + X[ 3] + unchecked((int) 0xc1bdceee)), S14) + c;
            a = RotateLeft((a + F(b, c, d) + X[ 4] + unchecked((int) 0xf57c0faf)), S11) + b;
            d = RotateLeft((d + F(a, b, c) + X[ 5] + unchecked((int) 0x4787c62a)), S12) + a;
            c = RotateLeft((c + F(d, a, b) + X[ 6] + unchecked((int) 0xa8304613)), S13) + d;
            b = RotateLeft((b + F(c, d, a) + X[ 7] + unchecked((int) 0xfd469501)), S14) + c;
            a = RotateLeft((a + F(b, c, d) + X[ 8] + unchecked((int) 0x698098d8)), S11) + b;
            d = RotateLeft((d + F(a, b, c) + X[ 9] + unchecked((int) 0x8b44f7af)), S12) + a;
            c = RotateLeft((c + F(d, a, b) + X[10] + unchecked((int) 0xffff5bb1)), S13) + d;
            b = RotateLeft((b + F(c, d, a) + X[11] + unchecked((int) 0x895cd7be)), S14) + c;
            a = RotateLeft((a + F(b, c, d) + X[12] + unchecked((int) 0x6b901122)), S11) + b;
            d = RotateLeft((d + F(a, b, c) + X[13] + unchecked((int) 0xfd987193)), S12) + a;
            c = RotateLeft((c + F(d, a, b) + X[14] + unchecked((int) 0xa679438e)), S13) + d;
            b = RotateLeft((b + F(c, d, a) + X[15] + unchecked((int) 0x49b40821)), S14) + c;

            //
            // Round 2 - G cycle, 16 times.
            //
            a = RotateLeft((a + G(b, c, d) + X[ 1] + unchecked((int) 0xf61e2562)), S21) + b;
            d = RotateLeft((d + G(a, b, c) + X[ 6] + unchecked((int) 0xc040b340)), S22) + a;
            c = RotateLeft((c + G(d, a, b) + X[11] + unchecked((int) 0x265e5a51)), S23) + d;
            b = RotateLeft((b + G(c, d, a) + X[ 0] + unchecked((int) 0xe9b6c7aa)), S24) + c;
            a = RotateLeft((a + G(b, c, d) + X[ 5] + unchecked((int) 0xd62f105d)), S21) + b;
            d = RotateLeft((d + G(a, b, c) + X[10] + unchecked((int) 0x02441453)), S22) + a;
            c = RotateLeft((c + G(d, a, b) + X[15] + unchecked((int) 0xd8a1e681)), S23) + d;
            b = RotateLeft((b + G(c, d, a) + X[ 4] + unchecked((int) 0xe7d3fbc8)), S24) + c;
            a = RotateLeft((a + G(b, c, d) + X[ 9] + unchecked((int) 0x21e1cde6)), S21) + b;
            d = RotateLeft((d + G(a, b, c) + X[14] + unchecked((int) 0xc33707d6)), S22) + a;
            c = RotateLeft((c + G(d, a, b) + X[ 3] + unchecked((int) 0xf4d50d87)), S23) + d;
            b = RotateLeft((b + G(c, d, a) + X[ 8] + unchecked((int) 0x455a14ed)), S24) + c;
            a = RotateLeft((a + G(b, c, d) + X[13] + unchecked((int) 0xa9e3e905)), S21) + b;
            d = RotateLeft((d + G(a, b, c) + X[ 2] + unchecked((int) 0xfcefa3f8)), S22) + a;
            c = RotateLeft((c + G(d, a, b) + X[ 7] + unchecked((int) 0x676f02d9)), S23) + d;
            b = RotateLeft((b + G(c, d, a) + X[12] + unchecked((int) 0x8d2a4c8a)), S24) + c;

            //
            // Round 3 - H cycle, 16 times.
            //
            a = RotateLeft((a + H(b, c, d) + X[ 5] + unchecked((int) 0xfffa3942)), S31) + b;
            d = RotateLeft((d + H(a, b, c) + X[ 8] + unchecked((int) 0x8771f681)), S32) + a;
            c = RotateLeft((c + H(d, a, b) + X[11] + unchecked((int) 0x6d9d6122)), S33) + d;
            b = RotateLeft((b + H(c, d, a) + X[14] + unchecked((int) 0xfde5380c)), S34) + c;
            a = RotateLeft((a + H(b, c, d) + X[ 1] + unchecked((int) 0xa4beea44)), S31) + b;
            d = RotateLeft((d + H(a, b, c) + X[ 4] + unchecked((int) 0x4bdecfa9)), S32) + a;
            c = RotateLeft((c + H(d, a, b) + X[ 7] + unchecked((int) 0xf6bb4b60)), S33) + d;
            b = RotateLeft((b + H(c, d, a) + X[10] + unchecked((int) 0xbebfbc70)), S34) + c;
            a = RotateLeft((a + H(b, c, d) + X[13] + unchecked((int) 0x289b7ec6)), S31) + b;
            d = RotateLeft((d + H(a, b, c) + X[ 0] + unchecked((int) 0xeaa127fa)), S32) + a;
            c = RotateLeft((c + H(d, a, b) + X[ 3] + unchecked((int) 0xd4ef3085)), S33) + d;
            b = RotateLeft((b + H(c, d, a) + X[ 6] + unchecked((int) 0x04881d05)), S34) + c;
            a = RotateLeft((a + H(b, c, d) + X[ 9] + unchecked((int) 0xd9d4d039)), S31) + b;
            d = RotateLeft((d + H(a, b, c) + X[12] + unchecked((int) 0xe6db99e5)), S32) + a;
            c = RotateLeft((c + H(d, a, b) + X[15] + unchecked((int) 0x1fa27cf8)), S33) + d;
            b = RotateLeft((b + H(c, d, a) + X[ 2] + unchecked((int) 0xc4ac5665)), S34) + c;

            //
            // Round 4 - K cycle, 16 times.
            //
            a = RotateLeft((a + K(b, c, d) + X[ 0] + unchecked((int) 0xf4292244)), S41) + b;
            d = RotateLeft((d + K(a, b, c) + X[ 7] + unchecked((int) 0x432aff97)), S42) + a;
            c = RotateLeft((c + K(d, a, b) + X[14] + unchecked((int) 0xab9423a7)), S43) + d;
            b = RotateLeft((b + K(c, d, a) + X[ 5] + unchecked((int) 0xfc93a039)), S44) + c;
            a = RotateLeft((a + K(b, c, d) + X[12] + unchecked((int) 0x655b59c3)), S41) + b;
            d = RotateLeft((d + K(a, b, c) + X[ 3] + unchecked((int) 0x8f0ccc92)), S42) + a;
            c = RotateLeft((c + K(d, a, b) + X[10] + unchecked((int) 0xffeff47d)), S43) + d;
            b = RotateLeft((b + K(c, d, a) + X[ 1] + unchecked((int) 0x85845dd1)), S44) + c;
            a = RotateLeft((a + K(b, c, d) + X[ 8] + unchecked((int) 0x6fa87e4f)), S41) + b;
            d = RotateLeft((d + K(a, b, c) + X[15] + unchecked((int) 0xfe2ce6e0)), S42) + a;
            c = RotateLeft((c + K(d, a, b) + X[ 6] + unchecked((int) 0xa3014314)), S43) + d;
            b = RotateLeft((b + K(c, d, a) + X[13] + unchecked((int) 0x4e0811a1)), S44) + c;
            a = RotateLeft((a + K(b, c, d) + X[ 4] + unchecked((int) 0xf7537e82)), S41) + b;
            d = RotateLeft((d + K(a, b, c) + X[11] + unchecked((int) 0xbd3af235)), S42) + a;
            c = RotateLeft((c + K(d, a, b) + X[ 2] + unchecked((int) 0x2ad7d2bb)), S43) + d;
            b = RotateLeft((b + K(c, d, a) + X[ 9] + unchecked((int) 0xeb86d391)), S44) + c;

            H1 += a;
            H2 += b;
            H3 += c;
            H4 += d;

            //
            // reset the offset and clean out the word buffer.
            //
            xOff = 0;
            for (int i = 0; i != X.Length; i++)
            {
                X[i] = 0;
            }
        }
    }

}
