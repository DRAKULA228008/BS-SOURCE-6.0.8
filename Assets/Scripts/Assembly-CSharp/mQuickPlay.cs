using System;
using System.Collections.Generic;
using UnityEngine;

public class mQuickPlay : MonoBehaviour
{
	public GameMode SelectMode;

	public UIPopupList SelectModePopupList;

	public UIPopupList SelectMapPopupList;

	public GameObject SelectedMaxPlayers;

	public UILabel[] MaxPlayersList;

	public UIGrid Grid;

	private int MaxPlayers;

	private RoomInfo selectRoom;

	private bool defaultMaxPlayers = true;

	public void Open()
	{
		SelectModePopupList.Clear();
		SelectModePopupList.AddItem("Any");
		GameMode[] gameMode = GameModeManager.gameMode;
		for (int i = 0; i < gameMode.Length; i++)
		{
			SelectModePopupList.AddItem(gameMode[i].ToString());
		}
		SelectModePopupList.value = SelectModePopupList.items[0];
		UpdateMaps();
	}

	private void UpdateMaps()
	{
		if (SelectModePopupList.value == "Any")
		{
			SelectMapPopupList.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			SelectMapPopupList.transform.parent.gameObject.SetActive(true);
			List<string> gameModeScenes = LevelManager.GetGameModeScenes((GameMode)(int)Enum.Parse(typeof(GameMode), SelectModePopupList.value));
			SelectMapPopupList.Clear();
			SelectMapPopupList.AddItem(Localization.Get("Any"));
			for (int i = 0; i < gameModeScenes.Count; i++)
			{
				SelectMapPopupList.AddItem(gameModeScenes[i]);
			}
			SelectMapPopupList.value = SelectMapPopupList.items[0];
		}
		Grid.repositionNow = true;
	}

	public void OnSelectGameMode()
	{
		UpdateMaps();
		if (SelectModePopupList.value != "Any" && SelectMapPopupList.value != "Any")
		{
			SelectMode = (GameMode)(int)Enum.Parse(typeof(GameMode), SelectModePopupList.value);
			mServerSettings.Check(SelectMode, SelectMapPopupList.value);
		}
		else
		{
			SetDefaultMaxPlayers();
		}
	}

	public void OnSelectMap()
	{
		if (SelectModePopupList.value != "Any" && SelectMapPopupList.value != "Any")
		{
			mServerSettings.Check(SelectMode, SelectMapPopupList.value);
		}
		else
		{
			SetDefaultMaxPlayers();
		}
	}

	public void SetMaxPlayer(GameObject go)
	{
		if (go.name == "-")
		{
			MaxPlayers = 0;
		}
		else
		{
			MaxPlayers = int.Parse(go.name);
		}
		TweenPosition.Begin(SelectedMaxPlayers, 0.2f, go.transform.localPosition);
	}

	public void SetDefaultMaxPlayers()
	{
		if (!defaultMaxPlayers)
		{
			SetMaxPlayers(new int[5] { 4, 6, 8, 10, 12 });
			defaultMaxPlayers = true;
		}
	}

	public void SetMaxPlayers(int[] list)
	{
		if (list.Length > 5)
		{
			Debug.LogError("Max list <=5");
			return;
		}
		defaultMaxPlayers = false;
		for (int i = 0; i < MaxPlayersList.Length; i++)
		{
			MonoBehaviour.print(i);
			MaxPlayersList[i].cachedGameObject.SetActive(false);
		}
		for (int j = 0; j < list.Length; j++)
		{
			MaxPlayersList[j].cachedGameObject.SetActive(true);
			MaxPlayersList[j].cachedGameObject.name = list[j].ToString();
			MaxPlayersList[j].text = list[j].ToString();
		}
		SetMaxPlayer(MaxPlayersList[0].cachedGameObject);
	}

	public void QuickPlay()
	{
		mPopUp.ShowText(Localization.Get("Search Server") + "...");
		TimerManager.In(0.2f + UnityEngine.Random.value, delegate
		{
			SelectServer(SelectModePopupList.value, SelectMapPopupList.value, MaxPlayers);
		});
	}

	private void SelectServer(string mode, string map, int maxPlayers)
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		List<RoomInfo> list = new List<RoomInfo>();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (string.IsNullOrEmpty(roomList[i].GetPassword()) && roomList[i].PlayerCount != roomList[i].MaxPlayers && roomList[i].GetCustomMapHash() == 0 && (mode == "Any" || roomList[i].GetGameMode().ToString() == mode) && (map == Localization.Get("Any") || roomList[i].GetSceneName() == map || mode == "Any") && (maxPlayers == 0 || roomList[i].MaxPlayers == maxPlayers))
			{
				list.Add(roomList[i]);
			}
		}
		if (list.Count == 0)
		{
			mPopUp.HideAll("Server");
			mPopUp.ShowPopup(Localization.Get("The server with the selected data was not found. You want to create your own server?"), Localization.Get("Search Server"), Localization.Get("Yes"), delegate
			{
				mCreateServer.OpenPanel();
				mPopUp.HideAll("CreateServer");
			}, Localization.Get("No"), delegate
			{
				mPopUp.HideAll("Server");
			});
			return;
		}
		if (maxPlayers == 0)
		{
			selectRoom = list[UnityEngine.Random.Range(0, list.Count)];
		}
		else
		{
			list.Sort(SortByPlayerCount);
			int playerCount = list[0].PlayerCount;
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].PlayerCount != playerCount)
				{
					list.RemoveAt(j);
					j = 0;
				}
			}
			selectRoom = list[UnityEngine.Random.Range(0, list.Count)];
		}
		mPhotonSettings.JoinServer(selectRoom);
	}

	public static int SortByPlayerCount(RoomInfo a, RoomInfo b)
	{
		return b.PlayerCount.CompareTo(a.PlayerCount);
	}
}
