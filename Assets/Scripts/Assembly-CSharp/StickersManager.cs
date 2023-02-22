using UnityEngine;

public class StickersManager : MonoBehaviour
{
	public GameObject StickersParent;

	public MeshAtlas[] Stickers;

	private void OnEnable()
	{
		if (Random.Range(0, 100) == 56)
		{
			StickersParent.SetActive(false);
			for (int i = 0; i < Stickers.Length; i++)
			{
				Stickers[i].spriteName = Random.Range(0, 10).ToString();
			}
		}
	}

	private void OnDisable()
	{
		StickersParent.SetActive(false);
	}
}
