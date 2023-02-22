using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraFirstPerson : MonoBehaviour
{
	public CameraManager cameraManager;

	private Camera weaponCamera;

	public Vector3 headPos;

	private ControllerManager target;

	private int index;

	private Dictionary<int, FPWeaponShooter> weaponList = new Dictionary<int, FPWeaponShooter>();

	private FPWeaponShooter selectWeapon;

	private Transform cameraTransform;

	private void Start()
	{
		cameraTransform = cameraManager.cameraTransform;
	}

	public void Active()
	{
		Active(-1);
	}

	public void Active(object[] parameters)
	{
		if (parameters != null && parameters.Length > 0)
		{
			Active((int)parameters[0]);
		}
		else
		{
			Active(-1);
		}
	}

	public void Active(int playerID)
	{
		cameraTransform.gameObject.SetActive(true);
		if (weaponCamera == null)
		{
			CreateWeaponCamera();
		}
		else
		{
			weaponCamera.gameObject.SetActive(true);
		}
		UpdateSelectPlayer(playerID);
		LODObject.Target = cameraTransform;
		SkyboxManager.GetCameraParent().localEulerAngles = Vector3.zero;
		UICrosshair.SetActiveCrosshair(true);
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
		ControllerManager.SetWeaponEvent += SetWeaponEvent;
		ControllerManager.SetDeadEvent += SetDeadEvent;
		ControllerManager.SetFireEvent += SetFireEvent;
		ControllerManager.SetReloadEvent += SetReloadEvent;
	}

	public void Deactive()
	{
		if (CameraManager.type == CameraType.FirstPerson)
		{
			cameraTransform.gameObject.SetActive(false);
			if (weaponCamera != null)
			{
				weaponCamera.gameObject.SetActive(false);
			}
			DeactiveWeapons();
			if (target != null)
			{
				target.playerSkin.PlayerAnimator.rootPos = Vector3.zero;
			}
			UICrosshair.SetActiveCrosshair(false);
			InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
			ControllerManager.SetWeaponEvent -= SetWeaponEvent;
			ControllerManager.SetDeadEvent -= SetDeadEvent;
			ControllerManager.SetFireEvent -= SetFireEvent;
			ControllerManager.SetReloadEvent -= SetReloadEvent;
		}
	}

	private void GetButtonDown(string name)
	{
		if (CameraManager.type == CameraType.FirstPerson)
		{
			switch (name)
			{
			case "Fire":
				UpdateSelectPlayer();
				break;
			}
		}
	}

	public void OnUpdate()
	{
		if (CameraManager.type == CameraType.FirstPerson)
		{
			if (target == null)
			{
				CameraManager.SetType(CameraType.Spectate);
			}
			cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, target.playerSkin.PhotonPosition + headPos, Time.deltaTime * 10f);
			cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, target.playerSkin.PhotonRotation * Quaternion.Euler(target.playerSkin.Rotate * -60f, 0f, 0f), Time.deltaTime * 10f);
			if (selectWeapon != null)
			{
				selectWeapon.FPWeapon.SpectatorVelocity = target.playerSkin.GetMove() * 30f;
			}
			SkyboxManager.GetCamera().rotation = cameraTransform.rotation;
		}
	}

	public void UpdateSelectPlayer()
	{
		UpdateSelectPlayer(-1);
	}

	public void UpdateSelectPlayer(int playerID)
	{
		if (playerID != -1)
		{
			for (int i = 0; i < ControllerManager.ControllerList.Count; i++)
			{
				ControllerManager controllerManager = ControllerManager.ControllerList[i];
				if (controllerManager.photonView.ownerId == playerID)
				{
					if (controllerManager.playerSkin != null && controllerManager.playerSkin.isPlayerActive)
					{
						if (target != null)
						{
							target.playerSkin.PlayerAnimator.rootPos = Vector3.zero;
						}
						target = controllerManager;
						target.playerSkin.PlayerAnimator.rootPos = Vector3.back * 2f;
						cameraTransform.localPosition = target.playerSkin.PhotonPosition + headPos;
						cameraTransform.localRotation = target.playerSkin.PhotonRotation * Quaternion.Euler(target.playerSkin.Rotate * -60f, 0f, 0f);
						UpdateWeapon();
						cameraManager.OnSelectPlayer(target.photonView.ownerId);
						return;
					}
					break;
				}
			}
		}
		List<ControllerManager> list = new List<ControllerManager>();
		for (int j = 0; j < ControllerManager.ControllerList.Count; j++)
		{
			ControllerManager controllerManager = ControllerManager.ControllerList[j];
			if (CameraManager.Team)
			{
				if (!controllerManager.photonView.owner.GetDead() && controllerManager.photonView.owner.GetTeam() == PhotonNetwork.player.GetTeam())
				{
					list.Add(ControllerManager.ControllerList[j]);
				}
			}
			else if (!controllerManager.photonView.owner.GetDead())
			{
				list.Add(ControllerManager.ControllerList[j]);
			}
		}
		index++;
		if (index > list.Count - 1)
		{
			index = 0;
		}
		if (list.Count != 0)
		{
			if (target != null)
			{
				target.playerSkin.PlayerAnimator.rootPos = Vector3.zero;
			}
			target = list[index];
			if (target == null)
			{
				UpdateSelectPlayer();
			}
			target.playerSkin.PlayerAnimator.rootPos = Vector3.back * 2f;
			cameraTransform.localPosition = target.playerSkin.PhotonPosition + headPos;
			cameraTransform.localRotation = target.playerSkin.PhotonRotation * Quaternion.Euler(target.playerSkin.Rotate * -60f, 0f, 0f);
			UpdateWeapon();
			cameraManager.OnSelectPlayer(target.photonView.ownerId);
		}
		else
		{
			CameraManager.SetType(CameraType.Static);
			cameraManager.OnSelectPlayer(-1);
		}
		list = null;
	}

	private void UpdateWeapon()
	{
		if (CameraManager.type != CameraType.FirstPerson)
		{
			return;
		}
		DeactiveWeapons();
		if (target.playerSkin.SelectWeapon != null)
		{
			TPWeaponShooter tPWeaponShooter = target.playerSkin.SelectWeapon;
			if (!weaponList.ContainsKey(tPWeaponShooter.Data.weapon))
			{
				WeaponData weaponData = WeaponManager.GetWeaponData(tPWeaponShooter.Data.weapon);
				GameObject fpsPrefab = weaponData.FpsPrefab;
				fpsPrefab = Utils.AddChild(fpsPrefab, cameraTransform);
				selectWeapon = fpsPrefab.GetComponent<FPWeaponShooter>();
				selectWeapon.FPWeapon.Spectator = true;
				weaponList.Add(weaponData.ID, selectWeapon);
				fpsPrefab.SetActive(true);
			}
			else
			{
				selectWeapon = weaponList[tPWeaponShooter.Data.weapon];
				selectWeapon.FPWeapon.Activate();
			}
			selectWeapon.UpdateWeaponData(tPWeaponShooter.Data.weapon, tPWeaponShooter.Data.skin, tPWeaponShooter.GetStickers(), tPWeaponShooter.FireStat.value);
			selectWeapon.UpdateHandAtlas(target.playerSkin.PlayerTeam, target.playerSkin.BodySkin);
			UICrosshair.SetAccuracy(WeaponManager.GetWeaponData(tPWeaponShooter.Data.weapon).Accuracy);
		}
	}

	private void SetWeaponEvent(int playerID, int weapon, int skin)
	{
		if (CameraManager.type == CameraType.FirstPerson && target != null && CameraManager.selectPlayer == playerID)
		{
			UpdateWeapon();
		}
	}

	private void SetDeadEvent(int playerID, bool dead)
	{
		if (CameraManager.type == CameraType.FirstPerson && target != null && CameraManager.selectPlayer == playerID)
		{
			if (dead)
			{
				DeactiveWeapons();
			}
			else
			{
				UpdateWeapon();
			}
		}
	}

	private void SetFireEvent(int playerID)
	{
		if (CameraManager.type == CameraType.FirstPerson && target != null && CameraManager.selectPlayer == playerID && selectWeapon != null)
		{
			selectWeapon.Fire();
		}
	}

	private void SetReloadEvent(int playerID)
	{
		if (CameraManager.type == CameraType.FirstPerson && target != null && CameraManager.selectPlayer == playerID && selectWeapon != null)
		{
			selectWeapon.Reload();
		}
	}

	private void CreateWeaponCamera()
	{
		if (!(weaponCamera != null))
		{
			GameObject gameObject = new GameObject("WeaponCamera");
			gameObject.transform.SetParent(cameraTransform);
			Camera camera = gameObject.AddComponent<Camera>();
			camera.transform.localPosition = Vector3.zero;
			camera.transform.localEulerAngles = Vector3.zero;
			camera.clearFlags = CameraClearFlags.Depth;
			camera.cullingMask = nValue.int1 << 31;
			camera.depth = nValue.int1;
			camera.farClipPlane = nValue.int100;
			camera.nearClipPlane = nValue.float001;
			camera.fieldOfView = nValue.int60;
			weaponCamera = camera;
			gameObject = null;
		}
	}

	public void DeactiveWeapons()
	{
		foreach (KeyValuePair<int, FPWeaponShooter> weapon in weaponList)
		{
			weapon.Value.Deactive();
		}
	}
}
