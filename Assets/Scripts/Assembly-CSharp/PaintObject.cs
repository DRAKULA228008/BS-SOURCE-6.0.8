using UnityEngine;

public class PaintObject : MonoBehaviour
{
	public int ID;

	public int ColorID;

	public Transform[] faces;

	public Renderer[] facesRenderer;

	public MeshFilter[] facesMesh;

	private Transform mTransform;

	private Transform CachedTransform
	{
		get
		{
			if (mTransform == null)
			{
				mTransform = base.transform;
			}
			return mTransform;
		}
	}

	public void Init(int id, int colorID)
	{
		ID = id;
		ColorID = colorID;
		for (int i = 0; i < faces.Length; i++)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(CachedTransform.position, -faces[i].forward, out hitInfo, 1.1f))
			{
				if (hitInfo.transform.CompareTag("PaintObject"))
				{
					if (hitInfo.transform.name != "Plane")
					{
						hitInfo.transform.GetComponent<Renderer>().enabled = false;
					}
					facesRenderer[i].GetComponent<Renderer>().enabled = false;
				}
				else
				{
					facesRenderer[i].GetComponent<Renderer>().enabled = true;
				}
			}
			else
			{
				facesRenderer[i].GetComponent<Renderer>().enabled = true;
			}
			Color32 color = PaintManager.GetColor(colorID);
			Color32[] colors = new Color32[4] { color, color, color, color };
			facesMesh[i].mesh.colors32 = colors;
		}
	}

	public void Delete()
	{
		Delete(true);
	}

	public void Delete(bool check)
	{
		if (check)
		{
			for (int i = 0; i < faces.Length; i++)
			{
				RaycastHit hitInfo;
				if (Physics.Raycast(CachedTransform.position, -faces[i].forward, out hitInfo, 1.1f) && hitInfo.transform.CompareTag("PaintObject") && hitInfo.transform.name != "Plane")
				{
					hitInfo.transform.GetComponent<Renderer>().enabled = true;
				}
			}
		}
		Object.Destroy(base.gameObject);
	}

	public byte[] GetData()
	{
		return new byte[4]
		{
			(byte)(int)mTransform.localPosition.x,
			(byte)(int)mTransform.localPosition.y,
			(byte)(int)mTransform.localPosition.z,
			(byte)ColorID
		};
	}
}
