public struct FirebaseParam
{
	private string Param;

	public static FirebaseParam Default
	{
		get
		{
			return default(FirebaseParam);
		}
	}

	public FirebaseParam(string param)
	{
		Param = param;
	}

	public FirebaseParam Add(string param)
	{
		if (!string.IsNullOrEmpty(Param))
		{
			Param += "&";
		}
		Param += param;
		return this;
	}

	public FirebaseParam Add(string header, string value)
	{
		return Add(header, value, true);
	}

	public FirebaseParam Add(string header, string value, bool quoted)
	{
		if (quoted)
		{
			return Add(header + "=\"" + value + "\"");
		}
		return Add(header + "=" + value);
	}

	public FirebaseParam Add(string header, int value)
	{
		return Add(header + "=" + value);
	}

	public FirebaseParam Add(string header, float value)
	{
		return Add(header + "=" + value);
	}

	public FirebaseParam Add(string header, long value)
	{
		return Add(header + "=" + value);
	}

	public FirebaseParam Add(string header, bool value)
	{
		return Add(header + "=" + value);
	}

	public FirebaseParam Auth(string auth)
	{
		return Add("auth", auth, false);
	}

	public FirebaseParam OrderBy(string key)
	{
		return Add("orderBy", key);
	}

	public FirebaseParam OrderByKey()
	{
		return Add("orderBy", "$key");
	}

	public FirebaseParam OrderByValue()
	{
		return Add("orderBy", "$value");
	}

	public FirebaseParam OrderByPriority()
	{
		return Add("orderBy", "$priority");
	}

	public FirebaseParam LimitToFirst(int limit)
	{
		return Add("limitToFirst", limit);
	}

	public FirebaseParam LimitToLast(int limit)
	{
		return Add("limitToLast", limit);
	}

	public FirebaseParam StartAt(string start)
	{
		return Add("startAt", start);
	}

	public FirebaseParam StartAt(int start)
	{
		return Add("startAt", start);
	}

	public FirebaseParam StartAt(float start)
	{
		return Add("startAt", start);
	}

	public FirebaseParam StartAt(long start)
	{
		return Add("startAt", start);
	}

	public FirebaseParam StartAt(bool start)
	{
		return Add("startAt", start);
	}

	public FirebaseParam EndAt(string end)
	{
		return Add("endAt", end);
	}

	public FirebaseParam EndAt(int end)
	{
		return Add("endAt", end);
	}

	public FirebaseParam EndAt(float end)
	{
		return Add("endAt", end);
	}

	public FirebaseParam EndAt(long end)
	{
		return Add("endAt", end);
	}

	public FirebaseParam EndAt(bool end)
	{
		return Add("endAt", end);
	}

	public FirebaseParam EqualTo(string at)
	{
		return Add("equalTo", at);
	}

	public FirebaseParam EqualTo(int at)
	{
		return Add("equalTo", at);
	}

	public FirebaseParam EqualTo(float at)
	{
		return Add("equalTo", at);
	}

	public FirebaseParam EqualTo(long at)
	{
		return Add("equalTo", at);
	}

	public FirebaseParam EqualTo(bool at)
	{
		return Add("equalTo", at);
	}

	public FirebaseParam PrintPretty()
	{
		return Add("print=pretty");
	}

	public FirebaseParam PrintSilent()
	{
		return Add("print=silent");
	}

	public FirebaseParam Shallow()
	{
		return Add("shallow=true");
	}

	public override string ToString()
	{
		return Param;
	}
}
