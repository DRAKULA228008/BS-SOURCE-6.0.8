using System;
using UnityEngine;

public class nTimer : MonoBehaviour
{
	private BetterList<TimerData> timers = new BetterList<TimerData>();

	private BetterList<TimerData> pool = new BetterList<TimerData>();

	private static int maxInvokeInUpdate = 5;

	private void OnEnable()
	{
		TimerManager.OnUpdate = (TimerDelegate)Delegate.Combine(TimerManager.OnUpdate, new TimerDelegate(OnUpdate));
	}

	private void OnDisable()
	{
		TimerManager.OnUpdate = (TimerDelegate)Delegate.Remove(TimerManager.OnUpdate, new TimerDelegate(OnUpdate));
	}

	public TimerData Create(string tag, float delay, TimerDelegate callback)
	{
		return Create(tag, delay, false, callback);
	}

	public TimerData Create(string tag, float delay, bool loop, TimerDelegate callback)
	{
		TimerData timerData = new TimerData();
		timerData.tag = tag;
		timerData.delay = delay;
		timerData.endTime = TimerManager.time + timerData.delay;
		timerData.loop = loop;
		timerData.callback = callback;
		timerData.stop = false;
		pool.Add(timerData);
		return timerData;
	}

	public TimerData In(string tag)
	{
		for (int i = 0; i < pool.size; i++)
		{
			if (pool[i].tag == tag)
			{
				pool[i].endTime = TimerManager.time + pool[i].delay;
				pool[i].stop = false;
				timers.Add(pool[i]);
				return pool[i];
			}
		}
		Debug.LogError("No Find Timer: " + tag);
		return null;
	}

	public TimerData In(string tag, float delay)
	{
		for (int i = 0; i < pool.size; i++)
		{
			if (pool[i].tag == tag)
			{
				pool[i].delay = delay;
				pool[i].endTime = TimerManager.time + delay;
				pool[i].stop = false;
				timers.Add(pool[i]);
				return pool[i];
			}
		}
		Debug.LogError("No Find Timer: " + tag);
		return null;
	}

	public TimerData In(float delay, TimerDelegate callback)
	{
		return In(string.Empty, delay, false, callback);
	}

	public TimerData In(float delay, bool loop, TimerDelegate callback)
	{
		return In(string.Empty, delay, loop, callback);
	}

	public TimerData In(string tag, float delay, bool loop, TimerDelegate callback)
	{
		TimerData timerData = new TimerData();
		timerData.tag = tag;
		timerData.delay = delay;
		timerData.endTime = TimerManager.time + timerData.delay;
		timerData.loop = loop;
		timerData.callback = callback;
		timerData.stop = false;
		timers.Add(timerData);
		return timerData;
	}

	public bool Contains(string tag)
	{
		for (int i = 0; i < pool.size; i++)
		{
			if (pool[i].tag == tag)
			{
				return true;
			}
		}
		return false;
	}

	public bool isActive(string tag)
	{
		for (int i = 0; i < timers.size; i++)
		{
			if (timers[i].tag == tag)
			{
				return true;
			}
		}
		return false;
	}

	public void Cancel(string tag)
	{
		for (int i = 0; i < timers.size; i++)
		{
			if (timers[i].tag == tag)
			{
				timers[i].stop = true;
			}
		}
	}

	private void OnUpdate()
	{
		nProfiler.BeginSample("OnUpdate");
		if (TimerManager.stopOnUpdate)
		{
			return;
		}
		for (int i = 0; i < timers.size; i++)
		{
			if (timers[i].Update(TimerManager.time))
			{
				timers.RemoveAt(i);
				i--;
				if (maxInvokeInUpdate < 0)
				{
					maxInvokeInUpdate = 5;
					TimerManager.stopOnUpdate = true;
					break;
				}
				maxInvokeInUpdate--;
			}
		}
		nProfiler.EndSample();
	}
}
