using System.Collections;
using UnityEngine;

public class ModeObject : MonoBehaviour
{
	public GameMode Mode;

	public GameObject[] Targets;

	private void Start()
	{
		if (PhotonNetwork.room.GetGameMode() == Mode)
		{
			StartCoroutine(LoadSync());
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator LoadSync()
	{
		for (int i = 0; i < Targets.Length; i++)
		{
			Targets[i].SetActive(true);
			yield return new WaitForSeconds(0.01f);
		}
		EventManager.Dispatch("ModeObject_Finish");
	}
}
