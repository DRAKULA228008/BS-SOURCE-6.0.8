using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	[Serializable]
	public class SoundGameMode
	{
		public GameMode mode;

		public SoundData[] sounds;
	}

	[Serializable]
	public class SoundData
	{
		public string name;

		public AudioClip[] clips;

		public AudioClip clip
		{
			get
			{
				return clips[UnityEngine.Random.Range(0, clips.Length)];
			}
		}
	}

	public SoundGameMode[] soundsMode;

	private SoundGameMode selectSoundsMode;

	private List<SoundClip> activeList = new List<SoundClip>();

	private List<SoundClip> poolList = new List<SoundClip>();

	private AudioSource cachedAudioSource;

	private static SoundManager instance;

	private void Awake()
	{
		instance = this;
		cachedAudioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		GameMode gameMode = PhotonNetwork.room.GetGameMode();
		for (int i = 0; i < soundsMode.Length; i++)
		{
			if (soundsMode[i].mode != gameMode)
			{
				for (int j = 0; j < soundsMode[i].sounds.Length; j++)
				{
					for (int k = 0; k < soundsMode[i].sounds[j].clips.Length; k++)
					{
						Resources.UnloadAsset(soundsMode[i].sounds[j].clips[k]);
						soundsMode[i].sounds[j].clips[k] = null;
					}
				}
			}
			else
			{
				selectSoundsMode = soundsMode[i];
			}
		}
	}

	public static void Play2D(string name)
	{
		if (instance.selectSoundsMode == null)
		{
			Debug.LogWarning("Select Sounds == null");
			return;
		}
		for (int i = 0; i < instance.selectSoundsMode.sounds.Length; i++)
		{
			if (instance.selectSoundsMode.sounds[i].name == name)
			{
				instance.cachedAudioSource.clip = instance.selectSoundsMode.sounds[i].clip;
				instance.cachedAudioSource.Play();
				break;
			}
		}
	}

	public static SoundClip Play3D(string name, Vector3 pos)
	{
		if (instance.selectSoundsMode == null)
		{
			Debug.LogWarning("Select Sounds == null");
			return null;
		}
		for (int i = 0; i < instance.selectSoundsMode.sounds.Length; i++)
		{
			if (instance.selectSoundsMode.sounds[i].name == name)
			{
				SoundClip soundClip = GetSoundClip();
				instance.activeList.Add(soundClip);
				soundClip.Play(instance.selectSoundsMode.sounds[i].clip, pos);
				return soundClip;
			}
		}
		return null;
	}

	private static SoundClip GetSoundClip()
	{
		if (instance.poolList.Count != 0)
		{
			SoundClip result = instance.poolList[0];
			instance.poolList.RemoveAt(0);
			return result;
		}
		return SoundClip.Create();
	}

	public static void AddSoundClipPool(SoundClip clip)
	{
		instance.activeList.Remove(clip);
		instance.poolList.Add(clip);
	}
}
