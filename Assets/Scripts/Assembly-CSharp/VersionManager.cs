using UnityEngine;

public class VersionManager
{
    private static bool isCheck;

    private static string mProductName;

    private static string mBundleIdentifier;

    private static string mBundleVersion;

    private static int mBundleVersionCode;

    private static bool mFullVersion;

    private static bool mTestVersion;

    public static string productName
    {
        get
        {
            CheckTextAsset();
            return mProductName;
        }
    }

    public static string bundleIdentifier
    {
        get
        {
            CheckTextAsset();
            return mBundleIdentifier;
        }
    }

    public static string bundleVersion
    {
        get
        {
            return " 6.0.8";
        }
    }

    public static int bundleVersionCode
    {
        get
        {
            CheckTextAsset();
            return mBundleVersionCode;
        }
    }

    public static bool fullVersion
    {
        get
        {
            CheckTextAsset();
            return mFullVersion;
        }
    }

    public static bool testVersion
    {
        get
        {
            CheckTextAsset();
            return mTestVersion;
        }
    }

    private static void CheckTextAsset()
    {
        if (!isCheck)
        {
            string text = Utils.XOR((Resources.Load("Others/VersionManager") as TextAsset).text);
            string[] array = text.Split("\n"[0]);
            mProductName = array[0];
            mBundleIdentifier = array[1];
            mBundleVersion = array[2];
            mBundleVersionCode = int.Parse(array[3]);
            mFullVersion = bool.Parse(array[4]);
            mTestVersion = bool.Parse(array[5]);
            isCheck = true;
        }
    }
}
