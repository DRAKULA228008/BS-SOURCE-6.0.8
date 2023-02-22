using UnityEngine;

public class SoundClip : MonoBehaviour
{
	private GameObject cachedGameObject;

	private Transform cachedTransform;

	private AudioSource cachedAudioSource;

	private void Awake()
	{
		cachedGameObject = base.gameObject;
		cachedTransform = base.transform;
		cachedAudioSource = GetComponent<AudioSource>();
	}

	public void Play(AudioClip clip, Vector3 pos)
	{
		cachedGameObject.SetActive(true);
		cachedAudioSource.clip = clip;
		cachedTransform.position = pos;
		cachedAudioSource.Play();
		TimerManager.In(clip.length, Stop);
	}

	public void Stop()
	{
		if (cachedGameObject.activeSelf)
		{
			cachedGameObject.SetActive(false);
			SoundManager.AddSoundClipPool(this);
		}
	}

	public static SoundClip Create()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "SoundClip";
		gameObject.AddComponent<AudioSource>();
		return gameObject.AddComponent<SoundClip>();
	}

	public AudioSource GetSource()
	{
		return cachedAudioSource;
	}
}
