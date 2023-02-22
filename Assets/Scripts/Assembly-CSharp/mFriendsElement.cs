using UnityEngine;

public class mFriendsElement : MonoBehaviour
{
	public UIWidget Widget;

	public UILabel Name;

	public UISprite StatusSprite;

	public UILabel InRoomLabel;

	private int playerID;

	public FriendInfo info;

	public int ID
	{
		get
		{
			return playerID;
		}
	}

	public void SetData(int id)
	{
		if (!Widget.cachedGameObject.activeSelf)
		{
			return;
		}
		playerID = id;
		string @string = CryptoPrefs.GetString("Friend_#" + id, "#" + id);
		Name.text = @string;
		if (PhotonNetwork.Friends != null)
		{
			for (int i = 0; i < PhotonNetwork.Friends.Count; i++)
			{
				if (!(PhotonNetwork.Friends[i].UserId == @string))
				{
					continue;
				}
				info = PhotonNetwork.Friends[i];
				StatusSprite.cachedGameObject.SetActive(true);
				StatusSprite.color = ((!info.IsOnline) ? new Color32(122, 122, 122, byte.MaxValue) : new Color32(79, 181, 82, byte.MaxValue));
				if (info.IsInRoom)
				{
					RoomInfo room = GetRoom(info.Room);
					if (room.isOfficialServer())
					{
						InRoomLabel.text = ((room != null) ? (room.Name.Replace("off", Localization.Get("Official Servers")) + " - " + Localization.Get(room.GetGameMode().ToString()) + " - " + room.GetSceneName() + " - " + room.PlayerCount + "/" + room.MaxPlayers) : string.Empty);
					}
					else
					{
						InRoomLabel.text = ((room != null) ? (room.Name + " - " + Localization.Get(room.GetGameMode().ToString()) + " - " + room.GetSceneName() + " - " + room.PlayerCount + "/" + room.MaxPlayers) : string.Empty);
					}
				}
				else
				{
					InRoomLabel.text = string.Empty;
				}
				base.name = ((!info.IsOnline) ? ("1-" + info.UserId) : "0");
			}
		}
		else
		{
			StatusSprite.cachedGameObject.SetActive(false);
		}
	}

	public void UpdateStatus(FriendInfo info)
	{
		if (info.UserId != Name.text || !Widget.cachedGameObject.activeSelf)
		{
			return;
		}
		StatusSprite.cachedGameObject.SetActive(true);
		StatusSprite.color = ((!info.IsOnline) ? new Color32(122, 122, 122, byte.MaxValue) : new Color32(79, 181, 82, byte.MaxValue));
		if (info.IsInRoom)
		{
			RoomInfo room = GetRoom(info.Room);
			if (room.isOfficialServer())
			{
				InRoomLabel.text = ((room != null) ? (room.Name.Replace("off", Localization.Get("Official Servers")) + " - " + Localization.Get(room.GetGameMode().ToString()) + " - " + room.GetSceneName() + " - " + room.PlayerCount + "/" + room.MaxPlayers) : string.Empty);
			}
			else
			{
				InRoomLabel.text = ((room != null) ? (room.Name + " - " + Localization.Get(room.GetGameMode().ToString()) + " - " + room.GetSceneName() + " - " + room.PlayerCount + "/" + room.MaxPlayers) : string.Empty);
			}
		}
		else
		{
			InRoomLabel.text = string.Empty;
		}
		base.name = ((!info.IsOnline) ? ("1-" + info.UserId) : "0");
	}

	private RoomInfo GetRoom(string name)
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].Name == name)
			{
				return roomList[i];
			}
		}
		return null;
	}
}
