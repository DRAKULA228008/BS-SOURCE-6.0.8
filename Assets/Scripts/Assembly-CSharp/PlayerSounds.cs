using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
	public AudioSource Source;

	public AudioClip FireKnife;

	public AudioClip FirePistol;

	public AudioClip FirePistol2;

	public AudioClip FirePistol3;

	public AudioClip FirePistol4;

	public AudioClip FirePistol5;

	public AudioClip FireRifle;

	public AudioClip FireRifle2;

	public AudioClip FireShotgun;

	public AudioClip FireSniper;

	public AudioClip FireSubMachineGun;

	public AudioClip ReloadPistol;

	public AudioClip ReloadRifle;

	public AudioClip AmmoEmpty;

	private WeaponSound LastSound = WeaponSound.None;

	private bool Sound = true;

	private void Start()
	{
		UpdateSettings();
		EventManager.AddListener("OnSettings", UpdateSettings);
	}

	public void OnDefault()
	{
		LastSound = WeaponSound.None;
		Source.Stop();
	}

	private void UpdateSettings()
	{
		Sound = Settings.Sound;
	}

	private void OnDisable()
	{
		Stop();
	}

	public void Play(WeaponSound sound)
	{
		Play(sound, 1f);
	}

	public void Play(WeaponSound sound, float volume)
	{
		nProfiler.BeginSample("PlayerSounds.Play");
		if (!Sound)
		{
			return;
		}
		if (LastSound != sound)
		{
			switch (sound)
			{
			case WeaponSound.Knife:
				Source.clip = FireKnife;
				break;
			case WeaponSound.Pistol:
				Source.clip = FirePistol;
				break;
			case WeaponSound.Pistol2:
				Source.clip = FirePistol2;
				break;
			case WeaponSound.Pistol3:
				Source.clip = FirePistol3;
				break;
			case WeaponSound.Pistol4:
				Source.clip = FirePistol4;
				break;
			case WeaponSound.Pistol5:
				Source.clip = FirePistol5;
				break;
			case WeaponSound.Rifle:
				Source.clip = FireRifle;
				break;
			case WeaponSound.Rifle2:
				Source.clip = FireRifle2;
				break;
			case WeaponSound.Shotgun:
				Source.clip = FireShotgun;
				break;
			case WeaponSound.SubMachineGun:
				Source.clip = FireSubMachineGun;
				break;
			case WeaponSound.Sniper:
				Source.clip = FireSniper;
				break;
			case WeaponSound.ReloadPistol:
				Source.PlayOneShot(ReloadPistol, volume);
				break;
			case WeaponSound.ReloadRifle:
				Source.PlayOneShot(ReloadRifle, volume);
				break;
			case WeaponSound.AmmoEmpty:
				Source.clip = AmmoEmpty;
				break;
			}
		}
		LastSound = sound;
		Source.volume = volume;
		if (sound != WeaponSound.ReloadPistol && sound != WeaponSound.ReloadRifle && sound != WeaponSound.None)
		{
			Source.Play();
		}
		nProfiler.EndSample();
	}

	public void Stop()
	{
		try
		{
			LastSound = WeaponSound.None;
			Source.Stop();
		}
		catch
		{
		}
	}
}
