using System;
using System.Collections.Generic;
using UnityEngine;

public class mOfficialServer : MonoBehaviour
{
	public UIPopupList selectModePopupList;

	public UIPopupList selectMapPopupList;

	public static GameMode[] officialGameMode = new GameMode[7]
	{
		GameMode.TeamDeathmatch,
		GameMode.Classic,
		GameMode.Bomb,
		GameMode.Bomb2,
		GameMode.GunGame,
		GameMode.Escort,
		GameMode.RandomMode
	};

	public void Open()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
		}
		else if (!PhotonNetwork.connected)
		{
			mPhotonSettings.connectedCallback = delegate
			{
				mPopUp.HideAll("OfficialServers");
				OnStart();
			};
			mPhotonSettings.Connect();
		}
		else
		{
			mPanelManager.Show("OfficialServers", true);
			OnStart();
		}
	}

	private void OnStart()
	{
		selectModePopupList.Clear();
		selectModePopupList.AddItem("Any");
		GameMode[] array = officialGameMode;
		for (int i = 0; i < array.Length; i++)
		{
			selectModePopupList.AddItem(array[i].ToString());
		}
		selectModePopupList.value = selectModePopupList.items[0];
		UpdateMaps();
	}

	private void UpdateMaps()
	{
		if (selectModePopupList.value == "Any")
		{
			selectMapPopupList.GetComponent<Collider>().enabled = false;
			selectMapPopupList.Clear();
			selectMapPopupList.AddItem(Localization.Get("Any"));
			selectMapPopupList.GetComponent<UIWidget>().alpha = 0.5f;
			selectMapPopupList.value = selectMapPopupList.items[0];
			return;
		}
		selectMapPopupList.GetComponent<Collider>().enabled = true;
		selectMapPopupList.GetComponent<UIWidget>().alpha = 1f;
		List<string> gameModeScenes = LevelManager.GetGameModeScenes((GameMode)(int)Enum.Parse(typeof(GameMode), selectModePopupList.value));
		selectMapPopupList.Clear();
		selectMapPopupList.AddItem(Localization.Get("Any"));
		for (int i = 0; i < gameModeScenes.Count; i++)
		{
			selectMapPopupList.AddItem(gameModeScenes[i]);
		}
		selectMapPopupList.value = selectMapPopupList.items[0];
	}

	public void OnSelectGameMode()
	{
		UpdateMaps();
	}

	public void Join()
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		List<RoomInfo> list = new List<RoomInfo>();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].isOfficialServer())
			{
				list.Add(roomList[i]);
			}
		}
		if (selectModePopupList.value != "Any")
		{
			GameMode gameMode = (GameMode)(int)Enum.Parse(typeof(GameMode), selectModePopupList.value);
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].GetGameMode() != gameMode)
				{
					list.RemoveAt(j);
					j--;
				}
			}
		}
		if (selectMapPopupList.value != Localization.Get("Any"))
		{
			string value = selectMapPopupList.value;
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].GetSceneName() != value)
				{
					list.RemoveAt(k);
					k--;
				}
			}
		}
		for (int l = 0; l < list.Count; l++)
		{
			if (list[l].PlayerCount == list[l].MaxPlayers)
			{
				list.RemoveAt(l);
				l--;
			}
		}
		byte b = (byte)AccountManager.GetLevel();
		for (int m = 0; m < list.Count; m++)
		{
			if (list[m].GetMinLevel() > b + 10)
			{
				list.RemoveAt(m);
				m--;
			}
		}
		list.Sort((RoomInfo x, RoomInfo y) => y.GetMinLevel().CompareTo(x.GetMinLevel()));
		if (list != null && list.Count != 0)
		{
			mPhotonSettings.JoinServer(list[0]);
			return;
		}
		string empty = string.Empty;
		string text = "off_" + UnityEngine.Random.Range(1000000, 100000000);
		GameMode mode = ((!(selectModePopupList.value == "Any")) ? ((GameMode)(int)Enum.Parse(typeof(GameMode), selectModePopupList.value)) : officialGameMode[UnityEngine.Random.Range(0, officialGameMode.Length)]);
		if (selectMapPopupList.value == Localization.Get("Any"))
		{
			List<string> gameModeScenes = LevelManager.GetGameModeScenes(mode);
			empty = gameModeScenes[UnityEngine.Random.Range(0, gameModeScenes.Count)];
		}
		else
		{
			empty = selectMapPopupList.value;
		}
		mPhotonSettings.CreateOfficialServer(text, empty, mode);
	}
}
