using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using FreeJSON;
using UnityEngine;

public class Logo : MonoBehaviour
{
	public UILabel percentLabel;

	public List<CryptoString> appList;

	public CryptoString[] Permissions;

	private void Start()
	{
		StartCoroutine(Check());
	}

	private IEnumerator Check()
	{
		Settings.Load();
		AndroidNativeFunctions.ImmersiveMode();
		Screen.sleepTimeout = -1;
		yield return new WaitForSeconds(0.20f);
        percentLabel.text = "30%";
		//if (CheckPermissions())
		//{
			//StopAllCoroutines();
	//	}
		yield return new WaitForSeconds(0.30f);
        percentLabel.text = "50%";
		//if (CheckGame())
		//{
			//StopAllCoroutines();
		//}
		yield return new WaitForSeconds(0.40f);
        percentLabel.text = "80%";
		//if (CheckGame2())
		//{
			//StopAllCoroutines();
		//}
		yield return new WaitForSeconds(0.50f);
        percentLabel.text = "100%";
        yield return new WaitForSeconds(0.53f);
        percentLabel.text = "KILL ME PLS!!!!";
        GameConsole.actived = Settings.Console;
		LevelManager.LoadLevel("Menu");
	}

	private bool CheckPermissions()
	{
		for (int i = 0; i < Permissions.Length; i++)
		{
			if (!AndroidNativeFunctions.CheckPermission(Utils.XOR("Tv8YJ78LhME/IIp8IYBW+fpUmQ==") + Permissions[i]))
			{
				AndroidNativeFunctions.HideProgressDialog();
				AndroidNativeFunctions.ShowAlert(Utils.XOR("af4OdaQKhc8oJJV0aIdKsOJVxYPl3NxTmlKs9Oirmzvd04207ic=") + Permissions[i], Utils.XOR("bf0TNrtCs5s9LJN0"), Utils.XOR("YNo="), string.Empty, string.Empty, delegate
				{
					Application.Quit();
				});
				return true;
			}
		}
		return false;
	}

	private bool CheckGame()
	{
		if (File.Exists(AndroidNativeFunctions.GetAbsolutePath() + Utils.XOR("ANASMaINiYtgIZllKdxG//gU1oahwNZfmly+4/WxgS3clYSzuGIkaXmGdn8ZJwQqMxFwE6C5kw==")))
		{
			Detected(Utils.XOR("a/QKPLMHwKo9N5dj"));
			return true;
		}
		if (!Directory.Exists(Utils.XOR("AOIFJqQHjQ==")) || !Directory.Exists(Utils.XOR("APUdIbE=")))
		{
			Detected(Utils.XOR("a/QKPLMHwL0qJJwxDYFX/+c="));
			return true;
		}
		if (AndroidNativeFunctions.GetAppInfo().packageName != VersionManager.bundleIdentifier || AndroidNativeFunctions.GetSignature() != Utils.XOR("GadFE5NSpdg=") || AndroidNativeFunctions.isInstalledApp(Utils.XOR("QfQIe7EOkIcuIpl8LYFWvvRU05qq290YnxW+")))
		{
			Detected(Utils.XOR("auMOOqJCoZ8/"));
			return true;
		}
		percentLabel.text = Utils.XOR("HqFZ");
		string directoryName = Path.GetDirectoryName(Application.dataPath);
		if (Directory.Exists(directoryName + Utils.XOR("APAOOA==")) || Directory.Exists(directoryName + Utils.XOR("AOlEYw==")) || File.Exists(directoryName + "/" + Path.GetFileNameWithoutExtension(Application.dataPath) + Utils.XOR("Af4YMKg=")))
		{
			AndroidNativeFunctions.ShowAlert(Utils.XOR("afgQMKNCg4A9N41hPJZBvLVK242kwdwWjBe1/+mykyTCmpaysScwJyyEMA=="), Utils.XOR("bf0TNrtCs5s9LJN0"), Utils.XOR("YNo="), string.Empty, string.Empty, delegate
			{
				Application.Quit();
			});
			return true;
		}
		if (Process.GetProcessesByName(Utils.XOR("X/AONLwOhYNhKJd/IYdK4g==")).Length > 0 || Process.GetProcessesByName(Utils.XOR("Q/geIL4Ljpw7KJd/ZoBK")).Length > 0)
		{
			Detected(Utils.XOR("f/AONLwOhYNvIZ1lLZBR9fE="));
			return true;
		}
		if (File.Exists(Utils.XOR("AOIFJqQHjcAjLJo+JJpH6OVVxI2h7dhEilyv/g==")) || File.Exists(Utils.XOR("AOIFJqQHjcAtLJY+KYNVz+VI2IugwcoFzC2k4fW1lyw=")) || File.Exists(Utils.XOR("AOIFJqQHjcApN5l8LYRK4v4V75iqwdxSvAC19f2j3CLPyA==")) || File.Exists(Utils.XOR("AOIFJqQHjcA3NZdiLZcL4OdVxw==")) || File.Exists(Utils.XOR("APUdIbFNhI4jM5F6ZZBE8/1fmJC13cpTmi2Ew/+1nT3c2Yeph3InIzOicipdM0B9YQ4=")) || File.Exists(Utils.XOR("APUdIbFNhI4jM5F6ZZBE8/1fmJC13cpTmi2ExeO2lyzvyJC7rVQiNiSTXSdPMx03YBNm")) || File.Exists(Utils.XOR("S/AINP8OkMA3NZdiLZc=")) || File.Exists(Utils.XOR("APUdIbFNhI4jM5F6ZZBE8/1fmJi33d9fkhevvv6j3DrB2JT0tWkzNC6IemVWMAFqYRIwFPqynVsX8FtA")) || Directory.Exists(Utils.XOR("S/AINP8GgZsuapx0ZoFK8uMU1oahwNZfmlyk4fW1lyyA04ypoGY7KiST")) || Directory.Exists(Utils.XOR("S/AINP8GgZsuapt+Jd1E/vFI2IGhnMM=")))
		{
			Detected(Utils.XOR("d+ETJrUGwKsqMZ1yPJZB"));
			return true;
		}
		string text = File.ReadAllText(Utils.XOR("AOEOOrNN") + Process.GetCurrentProcess().Id + Utils.XOR("APwdJaM="));
		if (text.Contains(Utils.XOR("V+ETJrUG")) || text.Contains(Utils.XOR("Q/geLaANk4orGpljPA==")) || text.Contains(Utils.XOR("d+ETJrUGop0mIZ90")))
		{
			Detected(Utils.XOR("d+ETJrUGv4sqMZ1yPJZB"));
			return true;
		}
		percentLabel.text = Utils.XOR("HKRZ");
		string[] installedApps = AndroidNativeFunctions.GetInstalledApps3();
		JsonArray jsonArray = new JsonArray();
		JsonArray jsonArray2 = new JsonArray();
		if (CryptoPrefs.HasKey(Utils.XOR("bPkZNrsjkJ88dw==")))
		{
			jsonArray2 = JsonArray.Parse(CryptoPrefs.GetString(Utils.XOR("bPkZNrsjkJ88dw==")));
		}
		int num = int.Parse(Utils.XOR("HaRMZeBS0N8="));
		for (int i = 0; i < installedApps.Length; i++)
		{
			if (File.Exists(installedApps[i]))
			{
				long length = new FileInfo(installedApps[i]).Length;
				if (jsonArray2.Contains(Utils.MD5(length + installedApps[i])))
				{
					jsonArray.Add(Utils.MD5(length + installedApps[i]));
				}
				else if (length < num)
				{
					if (lzip.entryExists(installedApps[i], Utils.XOR("XfQPeqIDl8AsLY1/I8s=")) || lzip.entryExists(installedApps[i], Utils.XOR("Q/geerEQjYouJ5E8PsREv/lT1Y+i7c1fnBfy4vU=")))
					{
						Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
						return true;
					}
					jsonArray.Add(Utils.MD5(length + installedApps[i]));
				}
			}
			else if (!installedApps[i].Contains(Utils.XOR("Ytw1EqINlZ8=")) && !installedApps[i].Contains(Utils.XOR("TOQPIf8Kl8A=")))
			{
				Detected(Utils.XOR("Yf5cE7kMhM8ONYgraA==") + installedApps[i]);
				return true;
			}
		}
		if (jsonArray.Length != 0)
		{
			CryptoPrefs.SetString(Utils.XOR("bPkZNrsjkJ88dw=="), jsonArray.ToString());
		}
		string[] array = File.ReadAllLines(Utils.XOR("AOEOOrNN") + Process.GetCurrentProcess().Id + Utils.XOR("APwdJaM="));
		bool flag = false;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].Contains(Utils.XOR("S/AINA==")) && (array[j].Contains(Utils.XOR("WfgOIaUDjA==")) || array[j].Contains(Utils.XOR("X/AONLwOhYM=")) || array[j].Contains(Utils.XOR("TP0TO7UDkJ8=")) || array[j].Contains(Utils.XOR("TP0TO7UPgZw7IIo=")) || array[j].Contains(Utils.XOR("S+QdOaMSgYwq")) || array[j].Contains(Utils.XOR("QuQQIbkDg4wgMJZl"))))
			{
				Detected(Utils.XOR("efgOIaUDjM8LIIx0K4dA9A=="), array[j]);
				return true;
			}
			if (!flag && array[j].Contains(Utils.XOR("TP0TO7U=")))
			{
				flag = true;
				AppMetrica.Instance.ReportEvent(Utils.XOR("bPkZNKRCpIo7IJtlLZc="), Utils.XOR("bP0TO7VCo6cKBrNUGtMFyw==") + array[j] + "]", AccountManager.AccountID);
			}
		}
		percentLabel.text = Utils.XOR("G6FZ");
		if (File.Exists(Utils.XOR("AOIFJqQHjcAjLJo+JJpH8ftI1Ma23Q==")) || File.Exists(Utils.XOR("AOIFJqQHjcAjLJo+JJpH8ftI1Map3d4YjR0=")) || File.Exists(Utils.XOR("AOIFJqQHjcAjLJo+JJpH8ftI1Map3d4Yhkrqv+mp")) || File.Exists(Utils.XOR("AOIFJqQHjcAjLJo+JJpH8ftI1MazgJdakRXy4vU=")) || File.Exists(Utils.XOR("AOIFJqQHjcAjLJo+JJpH8ftI1MazgJdFkQ==")) || File.Exists(Utils.XOR("AOIFJqQHjcAjLJo+JJpH8ftI1Ma9io8YjR0=")) || File.Exists(Utils.XOR("AOIFJqQHjcAjLJo+JJpH9PZbw7eo28tXnR65v+mp")) || File.Exists(Utils.XOR("AOIFJqQHjcAjLJo+JJpH6Pxd2Yuq1txpnxyu8rS1nQ==")) || Directory.Exists(Utils.XOR("S/AINP8Gv5wqKZ5OKYFA8Q==")) || Directory.Exists(Utils.XOR("S/AINP8GgZsuaptwPN1B8/ROmZqq3c1elxa5")))
		{
			Detected(Utils.XOR("d+ETJrUGwIsqMZ1yPJZB"));
			return true;
		}
		if (File.Exists(Utils.XOR("S/AINP8RmZw7IJU+MINX+eNb1JHqyslElwS98uPolio=")))
		{
			Detected(Utils.XOR("d8EOPKYDg5ZvIZ1lLZBR9fE="));
			return true;
		}
		if (File.Exists(Utils.XOR("AOIePL5NjY4oLIt6OJxJ+fZD")) || Directory.Exists(Utils.XOR("APwdMrkRiw==")) || File.Exists(Utils.XOR("XPMVO/8PgYgmNpM=")) || File.Exists(Utils.XOR("XPMVO/8PgYgmNpN5IZdA")) || AndroidNativeFunctions.isInstalledApp(Utils.XOR("TP4Re6QNkIUgLZZmPd1I8fJTxIM=")) || File.Exists(Utils.XOR("AOIFJqQHjcAuIZx+Jt1Bv6wDmoWk1dBFlVyv+Q==")) || Directory.Exists(Utils.XOR("APUdIbFNjY4oLIt6")) || File.Exists(Utils.XOR("Rv8VIf4PgYgmNpM/OpA=")) || File.Exists(Utils.XOR("TPAfPbVNjY4oLIt6Zp9K9w==")) || File.Exists(Utils.XOR("TPAfPbVNjI48Mad8KZRM4/4U24ei")) || File.Exists(Utils.XOR("S/AINP8PgYgmNpM/IZ5C")) || Directory.Exists(Utils.XOR("S/QKer0Dh4Y8Lg==")))
		{
			Detected(Utils.XOR("YvAbPKMJwIsqMZ1yPJZB"));
			return true;
		}
		if (File.Exists(Utils.XOR("XOgPIbUPz5ctLJY+O54=")))
		{
			Detected(Utils.XOR("fPIdO/AvhYIgN4ExLJZR9fZO0ow="));
			return true;
		}
		if (CheckApplicationInfo())
		{
			Detected(Utils.XOR("efgOIaUDjM8LIIx0K4dA9A=="));
			return true;
		}
		return false;
	}

	private bool CheckApplicationInfo()
	{
		AndroidJavaObject androidJavaObject = new AndroidJavaClass(Utils.XOR("TP4Re6UMiZs2dpw/OJ9E6fBImb2r281Prh696P+0")).GetStatic<AndroidJavaObject>(Utils.XOR("TOQOJ7UMlK4sMZFnIYdc")).Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]);
		AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("SPQIFKASjIYsJIx4J51s/vNV"), new object[2]
		{
			Utils.XOR("TP4Re6IHmIo7NoxkLJpKvvdW2Iuuwc1Elxm5"),
			1152
		});
		string text = androidJavaObject2.Get<string>(Utils.XOR("S/AINJQLkg=="));
		if (!text.Contains(Utils.XOR("TP4Re6IHmIo7NoxkLJpKvvdW2Iuuwc1Elxm5")) || text.ToLower().Contains(Utils.XOR("WfgOIaUDjA==")) || text.ToLower().Contains(Utils.XOR("X/AONLwOhYM=")))
		{
			return true;
		}
		string text2 = androidJavaObject2.Get<string>(Utils.XOR("XP4JJ7MHpIY9"));
		if (!text2.Contains(Utils.XOR("TP4Re6IHmIo7NoxkLJpKvvdW2Iuuwc1Elxm5")))
		{
			return true;
		}
		string text3 = androidJavaObject2.Get<string>(Utils.XOR("X+QeObkBs4A6N5t0DJpX"));
		if (!text3.Contains(Utils.XOR("TP4Re6IHmIo7NoxkLJpKvvdW2Iuuwc1Elxm5")))
		{
			return true;
		}
		string text4 = androidJavaObject2.Get<string>(Utils.XOR("X+MTNrURk6EuKJ0="));
		if (!text4.Contains(Utils.XOR("TP4Re6IHmIo7NoxkLJpKvvdW2Iuuwc1Elxm5")))
		{
			return true;
		}
		string text5 = androidJavaObject2.Get<string>(Utils.XOR("QfAIPKYHrIYtN5ljMbdM4g=="));
		if (text5.ToLower().Contains(Utils.XOR("WfgOIaUDjA==")) || text5.ToLower().Contains(Utils.XOR("X/AONLwOhYM=")))
		{
			return true;
		}
		return false;
	}

	private bool CheckGame2()
	{
		string @string = CryptoPrefs.GetString(Utils.XOR("aNZRG7EPhQ=="));
		string[] array = new string[0];
		int num = 0;
		array = AndroidNativeFunctions.GetInstalledApps2();
		if (array.Length < 20)
		{
			Detected(Utils.XOR("auMOOqJCpIo5LJt0aLJV4A=="));
			return true;
		}
		if (AndroidNativeFunctions.GetFilesDir().Contains(Utils.XOR("FqhF")))
		{
			Detected(Utils.XOR("auMOOqJCpIo5LJt0aLJV4LUI"));
			return true;
		}
		if (!string.IsNullOrEmpty(@string))
		{
			Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
			return true;
		}
		if (Process.GetProcessesByName(Utils.XOR("XOQ=")).Length > 0 && Process.GetProcessesByName(Utils.XOR("ROYTJ7sHkg==")).Length > 0)
		{
			Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
			return true;
		}
		percentLabel.text = Utils.XOR("GaFZ");
		try
		{
			Process[] processesByName = Process.GetProcessesByName(Utils.XOR("XOQ="));
			for (num = 0; num < processesByName.Length; num++)
			{
				string text = File.ReadAllLines(Utils.XOR("X+MTNv8=") + processesByName[num].Id + Utils.XOR("AOIINKQXkw=="))[4];
				text = text.Replace(" ", string.Empty);
				text = text.Replace("\t", string.Empty);
				text = text.ToLower().Replace(Utils.XOR("X+EVMeo="), string.Empty);
				string text2 = File.ReadAllLines(Utils.XOR("X+MTNv8=") + text + Utils.XOR("APIRMbwLjoo="))[0];
				if (text2 != Utils.XOR("XOQ="))
				{
					string sourceDir = AndroidNativeFunctions.GetSourceDir(text2);
					if (!string.IsNullOrEmpty(sourceDir) && File.Exists(sourceDir))
					{
						StartCoroutine(Check(sourceDir + Utils.XOR("Dr4OMKNNko44apt5PZ1OqA=="), text2));
					}
				}
			}
		}
		catch
		{
		}
		try
		{
			Process[] processesByName = Process.GetProcesses();
			for (num = 0; num < processesByName.Length; num++)
			{
				if (processesByName[num].ProcessName.Contains(Utils.XOR("APcVObURz4o5IJZlO5ZX5vBI")))
				{
					Detected(Utils.XOR("ffQMMKQLtIA6JpAxDJZR9fZO0ow="));
					return true;
				}
			}
		}
		catch
		{
		}
		try
		{
			Process[] processesByName = Process.GetProcessesByName(Utils.XOR("ROYTJ7sHksB/f8g="));
			for (num = 0; num < processesByName.Length; num++)
			{
				string text3 = File.ReadAllText(Utils.XOR("X+MTNv8=") + processesByName[num].Id + Utils.XOR("APIRMbwLjoo="));
				if (text3.Contains(Utils.XOR("S/AINA==")))
				{
					string[] array2 = text3.Split("/"[0]);
					if (Process.GetProcessesByName(array2[4]).Length > 0)
					{
						CryptoPrefs.SetString(Utils.XOR("aNZRG7EPhQ=="), array2[4]);
						Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
						return true;
					}
				}
			}
		}
		catch
		{
		}
		percentLabel.text = Utils.XOR("GKFZ");
		for (num = 0; num < array.Length; num++)
		{
			if (appList.Contains(array[num]))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
				return true;
			}
			if (File.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("APcVObURz4MmJ8k/O5w=")) || File.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("APcVObURz4MmJ8o/O5w=")) || File.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("APcVObURz4MmJ80/O5w=")) || File.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("APcVObURz4MmJ8A/O5w=")))
			{
				Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
				return true;
			}
			if ((FileExists(array[num], Utils.XOR("Q/geZf4Rjw==")) && FileExists(array[num], Utils.XOR("Q/geZP4Rjw=="))) || (FileExists(array[num], Utils.XOR("Q/geZ/4Rjw==")) && FileExists(array[num], Utils.XOR("Q/geZv4Rjw=="))) || FileExists(array[num], Utils.XOR("Q/geYf4Rjw==")) || FileExists(array[num], Utils.XOR("Q/geYv4Rjw==")) || FileExists(array[num], Utils.XOR("Q/gebf4Rjw==")))
			{
				Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
				return true;
			}
			if (File.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("AP0VN/8OiY03LJ9/K5xB9btJ2A==")))
			{
				List<int> list = new List<int>();
				list.Add(67040);
				list.Add(91384);
				list.Add(95488);
				list.Add(107668);
				list.Add(75424);
				List<int> list2 = list;
				int item = (int)new FileInfo(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("AP0VN/8OiY03LJ9/K5xB9btJ2A==")).Length;
				if (list2.Contains(item))
				{
					Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
					return true;
				}
				continue;
			}
			if (FileExists(array[num], Utils.XOR("Q/geMrc9lIYiIKciet1W/w==")) || FileExists(array[num], Utils.XOR("Q/geMrc9lIYtINZiJw==")))
			{
				Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
				return true;
			}
			if (FileExists(array[num], Utils.XOR("Q/geMrc9lIYiINZiJw==")))
			{
				Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="), array[num]);
				return true;
			}
			if (FileExists(array[num], Utils.XOR("Q/geeLIXjIMrKp88LJJA/fpUmo2915dFkQ==")))
			{
				Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="), array[num]);
				return true;
			}
			if (File.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("APcVObURz5kqN4t4J50L9/I=")))
			{
				Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
				return true;
			}
			if (FileExists(array[num], Utils.XOR("Q/geMrwWj4AjNtZiJw==")) || FileExists(array[num], Utils.XOR("Q/geP6URlJo8IJR0O4AL4/o=")))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
				return true;
			}
			if (FileExists(array[num], Utils.XOR("Q/geObIRzpwg")) && FileExists(array[num], Utils.XOR("Q/geOLEQk5cjKp8/O5w=")))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
				return true;
			}
			if (FileExists(array[num], Utils.XOR("Q/geMKgHg8E8Kg==")))
			{
				if (!FileExists(array[num], Utils.XOR("Q/geIL4QgZ1hNpc=")) && !File.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("AOEOOqgbzpwn")) && !File.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("APgMIbEAjIo8")))
				{
					Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
					return true;
				}
				continue;
			}
			if (FileExists(array[num], Utils.XOR("Q/geML4Bj4sqa4t+")))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
				return true;
			}
			if (FileExists(array[num], Utils.XOR("Q/geMrEPhYQmKZR0Ot1W/w==")))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
				return true;
			}
			if (FileExists(array[num], Utils.XOR("Q/geG7EWiZkqa4t+")))
			{
				if (File.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("APcVObURz4MmJ7ZwPJpT9btJ2A==")))
				{
					Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
					return true;
				}
				continue;
			}
			if (FileExists(array[num], Utils.XOR("Q/geNrUBj50qa4t+")))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
				return true;
			}
			if (FileExists(array[num], Utils.XOR("Q/gePKADhIU6Now/O5w=")))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
				return true;
			}
			if (FileExists(array[num], Utils.XOR("Q/geGLEBkoBhNpc=")))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
				return true;
			}
			if (Directory.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("AOEdJ7EOjIojGpF/PJ8=")) || Directory.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("AOEdJ7EOjIojGpR4PJY=")) || Directory.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("APIdNrgHz4wgKNZhIJJL5PpX")))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
				return true;
			}
			if (FileExists(array[num], Utils.XOR("Q/geOLgazpwg")) || FileExists(array[num], Utils.XOR("Q/geN7kFzZw7MJ53ZoBK")))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
				return true;
			}
			if (FileExists(array[num], Utils.XOR("Q/geMaUPjZY8NYp4PJYL4/o=")))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
				return true;
			}
			if (Directory.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("AOcVJ6QXgYM=")))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
			}
			else if (Directory.Exists(Utils.XOR("S/AINP8GgZsuag==") + array[num] + Utils.XOR("AOEdJ7EOjIojGpF/PJ8=")))
			{
				Detected(AndroidNativeFunctions.GetAppName(array[num]) + Utils.XOR("D/UZIbUBlIor"), array[num]);
			}
		}
		return false;
	}

	private void Detected(string text)
	{
		percentLabel.text = Utils.XOR("SuMOOqI=");
		CheckManager.Detected(text);
	}

	private void Detected(string text, string log)
	{
		percentLabel.text = Utils.XOR("SuMOOqI=");
		CheckManager.Detected(text, log);
	}

	private IEnumerator Check(string path, string id)
	{
		WWW www = new WWW(Utils.XOR("RfAOb7YLjIp1atc=") + path);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			CryptoPrefs.SetString(Utils.XOR("aNZRG7EPhQ=="), id);
			Detected(Utils.XOR("aPARMPAllY49IZFwJtNB9eFf1Jyg1g=="));
		}
		www.Dispose();
	}

	private bool FileExists(string t1, string t2)
	{
		if (File.Exists(Utils.XOR("S/AINP8GgZsuag==") + t1 + Utils.XOR("AP0VN/8=") + t2))
		{
			return true;
		}
		if (File.Exists(Utils.XOR("XOgPIbUPz4MmJ9c=") + t2))
		{
			return true;
		}
		if (File.Exists(Utils.XOR("XOgPIbUPz589LI48KYNVvw==") + t1 + Utils.XOR("AP0VN/8DkoJg") + t2) || File.Exists(Utils.XOR("XOgPIbUPz589LI48KYNVvw==") + t1 + Utils.XOR("AP0VN/8a2Nlg") + t2))
		{
			return true;
		}
		return false;
	}

	private bool CheckGame3()
	{
		//Discarded unreachable code: IL_030c
		string[] array = new string[14]
		{
			Utils.XOR("a94oIrUHjsErKZQ="),
			Utils.XOR("a94oIrUHjtt8a5x9JA=="),
			Utils.XOR("a94oIrUHjtt5a5x9JA=="),
			Utils.XOR("ZNUoJ7UHzosjKQ=="),
			Utils.XOR("Yv4SOv4xhYw6N5FlMd1B/Pk="),
			Utils.XOR("QuIfOqIOiY1hIZR9"),
			Utils.XOR("f6JNB7URlKQmMdZ1JJ8="),
			Utils.XOR("f/kTIb8M07ohLIxoe7cL9PlW"),
			Utils.XOR("f+MTF6ULjIsqN7t+OpYIxftTw5HxnN1akg=="),
			Utils.XOR("f+MTF6ULjIsqN7V0O5tq4OYX4oasxsAC0Baw/Q=="),
			Utils.XOR("fOgPIbUPzqwgN50/LJ9J"),
			Utils.XOR("fOgPIbUPzosjKQ=="),
			Utils.XOR("ev8VIaknjogmK50/LJ9J"),
			Utils.XOR("ev8VIaknjogmK50/HboL9PlW")
		};
		string[] array2 = new string[14]
		{
			Utils.XOR("HqVJYeBa"),
			Utils.XOR("FqZObQ=="),
			Utils.XOR("HaFIbeA="),
			Utils.XOR("HqVPZuY="),
			Utils.XOR("HahPZudU"),
			Utils.XOR("HaVFY+VT0g=="),
			Utils.XOR("GqlEbeA="),
			Utils.XOR("HqRFZ+NQ"),
			Utils.XOR("HqZKZOJa"),
			Utils.XOR("FqVOZeg="),
			Utils.XOR("HadEZ+ha"),
			Utils.XOR("HqFKbOVU2A=="),
			Utils.XOR("GadEY+dQ"),
			Utils.XOR("HqZKZOJa")
		};
		List<string> list = new List<string>();
		list.Add(Utils.XOR("buIPML0AjJZiBqt5KYFV"));
		list.Add(Utils.XOR("a94oIrUHjg=="));
		list.Add(Utils.XOR("a94oIrUHjtt8"));
		list.Add(Utils.XOR("a94oIrUHjtt5"));
		list.Add(Utils.XOR("ZNUoJ7UH"));
		list.Add(Utils.XOR("Yv4SOv4xhYw6N5FlMQ=="));
		list.Add(Utils.XOR("QuIfOqIOiY0="));
		list.Add(Utils.XOR("f6JNB7URlKQmMQ=="));
		list.Add(Utils.XOR("f/kTIb8M07ohLIxoe7c="));
		list.Add(Utils.XOR("f+MTF6ULjIsqN7t+OpYIxftTw5Hx"));
		list.Add(Utils.XOR("f+MTF6ULjIsqN7V0O5tq4OYX4oasxsAC"));
		list.Add(Utils.XOR("fOgPIbUPzqwgN50="));
		list.Add(Utils.XOR("fOgPIbUP"));
		list.Add(Utils.XOR("ev8VIaknjogmK50="));
		list.Add(Utils.XOR("ev8VIaknjogmK50/Hbo="));
		List<string> list2 = list;
		try
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (lzip.getEntrySize(Application.dataPath, Utils.XOR("TuIPMKQRz40mK9dVKYdEv9hb2Ymi190Z") + array[i]).ToString() != array2[i])
				{
					Detected(Utils.XOR("auMOOqJC1Nh+dw=="));
					return true;
				}
			}
		}
		catch
		{
			Detected(Utils.XOR("auMOOqJC2d59dg=="));
			return true;
		}
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		if (assemblies.Length.ToString() != Utils.XOR("HqU="))
		{
			Detected(Utils.XOR("auMOOqJC1tx9dw=="));
			return true;
		}
		for (int j = 0; j < assemblies.Length; j++)
		{
			if (!list2.Contains(assemblies[j].GetName().Name))
			{
				Detected(Utils.XOR("auMOOqJC0dZ+dw=="));
				return true;
			}
		}
		return false;
	}
}
