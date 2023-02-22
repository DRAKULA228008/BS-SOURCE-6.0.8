using UnityEngine;

public class GlassObject : MonoBehaviour
{
	public GameObject Glass;

	public GameObject BrokenGlass;

	public Transform[] Elements;

	private float[] Speed;

	public Vector2 MinMaxSpeed = new Vector2(30f, 40f);

	private float StartTime;

	private bool isActive;

	[ContextMenu("Active")]
	public void Active()
	{
		Glass.SetActive(false);
		BrokenGlass.SetActive(true);
		Speed = new float[Elements.Length];
		for (int i = 0; i < Elements.Length; i++)
		{
			Speed[i] = Random.Range(MinMaxSpeed.x, MinMaxSpeed.y);
		}
		StartTime = Time.time;
		isActive = true;
	}

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
		for (int i = 0; i < Elements.Length; i++)
		{
			Elements[i].localPosition -= new Vector3(0f, Speed[i] * Time.deltaTime, 0f);
		}
		if (StartTime + 1f < Time.time)
		{
			isActive = false;
			BrokenGlass.SetActive(false);
			for (int j = 0; j < Elements.Length; j++)
			{
				Elements[j].localPosition = Vector3.zero;
			}
		}
	}
}
