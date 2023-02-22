using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public CameraType cameraType;

	public Transform cameraTransform;

	public Transform cameraStaticPoint;

	public CameraDead dead;

	public CameraStatic statiс;

	public CameraSpectate spectate;

	public CameraFirstPerson firstPerson;

	private int playerID = -1;

	private static CameraManager instance;

	public static bool Team;

	public static bool ChangeType;

	public static int selectPlayer
	{
		get
		{
			return instance.playerID;
		}
		set
		{
			switch (type)
			{
			case CameraType.Spectate:
				instance.spectate.UpdateSelectPlayer(value);
				break;
			case CameraType.FirstPerson:
				instance.firstPerson.UpdateSelectPlayer(value);
				break;
			}
		}
	}

	public static CameraType type
	{
		get
		{
			return instance.cameraType;
		}
	}

	public static Transform ActiveCamera
	{
		get
		{
			if (type == CameraType.None)
			{
				if (PlayerInput.instance == null)
				{
					return null;
				}
				return PlayerInput.instance.FPCamera.Transform;
			}
			return instance.cameraTransform;
		}
	}

	public static CameraManager main
	{
		get
		{
			return instance;
		}
	}

	public static event Action<int> selectPlayerEvent;

	private void Awake()
	{
		instance = this;
	}

	private void OnDisable()
	{
		Team = false;
		ChangeType = false;
	}

	private void LateUpdate()
	{
		switch (cameraType)
		{
		case CameraType.Dead:
			dead.OnUpdate();
			break;
		case CameraType.Spectate:
			spectate.OnUpdate();
			break;
		case CameraType.FirstPerson:
			firstPerson.OnUpdate();
			break;
		case CameraType.Static:
			break;
		}
	}

	public void OnSelectPlayer(int id)
	{
		if (CameraManager.selectPlayerEvent != null)
		{
			playerID = id;
			CameraManager.selectPlayerEvent(playerID);
		}
	}

	public static void SetType(CameraType type, params object[] parameters)
	{
		instance.DeactiveAll();
		instance.cameraType = type;
		switch (type)
		{
		case CameraType.Dead:
			instance.dead.Active(parameters);
			break;
		case CameraType.Static:
			instance.statiс.Active();
			break;
		case CameraType.Spectate:
			instance.spectate.Active(parameters);
			break;
		case CameraType.FirstPerson:
			instance.firstPerson.Active(parameters);
			break;
		}
	}

	private void DeactiveAll()
	{
		dead.Deactive();
		statiс.Deactive();
		spectate.Deactive();
		firstPerson.Deactive();
		cameraType = CameraType.None;
	}
}
