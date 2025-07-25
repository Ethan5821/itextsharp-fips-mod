using System;

namespace iTextSharp.Org.BouncyCastle.Crypto
{
	/// <summary>The interface stream ciphers conform to.</summary>
    public interface IStreamCipher
    {
		/// <summary>The name of the algorithm this cipher implements.</summary>
		string AlgorithmName { get; }

		/// <summary>Initialise the cipher.</summary>
		/// <param name="forEncryption">If true the cipher is initialised for encryption,
		/// if false for decryption.</param>
		/// <param name="parameters">The key and other data required by the cipher.</param>
		/// <exception cref="ArgumentException">
		/// If the parameters argument is inappropriate.
		/// </exception>
        void Init(bool forEncryption, ICipherParameters parameters);

		/// <summary>encrypt/decrypt a single byte returning the result.</summary>
		/// <param name="input">the byte to be processed.</param>
		/// <returns>the result of processing the input byte.</returns>
        byte ReturnByte(byte input);

		/// <summary>
		/// Process a block of bytes from <c>input</c> putting the result into <c>output</c>.
		/// </summary>
		/// <param name="input">The input byte array.</param>
		/// <param name="inOff">
		/// The offset into <c>input</c> where the data to be processed starts.
		/// </param>
		/// <param name="length">The number of bytes to be processed.</param>
		/// <param name="output">The output buffer the processed bytes go into.</param>
		/// <param name="outOff">
		/// The offset into <c>output</c> the processed data starts at.
		/// </param>
		/// <exception cref="DataLengthException">If the output buffer is too small.</exception>
        void ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff);

		/// <summary>
		/// Reset the cipher to the same state as it was after the last init (if there was one).
		/// </summary>
		void Reset();
    }
}
