using System;
using DG.Tweening;
using UnityEngine;

public class Tram : MonoBehaviour, IPunObservable
{
	public GameObject model;

	public GameObject path;

	public Vector3[] pos;

	public byte[] controlIndex;

	public byte index;

	private byte lastIndex;

	public float speed;

	public float stepSpeed;

	public int players = 1;

	public bool back;

	public float distance;

	public float lerpSpeed = 10f;

	private Transform mTransform;

	private ControllerManager player;

	private Vector3 photonPosition;

	private Quaternion photonRotation;

	public static Action finishCallback;

	private float noPlayersTime;

	private static Tram instance;

	private void Awake()
	{
		if (PhotonNetwork.offlineMode || PhotonNetwork.room.GetGameMode() != GameMode.Escort)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		instance = this;
		GameManager.main.photonView.AddPunObservable(this);
		GameManager.main.photonView.synchronization = ViewSynchronization.UnreliableOnChange;
		mTransform = model.transform;
		photonPosition = mTransform.position;
		photonRotation = mTransform.rotation;
		model.SetActive(true);
		path.SetActive(true);
	}

	private void Start()
	{
		if (PhotonNetwork.room.GetGameMode() == GameMode.Escort)
		{
			TimerManager.In(0.5f, -1, 0.1f, CheckDistance);
		}
	}

	private void Update()
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (back)
			{
				if (mTransform.position == pos[index - 1])
				{
					if (lastIndex != index - 1)
					{
						index--;
						mTransform.position = Vector3.MoveTowards(mTransform.position, pos[index - 1], Time.deltaTime * (speed * (float)players));
						mTransform.DOLookAt(pos[index], 0.5f);
					}
				}
				else
				{
					mTransform.position = Vector3.MoveTowards(mTransform.position, pos[index - 1], Time.deltaTime * (speed * (float)players));
				}
			}
			else if (mTransform.position == pos[index])
			{
				if (pos.Length - 1 <= index)
				{
					if (finishCallback != null)
					{
						finishCallback();
						finishCallback = null;
					}
					return;
				}
				index++;
				for (int i = 0; i < controlIndex.Length; i++)
				{
					if (controlIndex[i] == index)
					{
						lastIndex = index;
						break;
					}
				}
				mTransform.DOLookAt(pos[index], 0.5f);
			}
			else
			{
				mTransform.position = Vector3.MoveTowards(mTransform.position, pos[index], Time.deltaTime * (speed * (float)players));
			}
		}
		else
		{
			mTransform.position = Vector3.Lerp(mTransform.position, photonPosition, Time.deltaTime * lerpSpeed);
			mTransform.rotation = Quaternion.Lerp(mTransform.rotation, photonRotation, Time.deltaTime * lerpSpeed);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream)
	{
		if (stream.isWriting)
		{
			stream.Write(mTransform.position);
			stream.Write(mTransform.rotation);
			stream.Write(index);
			stream.Write(lastIndex);
		}
		else
		{
			photonPosition = stream.ReadVector3();
			photonRotation = stream.ReadQuaternion();
			index = stream.ReadByte();
			lastIndex = stream.ReadByte();
		}
	}

	private void CheckDistance()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (GetPlayers() && GameManager.roundState == RoundState.PlayRound)
		{
			byte b = 0;
			byte b2 = 0;
			if (!GameManager.player.Dead && isZone(GameManager.player.PlayerTransform))
			{
				if (GameManager.team == Team.Blue)
				{
					b2 = (byte)(b2 + 1);
				}
				else if (GameManager.team == Team.Red)
				{
					b = (byte)(b + 1);
				}
			}
			else
			{
				speed = 0f;
			}
			for (int i = 0; i < ControllerManager.ControllerList.Count; i++)
			{
				player = ControllerManager.ControllerList[i];
				if (player != null && !player.playerSkin.Dead && isZone(player.playerSkin.PlayerTransform))
				{
					if (player.playerSkin.PlayerTeam == Team.Blue)
					{
						b2 = (byte)(b2 + 1);
					}
					else if (player.playerSkin.PlayerTeam == Team.Red)
					{
						b = (byte)(b + 1);
					}
				}
			}
			if (b != 0 && b2 != 0)
			{
				speed = 0f;
				noPlayersTime = 0f;
			}
			else if (b == 0 && b2 == 0)
			{
				if (noPlayersTime == 0f)
				{
					noPlayersTime = Time.time;
				}
				else if (noPlayersTime + 10f < Time.time)
				{
					back = true;
					speed = 0.2f;
				}
			}
			else if (b != 0)
			{
				speed = Mathf.Clamp((float)(int)b * stepSpeed, 0f, 1.2f);
				back = false;
				noPlayersTime = 0f;
			}
			else if (b2 != 0)
			{
				speed = 0.2f;
				back = true;
				noPlayersTime = 0f;
			}
		}
		else
		{
			speed = 0f;
		}
	}

	private bool isZone(Transform target)
	{
		if (target == null)
		{
			return false;
		}
		if (Mathf.Abs(Vector3.Distance(mTransform.position, target.position)) <= distance)
		{
			return true;
		}
		return false;
	}

	public static Transform GetModel()
	{
		return instance.mTransform;
	}

	private bool GetPlayers()
	{
		byte b = 0;
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			if (PhotonNetwork.playerList[i].GetTeam() != 0)
			{
				b = (byte)(b + 1);
				if (b > 1)
				{
					return true;
				}
			}
		}
		return false;
	}
}
