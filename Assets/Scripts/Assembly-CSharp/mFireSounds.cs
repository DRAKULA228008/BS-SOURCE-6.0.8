using UnityEngine;

public class mFireSounds : MonoBehaviour
{
	public PlayerSounds sounds;

	public float volume = 1f;

	private void Start()
	{
		TimerManager.In(1f, Fire);
	}

	private void Fire()
	{
		int num = Random.Range(0, 8);
		float delay = 0f;
		switch (num)
		{
		case 0:
			delay = Random.Range(0.095f, 0.1f);
			TimerManager.In(delay, Random.Range(3, 8), delay, delegate
			{
				sounds.Play(WeaponSound.Rifle, volume);
			});
			break;
		case 1:
			delay = Random.Range(0.85f, 0.095f);
			TimerManager.In(delay, Random.Range(3, 8), delay, delegate
			{
				sounds.Play(WeaponSound.Rifle2, volume);
			});
			break;
		case 2:
			delay = Random.Range(0.1f, 0.15f);
			TimerManager.In(delay, Random.Range(1, 5), delay, delegate
			{
				sounds.Play(WeaponSound.Pistol, volume);
			});
			break;
		case 3:
			delay = Random.Range(0.1f, 0.15f);
			TimerManager.In(delay, Random.Range(1, 5), delay, delegate
			{
				sounds.Play(WeaponSound.Pistol2, volume);
			});
			break;
		case 4:
			delay = Random.Range(0.1f, 0.15f);
			TimerManager.In(delay, Random.Range(1, 5), delay, delegate
			{
				sounds.Play(WeaponSound.Pistol3, volume);
			});
			break;
		case 5:
			delay = Random.Range(0.1f, 0.15f);
			TimerManager.In(delay, Random.Range(1, 5), delay, delegate
			{
				sounds.Play(WeaponSound.Pistol4, volume);
			});
			break;
		case 6:
			delay = Random.Range(0.1f, 0.15f);
			TimerManager.In(delay, Random.Range(1, 5), delay, delegate
			{
				sounds.Play(WeaponSound.Pistol5, volume);
			});
			break;
		case 7:
			delay = Random.Range(0.2f, 0.35f);
			TimerManager.In(delay, Random.Range(1, 3), delay, delegate
			{
				sounds.Play(WeaponSound.Shotgun, volume);
			});
			break;
		case 8:
			TimerManager.In(delay, delegate
			{
				sounds.Play(WeaponSound.Sniper, volume);
			});
			break;
		}
		if (Random.value > 0.75f)
		{
			TimerManager.In(Random.Range(0.7f, 1f), Fire);
		}
		else
		{
			TimerManager.In(Random.Range(1f, 2.5f), Fire);
		}
	}
}
