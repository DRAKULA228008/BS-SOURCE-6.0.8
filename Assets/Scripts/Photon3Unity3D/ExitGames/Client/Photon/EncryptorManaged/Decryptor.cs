using System;
using System.IO;
using System.Security.Cryptography;

namespace ExitGames.Client.Photon.EncryptorManaged
{
	public class Decryptor : CryptoBase
	{
		private readonly byte[] IV = new byte[16];

		private readonly byte[] readBuffer = new byte[16];

		public byte[] DecryptBufferWithIV(byte[] data, int offset, int len, out int outLen)
		{
			Buffer.BlockCopy(data, offset, IV, 0, 16);
			encryptor.IV = IV;
			using (ICryptoTransform transform = encryptor.CreateDecryptor())
			{
				using (MemoryStream stream = new MemoryStream(data, offset + 16, len - 16))
				{
					using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read))
					{
						using (MemoryStream memoryStream = new MemoryStream(len - 16))
						{
							int num;
							do
							{
								num = cryptoStream.Read(readBuffer, 0, 16);
								if (num != 0)
								{
									memoryStream.Write(readBuffer, 0, num);
								}
							}
							while (num != 0);
							outLen = (int)memoryStream.Length;
							return memoryStream.ToArray();
						}
					}
				}
			}
		}

		public bool CheckHMAC(byte[] data, int len)
		{
			hmacsha256.ComputeHash(data, 0, len - 32);
			byte[] hash = hmacsha256.Hash;
			bool flag = true;
			for (int i = 0; i < 4 && flag; i++)
			{
				int num = len - 32 + i * 8;
				int num2 = i * 8;
				flag = data[num] == hash[num2] && data[num + 1] == hash[num2 + 1] && data[num + 2] == hash[num2 + 2] && data[num + 3] == hash[num2 + 3] && data[num + 4] == hash[num2 + 4] && data[num + 5] == hash[num2 + 5] && data[num + 6] == hash[num2 + 6] && data[num + 7] == hash[num2 + 7];
			}
			return flag;
		}
	}
}
