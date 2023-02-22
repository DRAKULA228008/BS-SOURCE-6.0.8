public static class StringCache
{
	private static string[] cacheNumber;

	private static string[] cacheTime;

	private static FastString fastString = new FastString(200);

	public static string Create(params string[] str)
	{
		fastString.Clear();
		for (int i = 0; i < str.Length; i++)
		{
			fastString.Append(str[i]);
		}
		return fastString.ToString();
	}

	public static string Get(uint number)
	{
		return Get((int)number);
	}

	public static string Get(int number)
	{
		if (number < 0)
		{
			return number.ToString();
		}
		if (cacheNumber == null)
		{
			cacheNumber = new string[1010];
			for (int i = 0; i < 1010; i++)
			{
				cacheNumber[i] = i.ToString();
			}
		}
		if (cacheNumber.Length - 1 >= number)
		{
			return cacheNumber[number];
		}
		return number.ToString();
	}

	public static string GetTime(float time)
	{
		return GetTime((int)time);
	}

	public static string GetTime(int time)
	{
		if (time < 0)
		{
			time *= -1;
		}
		if (cacheTime == null)
		{
			cacheTime = new string[1000];
			int num = 0;
			int num2 = 0;
			cacheTime[0] = "0:00";
			for (int i = 1; i < 1000; i++)
			{
				num = i / 60;
				num2 = i - num * 60;
				cacheTime[i] = string.Format("{0:0}:{1:00}", num, num2);
			}
		}
		if (cacheTime != null && cacheTime.Length - 1 >= time)
		{
			return cacheTime[time];
		}
		int num3 = time / 60;
		int num4 = time - num3 * 60;
		return string.Format("{0:0}:{1:00}", num3, num4);
	}
}
