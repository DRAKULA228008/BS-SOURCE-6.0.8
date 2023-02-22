using UnityEngine;

public class ZombieSounds : MonoBehaviour
{
	public AudioClip[] Clips;

	public AudioSource Source;

	public Vector2 size = new Vector2(5f, 10f);

	private int id;

	private void OnEnable()
	{
		PlaySound();
	}

	private void OnDisable()
	{
		TimerManager.Cancel(id);
		Source.Stop();
	}

	private void PlaySound()
	{
		id = TimerManager.In(Random.Range(size.x, size.y), -nValue.int1, Random.Range(size.x, size.y), delegate
		{
			if (!Source.isPlaying && GameManager.roundState != RoundState.EndRound)
			{
				AudioClip clip = Clips[Random.Range(nValue.int0, Clips.Length)];
				Source.clip = clip;
				Source.Play();
			}
		});
	}
}
