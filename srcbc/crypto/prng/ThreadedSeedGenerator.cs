using System;
using System.Threading;

namespace iTextSharp.Org.BouncyCastle.Crypto.Prng
{
	/**
	 * A thread based seed generator - one source of randomness.
	 * <p>
	 * Based on an idea from Marcus Lippert.
	 * </p>
	 */
	public class ThreadedSeedGenerator
	{
		private class SeedGenerator
		{
#if NETCF_1_0
			// No volatile keyword, but all fields implicitly volatile anyway
			private int		counter = 0;
			private bool	stop = false;
#else
			private volatile int	counter = 0;
			private volatile bool	stop = false;
#endif

			private void Run(object ignored)
			{
				while (!this.stop)
				{
					this.counter++;
				}
			}

			public byte[] GenerateSeed(
				int		numBytes,
				bool	fast)
			{
				this.counter = 0;
				this.stop = false;

				byte[] result = new byte[numBytes];
				int last = 0;
				int end = fast ? numBytes : numBytes * 8;

				ThreadPool.QueueUserWorkItem(new WaitCallback(Run));

				for (int i = 0; i < end; i++)
				{
					while (this.counter == last)
					{
						try
						{
							Thread.Sleep(1);
						}
						catch (Exception)
						{
							// ignore
						}
					}

					last = this.counter;

					if (fast)
					{
						result[i] = (byte) last;
					}
					else
					{
						int bytepos = i / 8;
						result[bytepos] = (byte) ((result[bytepos] << 1) | (last & 1));
					}
				}

				this.stop = true;

				return result;
			}
		}

		/**
		 * Generate seed bytes. Set fast to false for best quality.
		 * <p>
		 * If fast is set to true, the code should be round about 8 times faster when
		 * generating a long sequence of random bytes. 20 bytes of random values using
		 * the fast mode take less than half a second on a Nokia e70. If fast is set to false,
		 * it takes round about 2500 ms.
		 * </p>
		 * @param numBytes the number of bytes to generate
		 * @param fast true if fast mode should be used
		 */
		public byte[] GenerateSeed(
			int		numBytes,
			bool	fast)
		{
			return new SeedGenerator().GenerateSeed(numBytes, fast);
		}
	}
}
