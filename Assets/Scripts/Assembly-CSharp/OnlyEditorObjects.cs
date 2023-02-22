using UnityEngine;

public class OnlyEditorObjects : MonoBehaviour
{
	public GameObject[] objs;

	private void Start()
	{
		for (int i = 0; i < objs.Length; i++)
		{
			objs[i].SetActive(false);
		}
	}
}
