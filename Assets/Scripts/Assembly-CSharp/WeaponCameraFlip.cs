using UnityEngine;

public class WeaponCameraFlip : MonoBehaviour
{
	private bool flip;

	private Camera cam;

	private float fov;

	private static WeaponCameraFlip instance;

	private void Start()
	{
		instance = this;
		cam = base.GetComponent<Camera>();
		fov = 60f;
		flip = GameConsole.Load("weapon_left_hand", false);
		if (flip)
		{
			cam.projectionMatrix *= Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
		}
	}

	private void Update()
	{
		if (cam.fieldOfView != fov && flip)
		{
			fov = cam.fieldOfView;
			cam.ResetProjectionMatrix();
			GL.invertCulling = false;
			cam.projectionMatrix *= Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
		}
	}

	[ContextMenu("Flip")]
	public static void OnFlip(bool value)
	{
		if (!(instance == null) && instance.gameObject.activeSelf && instance.flip != value)
		{
			instance.flip = value;
			if (!instance.flip)
			{
				instance.cam.ResetProjectionMatrix();
				GL.invertCulling = false;
			}
			else
			{
				instance.cam.projectionMatrix = instance.cam.projectionMatrix * Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
			}
		}
	}

	private void OnPreRender()
	{
		if (flip)
		{
			GL.invertCulling = true;
		}
	}

	private void OnPostRender()
	{
		if (flip)
		{
			GL.invertCulling = false;
		}
	}
}
