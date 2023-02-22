using System.Collections.Generic;

namespace Prime31
{
	public class GooglePurchase
	{
		public enum GooglePurchaseState
		{
			Purchased,
			Canceled,
			Refunded
		}

		public string purchaseJson { get; private set; }

		public string packageName { get; private set; }

		public string orderId { get; private set; }

		public string productId { get; private set; }

		public string developerPayload { get; private set; }

		public string type { get; private set; }

		public long purchaseTime { get; private set; }

		public GooglePurchaseState purchaseState { get; private set; }

		public string purchaseToken { get; private set; }

		public string signature { get; private set; }

		public string originalJson { get; private set; }

		public GooglePurchase(string json)
		{
			purchaseJson = json;
			Dictionary<string, object> dictionary = json.dictionaryFromJson();
			if (dictionary.ContainsKey("packageName"))
			{
				packageName = dictionary["packageName"].ToString();
			}
			if (dictionary.ContainsKey("orderId"))
			{
				orderId = dictionary["orderId"].ToString();
			}
			if (dictionary.ContainsKey("productId"))
			{
				productId = dictionary["productId"].ToString();
			}
			if (dictionary.ContainsKey("developerPayload"))
			{
				developerPayload = dictionary["developerPayload"].ToString();
			}
			if (dictionary.ContainsKey("type"))
			{
				type = dictionary["type"] as string;
			}
			if (dictionary.ContainsKey("purchaseTime"))
			{
				purchaseTime = long.Parse(dictionary["purchaseTime"].ToString());
			}
			if (dictionary.ContainsKey("purchaseState"))
			{
				purchaseState = (GooglePurchaseState)int.Parse(dictionary["purchaseState"].ToString());
			}
			if (dictionary.ContainsKey("purchaseToken"))
			{
				purchaseToken = dictionary["purchaseToken"].ToString();
			}
			if (dictionary.ContainsKey("signature"))
			{
				signature = dictionary["signature"].ToString();
			}
			if (dictionary.ContainsKey("originalJson"))
			{
				originalJson = dictionary["originalJson"].ToString();
			}
		}

		public GooglePurchase(Dictionary<string, object> dict)
		{
			if (dict.ContainsKey("packageName"))
			{
				packageName = dict["packageName"].ToString();
			}
			if (dict.ContainsKey("orderId"))
			{
				orderId = dict["orderId"].ToString();
			}
			if (dict.ContainsKey("productId"))
			{
				productId = dict["productId"].ToString();
			}
			if (dict.ContainsKey("developerPayload"))
			{
				developerPayload = dict["developerPayload"].ToString();
			}
			if (dict.ContainsKey("type"))
			{
				type = dict["type"] as string;
			}
			if (dict.ContainsKey("purchaseTime"))
			{
				purchaseTime = long.Parse(dict["purchaseTime"].ToString());
			}
			if (dict.ContainsKey("purchaseState"))
			{
				purchaseState = (GooglePurchaseState)int.Parse(dict["purchaseState"].ToString());
			}
			if (dict.ContainsKey("purchaseToken"))
			{
				purchaseToken = dict["purchaseToken"].ToString();
			}
			if (dict.ContainsKey("signature"))
			{
				signature = dict["signature"].ToString();
			}
			if (dict.ContainsKey("originalJson"))
			{
				originalJson = dict["originalJson"].ToString();
			}
		}

		public static List<GooglePurchase> fromList(List<object> items)
		{
			List<GooglePurchase> list = new List<GooglePurchase>();
			foreach (Dictionary<string, object> item in items)
			{
				list.Add(new GooglePurchase(item));
			}
			return list;
		}

		public override string ToString()
		{
			return string.Format("<GooglePurchase> packageName: {0}, orderId: {1}, productId: {2}, developerPayload: {3}, purchaseToken: {4}, purchaseState: {5}, signature: {6}, type: {7}, json: {8}", packageName, orderId, productId, developerPayload, purchaseToken, purchaseState, signature, type, originalJson);
		}
	}
}
