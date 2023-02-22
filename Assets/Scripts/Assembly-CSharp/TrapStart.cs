using UnityEngine;

public class TrapStart : MonoBehaviour
{
	public GameObject Target;

	private int Timer;

	private void Start()
	{
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", WaitPlayer);
	}

	private void StartRound()
	{
		Target.SetActive(true);
		TimerManager.Cancel(Timer);
		Timer = TimerManager.In(5f, delegate
		{
			Target.SetActive(false);
		});
	}

	private void WaitPlayer()
	{
		TimerManager.Cancel(Timer);
		Target.SetActive(false);
	}
}
