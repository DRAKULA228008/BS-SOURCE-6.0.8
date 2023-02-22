using System;
using UnityEngine;

public class MaxScoreManager : MonoBehaviour
{
	[Serializable]
	public struct MaxScoreData
	{
		public GameMode mode;

		public CryptoInt score;

		public MaxScoreData(GameMode m, int s)
		{
			mode = m;
			score = s;
		}
	}

	public MaxScoreData[] modes;

	private static MaxScoreManager instance;

	private void Awake()
	{
		instance = this;
	}

	public static int Get(GameMode mode)
	{
		for (int i = 0; i < instance.modes.Length; i++)
		{
			if (instance.modes[i].mode == mode)
			{
				return instance.modes[i].score;
			}
		}
		Debug.Log("No Find Score: " + mode);
		return 0;
	}
}
