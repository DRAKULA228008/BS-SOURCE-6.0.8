using UnityEngine;

namespace Prime31
{
	public class IABUIManager : MonoBehaviourGUI
	{
		private void OnGUI()
		{
			beginColumn();
			if (GUILayout.Button("Initialize IAB"))
			{
				string text = "your public key from the Android developer portal here";
				text = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAkW8ls1E1B9/P6CXoZKl0tBNP+EUX6FIwbGbm/9LYW5iVOXASnng3+egvlSPnMekvcv9p/NoDRRY01pZhmlq1uShTlmaNT2pJipP2YXcyDvIPuO2rQSGKG8dIYmTAaIcVqqOl+22BQ321M8sSnCvhNOirCFbAEG5dCC0SoT3AOTFYB0GH5QbioLt0P+oV+a37c3GXbNJwlsMEmfeGxLEbgrpmrKfGT2E1JZta3JmcAXj+SVsxo5jOytiS7SluhBopirVU2nDDy+MawAVLCBGgOfeTfNBfFiwiFkLK7sS/wqRzl93/mlv2uZI62MNT/zHdfDWI+1szhUrckwvEwuGeHQIDAQAB";
				GoogleIAB.init(text);
			}
			if (GUILayout.Button("Query Inventory"))
			{
				string[] skus = new string[4] { "com.rexetstudio.blockstrike.g100", "com.rexetstudio.blockstrike.g250", "com.rexetstudio.blockstrike.g600", "com.rexetstudio.blockstrike.g1000" };
				GoogleIAB.queryInventory(skus);
			}
			if (GUILayout.Button("Are subscriptions supported?"))
			{
				Debug.Log("subscriptions supported: " + GoogleIAB.areSubscriptionsSupported());
			}
			if (GUILayout.Button("Purchase Test Product"))
			{
				GoogleIAB.purchaseProduct("android.test.purchased");
			}
			if (GUILayout.Button("Consume Test Purchase"))
			{
				GoogleIAB.consumeProduct("android.test.purchased");
			}
			if (GUILayout.Button("Test Unavailable Item"))
			{
				GoogleIAB.purchaseProduct("android.test.item_unavailable");
			}
			endColumn(true);
			if (GUILayout.Button("Purchase Real Product"))
			{
				GoogleIAB.purchaseProduct("com.rexetstudio.blockstrike.g100");
			}
			if (GUILayout.Button("Purchase Real Subscription"))
			{
				GoogleIAB.purchaseProduct("com.rexetstudio.blockstrike.g250");
			}
			if (GUILayout.Button("Consume Real Purchase"))
			{
				GoogleIAB.consumeProduct("com.rexetstudio.blockstrike.g100");
			}
			if (GUILayout.Button("Enable High Details Logs"))
			{
				GoogleIAB.enableLogging(true);
			}
			if (GUILayout.Button("Consume Multiple Purchases"))
			{
				string[] skus2 = new string[2] { "com.prime31.testproduct", "android.test.purchased" };
				GoogleIAB.consumeProducts(skus2);
			}
			endColumn();
		}
	}
}
