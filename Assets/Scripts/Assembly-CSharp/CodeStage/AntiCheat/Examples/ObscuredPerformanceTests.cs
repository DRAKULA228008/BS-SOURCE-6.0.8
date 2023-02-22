using System.Diagnostics;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace CodeStage.AntiCheat.Examples
{
	[AddComponentMenu("")]
	public class ObscuredPerformanceTests : MonoBehaviour
	{
		public bool boolTest = true;

		public int boolIterations = 2500000;

		public bool byteTest = true;

		public int byteIterations = 2500000;

		public bool shortTest = true;

		public int shortIterations = 2500000;

		public bool ushortTest = true;

		public int ushortIterations = 2500000;

		public bool intTest = true;

		public int intIterations = 2500000;

		public bool uintTest = true;

		public int uintIterations = 2500000;

		public bool longTest = true;

		public int longIterations = 2500000;

		public bool floatTest = true;

		public int floatIterations = 2500000;

		public bool doubleTest = true;

		public int doubleIterations = 2500000;

		public bool stringTest = true;

		public int stringIterations = 250000;

		public bool vector3Test = true;

		public int vector3Iterations = 2500000;

		public bool prefsTest = true;

		public int prefsIterations = 2500;

		private readonly StringBuilder logBuilder = new StringBuilder();

		private void Start()
		{
			Invoke("StartTests", 1f);
		}

		private void StartTests()
		{
			logBuilder.Length = 0;
			logBuilder.AppendLine("[ACTk] <b>[ Performance tests ]</b>");
			if (boolTest)
			{
				TestBool();
			}
			if (byteTest)
			{
				TestByte();
			}
			if (shortTest)
			{
				TestShort();
			}
			if (ushortTest)
			{
				TestUShort();
			}
			if (intTest)
			{
				TestInt();
			}
			if (uintTest)
			{
				TestUInt();
			}
			if (longTest)
			{
				TestLong();
			}
			if (floatTest)
			{
				TestFloat();
			}
			if (doubleTest)
			{
				TestDouble();
			}
			if (stringTest)
			{
				TestString();
			}
			if (vector3Test)
			{
				TestVector3();
			}
			if (prefsTest)
			{
				TestPrefs();
			}
			UnityEngine.Debug.Log(logBuilder);
		}

		private void TestBool()
		{
			logBuilder.AppendLine("ObscuredBool vs bool, " + boolIterations + " iterations for read and write");
			ObscuredBool obscuredBool = true;
			bool flag = obscuredBool;
			bool flag2 = false;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < boolIterations; i++)
			{
				flag2 = obscuredBool;
			}
			for (int j = 0; j < boolIterations; j++)
			{
				obscuredBool = flag2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ObscuredBool:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < boolIterations; k++)
			{
				flag2 = flag;
			}
			for (int l = 0; l < boolIterations; l++)
			{
				flag = flag2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("bool:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (flag2)
			{
			}
			if ((bool)obscuredBool)
			{
			}
			if (!flag)
			{
			}
		}

		private void TestByte()
		{
			logBuilder.AppendLine("ObscuredByte vs byte, " + byteIterations + " iterations for read and write");
			ObscuredByte obscuredByte = (byte)100;
			byte b = obscuredByte;
			byte b2 = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < byteIterations; i++)
			{
				b2 = obscuredByte;
			}
			for (int j = 0; j < byteIterations; j++)
			{
				obscuredByte = b2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ObscuredByte:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < byteIterations; k++)
			{
				b2 = b;
			}
			for (int l = 0; l < byteIterations; l++)
			{
				b = b2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("byte:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (b2 != 0)
			{
			}
			if ((byte)obscuredByte != 0)
			{
			}
			if (b == 0)
			{
			}
		}

		private void TestShort()
		{
			logBuilder.AppendLine("ObscuredShort vs short, " + shortIterations + " iterations for read and write");
			ObscuredShort obscuredShort = (short)100;
			short num = obscuredShort;
			short num2 = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < shortIterations; i++)
			{
				num2 = obscuredShort;
			}
			for (int j = 0; j < shortIterations; j++)
			{
				obscuredShort = num2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ObscuredShort:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < shortIterations; k++)
			{
				num2 = num;
			}
			for (int l = 0; l < shortIterations; l++)
			{
				num = num2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("short:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num2 != 0)
			{
			}
			if ((short)obscuredShort != 0)
			{
			}
			if (num == 0)
			{
			}
		}

		private void TestUShort()
		{
			logBuilder.AppendLine("ObscuredUShort vs ushort, " + ushortIterations + " iterations for read and write");
			ObscuredUShort obscuredUShort = (ushort)100;
			ushort num = obscuredUShort;
			ushort num2 = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < ushortIterations; i++)
			{
				num2 = obscuredUShort;
			}
			for (int j = 0; j < ushortIterations; j++)
			{
				obscuredUShort = num2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ObscuredUShort:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < ushortIterations; k++)
			{
				num2 = num;
			}
			for (int l = 0; l < ushortIterations; l++)
			{
				num = num2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ushort:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num2 != 0)
			{
			}
			if ((ushort)obscuredUShort != 0)
			{
			}
			if (num == 0)
			{
			}
		}

		private void TestDouble()
		{
			logBuilder.AppendLine("ObscuredDouble vs double, " + doubleIterations + " iterations for read and write");
			ObscuredDouble obscuredDouble = 100.0;
			double num = obscuredDouble;
			double num2 = 0.0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < doubleIterations; i++)
			{
				num2 = obscuredDouble;
			}
			for (int j = 0; j < doubleIterations; j++)
			{
				obscuredDouble = num2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ObscuredDouble:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < doubleIterations; k++)
			{
				num2 = num;
			}
			for (int l = 0; l < doubleIterations; l++)
			{
				num = num2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("double:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num2 != 0.0)
			{
			}
			if ((double)obscuredDouble != 0.0)
			{
			}
			if (num == 0.0)
			{
			}
		}

		private void TestFloat()
		{
			logBuilder.AppendLine("ObscuredFloat vs float, " + floatIterations + " iterations for read and write");
			ObscuredFloat obscuredFloat = 100f;
			CryptoFloat cryptoFloat = 100f;
			float num = 0f;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < floatIterations; i++)
			{
				num = obscuredFloat;
			}
			for (int j = 0; j < floatIterations; j++)
			{
				obscuredFloat = num;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ObscuredFloat:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < floatIterations; k++)
			{
				num = cryptoFloat;
			}
			for (int l = 0; l < floatIterations; l++)
			{
				cryptoFloat = num;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("float:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num != 0f)
			{
			}
			if ((float)obscuredFloat != 0f)
			{
			}
			if ((float)cryptoFloat == 0f)
			{
			}
		}

		private void TestInt()
		{
			logBuilder.AppendLine("ObscuredInt vs int, " + intIterations + " iterations for read and write");
			ObscuredInt obscuredInt = 100;
			CryptoInt cryptoInt = 100;
			int num = 0;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < intIterations; i++)
			{
				num = obscuredInt;
			}
			for (int j = 0; j < intIterations; j++)
			{
				obscuredInt = num;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ObscuredInt:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < intIterations; k++)
			{
				num = cryptoInt;
			}
			for (int l = 0; l < intIterations; l++)
			{
				cryptoInt = num;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("int:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num != 0)
			{
			}
			if ((int)obscuredInt != 0)
			{
			}
			if ((int)cryptoInt == 0)
			{
			}
		}

		private void TestLong()
		{
			logBuilder.AppendLine("ObscuredLong vs long, " + longIterations + " iterations for read and write");
			ObscuredLong obscuredLong = 100L;
			long num = obscuredLong;
			long num2 = 0L;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < longIterations; i++)
			{
				num2 = obscuredLong;
			}
			for (int j = 0; j < longIterations; j++)
			{
				obscuredLong = num2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ObscuredLong:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < longIterations; k++)
			{
				num2 = num;
			}
			for (int l = 0; l < longIterations; l++)
			{
				num = num2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("long:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num2 != 0L)
			{
			}
			if ((long)obscuredLong != 0L)
			{
			}
			if (num == 0L)
			{
			}
		}

		private void TestString()
		{
			logBuilder.AppendLine("ObscuredString vs string, " + stringIterations + " iterations for read and write");
			ObscuredString obscuredString = "abcd";
			string text = obscuredString;
			string text2 = string.Empty;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < stringIterations; i++)
			{
				text2 = obscuredString;
			}
			for (int j = 0; j < stringIterations; j++)
			{
				obscuredString = text2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ObscuredString:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < stringIterations; k++)
			{
				text2 = text;
			}
			for (int l = 0; l < stringIterations; l++)
			{
				text = text2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("string:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (text2 != string.Empty)
			{
			}
			if (obscuredString != string.Empty)
			{
			}
			if (!(text != string.Empty))
			{
			}
		}

		private void TestUInt()
		{
			logBuilder.AppendLine("ObscuredUInt vs uint, " + uintIterations + " iterations for read and write");
			ObscuredUInt obscuredUInt = 100u;
			uint num = obscuredUInt;
			uint num2 = 0u;
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < uintIterations; i++)
			{
				num2 = obscuredUInt;
			}
			for (int j = 0; j < uintIterations; j++)
			{
				obscuredUInt = num2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ObscuredUInt:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < uintIterations; k++)
			{
				num2 = num;
			}
			for (int l = 0; l < uintIterations; l++)
			{
				num = num2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("uint:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (num2 != 0)
			{
			}
			if ((uint)obscuredUInt != 0)
			{
			}
			if (num == 0)
			{
			}
		}

		private void TestVector3()
		{
			logBuilder.AppendLine("ObscuredVector3 vs Vector3, " + vector3Iterations + " iterations for read and write");
			ObscuredVector3 obscuredVector = new Vector3(1f, 2f, 3f);
			Vector3 vector = obscuredVector;
			Vector3 vector2 = new Vector3(0f, 0f, 0f);
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < vector3Iterations; i++)
			{
				vector2 = obscuredVector;
			}
			for (int j = 0; j < vector3Iterations; j++)
			{
				obscuredVector = vector2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ObscuredVector3:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < vector3Iterations; k++)
			{
				vector2 = vector;
			}
			for (int l = 0; l < vector3Iterations; l++)
			{
				vector = vector2;
			}
			stopwatch.Stop();
			logBuilder.AppendLine("Vector3:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			if (vector2 != Vector3.zero)
			{
			}
			if (obscuredVector != Vector3.zero)
			{
			}
			if (!(vector != Vector3.zero))
			{
			}
		}

		private void TestPrefs()
		{
			logBuilder.AppendLine("ObscuredPrefs vs PlayerPrefs, " + prefsIterations + " iterations for read and write");
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < prefsIterations; i++)
			{
				ObscuredPrefs.SetInt("__a", 1);
				ObscuredPrefs.SetFloat("__b", 2f);
				ObscuredPrefs.SetString("__c", "3");
			}
			for (int j = 0; j < prefsIterations; j++)
			{
				ObscuredPrefs.GetInt("__a", 1);
				ObscuredPrefs.GetFloat("__b", 2f);
				ObscuredPrefs.GetString("__c", "3");
			}
			stopwatch.Stop();
			logBuilder.AppendLine("ObscuredPrefs:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			ObscuredPrefs.DeleteKey("__a");
			ObscuredPrefs.DeleteKey("__b");
			ObscuredPrefs.DeleteKey("__c");
			stopwatch.Reset();
			stopwatch.Start();
			for (int k = 0; k < prefsIterations; k++)
			{
				PlayerPrefs.SetInt("__a", 1);
				PlayerPrefs.SetFloat("__b", 2f);
				PlayerPrefs.SetString("__c", "3");
			}
			for (int l = 0; l < prefsIterations; l++)
			{
				PlayerPrefs.GetInt("__a", 1);
				PlayerPrefs.GetFloat("__b", 2f);
				PlayerPrefs.GetString("__c", "3");
			}
			stopwatch.Stop();
			logBuilder.AppendLine("PlayerPrefs:").AppendLine(stopwatch.ElapsedMilliseconds + " ms");
			PlayerPrefs.DeleteKey("__a");
			PlayerPrefs.DeleteKey("__b");
			PlayerPrefs.DeleteKey("__c");
		}
	}
}
