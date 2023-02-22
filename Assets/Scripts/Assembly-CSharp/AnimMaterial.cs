using UnityEngine;

public class AnimMaterial : MonoBehaviour
{
	public Material material;

	public Vector2 speed;

	private bool visible;

	private void Reset()
	{
		if (material != null)
		{
			material.mainTextureOffset = Vector2.zero;
		}
	}

	private void OnDisable()
	{
		Reset();
	}

	private void OnBecameVisible()
	{
		visible = true;
	}

	private void OnBecameInvisible()
	{
		visible = false;
	}

	private void Update()
	{
		if (visible)
		{
			material.mainTextureOffset = speed * Time.time;
		}
	}
}
