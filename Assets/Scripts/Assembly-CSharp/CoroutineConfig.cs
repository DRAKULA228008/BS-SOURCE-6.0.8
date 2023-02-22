using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public static class CoroutineConfig
{
	[LuaCallCSharp(GenFlag.No)]
	public static List<Type> LuaCallCSharp
	{
		get
		{
			List<Type> list = new List<Type>();
			list.Add(typeof(WaitForSeconds));
			list.Add(typeof(WWW));
			return list;
		}
	}
}
