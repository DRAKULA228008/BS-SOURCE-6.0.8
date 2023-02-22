using System;
using System.Collections.Generic;
using BSCM;
using ExitGames.Client.Photon;
using FreeJSON;
using UnityEngine;

public class mPhotonSettings : MonoBehaviour
{
	private static string selectMap;

	private static bool newRegion;

	private static RoomInfo queueRoom;

	public static Action connectedCallback;

	public static string region
	{
		get
		{
			return nPlayerPrefs.GetString("Region", "Best");
		}
		set
		{
			nPlayerPrefs.SetString("Region", value);
		}
	}

	private void Start()
	{
		PhotonNetwork.automaticallySyncScene = true;
		PhotonNetwork.offlineMode = false;
		PhotonNetwork.UseRpcMonoBehaviourCache = true;
	}

	private void OnEnable()
	{
		PhotonNetwork.onConnectedToPhoton = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onConnectedToPhoton, new PhotonNetwork.VoidDelegate(OnConnectedToPhoton));
		PhotonNetwork.onDisconnectedFromPhoton = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onDisconnectedFromPhoton, new PhotonNetwork.VoidDelegate(OnDisconnectedFromPhoton));
		PhotonNetwork.onConnectionFail = (PhotonNetwork.DisconnectCauseDelegate)Delegate.Combine(PhotonNetwork.onConnectionFail, new PhotonNetwork.DisconnectCauseDelegate(OnConnectionFail));
		PhotonNetwork.onJoinedRoom = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onJoinedRoom, new PhotonNetwork.VoidDelegate(OnJoinedRoom));
		PhotonNetwork.onPhotonJoinRoomFailed = (PhotonNetwork.ResponseDelegate)Delegate.Combine(PhotonNetwork.onPhotonJoinRoomFailed, new PhotonNetwork.ResponseDelegate(OnPhotonJoinRoomFailed));
		PhotonNetwork.onPhotonCreateRoomFailed = (PhotonNetwork.ResponseDelegate)Delegate.Combine(PhotonNetwork.onPhotonCreateRoomFailed, new PhotonNetwork.ResponseDelegate(OnPhotonCreateRoomFailed));
		PhotonNetwork.onCustomAuthenticationFailed = (PhotonNetwork.StringDelegate)Delegate.Combine(PhotonNetwork.onCustomAuthenticationFailed, new PhotonNetwork.StringDelegate(OnCustomAuthenticationFailed));
		PhotonNetwork.onCustomAuthenticationResponse = (PhotonNetwork.ObjectsDelegate)Delegate.Combine(PhotonNetwork.onCustomAuthenticationResponse, new PhotonNetwork.ObjectsDelegate(OnCustomAuthenticationResponse));
	}

	private void OnDisable()
	{
		PhotonNetwork.onConnectedToPhoton = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onConnectedToPhoton, new PhotonNetwork.VoidDelegate(OnConnectedToPhoton));
		PhotonNetwork.onDisconnectedFromPhoton = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onDisconnectedFromPhoton, new PhotonNetwork.VoidDelegate(OnDisconnectedFromPhoton));
		PhotonNetwork.onConnectionFail = (PhotonNetwork.DisconnectCauseDelegate)Delegate.Remove(PhotonNetwork.onConnectionFail, new PhotonNetwork.DisconnectCauseDelegate(OnConnectionFail));
		PhotonNetwork.onJoinedRoom = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onJoinedRoom, new PhotonNetwork.VoidDelegate(OnJoinedRoom));
		PhotonNetwork.onPhotonJoinRoomFailed = (PhotonNetwork.ResponseDelegate)Delegate.Remove(PhotonNetwork.onPhotonJoinRoomFailed, new PhotonNetwork.ResponseDelegate(OnPhotonJoinRoomFailed));
		PhotonNetwork.onPhotonCreateRoomFailed = (PhotonNetwork.ResponseDelegate)Delegate.Remove(PhotonNetwork.onPhotonCreateRoomFailed, new PhotonNetwork.ResponseDelegate(OnPhotonCreateRoomFailed));
		PhotonNetwork.onCustomAuthenticationFailed = (PhotonNetwork.StringDelegate)Delegate.Remove(PhotonNetwork.onCustomAuthenticationFailed, new PhotonNetwork.StringDelegate(OnCustomAuthenticationFailed));
		PhotonNetwork.onCustomAuthenticationResponse = (PhotonNetwork.ObjectsDelegate)Delegate.Remove(PhotonNetwork.onCustomAuthenticationResponse, new PhotonNetwork.ObjectsDelegate(OnCustomAuthenticationResponse));
	}

	public static void Connect()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		if (PhotonNetwork.connected && !newRegion)
		{
			connectedCallback();
			return;
		}
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
		mPopUp.ShowText(Localization.Get("Connecting to the region") + "...");
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
        PhotonNetwork.AuthValues.UserId = AccountManager.instance.Data.AccountName;
        string bundleVersion = VersionManager.bundleVersion;
        string appID = mOthers.appid;
        if (PlayerPrefs.GetString("SelectRegion", "null") == "null")
        {
            print("1");
            PhotonNetwork.ConnectToMaster("app-eu.exitgamescloud.com", 5055, appID, bundleVersion);
        }
        else
        {
            print("1");
            PhotonNetwork.ConnectToMaster("app-eu.exitgamescloud.com", 5055, appID, bundleVersion);
        }
        newRegion = false;
    }

    public void SelectRegion(string region)
	{
		if (!(mPhotonSettings.region == region))
		{
			mPhotonSettings.region = region;
			newRegion = true;
			mVersionManager.UpdateRegion();
			if (PhotonNetwork.connected)
			{
				PhotonNetwork.Disconnect();
			}
		}
	}

	private void OnConnectedToPhoton()
	{
		if (PhotonNetwork.connected)
		{
			mVersionManager.UpdateRegion();
			PhotonNetwork.player.SetLevel(AccountManager.GetLevel());
			PhotonNetwork.player.SetClan(AccountManager.GetClan());
			PhotonNetwork.player.SetAvatarUrl(AccountManager.instance.Data.AvatarUrl);
			TimerManager.In("PhotonConnected", 1.5f, delegate
			{
				connectedCallback();
			});
		}
	}

	private void OnDisconnectedFromPhoton()
	{
		mPopUp.HideAll("Menu");
	}

	private void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		TimerManager.Cancel("PhotonConnected");
		UIToast.Show("Failed: " + cause);
	}

	private void OnConnectionFail(DisconnectCause cause)
	{
		TimerManager.Cancel("PhotonConnected");
		UIToast.Show("Fail: " + cause);
	}

	private void OnCustomAuthenticationFailed(string error)
	{
		TimerManager.Cancel("PhotonConnected");
		if (error.Contains("Custom authentication deserialization failed"))
		{
			UIToast.Show("Auth Fail: custom error");
			return;
		}
		switch (error)
		{
		case "oldversion":
		case "badversion":
			error = Localization.Get("Old Version");
			AccountManager.isConnect = false;
			break;
		case "session":
			error = Localization.Get("Session error");
			break;
		}
		UIToast.Show("Auth Fail: " + error);
	}

	private void OnCustomAuthenticationResponse(object[] obj)
	{
		TimerManager.In(0.1f, delegate
		{
			MonoBehaviour.print(obj.Length);
			for (int i = 0; i < obj.Length; i++)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)obj[i];
				MonoBehaviour.print(dictionary["Test"]);
			}
		});
	}

	private void OnJoinedRoom()
	{
		PlayerRoundManager.Clear();
		PlayerRoundManager.SetMode(PhotonNetwork.room.GetGameMode());
		if (PhotonNetwork.offlineMode)
		{
			LevelManager.LoadLevel(selectMap);
			return;
		}
		PhotonNetwork.isMessageQueueRunning = false;
		PhotonNetwork.LoadLevel(selectMap);
	}

	private void OnPhotonJoinRoomFailed(short code, string message)
	{
		mJoinServer.onBack();
		if (message == "Game full")
		{
			UIToast.Show(Localization.Get("The server is full"));
			return;
		}
		UIToast.Show("Error Code: " + code + " Message: " + message);
	}

	private void OnPhotonCreateRoomFailed(short code, string message)
	{
		mPopUp.HideAll("CreateServer");
		if (code == 32766)
		{
			UIToast.Show(Localization.Get("Server with this name already exists"));
			return;
		}
		UIToast.Show("Error Code: " + code + " Message: " + message);
	}

	public static void CreateOfficialServer(string name, string map, GameMode mode)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		mPopUp.ShowText(Localization.Get("Connecting") + "...");
		PhotonNetwork.player.SetPlayerID(AccountManager.instance.Data.ID);
		PhotonNetwork.player.ClearProperties();
		selectMap = map;
		Hashtable hashtable = PhotonNetwork.room.CreateRoomHashtable("off", mode, true);
		hashtable[PhotonCustomValue.minLevelKey] = (byte)AccountManager.GetLevel();
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 12;
		roomOptions.IsOpen = true;
		roomOptions.IsVisible = true;
		roomOptions.PublishUserId = true;
		roomOptions.CustomRoomProperties = hashtable;
		roomOptions.CustomRoomPropertiesForLobby = new string[5]
		{
			PhotonCustomValue.sceneNameKey,
			PhotonCustomValue.passwordKey,
			PhotonCustomValue.gameModeKey,
			PhotonCustomValue.officialServerKey,
			PhotonCustomValue.minLevelKey
		};
		PhotonNetwork.CreateRoom(name, roomOptions, null);
		AppMetrica.Instance.ReportEvent("Create Official Server", mode.ToString(), selectMap);
	}

	public static void CreateServer(string name, string map, GameMode mode, string password, int maxPlayers, bool custom)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		mPopUp.ShowText(Localization.Get("Creating Server") + "...");
		PhotonNetwork.player.SetPlayerID(AccountManager.instance.Data.ID);
		PhotonNetwork.player.ClearProperties();
		maxPlayers = ((!(map == "50Traps")) ? Mathf.Clamp(maxPlayers, 4, 12) : Mathf.Clamp(maxPlayers, 4, 32));
		selectMap = map;
		Hashtable hashtable = PhotonNetwork.room.CreateRoomHashtable(password, mode, false);
		if (mode == GameMode.Only)
		{
			hashtable[PhotonCustomValue.onlyWeaponKey] = (byte)mServerSettings.GetOnlyWeapon();
		}
		if (custom)
		{
			hashtable[PhotonCustomValue.customMapHash] = Manager.hash;
			hashtable[PhotonCustomValue.customMapUrl] = Manager.bundleUrl;
			hashtable[PhotonCustomValue.customMapModes] = Manager.modes;
		}
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = (byte)maxPlayers;
		roomOptions.IsOpen = true;
		roomOptions.IsVisible = true;
		roomOptions.PublishUserId = true;
		roomOptions.CustomRoomProperties = hashtable;
		if (custom)
		{
			roomOptions.CustomRoomPropertiesForLobby = new string[6]
			{
				PhotonCustomValue.sceneNameKey,
				PhotonCustomValue.passwordKey,
				PhotonCustomValue.gameModeKey,
				PhotonCustomValue.customMapHash,
				PhotonCustomValue.customMapUrl,
				PhotonCustomValue.customMapModes
			};
		}
		else if (mode == GameMode.Only)
		{
			roomOptions.CustomRoomPropertiesForLobby = new string[4]
			{
				PhotonCustomValue.sceneNameKey,
				PhotonCustomValue.passwordKey,
				PhotonCustomValue.gameModeKey,
				PhotonCustomValue.onlyWeaponKey
			};
		}
		else
		{
			roomOptions.CustomRoomPropertiesForLobby = new string[3]
			{
				PhotonCustomValue.sceneNameKey,
				PhotonCustomValue.passwordKey,
				PhotonCustomValue.gameModeKey
			};
		}
		LevelManager.customScene = custom;
		PhotonNetwork.CreateRoom(name, roomOptions, null);
		AppMetrica.Instance.ReportEvent((!custom) ? "Create Server" : "Create Custom Server", mode.ToString(), selectMap);
	}

	public static void QueueServer(RoomInfo room)
	{
		queueRoom = room;
		mPopUp.ShowPopup(Localization.Get("Please wait") + "...", Localization.Get("Queue"), Localization.Get("Exit"), delegate
		{
			queueRoom = null;
			TimerManager.Cancel("QueueTimer");
			mJoinServer.onBack();
		});
		TimerManager.In("QueueTimer", 1f, -1, 1f, delegate
		{
			if (queueRoom == null)
			{
				TimerManager.Cancel("QueueTimer");
			}
			else
			{
				RoomInfo[] roomList = PhotonNetwork.GetRoomList();
				for (int i = 0; i < roomList.Length; i++)
				{
					if (roomList[i].Name == queueRoom.Name)
					{
						if (roomList[i].PlayerCount != roomList[i].MaxPlayers)
						{
							TimerManager.Cancel("QueueTimer");
							JoinServer(queueRoom);
						}
						break;
					}
				}
			}
		});
	}

	public void CreateServerOffline(string map)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
		mPopUp.ShowText(Localization.Get("Loading") + "...");
		TimerManager.In(0.2f, delegate
		{
			mPopUp.ShowText(Localization.Get("Loading") + "...");
			selectMap = map;
			PhotonNetwork.offlineMode = true;
			LevelManager.customScene = false;
			PhotonNetwork.CreateRoom(map);
		});
	}

	public static void CreateCustomServerOffline(string map, GameMode mode)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
		mPopUp.ShowText(Localization.Get("Loading") + "...");
		Hashtable customRoomProperties = PhotonNetwork.room.CreateRoomHashtable(string.Empty, mode, false);
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 1;
		roomOptions.IsOpen = false;
		roomOptions.IsVisible = false;
		roomOptions.CustomRoomProperties = customRoomProperties;
		selectMap = map;
		PhotonNetwork.offlineMode = true;
		LevelManager.customScene = true;
		PhotonNetwork.CreateRoom(map, roomOptions, null);
	}

    public static void JoinServer(RoomInfo room)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		if (queueRoom == null)
		{
			mPopUp.ShowText(Localization.Get("Connecting") + "...");
			AppMetrica.Instance.ReportEvent("Join Server", room.GetGameMode().ToString(), room.GetSceneName());
		}
		PhotonNetwork.player.SetPlayerID(AccountManager.instance.Data.ID);
		PhotonNetwork.player.ClearProperties();
		selectMap = room.GetSceneName();
		PhotonNetwork.JoinRoom(room.Name);
	}
}
