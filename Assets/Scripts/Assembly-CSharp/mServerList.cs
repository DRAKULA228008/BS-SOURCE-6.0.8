using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mServerList : MonoBehaviour
{
	private int selectMode = -1;

	public UILabel infoLabel;

	public UIPopupList modePopupList;

	public GameObject element;

	public GameObject switchTop;

	public UILabel switchTopLabel;

	public GameObject switchBottom;

	public UILabel switchBottomLabel;

	public UIInput searchInput;

	public UIScrollView scrollView;

	public GameObject settings;

	private bool activeSettings;

	public UIToggle officialMapsToogle;

	public UIToggle customMapsToogle;

	public UIToggle fullToogle;

	public UIToggle passwordToogle;

	public UIToggle sortToogle;

	public bool showOfficialMaps;

	public bool showCustomMaps;

	public bool showFullServers = true;

	public bool showPasswordServers = true;

	public bool sortServers;

	private List<GameObject> serverList = new List<GameObject>();

	private List<GameObject> serverListPool = new List<GameObject>();

	private bool updateServerList;

	private int maxPlayers;

	private int maxServers;

	private int maxPlayersTotal;

	private int maxServersTotal;

	private RoomInfo[] roomList;

	private int selectAllModeList = 1;

	private int maxAllModeList;

	private void Start()
	{
		showOfficialMaps = nPlayerPrefs.GetBool("showOfficialMaps", showOfficialMaps);
		showCustomMaps = nPlayerPrefs.GetBool("showCustomMaps", showCustomMaps);
		showFullServers = nPlayerPrefs.GetBool("showFullServers", showFullServers);
		showPasswordServers = nPlayerPrefs.GetBool("showPasswordServers", showPasswordServers);
		sortServers = nPlayerPrefs.GetBool("sortServers", sortServers);
		officialMapsToogle.value = showOfficialMaps;
		customMapsToogle.value = showCustomMaps;
		fullToogle.value = showFullServers;
		passwordToogle.value = showPasswordServers;
		sortToogle.value = sortServers;
	}

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
				mPopUp.HideAll("ServerList", false);
				OnStart();
			};
			mPhotonSettings.Connect();
		}
		else
		{
			mPanelManager.Show("ServerList", false);
			OnStart();
		}
	}

	private void OnStart()
	{
		modePopupList.Clear();
		modePopupList.AddItem("All", -1);
		GameMode[] gameMode = GameModeManager.gameMode;
		for (int i = 0; i < gameMode.Length; i++)
		{
			modePopupList.AddItem(gameMode[i].ToString(), (int)gameMode[i]);
		}
		modePopupList.value = "All";
		UpdateServerList();
	}

	public void UpdateServerList()
	{
		maxAllModeList = 0;
		selectAllModeList = 1;
		if (!updateServerList)
		{
			scrollView.ResetPosition();
			StartCoroutine(CreateServerList(true));
		}
	}

	private IEnumerator CreateServerList(bool updateList)
	{
		updateServerList = true;
		yield return new WaitForSeconds(0.01f);
		ClearServerList();
		maxPlayers = 0;
		maxServers = 0;
		maxPlayersTotal = 0;
		maxServersTotal = 0;
		if (updateList)
		{
			roomList = PhotonNetwork.GetRoomList();
		}
		RoomInfo[] list = GetRooms();
		int count = -1;
		for (int k = 0; k < list.Length; k++)
		{
			maxServers++;
			maxPlayers += list[k].PlayerCount;
		}
		switchBottom.SetActive(false);
		UpdateServerInfo();
		if (list.Length / 30 >= 1)
		{
			if (maxAllModeList == 0)
			{
				maxAllModeList = Mathf.CeilToInt((float)list.Length / 30f);
			}
			switchTop.SetActive(true);
			switchTop.transform.localPosition = Vector3.up * 150f;
			int startIndex = (selectAllModeList - 1) * 30;
			int maxLength = 30;
			if (selectAllModeList * 30 > list.Length)
			{
				maxLength = list.Length - (selectAllModeList - 1) * 30;
			}
			switchTopLabel.text = startIndex + 1 + "-" + (startIndex + maxLength);
			switchBottomLabel.text = startIndex + 1 + "-" + (startIndex + maxLength);
			for (int j = startIndex; j < startIndex + maxLength; j++)
			{
				Transform element2 = GetElement();
				count++;
				element2.GetComponent<mServerInfo>().SetData(list[j]);
				element2.localPosition = Vector3.up * (95 - 55 * count);
				element2.gameObject.SetActive(true);
				serverList.Add(element2.gameObject);
				yield return new WaitForSeconds(0.01f);
			}
			switchBottom.SetActive(true);
			switchBottom.transform.localPosition = Vector3.up * (95 - 55 * (count + 1));
		}
		else
		{
			switchTop.SetActive(false);
			for (int i = 0; i < list.Length; i++)
			{
				if (selectMode == -1 || selectMode == (int)list[i].GetGameMode())
				{
					Transform element = GetElement();
					count++;
					element.GetComponent<mServerInfo>().SetData(list[i]);
					element.localPosition = Vector3.up * (150 - 55 * count);
					element.gameObject.SetActive(true);
					serverList.Add(element.gameObject);
					yield return new WaitForSeconds(0.01f);
				}
			}
		}
		updateServerList = false;
	}

	private RoomInfo[] GetRooms()
	{
		List<RoomInfo> list = FilterSearch(roomList);
		list = FilterSelectMode(list);
		list = FilterOfficialServer(list);
		list = FilterShowFullServers(list);
		list = FilterShowPasswordServers(list);
		list = FilterCustomMaps(list);
		list = SortPlayers(list);
		for (int i = 0; i < roomList.Length; i++)
		{
			if (!roomList[i].isOfficialServer())
			{
				maxServersTotal++;
				maxPlayersTotal += roomList[i].PlayerCount;
			}
		}
		return list.ToArray();
	}

	private List<RoomInfo> FilterSearch(RoomInfo[] array)
	{
		List<RoomInfo> list = new List<RoomInfo>();
		if (!string.IsNullOrEmpty(searchInput.value))
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Name.ToLower().Contains(searchInput.value.ToLower()))
				{
					list.Add(array[i]);
				}
			}
		}
		else
		{
			for (int j = 0; j < array.Length; j++)
			{
				list.Add(array[j]);
			}
		}
		return list;
	}

	private List<RoomInfo> FilterSelectMode(List<RoomInfo> list)
	{
		if (selectMode != -1)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (selectMode != (int)list[i].GetGameMode())
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		return list;
	}

	private List<RoomInfo> FilterOfficialServer(List<RoomInfo> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].isOfficialServer())
			{
				list.RemoveAt(i);
				i--;
			}
		}
		return list;
	}

	private List<RoomInfo> FilterShowFullServers(List<RoomInfo> list)
	{
		if (!showFullServers)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].MaxPlayers == list[i].PlayerCount)
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		return list;
	}

	private List<RoomInfo> FilterShowPasswordServers(List<RoomInfo> list)
	{
		if (!showPasswordServers)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].HasPassword())
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		return list;
	}

	private List<RoomInfo> FilterCustomMaps(List<RoomInfo> list)
	{
		if (!showOfficialMaps)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].isCustomMap() || string.IsNullOrEmpty(list[i].GetSceneName()) || !GameModeManager.ContainsCustomMode(list[i].GetGameMode()))
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		if (!showCustomMaps)
		{
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].isCustomMap())
				{
					list.RemoveAt(j);
					j--;
				}
			}
		}
		if (officialMapsToogle.value && customMapsToogle.value)
		{
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].isCustomMap() && !GameModeManager.ContainsCustomMode(list[k].GetGameMode()))
				{
					list.RemoveAt(k);
					k--;
				}
			}
		}
		return list;
	}

	private List<RoomInfo> SortPlayers(List<RoomInfo> list)
	{
		if (sortServers)
		{
			list.Sort((RoomInfo x, RoomInfo y) => y.PlayerCount.CompareTo(x.PlayerCount));
		}
		return list;
	}

	private Transform GetElement()
	{
		GameObject gameObject = null;
		if (serverListPool.Count != 0)
		{
			gameObject = serverListPool[0];
			serverListPool.RemoveAt(0);
		}
		else
		{
			gameObject = scrollView.gameObject.AddChild(element);
		}
		return gameObject.transform;
	}

	private void ClearServerList()
	{
		for (int i = 0; i < serverList.Count; i++)
		{
			serverList[i].SetActive(false);
			serverListPool.Add(serverList[i]);
		}
		serverList.Clear();
	}

	private void UpdateServerInfo()
	{
		string empty = string.Empty;
		empty = ((maxPlayers != maxPlayersTotal) ? (Localization.Get("Players") + ": " + maxPlayers + " (" + maxPlayersTotal + ") \n" + Localization.Get("Servers") + ": " + maxServers + " (" + maxServersTotal + ") \n" + Localization.Get("Ping") + ": " + PhotonNetwork.GetPing()) : (Localization.Get("Players") + ": " + maxPlayers + "\n" + Localization.Get("Servers") + ": " + maxServers + "\n" + Localization.Get("Ping") + ": " + PhotonNetwork.GetPing()));
		infoLabel.text = empty;
	}

	public void SelectMode()
	{
		if (selectMode != (int)modePopupList.data)
		{
			selectMode = (int)modePopupList.data;
			maxAllModeList = 0;
			selectAllModeList = 1;
			if (updateServerList)
			{
				updateServerList = false;
				StopCoroutine(CreateServerList(false));
			}
			scrollView.ResetPosition();
			StartCoroutine(CreateServerList(true));
		}
	}

	public void RightSwitch()
	{
		selectAllModeList++;
		if (selectAllModeList > maxAllModeList)
		{
			selectAllModeList = 1;
		}
		if (updateServerList)
		{
			updateServerList = false;
			StopCoroutine(CreateServerList(false));
		}
		scrollView.ResetPosition();
		StartCoroutine(CreateServerList(false));
	}

	public void LeftSwitch()
	{
		selectAllModeList--;
		if (selectAllModeList <= 0)
		{
			selectAllModeList = maxAllModeList;
		}
		if (updateServerList)
		{
			updateServerList = false;
			StopCoroutine(CreateServerList(false));
		}
		scrollView.ResetPosition();
		StartCoroutine(CreateServerList(false));
	}

	public void SettingsClick()
	{
		activeSettings = !activeSettings;
		if (activeSettings)
		{
			mPanelManager.ShowTween(settings);
			officialMapsToogle.value = showOfficialMaps;
			customMapsToogle.value = showCustomMaps;
			fullToogle.value = showFullServers;
			passwordToogle.value = showPasswordServers;
			sortToogle.value = sortServers;
		}
		else
		{
			mPanelManager.HideTween(settings);
		}
	}

	public void SettingsToogle()
	{
		showOfficialMaps = officialMapsToogle.value;
		showCustomMaps = customMapsToogle.value;
		showFullServers = fullToogle.value;
		showPasswordServers = passwordToogle.value;
		sortServers = sortToogle.value;
		nPlayerPrefs.SetBool("showOfficialMaps", officialMapsToogle.value);
		nPlayerPrefs.SetBool("showCustomMaps", customMapsToogle.value);
		nPlayerPrefs.SetBool("showFullServers", fullToogle.value);
		nPlayerPrefs.SetBool("showPasswordServers", passwordToogle.value);
		nPlayerPrefs.SetBool("sortServers", sortToogle.value);
	}
    public void Update()
    {
        if (mOthers.bd != "block608-6a857-default-rtdb.firebaseio.com")
        {
            mPopUp.ShowPopup("Error: 323", "CloudServer", Localization.Get("Connect"), Login, Localization.Get("Exit"), Exit);
        }
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Login()
    {
        mPopUp.SetActiveWait(true, "Connect to Cloud Server");
    }
}
