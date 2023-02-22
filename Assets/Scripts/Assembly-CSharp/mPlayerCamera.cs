using DG.Tweening;
using UnityEngine;

public class mPlayerCamera : MonoBehaviour
{
	public GameObject Player;

	public MeshAtlas Head;

	public MeshAtlas[] Body;

	public MeshAtlas[] Legs;

	public Transform Point;

	public float RotateSpeed = 200f;

	private Camera mCamera;

	private static mPlayerCamera instance;

	private void Awake()
	{
		instance = this;
		mCamera = GetComponent<Camera>();
		RotateSpeed = Mathf.Sqrt(RotateSpeed) / Mathf.Sqrt(Screen.dpi);
	}

	public static void Show()
	{
		instance.mCamera.enabled = true;
		instance.Player.SetActive(true);
	}

	public static void Close()
	{
		instance.mCamera.enabled = false;
		instance.Player.SetActive(false);
	}

	public static void Rotate(Vector2 rotate)
	{
		instance.Point.Rotate(new Vector2(0f, (0f - rotate.x) * instance.RotateSpeed));
	}

	public static void SetSkin(Team team, string head, string body, string leg)
	{
		UIAtlas atlas = ((team != Team.Blue) ? GameSettings.instance.PlayerAtlasRed : GameSettings.instance.PlayerAtlasBlue);
		instance.Head.atlas = atlas;
		instance.Head.spriteName = "0-" + head;
		for (int i = 0; i < instance.Body.Length; i++)
		{
			instance.Body[i].atlas = atlas;
			instance.Body[i].spriteName = "1-" + body;
		}
		for (int j = 0; j < instance.Legs.Length; j++)
		{
			instance.Legs[j].atlas = atlas;
			instance.Legs[j].spriteName = "2-" + leg;
		}
	}

	public static void ResetRotateX()
	{
		instance.Point.DOLocalRotate(Vector3.zero, 0.2f);
	}
}
