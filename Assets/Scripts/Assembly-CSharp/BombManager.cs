using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class BombManager : Photon.MonoBehaviour
{
	public GameObject Bomb;

	public Transform ZoneA;

	public Transform ZoneB;

	private int PlayerBomb = -1;

	private int PlayerDeactiveBomb = -1;

	private int Zone = -1;

	public static bool BombPlaced;

	public static bool BuyTime;

	private bool BombPlacing;

	public BombAudio BombAudio;

	public ParticleSystem Effect;

	public ControllerManager BombController;

	public Transform BombPlayerModel;

	public GameObject BombPlayerModel2;

	public GameObject LineBomb;

	private Vector3 BombPosition;

	private bool BuyZone;

	private UISprite DefuseBombIcon;

	private float IgnoreDropBombTime = -1f;

	private float DropBombTime = -1f;

	private static BombManager instance;

	public UISprite defuseBombIcon
	{
		get
		{
			if (DefuseBombIcon == null)
			{
				DefuseBombIcon = UIElements.Get<UISprite>("BombIcon");
			}
			return DefuseBombIcon;
		}
	}

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.Bomb && PhotonNetwork.room.GetGameMode() != GameMode.Bomb2)
		{
			Resources.UnloadAsset(BombAudio.BombAudioClip);
			Resources.UnloadAsset(Effect.GetComponent<AudioSource>().clip);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			instance = this;
			ZoneA.gameObject.SetActive(true);
			ZoneB.gameObject.SetActive(true);
			GameManager.defaultScore = false;
		}
	}

	private void Start()
	{
		PhotonRPC.AddMessage("SetStartRoundBomb", SetStartRoundBomb);
		PhotonRPC.AddMessage("PhotonMasterPickupBomb", PhotonMasterPickupBomb);
		PhotonRPC.AddMessage("PhotonPickupBomb", PhotonPickupBomb);
		PhotonRPC.AddMessage("PhotonSetBomb", PhotonSetBomb);
		PhotonRPC.AddMessage("PhotonDeactiveBomb", PhotonDeactiveBomb);
		PhotonRPC.AddMessage("PhotonDeactiveBoom", PhotonDeactiveBoom);
		PhotonRPC.AddMessage("PhotonDeactiveBombExit", PhotonDeactiveBombExit);
		PhotonRPC.AddMessage("PhotonSetPosition", PhotonSetPosition);
		PhotonRPC.AddMessage("PhotonOnPhotonPlayerConnected", PhotonOnPhotonPlayerConnected);
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
		InputManager.GetButtonUpEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonUpEvent, new InputManager.ButtonDelegate(GetButtonUp));
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerDisconnected));
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
		InputManager.GetButtonUpEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonUpEvent, new InputManager.ButtonDelegate(GetButtonUp));
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerDisconnected));
	}

	private void GetButtonDown(string name)
	{
		if (name == "Store" && BuyZone)
		{
			UIControllerList.BuyWeapons.cachedGameObject.SetActive(true);
		}
		if (name == "Bomb")
		{
			if (PhotonNetwork.player.GetTeam() == Team.Red && Zone != -nValue.int1 && PlayerBomb == PhotonNetwork.player.ID && !BombPlaced)
			{
				if (PlayerInput.instance.PlayerWeapon.isFire)
				{
					return;
				}
				UIDuration.StartDuration(nValue.int5, true, SetBomb);
				PlayerInput.instance.SetMove(false);
				PlayerInput.instance.SetLook(false);
				BombPlacing = true;
			}
			else if (PhotonNetwork.player.GetTeam() == Team.Blue && Zone != -nValue.int1 && PlayerDeactiveBomb == PhotonNetwork.player.ID && BombPlaced)
			{
				if (PhotonNetwork.room.GetGameMode() == GameMode.Bomb)
				{
					UIDuration.StartDuration(nValue.int5, true, DeactiveBoom);
				}
				else
				{
					UIDuration.StartDuration((!UIDefuseKit.defuseKit) ? nValue.int8 : nValue.int4, true, DeactiveBoom);
				}
				PlayerInput.instance.SetMove(false);
				PlayerInput.instance.SetLook(false);
				BombPlacing = true;
			}
		}
		else if (BombPlacing)
		{
			UIDuration.StopDuration();
			if (PlayerInput.instance != null)
			{
				PlayerInput.instance.SetMove(true);
				PlayerInput.instance.SetLook(true);
			}
			BombPlacing = false;
		}
		if (name == "Reload")
		{
			DropBombTime = 0f;
		}
	}

	private void GetButtonUp(string name)
	{
		if (name == "Bomb")
		{
			UIDuration.StopDuration();
			PlayerInput.instance.SetMove(true);
			PlayerInput.instance.SetLook(true);
			BombPlacing = false;
		}
		if (name == "Reload")
		{
			DropBombTime = -1f;
		}
	}

	private void Update()
	{
		if (DropBombTime >= 0f)
		{
			if (DropBombTime >= 0.5f)
			{
				DropBomb();
				DropBombTime = -1f;
			}
			DropBombTime += Time.deltaTime;
		}
	}

	private void StartRound()
	{
		BombPlaced = false;
		BombPlacing = false;
		Bomb.SetActive(false);
		PlayerBomb = -nValue.int1;
		PlayerDeactiveBomb = -nValue.int1;
		Zone = -nValue.int1;
		BombPosition = Vector3.zero;
		BombAudio.Stop();
		Effect.Stop();
		Effect.GetComponent<AudioSource>().Stop();
		if (GameManager.roundState == RoundState.WaitPlayer)
		{
			return;
		}
		TimerManager.In(nValue.int1, delegate
		{
			if (GameManager.roundState != 0 && PhotonNetwork.isMasterClient)
			{
				List<PhotonPlayer> list = new List<PhotonPlayer>();
				for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
				{
					if (PhotonNetwork.playerList[i].GetTeam() == Team.Red)
					{
						list.Add(PhotonNetwork.playerList[i]);
					}
				}
				if (list.Count > nValue.int0)
				{
					PhotonPlayer photonPlayer;
					do
					{
						photonPlayer = list[UnityEngine.Random.Range(nValue.int0, list.Count)];
					}
					while (photonPlayer.GetDead());
					PhotonDataWrite data = PhotonRPC.GetData();
					data.Write(photonPlayer.ID);
					PhotonRPC.RPC("SetStartRoundBomb", PhotonTargets.All, data);
				}
			}
		});
	}

	[PunRPC]
	private void SetStartRoundBomb(PhotonMessage message)
	{
		PlayerBomb = message.ReadInt();
		if (PlayerBomb == PhotonNetwork.player.ID)
		{
			UIWarningToast.Show(Localization.Get("You have a bomb"));
			defuseBombIcon.cachedGameObject.SetActive(true);
			defuseBombIcon.spriteName = "BombIcon";
			if (BombPlayerModel == null)
			{
				BombPlayerModel = ((GameObject)UnityEngine.Object.Instantiate(BombPlayerModel2)).transform;
			}
			BombPlayerModel.parent = null;
			BombPlayerModel.position = Vector3.up * 1000f;
			return;
		}
		if (PhotonNetwork.player.GetTeam() == Team.Blue && UIDefuseKit.defuseKit)
		{
			defuseBombIcon.cachedGameObject.SetActive(true);
			defuseBombIcon.spriteName = "Pilers";
		}
		else
		{
			defuseBombIcon.cachedGameObject.SetActive(false);
		}
		if (BombPlayerModel == null)
		{
			BombPlayerModel = ((GameObject)UnityEngine.Object.Instantiate(BombPlayerModel2)).transform;
		}
		BombPlayerModel.parent = null;
		BombPlayerModel.position = Vector3.up * 1000f;
		if (PhotonNetwork.player.GetTeam() == Team.Red)
		{
			UIWarningToast.Show(PhotonPlayer.Find(PlayerBomb).UserId + " " + Localization.Get("picked up a bomb"));
		}
		BombController = ControllerManager.FindController(PlayerBomb);
		if (BombController != null)
		{
			BombPlayerModel.SetParent(BombController.playerSkin.PlayerWeaponContainers[2]);
			BombPlayerModel.localPosition = new Vector3(0.1f, 0f, 0.1f);
			BombPlayerModel.localEulerAngles = new Vector3(-90f, 0f, 0f);
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		TimerManager.In(nValue.float05, delegate
		{
			PhotonDataWrite data = PhotonRPC.GetData();
			data.Write(BombPlaced);
			if (BombPlaced)
			{
				float value = UIScore2.timeData.endTime - Time.time;
				data.Write(BombPosition);
				data.Write(value);
				PhotonRPC.RPC("PhotonOnPhotonPlayerConnected", playerConnect, data);
			}
			else
			{
				PhotonRPC.RPC("PhotonOnPhotonPlayerConnected", playerConnect, data);
			}
		});
	}

	[PunRPC]
	private void PhotonOnPhotonPlayerConnected(PhotonMessage message)
	{
		BombPlaced = message.ReadBool();
		if (BombPlaced)
		{
			BombPosition = message.ReadVector3();
			float time = message.ReadFloat() - (float)(PhotonNetwork.time - message.timestamp);
			UIScore2.StartTime(time, Boom);
			BombAudio.Play(time);
			SetPosition(BombPosition);
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		if (PhotonNetwork.isMasterClient && playerDisconnect.ID == PlayerBomb)
		{
			SetRandomPlayer();
		}
	}

	private void SetRandomPlayer()
	{
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			if (PhotonNetwork.playerList[i].GetTeam() == Team.Red && !PhotonNetwork.playerList[i].GetDead())
			{
				list.Add(PhotonNetwork.playerList[i]);
			}
		}
		if (list.Count > nValue.int0)
		{
			PhotonPlayer photonPlayer = list[UnityEngine.Random.Range(nValue.int0, list.Count)];
			PhotonDataWrite data = PhotonRPC.GetData();
			data.Write(photonPlayer.ID);
			PhotonRPC.RPC("SetStartRoundBomb", PhotonTargets.All, data);
		}
	}

	public static void DeadPlayer()
	{
		if (PhotonNetwork.player.ID == instance.PlayerBomb)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(PlayerInput.instance.PlayerTransform.position, Vector3.down, out hitInfo, nValue.int50))
			{
				PhotonDataWrite data = PhotonRPC.GetData();
				data.Write(hitInfo.point);
				data.Write(hitInfo.normal);
				data.Write(true);
				PhotonRPC.RPC("PhotonSetPosition", PhotonTargets.All, data);
			}
			else
			{
				instance.SetRandomPlayer();
			}
		}
		instance.defuseBombIcon.cachedGameObject.SetActive(false);
		if (PhotonNetwork.player.ID == instance.PlayerDeactiveBomb)
		{
			PhotonRPC.RPC("PhotonDeactiveBombExit", PhotonTargets.All);
		}
		instance.BombPlacing = false;
		UIControllerList.Bomb.cachedGameObject.SetActive(false);
		UIControllerList.Store.cachedGameObject.SetActive(false);
		UIControllerList.BuyWeapons.cachedGameObject.SetActive(false);
		UIDuration.StopDuration();
		PlayerInput.instance.SetMove(true);
		PlayerInput.instance.SetLook(true);
	}

	public static void DropBomb()
	{
		RaycastHit hitInfo;
		if (GameManager.roundState == RoundState.PlayRound && PhotonNetwork.player.ID == instance.PlayerBomb && Physics.Raycast(PlayerInput.instance.PlayerTransform.position, Vector3.down, out hitInfo, nValue.int50))
		{
			PhotonDataWrite data = PhotonRPC.GetData();
			data.Write(hitInfo.point);
			data.Write(hitInfo.normal);
			data.Write(false);
			PhotonRPC.RPC("PhotonSetPosition", PhotonTargets.All, data);
			instance.IgnoreDropBombTime = Time.time + 2f;
			instance.defuseBombIcon.cachedGameObject.SetActive(false);
		}
	}

	[PunRPC]
	private void PhotonSetPosition(PhotonMessage message)
	{
		Vector3 pos = message.ReadVector3();
		Vector3 normal = message.ReadVector3();
		if (message.ReadBool() && PlayerBomb != -nValue.int1 && PhotonNetwork.player.GetTeam() == Team.Red)
		{
			UIWarningToast.Show(PhotonPlayer.Find(PlayerBomb).UserId + " " + Localization.Get("lost the bomb"));
		}
		PlayerBomb = -nValue.int1;
		SetPosition(pos, normal);
		if (BombPlayerModel == null)
		{
			BombPlayerModel = ((GameObject)UnityEngine.Object.Instantiate(BombPlayerModel2)).transform;
		}
		BombPlayerModel.parent = null;
		BombPlayerModel.position = Vector3.up * 1000f;
	}

	public static void SetPosition(Vector3 pos, Vector3 normal)
	{
		instance.Bomb.transform.position = pos + normal * nValue.float001;
		instance.Bomb.transform.rotation = Quaternion.LookRotation(normal);
		instance.Bomb.SetActive(true);
	}

	public static void SetPosition(Vector3 pos)
	{
		instance.Bomb.transform.position = pos;
		instance.Bomb.transform.rotation = Quaternion.Euler(-nValue.int90, UnityEngine.Random.Range(nValue.int0, nValue.int360), nValue.int0);
		instance.Bomb.SetActive(true);
	}

	public void OnTriggerEnterBomb()
	{
		if (GameManager.roundState == RoundState.PlayRound && !(IgnoreDropBombTime > Time.time))
		{
			PhotonRPC.RPC("PhotonMasterPickupBomb", PhotonTargets.MasterClient);
		}
	}

	public void OnTriggerExitBomb()
	{
		if (BombPlaced)
		{
			UIControllerList.Bomb.cachedGameObject.SetActive(false);
			if (PhotonNetwork.player.ID == PlayerDeactiveBomb)
			{
				PhotonRPC.RPC("PhotonDeactiveBombExit", PhotonTargets.All);
			}
		}
	}

	[PunRPC]
	private void PhotonMasterPickupBomb(PhotonMessage message)
	{
		PhotonPlayer sender = message.sender;
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write(message.sender.ID);
		if (BombPlaced)
		{
			if (sender.GetTeam() == Team.Blue && PlayerDeactiveBomb == -nValue.int1)
			{
				PhotonRPC.RPC("PhotonDeactiveBomb", PhotonTargets.All, data);
			}
		}
		else if (sender.GetTeam() == Team.Red && PlayerBomb == -nValue.int1)
		{
			PhotonRPC.RPC("PhotonPickupBomb", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void PhotonPickupBomb(PhotonMessage message)
	{
		Bomb.SetActive(false);
		PlayerBomb = message.ReadInt();
		if (PlayerBomb == PhotonNetwork.player.ID)
		{
			UIWarningToast.Show(Localization.Get("You have a bomb"));
			defuseBombIcon.cachedGameObject.SetActive(true);
			defuseBombIcon.spriteName = "BombIcon";
			return;
		}
		if (BombPlayerModel == null)
		{
			BombPlayerModel = ((GameObject)UnityEngine.Object.Instantiate(BombPlayerModel2)).transform;
		}
		BombPlayerModel.parent = null;
		BombPlayerModel.position = Vector3.up * 1000f;
		if (PhotonNetwork.player.GetTeam() == Team.Red)
		{
			UIWarningToast.Show(PhotonPlayer.Find(PlayerBomb).UserId + " " + Localization.Get("picked up a bomb"));
		}
		BombController = ControllerManager.FindController(PlayerBomb);
		if (BombController != null)
		{
			BombPlayerModel.SetParent(BombController.playerSkin.PlayerWeaponContainers[2]);
			BombPlayerModel.localPosition = new Vector3(0.1f, 0f, 0.1f);
			BombPlayerModel.localEulerAngles = new Vector3(-90f, 0f, 0f);
		}
	}

	[PunRPC]
	private void PhotonDeactiveBomb(PhotonMessage message)
	{
		PlayerDeactiveBomb = message.ReadInt();
		if (PlayerDeactiveBomb == PhotonNetwork.player.ID)
		{
			UIControllerList.Bomb.cachedGameObject.SetActive(true);
		}
	}

	[PunRPC]
	private void PhotonDeactiveBombExit(PhotonMessage message)
	{
		PlayerDeactiveBomb = -nValue.int1;
	}

	public void OnTriggerEnterZone(int zone)
	{
		Zone = zone;
		if (PhotonNetwork.player.GetTeam() == Team.Red && PlayerBomb == PhotonNetwork.player.ID && GameManager.roundState == RoundState.PlayRound)
		{
			UIControllerList.Bomb.cachedGameObject.SetActive(true);
		}
	}

	public void OnTriggerExitZone()
	{
		Zone = -nValue.int1;
		UIControllerList.Bomb.cachedGameObject.SetActive(false);
		UIDuration.StopDuration();
		PlayerInput.instance.SetMove(true);
		PlayerInput.instance.SetLook(true);
		BombPlacing = false;
	}

	private void SetBomb()
	{
		UIControllerList.Bomb.cachedGameObject.SetActive(false);
		UIDuration.StopDuration();
		PlayerInput.instance.SetMove(true);
		PlayerInput.instance.SetLook(true);
		if (GameManager.roundState == RoundState.PlayRound)
		{
			PhotonDataWrite data = PhotonRPC.GetData();
			data.Write(PlayerInput.instance.PlayerTransform.position - new Vector3(nValue.int0, nValue.float008, nValue.int0));
			PhotonRPC.RPC("PhotonSetBomb", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void PhotonSetBomb(PhotonMessage message)
	{
		if (GameManager.roundState == RoundState.PlayRound)
		{
			UIWarningToast.Show(Localization.Get("The bomb has been planted"));
			BombPlacing = false;
			BombPlaced = true;
			if (BombPlayerModel == null)
			{
				BombPlayerModel = ((GameObject)UnityEngine.Object.Instantiate(BombPlayerModel2)).transform;
			}
			BombPlayerModel.parent = null;
			BombPlayerModel.position = Vector3.up * 1000f;
			BombController = null;
			PlayerBomb = -nValue.int1;
			float time = 35f - (float)(PhotonNetwork.time - message.timestamp);
			UIScore2.StartTime(time, Boom);
			SetPosition(message.ReadVector3());
			BombAudio.Play(time);
			defuseBombIcon.cachedGameObject.SetActive(false);
		}
	}

	private void Boom()
	{
		BombAudio.Boom();
		Effect.Play();
		Effect.GetComponent<AudioSource>().Play();
		Effect.transform.position = Bomb.transform.position;
		BombAudio.Stop();
		Bomb.SetActive(false);
		BombPlacing = false;
		BombPlaced = false;
		PlayerBomb = -nValue.int1;
		PlayerDeactiveBomb = -nValue.int1;
		UIScore2.timeData.active = false;
		if (PhotonNetwork.room.GetGameMode() == GameMode.Bomb)
		{
			BombMode.instance.Boom();
		}
		else
		{
			BombMode2.instance.Boom();
		}
		UIDuration.StopDuration();
		if (GameManager.player != null && !GameManager.player.Dead)
		{
			GameManager.player.SetMove(true);
			int value = (nValue.int50 - (int)Vector3.Distance(PlayerInput.instance.PlayerTransform.position, Bomb.transform.position)) * nValue.int5;
			value = Mathf.Clamp(value, nValue.int0, nValue.int100);
			if (value > nValue.int0)
			{
				DamageInfo damageInfo = DamageInfo.Get(value, Vector3.zero, Team.None, nValue.int0, nValue.int0, -nValue.int1, false);
				PlayerInput.instance.Damage(damageInfo);
			}
			PlayerInput.instance.FPCamera.AddRollForce(UnityEngine.Random.Range((float)(-nValue.int3), (float)nValue.int3));
		}
	}

	private void DeactiveBoom()
	{
		UIControllerList.Bomb.cachedGameObject.SetActive(false);
		UIDuration.StopDuration();
		PlayerInput.instance.SetMove(true);
		PlayerInput.instance.SetLook(true);
		PhotonRPC.RPC("PhotonDeactiveBoom", PhotonTargets.All);
	}

	[PunRPC]
	private void PhotonDeactiveBoom(PhotonMessage message)
	{
		BombAudio.Stop();
		Bomb.SetActive(false);
		Bomb.SetActive(false);
		BombPlacing = false;
		BombPlaced = false;
		PlayerBomb = -nValue.int1;
		PlayerDeactiveBomb = -nValue.int1;
		UIScore2.timeData.active = false;
		if (PhotonNetwork.room.GetGameMode() == GameMode.Bomb)
		{
			BombMode.instance.DeactiveBoom();
		}
		else
		{
			BombMode2.instance.DeactiveBoom();
		}
	}

	public void OnTriggerEnterZoneBuy(int team)
	{
		if (PhotonNetwork.room.GetGameMode() == GameMode.Bomb2 && PhotonNetwork.player.GetTeam() == (Team)team && GameManager.roundState == RoundState.PlayRound && (BuyTime || UIScore2.timeData.endTime - Time.time > 90f))
		{
			UIControllerList.Store.cachedGameObject.SetActive(true);
			BuyZone = true;
		}
	}

	public void OnTriggerExitZoneBuy()
	{
		if (PhotonNetwork.room.GetGameMode() == GameMode.Bomb2)
		{
			UIControllerList.Store.cachedGameObject.SetActive(false);
			UIControllerList.BuyWeapons.cachedGameObject.SetActive(false);
			BuyZone = false;
		}
	}

	public static int GetPlayerBombID()
	{
		return instance.PlayerBomb;
	}
}
