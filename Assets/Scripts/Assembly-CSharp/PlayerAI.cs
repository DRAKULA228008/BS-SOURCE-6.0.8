using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class PlayerAI : Photon.MonoBehaviour, IPunObservable
{
	public static List<PlayerAI> list = new List<PlayerAI>();

	public Transform target;

	public int health = 1000;

	public bool offlineMode;

	public bool dead;

	public int attackDamage = 20;

	public float attackDistance = 5f;

	public float attackWalkDistance = 1.5f;

	public float attackIdleDistance = 3f;

	public float attackSpeed = 0.5f;

	public Vector3 position;

	public float checkPlayers = 1.5f;

	public UnityEngine.AI.NavMeshAgent nav;

	public PlayerAnimator playerAnimator;

	public GameObject attackEffect;

	public float photonSpeed = 11f;

	private Vector3 photonPosition;

	private Quaternion photonRotation;

	public Transform head;

	public float rotateAngle = 5f;

	public nTimer Timer;

	private Vector3 customVector3;

	private bool update1;

	private Transform cachedTransform;

	private bool isJump;

	public static event Action<PlayerAI> startEvent;

	public static event Action<PlayerAI> deadEvent;

	private void Start()
	{
		cachedTransform = base.transform;
		playerAnimator.grounded = true;
		playerAnimator.SetWeapon(WeaponType.Knife);
		nav.enabled = base.photonView.isMine || offlineMode;
		Timer.In(attackSpeed + UnityEngine.Random.Range(-0.25f, 0.25f), true, CheckDistance);
		Timer.In(0.1f, true, UpdateNavigator);
		Timer.In(checkPlayers, true, CheckPlayers);
	}

	private void OnEnable()
	{
		list.Add(this);
		if (PlayerAI.startEvent != null)
		{
			PlayerAI.startEvent(this);
		}
		PhotonNetwork.onMasterClientSwitched = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onMasterClientSwitched, new PhotonNetwork.PhotonPlayerDelegate(OnMasterClientSwitched));
		PhotonNetwork.onOwnershipTransfered = (PhotonNetwork.ObjectsDelegate)Delegate.Combine(PhotonNetwork.onOwnershipTransfered, new PhotonNetwork.ObjectsDelegate(OnOwnershipTransfered));
	}

	private void OnDisable()
	{
		list.Remove(this);
		PhotonNetwork.onMasterClientSwitched = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onMasterClientSwitched, new PhotonNetwork.PhotonPlayerDelegate(OnMasterClientSwitched));
		PhotonNetwork.onOwnershipTransfered = (PhotonNetwork.ObjectsDelegate)Delegate.Remove(PhotonNetwork.onOwnershipTransfered, new PhotonNetwork.ObjectsDelegate(OnOwnershipTransfered));
	}

	private void UpdateNavigator()
	{
		if (!base.photonView.isMine && !offlineMode)
		{
			return;
		}
		if (target == null)
		{
			if (PlayerInput.instance != null)
			{
				target = PlayerInput.instance.PlayerTransform;
			}
			return;
		}
		if (nav.isOnOffMeshLink)
		{
			nav.SetDestination(target.position);
		}
		playerAnimator.move = Mathf.Clamp01(nav.velocity.sqrMagnitude);
		nav.updateRotation = (target.position - cachedTransform.position).sqrMagnitude >= 10f;
	}

	private void Update()
	{
		if (base.photonView.isMine || offlineMode)
		{
			if (target != null)
			{
				if (!nav.updateRotation)
				{
					customVector3 = target.position - cachedTransform.position;
					customVector3.y = 0f;
					cachedTransform.rotation = Quaternion.LookRotation(customVector3);
				}
				customVector3 = target.position - cachedTransform.position;
				playerAnimator.rotate = customVector3.y / customVector3.sqrMagnitude * rotateAngle;
				if (nav.isOnOffMeshLink && !isJump)
				{
					playerAnimator.grounded = false;
					isJump = true;
				}
				else if (!nav.isOnOffMeshLink && isJump)
				{
					playerAnimator.grounded = true;
					isJump = false;
				}
			}
		}
		else
		{
			cachedTransform.position = Vector3.Lerp(cachedTransform.position, photonPosition, Time.deltaTime * photonSpeed);
			cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, photonRotation, Time.deltaTime * photonSpeed);
		}
	}

	private void CheckDistance()
	{
		if (!base.photonView.isMine || target == null)
		{
			return;
		}
		if (nav.velocity.sqrMagnitude <= 0.1f)
		{
			if (Vector3.Distance(target.position, cachedTransform.position) <= attackIdleDistance)
			{
				base.photonView.RPC("PhotonFire", PhotonTargets.All);
			}
		}
		else if (Vector3.Distance(target.position, cachedTransform.position) <= attackWalkDistance)
		{
			base.photonView.RPC("PhotonFire", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void PhotonFire()
	{
		attackEffect.SetActive(true);
		TimerManager.In(0.05f, delegate
		{
			attackEffect.SetActive(false);
		});
		RaycastHit hitInfo;
		if ((base.photonView.isMine || offlineMode) && Physics.Raycast(head.position + position, head.forward, out hitInfo, attackDistance) && hitInfo.transform == PlayerInput.instance.PlayerTransform)
		{
			DamageInfo damageInfo = DamageInfo.Get(attackDamage + UnityEngine.Random.Range(-5, 5), cachedTransform.position, Team.None, 0, 0, -1, false);
			PlayerInput.instance.Damage(damageInfo);
		}
	}

	public void Damage(DamageInfo damageInfo)
	{
		if (!dead)
		{
			UICrosshair.Hit();
		}
	}

	[PunRPC]
	private void PhotonDamage(DamageInfo damageInfo)
	{
		if (base.photonView.isMine || offlineMode)
		{
			health -= damageInfo.damage;
			if (health <= 0)
			{
				dead = true;
			}
		}
	}

	[PunRPC]
	private void PhotonDead()
	{
		health = 0;
		dead = true;
		PoolManager.Despawn("Zombie", base.gameObject);
		if (PlayerAI.deadEvent != null)
		{
			PlayerAI.deadEvent(this);
		}
	}

	private void CheckPlayers()
	{
		if (offlineMode || !base.photonView.isMine)
		{
			return;
		}
		ControllerManager controllerManager = null;
		float num = Vector3.Distance(target.position, cachedTransform.position) + 1f;
		float num2 = num;
		for (int i = 0; i < ControllerManager.ControllerList.Count; i++)
		{
			if (ControllerManager.ControllerList[i] != null && ControllerManager.ControllerList[i].playerSkin != null && !ControllerManager.ControllerList[i].playerSkin.Dead)
			{
				num2 = Vector3.Distance(ControllerManager.ControllerList[i].playerSkin.PlayerTransform.position, cachedTransform.position);
				if (num2 + 4f < num)
				{
					num = num2;
					controllerManager = ControllerManager.ControllerList[i];
				}
			}
		}
		if (!(controllerManager == null))
		{
			base.photonView.TransferOwnership(controllerManager.photonView.owner);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream)
	{
		if (stream.isWriting)
		{
			photonPosition = cachedTransform.position;
			photonRotation = cachedTransform.rotation;
			stream.Write(cachedTransform.position);
			stream.Write(cachedTransform.rotation);
			stream.Write(nav.velocity.sqrMagnitude);
			stream.Write(health);
		}
		else
		{
			photonPosition = stream.ReadVector3();
			photonRotation = stream.ReadQuaternion();
			playerAnimator.move = stream.ReadFloat();
			health = stream.ReadInt();
		}
	}

	private void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		nav.enabled = newMasterClient.ID == PhotonNetwork.player.ID;
	}

	private void OnOwnershipTransfered(object[] viewAndPlayers)
	{
		nav.enabled = ((PhotonPlayer)viewAndPlayers[1]).ID == PhotonNetwork.player.ID;
	}
}
