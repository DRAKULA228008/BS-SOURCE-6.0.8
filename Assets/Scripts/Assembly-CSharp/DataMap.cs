using System;
using UnityEngine;

public class DataMap : MonoBehaviour
{
	[Serializable]
	public class ObjData
	{
		public byte id;

		public byte[] activeArea;

		public Vector3 pos;

		public byte rot;

		public bool active;

		public byte weaponZone;

		public byte weaponZoneType;

		public byte weaponID;

		public bool weaponDrop;

		public GameObject obj;

		public GameObject weaponBox;
	}

	public ObjData[] dataList;

	public GameObject[] objList;

	public Material[] objMaterials;

	public byte[] objWeaponID;

	public Transform objParent;

	public GameObject weaponBox;

	public Rect[] zones;

	public float procent = 0.3f;

	public int x;

	public int y;

	public CullArea area;

	public Transform[] spawns;

	public byte selectWeaponZone = 1;

	public byte randomWeapon = 1;

	[SelectedWeapon]
	public int[] easyWeaponZone;

	[SelectedWeapon]
	public int[] mediumWeaponZone;

	[SelectedWeapon]
	public int[] hardWeaponZone;

	private byte[] playerArea;

	private byte[] lastPlayerArea = new byte[0];

	private int pGroup;

	private PhotonView pView;

	private void Start()
	{
		selectWeaponZone = (byte)UnityEngine.Random.Range(1, 4);
		randomWeapon = (byte)UnityEngine.Random.Range(0, 7);
		InvokeRepeating("Check", 2f, 1f);
	}

	private void Check()
	{
		if (PhotonNetwork.player.GetDead())
		{
			return;
		}
		if (pView == null)
		{
			pView = GameManager.controller.photonView;
			InvokeRepeating("UpdateActiveGroup", 0f, 1f / (float)PhotonNetwork.sendRateOnSerialize);
			GameManager.player.FPCamera.GetComponent<Camera>().farClipPlane = 120f;
			GameObject gameObject = new GameObject("Camera");
			gameObject.transform.SetParent(GameManager.player.FPCamera.Transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = Vector3.zero;
			Camera camera = gameObject.AddComponent<Camera>();
			camera.cullingMask = 131072;
			camera.depth = -0.2f;
			camera.clearFlags = CameraClearFlags.Depth;
		}
		playerArea = area.GetActiveCells(PlayerInput.instance.PlayerTransform.position).ToArray();
		if (object.Equals(playerArea, lastPlayerArea))
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < dataList.Length; i++)
		{
			if (Contains(dataList[i].activeArea, playerArea))
			{
				if (dataList[i].active)
				{
					continue;
				}
				switch (dataList[i].rot)
				{
				case 0:
					dataList[i].obj = PoolManager.Spawn(dataList[i].id.ToString(), objList[dataList[i].id], dataList[i].pos, Vector3.zero, objParent);
					break;
				case 1:
					dataList[i].obj = PoolManager.Spawn(dataList[i].id.ToString(), objList[dataList[i].id], dataList[i].pos, new Vector3(0f, 90f, 0f), objParent);
					break;
				case 2:
					dataList[i].obj = PoolManager.Spawn(dataList[i].id.ToString(), objList[dataList[i].id], dataList[i].pos, new Vector3(0f, 180f, 0f), objParent);
					break;
				case 3:
					dataList[i].obj = PoolManager.Spawn(dataList[i].id.ToString(), objList[dataList[i].id], dataList[i].pos, new Vector3(0f, 270f, 0f), objParent);
					break;
				}
				dataList[i].active = true;
				if (!dataList[i].weaponDrop && dataList[i].weaponZone == selectWeaponZone)
				{
					dataList[i].weaponBox = PoolManager.Spawn("weaponBox", weaponBox, dataList[i].pos + new Vector3(0f, 0.5f, 0f), Vector3.zero, objParent);
					DropWeapon component = dataList[i].weaponBox.GetComponent<DropWeapon>();
					switch (dataList[i].weaponZoneType)
					{
					case 3:
						num = randomWeapon + dataList[i].weaponID - easyWeaponZone.Length;
						if (num < 0)
						{
							num = Mathf.Abs(num);
						}
						component.weaponID = easyWeaponZone[num];
						break;
					case 2:
						num = randomWeapon + dataList[i].weaponID - mediumWeaponZone.Length;
						if (num < 0)
						{
							num = Mathf.Abs(num);
						}
						component.weaponID = mediumWeaponZone[num];
						break;
					case 1:
						num = randomWeapon + dataList[i].weaponID - hardWeaponZone.Length;
						if (num < 0)
						{
							num = Mathf.Abs(num);
						}
						component.weaponID = hardWeaponZone[num];
						break;
					}
				}
				Vector3 position = dataList[i].obj.transform.position;
				position.y = position.z;
				for (int j = 0; j < zones.Length; j++)
				{
					if (zones[j].Contains(position))
					{
						dataList[i].obj.GetComponent<Renderer>().material = objMaterials[j];
						break;
					}
				}
			}
			else if (dataList[i].active)
			{
				PoolManager.Despawn(dataList[i].id.ToString(), dataList[i].obj);
				dataList[i].obj = null;
				dataList[i].active = false;
				if (dataList[i].weaponBox != null)
				{
					PoolManager.Despawn("weaponBox", dataList[i].weaponBox);
					dataList[i].weaponBox = null;
				}
			}
		}
		lastPlayerArea = playerArea;
		PhotonNetwork.SetInterestGroups(lastPlayerArea, playerArea);
	}

	public void DropWeapon(DropWeapon weapon)
	{
	}

	private bool Contains(byte[] a, byte b)
	{
		return Contains(a, new byte[1] { b });
	}

	private bool Contains(byte[] a, byte[] b)
	{
		for (int i = 0; i < a.Length; i++)
		{
			for (int j = 0; j < b.Length; j++)
			{
				if (a[i] == b[j])
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool Equals(int[] a, int[] b)
	{
		if (a.Length != b.Length)
		{
			return false;
		}
		Array.Sort(a);
		Array.Sort(b);
		for (int i = 0; i < a.Length; i++)
		{
			if (a[i] != b[i])
			{
				return false;
			}
		}
		return true;
	}

	private void UpdateActiveGroup()
	{
		pGroup = ++pGroup % area.SUBDIVISION_SECOND_LEVEL_ORDER.Length;
		pView.group = playerArea[area.SUBDIVISION_SECOND_LEVEL_ORDER[pGroup]];
	}
}
