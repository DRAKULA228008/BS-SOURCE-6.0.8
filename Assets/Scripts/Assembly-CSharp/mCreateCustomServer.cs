using System;
using BSCM;
using UnityEngine;

public class mCreateCustomServer : MonoBehaviour
{
	public GameMode selectMode;

	public UIPopupList modePopupList;

	public UIPopupList mapPopupList;

	public UIInput serverNameInput;

	public GameObject selectedMaxPlayers;

	public int selectMaxPlayers = 4;

	public UILabel[] maxPlayersList;

	public UIInput passwordInput;

	private static mCreateCustomServer instance;

	public static GameMode mode
	{
		get
		{
			return instance.selectMode;
		}
	}

	public static string map
	{
		get
		{
			return instance.mapPopupList.value;
		}
	}

	public static string serverName
	{
		get
		{
			return instance.serverNameInput.value;
		}
		set
		{
			instance.serverNameInput.value = value;
		}
	}

	public static int maxPlayers
	{
		get
		{
			return instance.selectMaxPlayers;
		}
	}

	public static string password
	{
		get
		{
			return instance.passwordInput.value;
		}
	}

	private void Start()
	{
		instance = this;
	}

	public void Open()
	{
		mPanelManager.Show("CreateCustomServer", true);
		serverName = "Room " + UnityEngine.Random.Range(0, 99999);
		modePopupList.Clear();
		GameMode[] customGameMode = GameModeManager.customGameMode;
		for (int i = 0; i < customGameMode.Length; i++)
		{
			modePopupList.AddItem(customGameMode[i].ToString());
		}
		modePopupList.value = modePopupList.items[0];
		UpdateMaps();
	}

	public static void OpenPanel()
	{
		instance.Open();
	}

	private void UpdateMaps()
	{
		string[] mapsList = Manager.GetMapsList(mode);
		mapPopupList.Clear();
		for (int i = 0; i < mapsList.Length; i++)
		{
			mapPopupList.AddItem(Manager.GetBundleName(mapsList[i]), mapsList[i]);
		}
		if (mapsList.Length > 0)
		{
			mapPopupList.value = mapPopupList.items[0];
			return;
		}
		mapPopupList.AddItem("-----");
		mapPopupList.value = mapPopupList.items[0];
		mapPopupList.Clear();
	}

	public void SelectGameMode()
	{
		selectMode = (GameMode)(int)Enum.Parse(typeof(GameMode), modePopupList.value);
		UpdateMaps();
		mServerSettings.Check(mode, map);
	}

	public void SelectMap()
	{
		mServerSettings.Check(mode, map);
	}

	public void CheckServerName()
	{
		if (serverName.Length < 4 || Utils.IsNullOrWhiteSpace(serverName) || BadWordsManager.Contains(serverName))
		{
			serverName = "Room " + UnityEngine.Random.Range(0, 99999);
		}
		serverName = NGUIText.StripSymbols(serverName);
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].Name == serverName)
			{
				serverName = "Room " + UnityEngine.Random.Range(0, 99999);
				UIToast.Show(Localization.Get("Name already taken"));
				break;
			}
		}
	}

	public void SetMaxPlayer(GameObject go)
	{
		selectMaxPlayers = int.Parse(go.name);
		TweenPosition.Begin(selectedMaxPlayers, 0.2f, go.transform.localPosition);
	}

	public void SetDefaultMaxPlayers()
	{
		SetMaxPlayers(new int[5] { 4, 6, 8, 10, 12 });
	}

	public void SetMaxPlayers(int[] list)
	{
		if (list.Length > 5)
		{
			Debug.LogError("Max list  <=5");
			return;
		}
		for (int i = 0; i < maxPlayersList.Length; i++)
		{
			maxPlayersList[i].cachedGameObject.SetActive(false);
		}
		for (int j = 0; j < list.Length; j++)
		{
			maxPlayersList[j].cachedGameObject.SetActive(true);
			maxPlayersList[j].cachedGameObject.name = list[j].ToString();
			maxPlayersList[j].text = list[j].ToString();
		}
		SetMaxPlayer(maxPlayersList[0].cachedGameObject);
	}

	public void DeleteMap()
	{
		if (mapPopupList.items.Count > 0)
		{
			mPopUp.ShowPopup("Удалить карту: " + map, Localization.Get("Delete"), Localization.Get("Yes"), delegate
			{
				Manager.DeleteBundle(map);
				Open();
				mPopUp.HideAll();
			}, Localization.Get("No"), delegate
			{
				mPopUp.HideAll();
			});
		}
	}

	public void CreateServer()
	{
		if (mapPopupList.items.Count > 0)
		{
			Manager.LoadBundle((string)mapPopupList.data);
			mPhotonSettings.CreateServer(serverName, map, mode, password, maxPlayers, true);
		}
	}

	public void ShowToast(string text)
	{
		UIToast.Show(Localization.Get(text));
	}
}
