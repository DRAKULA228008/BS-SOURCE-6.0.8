using DG.Tweening;
using UnityEngine;

public class BombAudio : MonoBehaviour
{
	public AudioSource BombAudioSource;

	public AudioClip BombAudioClip;

	public Light BombLight;

	private int BombAudioID;

	private float BombTime;

	private int BombCount;

	private void OnEnable()
	{
		if (BombManager.BombPlaced)
		{
			BombLight.intensity = nValue.int0;
			BombLight.DOIntensity(nValue.int8, nValue.float07).SetLoops(-nValue.int1, LoopType.Yoyo);
		}
	}

	private void OnDisable()
	{
		BombLight.DOKill();
	}

	public void Play(float time)
	{
		BombTime = time;
	}

	public void Boom()
	{
		Stop();
	}

	public void Stop()
	{
		BombCount = nValue.int0;
		BombTime = -nValue.int1;
		TimerManager.Cancel(BombAudioID);
		BombAudioID = nValue.int0;
	}

	private void Update()
	{
		if (BombTime > nValue.float08)
		{
			if (BombTime > (float)nValue.int20 && BombTime < (float)nValue.int35 && BombCount != nValue.int1)
			{
				BombCount = nValue.int1;
				TimerManager.Cancel(BombAudioID);
				BombAudioID = TimerManager.In(nValue.int0, -nValue.int1, nValue.int1, delegate
				{
					BombAudioSource.PlayOneShot(BombAudioClip);
				});
			}
			else if (BombTime > (float)nValue.int10 && BombTime < (float)nValue.int20 && BombCount != nValue.int2)
			{
				BombCount = nValue.int2;
				TimerManager.Cancel(BombAudioID);
				BombAudioID = TimerManager.In(nValue.int0, -nValue.int1, nValue.float05, delegate
				{
					BombAudioSource.PlayOneShot(BombAudioClip);
				});
			}
			else if (BombTime > (float)nValue.int0 && BombTime < (float)nValue.int10 && BombCount != nValue.int3)
			{
				BombCount = nValue.int3;
				TimerManager.Cancel(BombAudioID);
				BombAudioID = TimerManager.In(nValue.int0, -nValue.int1, nValue.float025, delegate
				{
					BombAudioSource.PlayOneShot(BombAudioClip);
				});
			}
			BombTime -= Time.deltaTime;
		}
		else if (BombCount != nValue.int0)
		{
			BombCount = nValue.int0;
			BombTime = -nValue.int1;
			TimerManager.Cancel(BombAudioID);
			BombAudioID = nValue.int0;
		}
	}
}
