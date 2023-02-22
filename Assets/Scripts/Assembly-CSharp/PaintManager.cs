using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintManager : MonoBehaviour
{
	public GameObject Block;

	public int SelectColor;

	public bool BlockDelete;

	public Color32[] Colors;

	public List<PaintObject> BlockList = new List<PaintObject>();

	private int LastID;

	private static PaintManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		EventManager.AddListener<RaycastHit>("Paint", OnPaint);
	}

	[ContextMenu("Get Data")]
	private void GetData()
	{
		byte[] array = new byte[BlockList.Count * 4];
		int num = 0;
		for (int i = 0; i < BlockList.Count; i++)
		{
			byte[] data = BlockList[i].GetData();
			array[num] = data[0];
			array[num + 1] = data[1];
			array[num + 2] = data[2];
			array[num + 3] = data[3];
			num += 4;
		}
	}

	private void OnPaint(RaycastHit hit)
	{
		if (hit.transform.name == "Color")
		{
			return;
		}
		if (BlockDelete)
		{
			if (!(hit.transform.name != "Plane"))
			{
				return;
			}
			int iD = hit.transform.parent.GetComponent<PaintObject>().ID;
			for (int i = 0; i < BlockList.Count; i++)
			{
				if (BlockList[i].ID == iD)
				{
					BlockList[i].Delete();
					BlockList.RemoveAt(i);
					break;
				}
			}
		}
		else if (hit.distance > 1.5f)
		{
			if (hit.transform.name == "Plane")
			{
				Vector3 zero = Vector3.zero;
				zero.x = Mathf.Round(hit.point.x);
				zero.y = 0f;
				zero.z = Mathf.Round(hit.point.z);
				Paint(zero);
			}
			else
			{
				Paint(hit.transform.parent.position + hit.normal);
			}
		}
	}

	public static void Paint(Vector3 pos)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(instance.Block, pos, Quaternion.identity);
		gameObject.name = instance.LastID.ToString();
		gameObject.transform.SetParent(instance.transform);
		PaintObject component = gameObject.GetComponent<PaintObject>();
		component.Init(instance.LastID, instance.SelectColor);
		instance.BlockList.Add(component);
		instance.LastID++;
	}

	public static void Paint(int x, int y, int z, int color)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(instance.Block, Vector3.zero, Quaternion.identity);
		gameObject.name = instance.LastID.ToString();
		gameObject.transform.SetParent(instance.transform);
		gameObject.transform.localPosition = new Vector3(x, y, z);
		PaintObject component = gameObject.GetComponent<PaintObject>();
		component.Init(instance.LastID, color);
		instance.BlockList.Add(component);
		instance.LastID++;
	}

	public static void SetColor(int color)
	{
		instance.SelectColor = color;
	}

	public static Color32 GetColor(int id)
	{
		return instance.Colors[id];
	}

	public static void Clear()
	{
		for (int i = 0; i < instance.BlockList.Count; i++)
		{
			instance.BlockList[i].Delete(false);
		}
		instance.BlockList.Clear();
	}

	private IEnumerator LoadData(byte[] bytes)
	{
		Clear();
		PlayerInput.instance.SetMove(false);
		yield return new WaitForSeconds(0.2f);
		int length = bytes.Length / 4;
		int count = 0;
		for (int i = 0; i < length; i++)
		{
			Paint(bytes[count], bytes[count + 1], bytes[count + 2], bytes[count + 3]);
			count += 4;
			yield return new WaitForEndOfFrame();
		}
		PlayerInput.instance.SetMove(true);
	}
}
