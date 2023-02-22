using AppodealAds.Unity.Common;

namespace AppodealAds.Unity.Api
{
	public class UserSettings
	{
		public enum Gender
		{
			OTHER,
			MALE,
			FEMALE
		}

		private static IAppodealAdsClient client;

		public UserSettings()
		{
			getInstance().getUserSettings();
		}

		private static IAppodealAdsClient getInstance()
		{
			if (client == null)
			{
				client = AppodealAdsClientFactory.GetAppodealAdsClient();
			}
			return client;
		}

		public UserSettings setUserId(string id)
		{
			getInstance().setUserId(id);
			return this;
		}

		public UserSettings setAge(int age)
		{
			getInstance().setAge(age);
			return this;
		}

		public UserSettings setGender(Gender gender)
		{
			getInstance().setGender(gender);
			return this;
		}
	}
}
