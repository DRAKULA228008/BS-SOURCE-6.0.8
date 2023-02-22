using UnityEngine;

public class BunnyAutoJump : MonoBehaviour
{
	private PlayerInput player;

	public CryptoInt jumpTime = 2;

	private int TimerID;

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		player = other.GetComponent<PlayerInput>();
		if (player != null)
		{
			player.SetBunnyHopAutoJump(true);
			TimerID = TimerManager.In((int)jumpTime, delegate
			{
				BunnyHop.SpawnDead();
			});
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") && player != null)
		{
			player.SetBunnyHopAutoJump(false);
			player = null;
			TimerManager.Cancel(TimerID);
		}
	}
}
