using FreeJSON;

public static class ClanManager
{
	public static int admin;

	public static string name;

	public static string tag;

	public static int[] players;

	private static bool loaded;

	public static void Save()
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.Add("a", admin);
		jsonObject.Add("n", name);
		jsonObject.Add("t", tag);
		jsonObject.Add("p", players);
		CryptoPrefs.SetString("ClanData", jsonObject.ToString());
	}

	public static void Load()
	{
		if (!loaded && CryptoPrefs.HasKey("ClanData"))
		{
			JsonObject jsonObject = JsonObject.Parse(CryptoPrefs.GetString("ClanData"));
			admin = jsonObject.Get<int>("a");
			name = jsonObject.Get<string>("n");
			tag = jsonObject.Get<string>("t");
			players = jsonObject.Get<int[]>("p");
			loaded = true;
		}
	}
}
