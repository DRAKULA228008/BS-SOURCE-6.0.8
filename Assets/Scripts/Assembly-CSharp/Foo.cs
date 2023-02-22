using System;
using UnityEngine;
using XLua;

[LuaCallCSharp(GenFlag.No)]
public class Foo
{
	public void Test1<T>(T a) where T : Foo1Parent
	{
		Debug.Log(string.Format("Test1<{0}>", typeof(T)));
	}

	public T1 Test2<T1, T2>(T1 a, T2 b, GameObject c) where T1 : Foo1Parent where T2 : Foo2Parent
	{
		Debug.Log(string.Format("Test2<{0},{1}>", typeof(T1), typeof(T2)), c);
		return a;
	}

	public void UnsupportedMethod1<T>(T a)
	{
		Debug.Log("UnsupportedMethod1");
	}

	public void UnsupportedMethod2<T>() where T : Foo1Parent
	{
		Debug.Log(string.Format("UnsupportedMethod2<{0}>", typeof(T)));
	}

	public void UnsupportedMethod3<T>(T a) where T : IDisposable
	{
		Debug.Log(string.Format("UnsupportedMethod3<{0}>", typeof(T)));
	}
}
