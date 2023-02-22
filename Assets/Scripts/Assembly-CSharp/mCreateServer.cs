using System;
using System.Collections.Generic;
using BSCM;
using UnityEngine;

public class mCreateServer : MonoBehaviour
{
	public GameMode selectMode;

	public UIPopupList modePopupList;

	public UIPopupList mapPopupList;

	public UIInput serverNameInput;

	public UISprite[] maxPlayersSprite;

	public int selectMaxPlayers = 4;

	public UILabel[] maxPlayersLabel;

	public UIInput passwordInput;

	public GameObject customMaps;

	public UIToggle customMapsToggle;

	private static mCreateServer instance;

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

	public static bool custom
	{
		get
		{
			return instance.customMapsToggle.value;
		}
	}

	private void Start()
	{
		instance = this;
	}

	public void Open()
	{
		if (custom)
		{
			Manager.Start();
		}
		customMaps.SetActive(Manager.enabled && AccountManager.GetLevel() >= 10);
		serverName = Localization.Get("Room") + " " + UnityEngine.Random.Range(0, 99999);
		modePopupList.Clear();
		GameMode[] array = ((!custom) ? GameModeManager.gameMode : GameModeManager.customGameMode);
		for (int i = 0; i < array.Length; i++)
		{
			modePopupList.AddItem(array[i].ToString());
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
		if (!custom)
		{
			List<string> gameModeScenes = LevelManager.GetGameModeScenes(selectMode);
			mapPopupList.Clear();
			for (int i = 0; i < gameModeScenes.Count; i++)
			{
				mapPopupList.AddItem(gameModeScenes[i]);
			}
			mapPopupList.value = mapPopupList.items[0];
			return;
		}
		string[] mapsList = Manager.GetMapsList(mode);
		mapPopupList.Clear();
		for (int j = 0; j < mapsList.Length; j++)
		{
			mapPopupList.AddItem(Manager.GetBundleName(mapsList[j]), mapsList[j]);
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
		mServerSettings.Check(selectMode, map);
	}

	public void SelectMap()
	{
		mServerSettings.Check(selectMode, map);
	}

	public void CheckServerName()
	{
		if (serverName.Length < 4 || Utils.IsNullOrWhiteSpace(serverName) || serverName.Contains("\n") || BadWordsManager.Contains(serverName))
		{
			serverName = Localization.Get("Room") + " " + UnityEngine.Random.Range(0, 99999);
		}
		serverName = NGUIText.StripSymbols(serverName);
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].Name == serverName)
			{
				serverName = Localization.Get("Room") + " " + UnityEngine.Random.Range(0, 99999);
				UIToast.Show(Localization.Get("Name already taken"));
				break;
			}
		}
	}

	public void SetMaxPlayer(GameObject go)
	{
		selectMaxPlayers = int.Parse(go.name);
		for (int i = 0; i < maxPlayersSprite.Length; i++)
		{
			if (maxPlayersLabel[i].text != go.name)
			{
				maxPlayersSprite[i].color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 40);
			}
			else
			{
				maxPlayersSprite[i].color = new Color32(235, 242, byte.MaxValue, byte.MaxValue);
			}
		}
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
		for (int i = 0; i < maxPlayersSprite.Length; i++)
		{
			maxPlayersSprite[i].cachedGameObject.SetActive(false);
		}
		for (int j = 0; j < list.Length; j++)
		{
			maxPlayersSprite[j].cachedGameObject.SetActive(true);
			maxPlayersSprite[j].cachedGameObject.name = list[j].ToString();
			maxPlayersLabel[j].text = list[j].ToString();
		}
		SetMaxPlayer(maxPlayersSprite[0].cachedGameObject);
	}

	public void CreateServer()
	{
		if (!custom)
		{
			mPhotonSettings.CreateServer(serverName, map, mode, password, maxPlayers, false);
		}
		else if (mapPopupList.items.Count > 0)
		{
			Manager.LoadBundle((string)mapPopupList.data);
			mPhotonSettings.CreateServer(serverName, map, mode, password, maxPlayers, true);
		}
	}
}
