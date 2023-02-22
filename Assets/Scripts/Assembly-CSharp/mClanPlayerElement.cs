using UnityEngine;

public class mClanPlayerElement : MonoBehaviour
{
	public UIWidget Widget;

	public UILabel Name;

	private int playerID;

	public int ID
	{
		get
		{
			return playerID;
		}
	}

	public void SetData(int id)
	{
		if (Widget.cachedGameObject.activeSelf)
		{
			playerID = id;
			string @string = CryptoPrefs.GetString("Friend_#" + id, "#" + id);
            Name.text = @string;

        }
	}
}
