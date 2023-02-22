using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Photon;
using UnityEngine;

public class UIPlayerStatistics : Photon.MonoBehaviour
{
	public class PlayerInfoClass
	{
		public int xp;

		public int deaths;

		public int kills;

		public int headshot;
	}

	[Header("Panel")]
	public GameObject RedBluePanel;

	public GameObject OnlyBluePanel;

	[Header("Scrolls")]
	public UIScrollView RedScroll;

	public UIScrollView BlueScroll;

	public UIScrollView BlueScroll2;

	[Header("Parents")]
	public Transform BlueTeamParent;

	public Transform RedTeamParent;

	public Transform BlueTeamParent2;

	[Header("Panel Labels")]
	public UILabel SpectatorsLabel;

	public UILabel SpectatorsLabel2;

	[Header("Server")]
	public UILabel ServerNameLabel;

	public UILabel MapNameLabel;

	public UILabel ServerNameLabel2;

	public UILabel MapNameLabel2;

	[Header("Score")]
	public UILabel BlueScore;

	public UILabel RedScore;

	[Header("Deaths")]
	public UILabel BlueDeath;

	public UILabel RedDeath;

	[Header("Player Info")]
	public GameObject PlayerInfo;

	public UILabel PlayerInfoName;

	public UILabel PlayerInfoID;

	public UILabel PlayerInfoLevel;

	public UILabel PlayerInfoXP;

	public UILabel PlayerInfoKD;

	public UILabel PlayerInfoHK;

	public UITexture PlayerInfoAvatar;

	private Dictionary<string, PlayerInfoClass> PlayerInfoList = new Dictionary<string, PlayerInfoClass>();

	[Header("Others")]
	public GameObject Container;

	public float ContainerGrid = 25f;

	public GameObject Root;

	public GameObject Root2;

	public static PhotonPlayer SelectPlayer;

	public static bool isOnlyBluePanel;

	private bool isShow;

	private List<UIPlayerStatisticsElement> PlayerList = new List<UIPlayerStatisticsElement>();

	private List<UIPlayerStatisticsElement> PlayerListPool = new List<UIPlayerStatisticsElement>();

	private void Start()
	{
		PhotonRPC.AddMessage("PhotonGetSelectPlayerInfo", PhotonGetSelectPlayerInfo);
		PhotonRPC.AddMessage("PhotonSetSelectPlayerInfo", PhotonSetSelectPlayerInfo);
	}

	private void OnEnable()
	{
		isOnlyBluePanel = false;
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
	}

	private void GetButtonDown(string name)
	{
		if (name == "Statistics")
		{
			Show();
		}
	}

	private void Show()
	{
		if (!PhotonNetwork.offlineMode && !GameManager.loadingLevel)
		{
			UIPanelManager.ShowPanel("Statistics");
			if (isOnlyBluePanel)
			{
				ShowOnlyBluePanel();
			}
			else
			{
				StartCoroutine(ShowRedBluePanel());
			}
		}
	}

	private IEnumerator ShowRedBluePanel()
	{
		RedBluePanel.SetActive(true);
		StringBuilder builderServerName = new StringBuilder();
		StringBuilder builderMapName = new StringBuilder();
		if (PhotonNetwork.room.isOfficialServer())
		{
			builderServerName.Append(PhotonNetwork.room.Name.Replace("off", Localization.Get("Official Servers")));
		}
		else
		{
			builderServerName.Append(PhotonNetwork.room.Name);
		}
		builderServerName.Append(" | ");
		if (PhotonNetwork.room.GetGameMode() == GameMode.Only)
		{
			builderServerName.Append(Localization.Get(PhotonNetwork.room.GetGameMode().ToString()) + " (" + WeaponManager.GetWeaponName(PhotonNetwork.room.GetOnlyWeapon()) + ")");
		}
		else
		{
			builderServerName.Append(Localization.Get(PhotonNetwork.room.GetGameMode().ToString()));
		}
		builderMapName.Append(PhotonNetwork.room.GetSceneName());
		builderMapName.Append(" | ");
		builderMapName.Append(StringCache.Get(PhotonNetwork.room.PlayerCount) + "/" + StringCache.Get(PhotonNetwork.room.MaxPlayers));
		ServerNameLabel.text = builderServerName.ToString();
		MapNameLabel.text = builderMapName.ToString();
		BlueScore.text = StringCache.Get(GameManager.blueScore);
		RedScore.text = StringCache.Get(GameManager.redScore);
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> bluePlayers = new List<PhotonPlayer>();
		List<PhotonPlayer> redPlayers = new List<PhotonPlayer>();
		List<string> spectatorPlayers = new List<string>();
		byte blueDeath = 0;
		byte redDeath = 0;
		for (int k = 0; k < playerList.Length; k++)
		{
			if (playerList[k].GetTeam() == Team.Blue)
			{
				bluePlayers.Add(playerList[k]);
				if (!playerList[k].GetDead())
				{
					blueDeath = (byte)(blueDeath + 1);
				}
			}
			else if (playerList[k].GetTeam() == Team.Red)
			{
				redPlayers.Add(playerList[k]);
				if (!playerList[k].GetDead())
				{
					redDeath = (byte)(redDeath + 1);
				}
			}
			else if (playerList[k].GetTeam() == Team.None)
			{
				spectatorPlayers.Add(playerList[k].UserId);
			}
		}
		BlueDeath.text = StringCache.Get(blueDeath) + "/" + StringCache.Get(bluePlayers.Count);
		RedDeath.text = StringCache.Get(redDeath) + "/" + StringCache.Get(redPlayers.Count);
		bluePlayers.Sort(SortByKills);
		redPlayers.Sort(SortByKills);
		if (spectatorPlayers.Count == 0)
		{
			SpectatorsLabel.text = string.Empty;
		}
		else
		{
			SpectatorsLabel.text = Localization.Get("Spectators") + ": " + string.Join(",", spectatorPlayers.ToArray());
		}
		for (int j = 0; j < bluePlayers.Count; j++)
		{
			yield return new WaitForSeconds(0.005f);
			UIPlayerStatisticsElement container = GetPlayerContainer(bluePlayers[j].UserId);
			container.SetData(bluePlayers[j]);
			container.DragScroll.scrollView = BlueScroll;
			container.cachedTransform.SetParent(BlueTeamParent);
			if (j == 0)
			{
				container.cachedTransform.localPosition = Vector3.zero;
			}
			else
			{
				container.cachedTransform.localPosition = Vector3.down * ContainerGrid * j;
			}
			container.cachedTransform.localPosition = new Vector3(container.cachedTransform.localPosition.x, container.cachedTransform.localPosition.y, 0f);
			PlayerList.Add(container);
		}
		for (int i = 0; i < redPlayers.Count; i++)
		{
			yield return new WaitForSeconds(0.005f);
			UIPlayerStatisticsElement container2 = GetPlayerContainer(redPlayers[i].UserId);
			container2.SetData(redPlayers[i]);
			container2.DragScroll.scrollView = RedScroll;
			container2.cachedTransform.SetParent(RedTeamParent);
			if (i == 0)
			{
				container2.cachedTransform.localPosition = Vector3.zero;
			}
			else
			{
				container2.cachedTransform.localPosition = Vector3.down * ContainerGrid * i;
			}
			container2.cachedTransform.localPosition = new Vector3(container2.cachedTransform.localPosition.x, container2.cachedTransform.localPosition.y, 0f);
			PlayerList.Add(container2);
		}
	}

	private void ShowOnlyBluePanel()
	{
		OnlyBluePanel.SetActive(true);
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder.Append(PhotonNetwork.room.Name);
		stringBuilder.Append(" | ");
		stringBuilder.Append(Localization.Get(PhotonNetwork.room.GetGameMode().ToString()));
		stringBuilder2.Append(PhotonNetwork.room.GetSceneName());
		stringBuilder2.Append(" | ");
		stringBuilder2.Append(StringCache.Get(PhotonNetwork.room.PlayerCount) + "/" + StringCache.Get(PhotonNetwork.room.MaxPlayers));
		ServerNameLabel2.text = stringBuilder.ToString();
		MapNameLabel2.text = stringBuilder2.ToString();
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		List<string> list2 = new List<string>();
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue)
			{
				list.Add(playerList[i]);
			}
			else if (playerList[i].GetTeam() == Team.None)
			{
				list2.Add(playerList[i].UserId);
			}
		}
		list.Sort(SortByKills);
		if (list2.Count == 0)
		{
			SpectatorsLabel2.text = string.Empty;
		}
		else
		{
			SpectatorsLabel2.text = Localization.Get("Spectators") + ": " + string.Join(",", list2.ToArray());
		}
		for (int j = 0; j < list.Count; j++)
		{
			UIPlayerStatisticsElement playerContainer = GetPlayerContainer(list[j].UserId);
			playerContainer.SetData(list[j]);
			playerContainer.DragScroll.scrollView = BlueScroll2;
			playerContainer.cachedTransform.SetParent(BlueTeamParent2);
			if (j == 0)
			{
				playerContainer.cachedTransform.localPosition = new Vector3(0f, 50f, 0f);
			}
			else
			{
				playerContainer.cachedTransform.localPosition = new Vector3(0f, 50f - ContainerGrid * (float)j, 0f);
			}
			PlayerList.Add(playerContainer);
		}
	}

	public void Close()
	{
		StopAllCoroutines();
		UIPanelManager.ShowPanel("Display");
		ClearList();
	}

	private UIPlayerStatisticsElement GetPlayerContainer(string name)
	{
		if (PlayerListPool.Count > 1)
		{
			UIPlayerStatisticsElement uIPlayerStatisticsElement = null;
			for (int i = 0; i < PlayerListPool.Count; i++)
			{
				if (PlayerListPool[i].PlayerNameLabel.text == name)
				{
					uIPlayerStatisticsElement = PlayerListPool[i];
					PlayerListPool.RemoveAt(i);
					return uIPlayerStatisticsElement;
				}
			}
			GameObject gameObject = ((!isOnlyBluePanel) ? Root : Root2).AddChild(Container);
			return gameObject.GetComponent<UIPlayerStatisticsElement>();
		}
		GameObject gameObject2 = ((!isOnlyBluePanel) ? Root : Root2).AddChild(Container);
		return gameObject2.GetComponent<UIPlayerStatisticsElement>();
	}

	private void ClearList()
	{
		if (PlayerList.Count != 0)
		{
			for (int i = 0; i < PlayerList.Count; i++)
			{
				PlayerListPool.Add(PlayerList[i]);
			}
			PlayerList.Clear();
			for (int j = 0; j < PlayerListPool.Count; j++)
			{
				PlayerListPool[j].Widget.alpha = 0f;
			}
		}
	}

	public static int SortByKills(PhotonPlayer a, PhotonPlayer b)
	{
		if (a.GetKills() == b.GetKills())
		{
			if (a.GetDeaths() == b.GetDeaths())
			{
				if (a.GetLevel() == b.GetLevel())
				{
					return b.UserId.CompareTo(a.UserId);
				}
				return b.GetLevel().CompareTo(a.GetLevel());
			}
			return a.GetDeaths().CompareTo(b.GetDeaths());
		}
		return b.GetKills().CompareTo(a.GetKills());
	}

	public static int GetPlayerStatsPosition(PhotonPlayer player)
	{
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			if (PhotonNetwork.playerList[i].GetTeam() == player.GetTeam())
			{
				list.Add(PhotonNetwork.playerList[i]);
			}
		}
		list.Sort(SortByKills);
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j] == player)
			{
				return j + 1;
			}
		}
		return 1;
	}

	public void OnSelectPlayer(UIPlayerStatisticsElement player)
	{
		if (player.PlayerInfo == null || player.PlayerInfo.IsLocal)
		{
			return;
		}
		PlayerInfoAvatar.cachedGameObject.SetActive(false);
		SelectPlayer = player.PlayerInfo;
		PlayerInfo.SetActive(true);
		PlayerInfoName.text = player.PlayerInfo.UserId;
		PlayerInfoID.text = "ID: " + player.PlayerInfo.GetPlayerID();
		PlayerInfoLevel.text = StringCache.Get(player.PlayerInfo.GetLevel());
		if (PlayerInfoList.ContainsKey(player.PlayerInfo.UserId))
		{
			PlayerInfoClass playerInfoClass = PlayerInfoList[player.PlayerInfo.UserId];
			PhotonSetSelectPlayerInfo(playerInfoClass.xp, playerInfoClass.deaths, playerInfoClass.kills, playerInfoClass.headshot);
		}
		else
		{
			PlayerInfoXP.text = "-";
			PlayerInfoKD.text = "-";
			PlayerInfoHK.text = "-";
			PhotonRPC.RPC("PhotonGetSelectPlayerInfo", player.PlayerInfo);
		}
		if (Settings.ShowAvatars)
		{
			AvatarManager.Get(player.PlayerInfo.GetAvatarUrl(), delegate(Texture2D r)
			{
				PlayerInfoAvatar.cachedGameObject.SetActive(true);
				PlayerInfoAvatar.mainTexture = r;
			});
		}
		else
		{
			PlayerInfoAvatar.cachedGameObject.SetActive(true);
			PlayerInfoAvatar.mainTexture = GameSettings.instance.NoAvatarTexture;
		}
	}

	[PunRPC]
	private void PhotonGetSelectPlayerInfo(PhotonMessage message)
	{
		int xP = AccountManager.GetXP();
		int deaths = AccountManager.GetDeaths();
		int kills = AccountManager.GetKills();
		int headshot = AccountManager.GetHeadshot();
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write(xP);
		data.Write(deaths);
		data.Write(kills);
		data.Write(headshot);
		PhotonRPC.RPC("PhotonSetSelectPlayerInfo", message.sender, data);
	}

	[PunRPC]
	private void PhotonSetSelectPlayerInfo(PhotonMessage message)
	{
		int xp = message.ReadInt();
		int deaths = message.ReadInt();
		int kills = message.ReadInt();
		int headshot = message.ReadInt();
		if (!PlayerInfoList.ContainsKey(message.sender.UserId))
		{
			PlayerInfoClass playerInfoClass = new PlayerInfoClass();
			playerInfoClass.xp = xp;
			playerInfoClass.deaths = deaths;
			playerInfoClass.kills = kills;
			playerInfoClass.headshot = headshot;
			PlayerInfoList.Add(message.sender.UserId, playerInfoClass);
		}
		PhotonSetSelectPlayerInfo(xp, deaths, kills, headshot);
	}

	private void PhotonSetSelectPlayerInfo(int xp, int deaths, int kills, int headshot)
	{
		PlayerInfoXP.text = StringCache.Get(xp);
		PlayerInfoKD.text = ((float)kills / (float)deaths).ToString("f2");
		PlayerInfoHK.text = ((float)headshot / (float)kills).ToString("f2");
	}
}
