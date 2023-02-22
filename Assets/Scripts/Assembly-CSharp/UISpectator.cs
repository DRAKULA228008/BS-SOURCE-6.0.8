using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class UISpectator : Photon.MonoBehaviour
{
	public GameObject Panel;

	public UIGrid BlueGrid;

	public UIGrid RedGrid;

	public UISpectatorElement[] BlueTeam;

	public UISpectatorElement[] RedTeam;

	public UISprite InfoSprite;

	public UISprite GradientSprite;

	public UILabel NameLabel;

	public UITexture AvatarTexture;

	public UILabel PlayerIdLabel;

	public UILabel ClanLabel;

	public UILabel KillsLabel;

	public UILabel DeathsLabel;

	public UILabel PingLabel;

	public UILabel HealthLabel;

	private PhotonPlayer selectPlayer;

	private bool isActive;

	private int InfoTimerId = -1;

	private static UISpectator instance;

	private void Start()
	{
		instance = this;
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerDisconnected));
	}

	private void OnDisable()
	{
		ControllerManager.SetWeaponEvent -= SetWeapon;
		ControllerManager.SetDeadEvent -= SetDead;
		ControllerManager.SetTeamEvent -= SetTeam;
		ControllerManager.SetHealthEvent -= SetHealth;
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
		CameraManager.selectPlayerEvent -= SetSelectPlayer;
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerDisconnected));
	}

	private void GetButtonDown(string name)
	{
		if (isActive && name == "Reload")
		{
			BlueGrid.gameObject.SetActive(!BlueGrid.gameObject.activeSelf);
			RedGrid.gameObject.SetActive(!RedGrid.gameObject.activeSelf);
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		if (isActive)
		{
			UpdateList();
		}
	}

	public static void SetActive(bool active)
	{
		instance.Panel.SetActive(active);
		instance.isActive = active;
		if (active)
		{
			ControllerManager.SetWeaponEvent += instance.SetWeapon;
			ControllerManager.SetDeadEvent += instance.SetDead;
			ControllerManager.SetTeamEvent += instance.SetTeam;
			ControllerManager.SetHealthEvent += instance.SetHealth;
			InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(instance.GetButtonDown));
			CameraManager.selectPlayerEvent += SetSelectPlayer;
			instance.UpdateList();
		}
		else
		{
			instance.OnDisable();
		}
	}

	public static bool GetActive()
	{
		return instance.isActive;
	}

	public static void SetSelectPlayer(int playerID)
	{
		if (instance.isActive)
		{
			instance.selectPlayer = PhotonPlayer.Find(playerID);
			if (instance.selectPlayer != null && CameraManager.type == CameraType.FirstPerson)
			{
				UIToast.Show(instance.selectPlayer.UserId);
			}
		}
	}

	private void UpdateDataSelectPlayer()
	{
		if (selectPlayer == null)
		{
			InfoSprite.cachedGameObject.SetActive(false);
			TimerManager.Cancel(InfoTimerId);
		}
		else
		{
			KillsLabel.text = StringCache.Get(selectPlayer.GetKills());
			DeathsLabel.text = StringCache.Get(selectPlayer.GetDeaths());
			PingLabel.text = StringCache.Get(selectPlayer.GetPing());
		}
	}

	private void UpdateList()
	{
		for (int i = 0; i < BlueTeam.Length; i++)
		{
			BlueTeam[i].LineSprite.cachedGameObject.SetActive(false);
		}
		for (int j = 0; j < RedTeam.Length; j++)
		{
			RedTeam[j].LineSprite.cachedGameObject.SetActive(false);
		}
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		List<PhotonPlayer> list2 = new List<PhotonPlayer>();
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		for (int k = 0; k < playerList.Length; k++)
		{
			if (playerList[k].GetTeam() == Team.Blue && !playerList[k].IsLocal)
			{
				list.Add(playerList[k]);
			}
			else if (playerList[k].GetTeam() == Team.Red && !playerList[k].IsLocal)
			{
				list2.Add(playerList[k]);
			}
		}
		list.Sort(SortByKills);
		list2.Sort(SortByKills);
		ControllerManager controllerManager = null;
		if (PhotonNetwork.player.GetTeam() != Team.Red)
		{
			for (int l = 0; l < list.Count && l <= 5; l++)
			{
				BlueTeam[l].LineSprite.cachedGameObject.SetActive(true);
				BlueTeam[l].SetData(list[l]);
				controllerManager = ControllerManager.FindController(list[l].ID);
				if (controllerManager != null && controllerManager.playerSkin != null)
				{
					BlueTeam[l].SetHealth(controllerManager.playerSkin.Health);
					if (controllerManager.playerSkin.SelectWeapon != null)
					{
						BlueTeam[l].SetWeapon(controllerManager.playerSkin.SelectWeapon.Data.weapon, controllerManager.playerSkin.SelectWeapon.Data.skin);
					}
				}
			}
			BlueGrid.Reposition();
			for (int m = 0; m < BlueTeam.Length; m++)
			{
				BlueTeam[m].UpdateWidget();
			}
		}
		if (PhotonNetwork.player.GetTeam() == Team.Blue)
		{
			return;
		}
		for (int n = 0; n < list2.Count && n <= 5; n++)
		{
			RedTeam[n].LineSprite.cachedGameObject.SetActive(true);
			RedTeam[n].SetData(list2[n]);
			controllerManager = ControllerManager.FindController(list2[n].ID);
			if (controllerManager != null && controllerManager.playerSkin != null)
			{
				RedTeam[n].SetHealth(controllerManager.playerSkin.Health);
				if (controllerManager.playerSkin.SelectWeapon != null)
				{
					RedTeam[n].SetWeapon(controllerManager.playerSkin.SelectWeapon.Data.weapon, controllerManager.playerSkin.SelectWeapon.Data.skin);
				}
			}
		}
		RedGrid.Reposition();
		for (int num = 0; num < RedTeam.Length; num++)
		{
			RedTeam[num].UpdateWidget();
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

	private void SetWeapon(int playerID, int weaponID, int skinID)
	{
		if (!isActive)
		{
			return;
		}
		for (int i = 0; i < BlueTeam.Length; i++)
		{
			if (BlueTeam[i].SetWeapon(playerID, weaponID, skinID))
			{
				return;
			}
		}
		for (int j = 0; j < RedTeam.Length && !RedTeam[j].SetWeapon(playerID, weaponID, skinID); j++)
		{
		}
	}

	private void SetDead(int playerID, bool dead)
	{
		if (!isActive)
		{
			return;
		}
		for (int i = 0; i < BlueTeam.Length; i++)
		{
			if (BlueTeam[i].SetDead(playerID, dead))
			{
				return;
			}
		}
		for (int j = 0; j < RedTeam.Length && !RedTeam[j].SetDead(playerID, dead); j++)
		{
		}
	}

	private void SetHealth(int playerID, byte health)
	{
		if (!isActive)
		{
			return;
		}
		if (selectPlayer != null && selectPlayer.ID == playerID)
		{
			HealthLabel.text = "+" + StringCache.Get(health);
		}
		for (int i = 0; i < BlueTeam.Length; i++)
		{
			if (BlueTeam[i].SetHealth(playerID, health))
			{
				return;
			}
		}
		for (int j = 0; j < RedTeam.Length && !RedTeam[j].SetHealth(playerID, health); j++)
		{
		}
	}

	private void SetTeam(int playerID, Team team)
	{
		if (isActive)
		{
			UpdateList();
		}
	}
}
