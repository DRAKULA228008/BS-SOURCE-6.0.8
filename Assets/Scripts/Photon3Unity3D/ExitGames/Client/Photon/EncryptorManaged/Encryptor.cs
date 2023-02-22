using System.IO;
using System.Security.Cryptography;

namespace ExitGames.Client.Photon.EncryptorManaged
{
	public class Encryptor : CryptoBase
	{
		private static readonly byte[] zeroBytes = new byte[0];

		public void Encrypt(byte[] data, int len, byte[] output, ref int offset)
		{
			using (ICryptoTransform transform = encryptor.CreateEncryptor())
			{
				using (MemoryStream memoryStream = new MemoryStream(output, offset, output.Length - offset))
				{
					byte[] iV = encryptor.IV;
					memoryStream.Write(iV, 0, iV.Length);
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						cryptoStream.Write(data, 0, len);
						cryptoStream.FlushFinalBlock();
						encryptor.GenerateIV();
						offset += (int)memoryStream.Position;
					}
				}
			}
		}

		public void HMAC(byte[] data, int offset, int count)
		{
			hmacsha256.TransformBlock(data, offset, count, data, offset);
		}

		public byte[] FinishHMAC()
		{
			hmacsha256.TransformFinalBlock(zeroBytes, 0, 0);
			byte[] hash = hmacsha256.Hash;
			hmacsha256.Initialize();
			return hash;
		}

		public byte[] FinishHMAC(byte[] data, int offset, int count)
		{
			hmacsha256.TransformFinalBlock(data, offset, count);
			byte[] hash = hmacsha256.Hash;
			hmacsha256.Initialize();
			return hash;
		}
	}
}
