using System;
using System.Security.Cryptography;

namespace ExitGames.Client.Photon.EncryptorManaged
{
	public class CryptoBase : IDisposable
	{
		public const int BLOCK_SIZE = 16;

		public const int IV_SIZE = 16;

		public const int HMAC_SIZE = 32;

		protected Aes encryptor;

		protected HMACSHA256 hmacsha256;

		~CryptoBase()
		{
			Dispose(false);
		}

		public void Init(byte[] encryptionSecret, byte[] hmacSecret)
		{
			encryptor = new AesManaged
			{
				Key = encryptionSecret
			};
			encryptor.GenerateIV();
			hmacsha256 = new HMACSHA256(hmacSecret);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool dispose)
		{
			if (encryptor != null)
			{
				encryptor.Clear();
				encryptor = null;
			}
			if (hmacsha256 != null)
			{
				hmacsha256.Clear();
				hmacsha256 = null;
			}
		}
	}
}
