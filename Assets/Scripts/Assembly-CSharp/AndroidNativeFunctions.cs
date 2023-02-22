using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using FreeJSON;
using UnityEngine;
using UnityEngine.Events;

public class AndroidNativeFunctions : MonoBehaviour
{
	private class ShowAlertListener : AndroidJavaProxy
	{
		private UnityAction<DialogInterface> action;

		public ShowAlertListener(UnityAction<DialogInterface> a)
			: base(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtFT1oSq1fBYiheu9/ull2zh1KG2vWQ8CiiSai5AJRw="))
		{
			action = a;
		}

		public void onClick(AndroidJavaObject obj, int which)
		{
			if (action != null)
			{
				action((DialogInterface)which);
			}
		}
	}

	private class ShowAlertInputListener : AndroidJavaProxy
	{
		private UnityAction<DialogInterface, string> action;

		private AndroidJavaObject editText;

		public ShowAlertInputListener(UnityAction<DialogInterface, string> a, AndroidJavaObject et)
			: base(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtFT1oSq1fBYiheu9/ull2zh1KG2vWQ8CiiSai5AJRw="))
		{
			action = a;
			editText = et;
		}

		public void onClick(AndroidJavaObject obj, int which)
		{
			if (action != null)
			{
				action((DialogInterface)which, editText.Call<AndroidJavaObject>(Utils.XOR("SPQIAbUalA=="), new object[0]).Call<string>(Utils.XOR("W/4vIaILjog="), new object[0]));
			}
		}
	}

	private class ShowAlertListListener : AndroidJavaProxy
	{
		private string[] list;

		private UnityAction<string> action;

		public ShowAlertListListener(UnityAction<string> w, string[] a)
			: base(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtFT1oSq1fBYiheu9/ull2zh1KG2vWQ8CiiSai5AJRw="))
		{
			action = w;
			list = a;
		}

		public void onClick(AndroidJavaObject obj, int which)
		{
			action(list[which]);
		}
	}

	public delegate void Callback();

	private static bool immersiveMode;

	private static AndroidNativeFunctions instance;

	private static AndroidJavaObject progressDialog;

	private static AndroidJavaObject _currentActivity;

	private static AndroidJavaObject currentActivity
	{
		get
		{
			if (_currentActivity == null)
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("TP4Re6UMiZs2dpw/OJ9E6fBImb2r281Prh696P+0")))
				{
					_currentActivity = androidJavaClass.GetStatic<AndroidJavaObject>(Utils.XOR("TOQOJ7UMlK4sMZFnIYdc"));
				}
			}
			return _currentActivity;
		}
	}

	private static void CreateGO()
	{
		if (!(instance != null))
		{
			GameObject gameObject = new GameObject(Utils.XOR("bv8YJ78LhKEuMZFnLbVQ/vZO3oerwQ=="));
			instance = gameObject.AddComponent<AndroidNativeFunctions>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		if (immersiveMode && focusStatus)
		{
			ImmersiveMode();
		}
	}

	public static void ImmersiveMode()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		int @static = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9LFs8rqW+/Z4")).GetStatic<int>(Utils.XOR("fNU3CpkstA=="));
		if (@static >= 19)
		{
			CreateGO();
			immersiveMode = true;
			currentActivity.Call(Utils.XOR("XeQSGr43ibsnN51wLA=="), (AndroidJavaRunnable)delegate
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhME5LJ1mZqVM9eI="));
				AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SfgSMYYLhZgNPLF1"), new object[1] { new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEdYZF1")).GetStatic<int>(Utils.XOR("TP4SIbUMlA==")) });
				androidJavaObject.Call(Utils.XOR("XPQIBqkRlIoiEJFHIYBM8vxW3py8"), androidJavaClass.GetStatic<int>(Utils.XOR("fMgvAZUvv7oGGr5dCbR63NRj+L2R7epivzCQ1A==")) | androidJavaClass.GetStatic<int>(Utils.XOR("fMgvAZUvv7oGGr5dCbR63NRj+L2R7fF/ujeD39uQuw/v7quVmg==")) | androidJavaClass.GetStatic<int>(Utils.XOR("fMgvAZUvv7oGGr5dCbR63NRj+L2R7f9jsj6P0siDtwY=")) | androidJavaClass.GetStatic<int>(Utils.XOR("fMgvAZUvv7oGGr5dCbR62Nx+8reL8+9/uTOI2NWI")) | androidJavaClass.GetStatic<int>(Utils.XOR("fMgvAZUvv7oGGr5dCbR61sB2+7uG4PxzsA==")) | androidJavaClass.GetStatic<int>(Utils.XOR("fMgvAZUvv7oGGr5dCbR62dh38rqW++9zoSGI2NmNqw==")));
			});
		}
	}

	public static string GetExternalStorageDirectory()
	{
		return new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZUJoVM4vpU2o2rxg==")).CallStatic<AndroidJavaObject>(Utils.XOR("SPQIEKgWhZ0hJJRCPJxX8fJf84G319pCkQCl"), new object[0]).Call<string>(Utils.XOR("W/4vIaILjog="), new object[0]);
	}

	public static string GetFilesDir()
	{
		return currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIE7kOhZwLLIo="), new object[0]).Call<string>(Utils.XOR("SPQIBbEWiA=="), new object[0]);
	}

	public static void StartApp(string packageName, bool isExitThisApp)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIGbEXjownDJZlLZ1R1vpI54mm2dhRmw=="), new object[1] { packageName });
			currentActivity.Call(Utils.XOR("XOUdJ6Qjg5smM5FlMQ=="), androidJavaObject);
			if (isExitThisApp)
			{
				Application.Quit();
			}
		}
	}

	public static string GetAppName(string packageName)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return string.Empty;
		}
		AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]);
		AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("SPQIFKASjIYsJIx4J51s/vNV"), new object[2] { packageName, 0 });
		return androidJavaObject.Call<string>(Utils.XOR("SPQIFKASjIYsJIx4J51p8fdf2w=="), new object[1] { androidJavaObject2 });
	}

	public static List<PackageInfo> GetInstalledApps()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return new List<PackageInfo>();
		}
		AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIHL4RlI4jKZ11GJJG+/Rd0ps="), new object[1] { 0 });
		int num = androidJavaObject.Call<int>(Utils.XOR("XPgGMA=="), new object[0]);
		List<PackageInfo> list = new List<PackageInfo>();
		for (int i = 0; i < num; i++)
		{
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("SPQI"), new object[1] { i });
			PackageInfo packageInfo = new PackageInfo();
			packageInfo.firstInstallTime = androidJavaObject2.Get<long>(Utils.XOR("SfgOJqQrjpw7JJR9HJpI9Q=="));
			packageInfo.packageName = androidJavaObject2.Get<string>(Utils.XOR("X/AfPrEFhaEuKJ0="));
			packageInfo.lastUpdateTime = androidJavaObject2.Get<long>(Utils.XOR("Q/APIYUShI47IKx4JZY="));
			packageInfo.versionCode = androidJavaObject2.Get<int>(Utils.XOR("WfQOJrkNjqwgIZ0="));
			packageInfo.versionName = androidJavaObject2.Get<string>(Utils.XOR("WfQOJrkNjqEuKJ0="));
			list.Add(packageInfo);
		}
		return list;
	}

	public static string[] GetInstalledApps2()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return new string[1] { Utils.XOR("e/QPIQ==") };
		}
		AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIHL4RlI4jKZ11GJJG+/Rd0ps="), new object[1] { 128 });
		int num = androidJavaObject.Call<int>(Utils.XOR("XPgGMA=="), new object[0]);
		string[] array = new string[num];
		for (int i = 0; i < num; i++)
		{
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("SPQI"), new object[1] { i });
			array[i] = androidJavaObject2.Get<string>(Utils.XOR("X/AfPrEFhaEuKJ0="));
		}
		return array;
	}

	public static string[] GetInstalledApps3()
	{
		//Discarded unreachable code: IL_0064
		if (Application.platform != RuntimePlatform.Android)
		{
			return new string[0];
		}
		string text = new AndroidJavaClass(Utils.XOR("TP4Re6IHmIo7NoxkLJpKvvZS0ouu08lGjQ==")).CallStatic<string>(Utils.XOR("aPQIFKASk6MmNow="), new object[0]);
		if (string.IsNullOrEmpty(text))
		{
			return new string[0];
		}
		JsonArray jsonArray;
		try
		{
			jsonArray = JsonArray.Parse(text);
		}
		catch
		{
			return new string[0];
		}
		string[] array = new string[jsonArray.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = jsonArray.Get<string>(i);
			array[i] = array[i].Replace("\\", string.Empty);
			array[i] = array[i].Replace("\"", string.Empty);
		}
		return array;
	}

	public static PackageInfo GetAppInfo()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return new PackageInfo();
		}
		return GetAppInfo(currentActivity.Call<string>(Utils.XOR("SPQIBbEBi44oILZwJZY="), new object[0]));
	}

	public static PackageInfo GetAppInfo(string packageName)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return null;
		}
		AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILF/Lpw="), new object[2] { packageName, 0 });
		PackageInfo packageInfo = new PackageInfo();
		packageInfo.firstInstallTime = androidJavaObject.Get<long>(Utils.XOR("SfgOJqQrjpw7JJR9HJpI9Q=="));
		packageInfo.packageName = androidJavaObject.Get<string>(Utils.XOR("X/AfPrEFhaEuKJ0="));
		packageInfo.lastUpdateTime = androidJavaObject.Get<long>(Utils.XOR("Q/APIYUShI47IKx4JZY="));
		packageInfo.versionCode = androidJavaObject.Get<int>(Utils.XOR("WfQOJrkNjqwgIZ0="));
		packageInfo.versionName = androidJavaObject.Get<string>(Utils.XOR("WfQOJrkNjqEuKJ0="));
		return packageInfo;
	}

	public static string GetSourceDir(string packageName)
	{
		//Discarded unreachable code: IL_0076, IL_0087
		if (Application.platform != RuntimePlatform.Android)
		{
			return null;
		}
		try
		{
			AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILF/Lpw="), new object[2] { packageName, 1024 });
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Get<AndroidJavaObject>(Utils.XOR("TuEMObkBgZsmKpZYJpVK"));
			return androidJavaObject2.Get<string>(Utils.XOR("XP4JJ7MHpIY9"));
		}
		catch
		{
			return string.Empty;
		}
	}

	public static DeviceInfo GetDeviceInfo()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return null;
		}
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9LFs8rqW+/Z4"));
		DeviceInfo deviceInfo = new DeviceInfo();
		deviceInfo.CODENAME = androidJavaClass.GetStatic<string>(Utils.XOR("bN44EJ4jrao="));
		deviceInfo.INCREMENTAL = androidJavaClass.GetStatic<string>(Utils.XOR("Zt8/B5UvpaEbBLQ="));
		deviceInfo.RELEASE = androidJavaClass.GetStatic<string>(Utils.XOR("fdQwEJExpQ=="));
		deviceInfo.SDK = androidJavaClass.GetStatic<int>(Utils.XOR("fNU3CpkstA=="));
		return deviceInfo;
	}

    public static string GetAndroidID()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
        AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>("getContentResolver", new object[0]);
        return new AndroidJavaClass("android.provider.Settings$Secure").CallStatic<string>("getString", new object[2] { androidJavaObject, "android_id" });
    }

    public static string GetAndroidID2()
	{
		string text = GetAndroidSerial();
		int @static = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9LFs8rqW+/Z4")).GetStatic<int>(Utils.XOR("fNU3CpkstA=="));
		if (string.IsNullOrEmpty(text) || text.Contains(Utils.XOR("H6BOZuRX1th3fA==")))
		{
			text = ((@static < 26) ? (Utils.XOR("Hs4=") + currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[1] { Utils.XOR("X/kTO7U=") }).Call<string>(Utils.XOR("SPQIEbUUiYwqDJw="), new object[0])) : (Utils.XOR("Hs4=") + currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[1] { Utils.XOR("X/kTO7U=") }).Call<string>(Utils.XOR("SPQIHL0HiQ=="), new object[0])));
			if (string.IsNullOrEmpty(text) || text == Utils.XOR("Hs4=") || text.Contains(Utils.XOR("H6BOZuRX")) || text.Length < 5)
			{
				return Utils.XOR("Hc4=") + GetAndroidID();
			}
		}
		try
		{
			if (text.Contains(Utils.XOR("Wv8XO78Vjg==")))
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9A=="));
				return androidJavaClass.GetStatic<string>(Utils.XOR("SPQIBrUQiY4j"));
			}
			return text;
		}
		catch
		{
			text = ((@static < 26) ? (Utils.XOR("Hs4=") + currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[1] { Utils.XOR("X/kTO7U=") }).Call<string>(Utils.XOR("SPQIEbUUiYwqDJw="), new object[0])) : (Utils.XOR("Hs4=") + currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[1] { Utils.XOR("X/kTO7U=") }).Call<string>(Utils.XOR("SPQIHL0HiQ=="), new object[0])));
			if (string.IsNullOrEmpty(text) || text == Utils.XOR("Hs4=") || text.Contains(Utils.XOR("H6BOZuRX")) || text.Length < 5)
			{
				return Utils.XOR("Hc4=") + GetAndroidID();
			}
			return text;
		}
	}

	public static string GetAndroidSerial()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return SystemInfo.deviceUniqueIdentifier;
		}
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9A=="));
		return androidJavaClass.GetStatic<string>(Utils.XOR("fNQuHJEu"));
	}

	public static void ShareText(string text, string subject, string chooser)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtxUw42rxg=="));
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIFLMWiYAh"), new object[1] { Utils.XOR("Tv8YJ78LhMEmK4x0JocL8fZO3oernOpzsDY=") });
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIAakShQ=="), new object[1] { Utils.XOR("W/QEIf8SjI4mKw==") });
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[2]
			{
				Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr5vxuqg=="),
				text
			});
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[2]
			{
				Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr4ex0tDefxQ=="),
				subject
			});
			AndroidJavaObject androidJavaObject2 = androidJavaObject.CallStatic<AndroidJavaObject>(Utils.XOR("TOMZNKQHo4cgKot0Og=="), new object[2] { androidJavaObject, chooser });
			currentActivity.Call(Utils.XOR("XOUdJ6Qjg5smM5FlMQ=="), androidJavaObject2);
		}
	}

	public static void ShareImage(string text, string subject, string chooser, Texture2D picture)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			byte[] array = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhME6MZF9ZrFE4/AMgw==")).CallStatic<byte[]>(Utils.XOR("S/QfOrQH"), new object[2]
			{
				Convert.ToBase64String(picture.EncodeToPNG()),
				0
			});
			AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEoN5lhIJpG47t43pyo08lwnxGo/ui/")).CallStatic<AndroidJavaObject>(Utils.XOR("S/QfOrQHopY7ILljOpJc"), new object[3] { array, 0, array.Length });
			AndroidJavaObject @static = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEoN5lhIJpG47t43pyo08kSvR2x4eijgTvo1ZC3tXM=")).GetStatic<AndroidJavaObject>(Utils.XOR("ZcE5Eg=="));
			androidJavaObject.Call<bool>(Utils.XOR("TP4RJaIHk5w="), new object[3]
			{
				@static,
				100,
				new AndroidJavaObject(Utils.XOR("RfAKNP4Lj8ENPIx0CYFX8ex1wpy1x81ligC58Pc="))
			});
			string text2 = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhME/N5dnIZdA4rt30oys0+pCkQC5tdOrky/LycaXsWM+Jw==")).CallStatic<string>(Utils.XOR("Rv8PMKIWqYIuIp0="), new object[4]
			{
				currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIFr8MlIohMap0O5xJ5vBI"), new object[0]),
				androidJavaObject,
				picture.name,
				string.Empty
			});
			AndroidJavaObject androidJavaObject2 = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEhIIw/HYFM")).CallStatic<AndroidJavaObject>(Utils.XOR("X/AOJrU="), new object[1] { text2 });
			AndroidJavaObject androidJavaObject3 = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtxUw42rxg=="));
			androidJavaObject3.Call<AndroidJavaObject>(Utils.XOR("XPQIFLMWiYAh"), new object[1] { Utils.XOR("Tv8YJ78LhMEmK4x0JocL8fZO3oernOpzsDY=") });
			androidJavaObject3.Call<AndroidJavaObject>(Utils.XOR("XPQIAakShQ=="), new object[1] { Utils.XOR("RvwdMrVNyg==") });
			androidJavaObject3.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[2]
			{
				Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr4e1kuzOR"),
				androidJavaObject2
			});
			androidJavaObject3.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[2]
			{
				Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr5vxuqg=="),
				text
			});
			androidJavaObject3.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[2]
			{
				Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr4ex0tDefxQ=="),
				subject
			});
			AndroidJavaObject androidJavaObject4 = androidJavaObject3.CallStatic<AndroidJavaObject>(Utils.XOR("TOMZNKQHo4cgKot0Og=="), new object[2] { androidJavaObject3, chooser });
			currentActivity.Call(Utils.XOR("XOUdJ6Qjg5smM5FlMQ=="), androidJavaObject4);
		}
	}

	public static void ShareScreenshot(string text, string subject, string chooser, string screenshotPath)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtxUw42rxg=="));
			AndroidJavaObject androidJavaObject2 = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEhIIw/HYFM")).CallStatic<AndroidJavaObject>(Utils.XOR("X/AOJrU="), new object[1] { Utils.XOR("SfgQMOpNzw==") + screenshotPath });
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIFLMWiYAh"), new object[1] { Utils.XOR("Tv8YJ78LhMEmK4x0JocL8fZO3oernOpzsDY=") });
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIAakShQ=="), new object[1] { Utils.XOR("RvwdMrVNkIEo") });
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[2]
			{
				Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr4e1kuzOR"),
				androidJavaObject2
			});
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[2]
			{
				Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr5vxuqg=="),
				text
			});
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[2]
			{
				Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr4ex0tDefxQ=="),
				subject
			});
			AndroidJavaObject androidJavaObject3 = androidJavaObject.CallStatic<AndroidJavaObject>(Utils.XOR("TOMZNKQHo4cgKot0Og=="), new object[2] { androidJavaObject, chooser });
			currentActivity.Call(Utils.XOR("XOUdJ6Qjg5smM5FlMQ=="), androidJavaObject3);
		}
	}

	public static void ShowAlert(string message, string title, string positiveButton, string negativeButton, string neutralButton, UnityAction<DialogInterface> action)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		currentActivity.Call(Utils.XOR("XeQSGr43ibsnN51wLA=="), (AndroidJavaRunnable)delegate
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMAuNYg+CZ9A4uF+3omp3d4SvAe1/f6jgA=="), currentActivity);
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIGLURk44oIA=="), new object[1] { message });
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIFrEMg4ojJJp9LQ=="), new object[1] { false });
			if (!string.IsNullOrEmpty(title))
			{
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIAbkWjIo="), new object[1] { title });
			}
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIBb8RiZsmM51TPYdR//s="), new object[2]
			{
				positiveButton,
				new ShowAlertListener(action)
			});
			if (!string.IsNullOrEmpty(negativeButton))
			{
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIG7UFgZsmM51TPYdR//s="), new object[2]
				{
					negativeButton,
					new ShowAlertListener(action)
				});
			}
			if (!string.IsNullOrEmpty(neutralButton))
			{
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIG7UXlJ0uKbpkPIdK/g=="), new object[2]
				{
					neutralButton,
					new ShowAlertListener(action)
				});
			}
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("TOMZNKQH"), new object[0]);
			androidJavaObject2.Call(Utils.XOR("XPkTIg=="));
		});
	}

	public static void ShowAlertInput(string text, string message, string title, string positiveButton, string negativeButton, UnityAction<DialogInterface, string> action)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		currentActivity.Call(Utils.XOR("XeQSGr43ibsnN51wLA=="), (AndroidJavaRunnable)delegate
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMAuNYg+CZ9A4uF+3omp3d4SvAe1/f6jgA=="), currentActivity);
			AndroidJavaObject androidJavaObject2 = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhME4LJx2LYcL1fFTw7ygys0="), currentActivity);
			if (!string.IsNullOrEmpty(text))
			{
				androidJavaObject2.Call(Utils.XOR("XPQIAbUalA=="), text);
			}
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIA7kHlw=="), new object[1] { androidJavaObject2 });
			if (!string.IsNullOrEmpty(message))
			{
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIGLURk44oIA=="), new object[1] { message });
			}
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIFrEMg4ojJJp9LQ=="), new object[1] { false });
			if (!string.IsNullOrEmpty(title))
			{
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIAbkWjIo="), new object[1] { title });
			}
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIBb8RiZsmM51TPYdR//s="), new object[2]
			{
				positiveButton,
				new ShowAlertInputListener(action, androidJavaObject2)
			});
			if (!string.IsNullOrEmpty(negativeButton))
			{
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIG7UFgZsmM51TPYdR//s="), new object[2]
				{
					negativeButton,
					new ShowAlertInputListener(action, androidJavaObject2)
				});
			}
			AndroidJavaObject androidJavaObject3 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("TOMZNKQH"), new object[0]);
			androidJavaObject3.Call(Utils.XOR("XPkTIg=="));
		});
	}

	public static void ShowAlertList(string title, string[] list, UnityAction<string> action)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		currentActivity.Call(Utils.XOR("XeQSGr43ibsnN51wLA=="), (AndroidJavaRunnable)delegate
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMAuNYg+CZ9A4uF+3omp3d4SvAe1/f6jgA=="), currentActivity);
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIFrEMg4ojJJp9LQ=="), new object[1] { false });
			if (!string.IsNullOrEmpty(title))
			{
				androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIAbkWjIo="), new object[1] { title });
			}
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIHKQHjZw="), new object[2]
			{
				list,
				new ShowAlertListListener(action, list)
			});
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("TOMZNKQH"), new object[0]);
			androidJavaObject2.Call(Utils.XOR("XPkTIg=="));
		});
	}

	public static void ShowToast(string message)
	{
		ShowToast(message, true);
	}

	public static void ShowToast(string message, bool shortDuration)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			currentActivity.Call(Utils.XOR("XeQSGr43ibsnN51wLA=="), (AndroidJavaRunnable)delegate
			{
				AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhME4LJx2LYcLxPpbxJw="), currentActivity);
				androidJavaObject.CallStatic<AndroidJavaObject>(Utils.XOR("QvAXMIQHmJs="), new object[3]
				{
					currentActivity,
					message,
					(!shortDuration) ? 1 : 0
				}).Call(Utils.XOR("XPkTIg=="));
			});
		}
	}

	public static void ShowProgressDialog(string message)
	{
		ShowProgressDialog(message, string.Empty);
	}

	public static void ShowProgressDialog(string message, string title)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		if (progressDialog != null)
		{
			HideProgressDialog();
		}
		currentActivity.Call(Utils.XOR("XeQSGr43ibsnN51wLA=="), (AndroidJavaRunnable)delegate
		{
			progressDialog = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEuNYg/GIFK9+dfxJuB29hakRU="), currentActivity);
			progressDialog.Call(Utils.XOR("XPQIBaINh50qNotCPIpJ9Q=="), 0);
			progressDialog.Call(Utils.XOR("XPQIHL4GhZsqN5V4JpJR9Q=="), true);
			progressDialog.Call(Utils.XOR("XPQIFrEMg4ojJJp9LQ=="), false);
			progressDialog.Call(Utils.XOR("XPQIGLURk44oIA=="), message);
			if (!string.IsNullOrEmpty(title))
			{
				progressDialog.Call(Utils.XOR("XPQIAbkWjIo="), title);
			}
			progressDialog.Call(Utils.XOR("XPkTIg=="));
		});
	}

	public static void HideProgressDialog()
	{
		if (progressDialog != null)
		{
			currentActivity.Call(Utils.XOR("XeQSGr43ibsnN51wLA=="), (AndroidJavaRunnable)delegate
			{
				progressDialog.Call(Utils.XOR("R/gYMA=="));
				progressDialog.Call(Utils.XOR("S/gPOLkRkw=="));
				progressDialog = null;
			});
		}
	}

	public static void OpenGooglePlay(string packageName)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Application.OpenURL(Utils.XOR("QvAOPrUW2sBgIZ1lKZpJ46pT09U=") + packageName);
		}
		else
		{
			Application.OpenURL(Utils.XOR("R+UIJaNYz8A/KZloZpRK//JW0sam3dQZjQaz4//pkzjeyc2+sXM2Ly2SISJKfQ==") + packageName);
		}
	}

	public static bool isDeviceRooted()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		string[] array = new string[9]
		{
			Utils.XOR("AOIFJqQHjcAuNYg+G4ZV9edPxI23nNhGlQ=="),
			Utils.XOR("AOIePL5Nk5o="),
			Utils.XOR("AOIFJqQHjcAtLJY+O4Y="),
			Utils.XOR("AOIFJqQHjcA3J5F/Z4BQ"),
			Utils.XOR("APUdIbFNjIAsJJQ+MJFM/rpJwg=="),
			Utils.XOR("APUdIbFNjIAsJJQ+KppLv+ZP"),
			Utils.XOR("AOIFJqQHjcA8IddpKppLv+ZP"),
			Utils.XOR("AOIFJqQHjcAtLJY+LpJM/OZb0Y3qwcw="),
			Utils.XOR("APUdIbFNjIAsJJQ+O4Y=")
		};
		string[] array2 = array;
		foreach (string path in array2)
		{
			if (File.Exists(path))
			{
				return true;
			}
		}
		string @static = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9A==")).GetStatic<string>(Utils.XOR("e9A7Bg=="));
		if (@static != null && @static.Contains(Utils.XOR("W/QPIf0JhZY8")))
		{
			return true;
		}
		return false;
	}

	public static bool isInstalledApp(string packageName)
	{
		//Discarded unreachable code: IL_0052, IL_005f
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		try
		{
			currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILF/Lpw="), new object[2] { packageName, 0 });
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool isTVDevice()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		int num = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[1] { Utils.XOR("WvgROrQH") }).Call<int>(Utils.XOR("SPQIFqUQkoohMbV+LJZx6eVf"), new object[0]);
		if (num == 4)
		{
			return true;
		}
		return false;
	}

	public static bool isWiredHeadset()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		return currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[1] { Utils.XOR("TuQYPL8=") }).Call<bool>(Utils.XOR("RuIrPKIHhKcqJJxiLYdq/g=="), new object[0]);
	}

	public static void SetTotalVolume(int volumeLevel)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			volumeLevel = Mathf.Clamp(volumeLevel, 0, 15);
			currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[1] { Utils.XOR("TuQYPL8=") }).Call(Utils.XOR("XPQIBqQQhY4iE5d9PZ5A"), 3, volumeLevel, 0);
		}
	}

	public static int GetTotalVolume()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return 0;
		}
		return currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[1] { Utils.XOR("TuQYPL8=") }).Call<int>(Utils.XOR("SPQIBqQQhY4iE5d9PZ5A"), new object[1] { 3 });
	}

	public static bool isConnectInternet()
	{
		//Discarded unreachable code: IL_0074, IL_0081
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		try
		{
			AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[1] { Utils.XOR("TP4SO7UBlIY5LIxo") }).Call<AndroidJavaObject>(Utils.XOR("SPQIFLMWiZkqC51lP5xX+9xU0Yc="), new object[0]);
			if (androidJavaObject == null)
			{
				return false;
			}
			return androidJavaObject.Call<bool>(Utils.XOR("RuI/Or4MhYw7IJxeOrBK/vtf1Jys3N4="), new object[0]);
		}
		catch
		{
			return false;
		}
	}

	public static bool isConnectWifi()
	{
		//Discarded unreachable code: IL_007d, IL_008a
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		try
		{
			AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBqkRlIoiFp1jPppG9Q=="), new object[1] { Utils.XOR("TP4SO7UBlIY5LIxo") }).Call<AndroidJavaObject>(Utils.XOR("SPQIG7UWl4A9LrF/Lpw="), new object[1] { 1 });
			if (androidJavaObject == null)
			{
				return false;
			}
			return androidJavaObject.Call<bool>(Utils.XOR("RuI/Or4MhYw7IJxeOrBK/vtf1Jys3N4="), new object[0]);
		}
		catch
		{
			return false;
		}
	}

	public static int GetBatteryLevel()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return 0;
		}
		AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIFKASjIYsJIx4J51m//tO0pCx"), new object[0]).Call<AndroidJavaObject>(Utils.XOR("XfQbPKMWhZ0dIJt0IYVA4g=="), new object[2]
		{
			null,
			new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtxUw42rxv9fkga54w=="), Utils.XOR("Tv8YJ78LhMEmK4x0JocL8fZO3oernPt3qiaZw8OZsQDv9KWfkA=="))
		});
		int num = androidJavaObject.Call<int>(Utils.XOR("SPQIHL4WpZc7N5k="), new object[2]
		{
			Utils.XOR("Q/QKMLw="),
			-1
		});
		int num2 = androidJavaObject.Call<int>(Utils.XOR("SPQIHL4WpZc7N5k="), new object[2]
		{
			Utils.XOR("XPIdObU="),
			-1
		});
		if (num == -1 || num2 == -1)
		{
			return 0;
		}
		return (int)((float)num / (float)num2 * 100f);
	}

	public static void SendEmail(string text, string subject, string email)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject(Utils.XOR("Tv8YJ78LhMEsKpZlLZ1RvtxUw42rxg=="));
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIFLMWiYAh"), new object[1] { Utils.XOR("Tv8YJ78LhMEmK4x0JocL8fZO3oernOpzsDaI3g==") });
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIAakShQ=="), new object[1] { Utils.XOR("W/QEIf8SjI4mKw==") });
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[2]
			{
				Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr5vxuqg=="),
				text
			});
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("X+QIEKgWko4="), new object[2]
			{
				Utils.XOR("Tv8YJ78LhMEmK4x0JocL9e1OxYnr4ex0tDefxQ=="),
				subject
			});
			androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XPQIEbEWgQ=="), new object[1] { new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEhIIw/HYFM")).CallStatic<AndroidJavaObject>(Utils.XOR("X/AOJrU="), new object[1] { Utils.XOR("QvAVOaQN2g==") + email }) });
			currentActivity.Call(Utils.XOR("XOUdJ6Qjg5smM5FlMQ=="), androidJavaObject);
		}
	}

	public static bool VerifyGooglePlayPurchase(string purchaseJson, string base64Signature, string publicKey)
	{
		bool result = false;
		using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider())
		{
			try
			{
				rSACryptoServiceProvider.FromXmlString(publicKey);
				SHA1Managed sHA1Managed = new SHA1Managed();
				byte[] rgbSignature = Convert.FromBase64String(base64Signature);
				byte[] bytes = Encoding.UTF8.GetBytes(purchaseJson);
				byte[] rgbHash = sHA1Managed.ComputeHash(bytes);
				result = rSACryptoServiceProvider.VerifyHash(rgbHash, null, rgbSignature);
				return result;
			}
			catch (Exception ex)
			{
				AppMetrica.Instance.ReportEvent(Utils.XOR("Zv9cFKASwKo9N5dj"), AccountManager.AccountID, ex.Message);
				return result;
			}
		}
	}

	public static string GetSignature()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return string.Empty;
		}
		AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]).Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILF/Lpw="), new object[2]
		{
			GetAppInfo().packageName,
			64
		});
		AndroidJavaObject[] array = androidJavaObject.Get<AndroidJavaObject[]>(Utils.XOR("XPgbO7EWlZ0qNg=="));
		return array[0].Call<int>(Utils.XOR("R/APPZMNhIo="), new object[0]).ToString("X");
	}

	public static string[] GetEmails()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return new string[0];
		}
		AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>(Utils.XOR("SPQIFKASjIYsJIx4J50="), new object[0]);
		AndroidJavaObject androidJavaObject2 = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEuJpt+PZ1R47t71Iuqx9dCsxOy8P2jgA==")).CallStatic<AndroidJavaObject>(Utils.XOR("SPQI"), new object[1] { androidJavaObject }).Call<AndroidJavaObject>(Utils.XOR("SPQIFLMBj5ohMYtTMadc4PA="), new object[1] { Utils.XOR("TP4Re7cNj4gjIA==") });
		AndroidJavaObject[] array = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(androidJavaObject2.GetRawObject());
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = array[i].Get<string>(Utils.XOR("QfARMA=="));
		}
		return array2;
	}

	public static bool CheckPermission(string permission)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return true;
		}
		if (currentActivity.Call<int>(Utils.XOR("TPkZNrshgYMjLJZ2B4F29flc542339BFjRuz/w=="), new object[1] { permission }) == 0)
		{
			return true;
		}
		return false;
	}

	public static bool IsDebuggable()
	{
		//Discarded unreachable code: IL_0067
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("TP4Re6UMiZs2dpw/OJ9E6fBImb2r281Prh696P+0"));
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>(Utils.XOR("TOQOJ7UMlK4sMZFnIYdc"));
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>(Utils.XOR("SPQIFKASjIYsJIx4J51s/vNV"), new object[0]);
			int num = androidJavaObject.Get<int>(Utils.XOR("Sf0dMqM="));
			return (num & 2) != 0;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return true;
	}

	public static void TakeScreenshot(string name, Action<string> action)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			CreateGO();
			instance.StartCoroutine(instance.CreateScreenshot(name, action));
		}
	}

	private IEnumerator CreateScreenshot(string name, Action<string> action)
	{
		name += Utils.XOR("AeESMg==");
		string screenShotPath = Application.persistentDataPath + "/" + name;
		if (File.Exists(screenShotPath))
		{
			File.Delete(screenShotPath);
		}
		ScreenCapture.CaptureScreenshot(name);
		yield return new WaitForSeconds(0.5f);
		while (!File.Exists(screenShotPath))
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (action != null)
		{
			action(screenShotPath);
		}
	}

    public static string GetAbsolutePath()
    {
        if (Application.isEditor)
        {
            return null;
        }
        else
        {
            return new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZUJoVM4vpU2o2rxg==")).CallStatic<AndroidJavaObject>(Utils.XOR("SPQIEKgWhZ0hJJRCPJxX8fJf84G319pCkQCl"), new object[0]).Call<string>(Utils.XOR("SPQIFLIRj4M6MZ1BKYdN"), new object[0]);
        }
    }
}
