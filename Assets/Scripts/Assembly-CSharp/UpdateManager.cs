using UnityEngine;

public class UpdateManager : MonoBehaviour
{
	private static UpdateManager instance;

	private int updateTimerCount;

	private nTimer[] updateTimer = new nTimer[0];

	private void Awake()
	{
		instance = this;
	}

	private static void Init()
	{
		if (!(instance != null))
		{
			GameObject gameObject = new GameObject("UpdateManager");
			gameObject.AddComponent<UpdateManager>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	public static void Add(nTimer behaviour)
	{
		Init();
		int num = instance.updateTimer.Length;
		nTimer[] array = new nTimer[num + 1];
		for (int i = 0; i < num; i++)
		{
			array[i] = instance.updateTimer[i];
		}
		array[array.Length - 1] = behaviour;
		instance.updateTimer = array;
		instance.updateTimerCount = array.Length;
	}

	public static void Remove(nTimer behaviour)
	{
		Init();
		int num = instance.updateTimer.Length;
		nTimer[] array = new nTimer[num - 1];
		num--;
		for (int i = 0; i < num; i++)
		{
			if (!(instance.updateTimer[i] == behaviour))
			{
				array[i] = instance.updateTimer[i];
			}
		}
		instance.updateTimer = array;
		instance.updateTimerCount = array.Length;
	}

	private void Update()
	{
		if (updateTimerCount == 0)
		{
			return;
		}
		for (int i = 0; i < updateTimerCount; i++)
		{
			if (updateTimer[i] == null)
			{
			}
		}
	}
}
