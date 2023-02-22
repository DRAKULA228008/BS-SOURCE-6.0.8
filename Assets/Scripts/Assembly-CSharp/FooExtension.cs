using UnityEngine;
using XLua;

[LuaCallCSharp(GenFlag.No)]
public static class FooExtension
{
	public static void PlainExtension(this Foo1Parent a)
	{
		Debug.Log("PlainExtension");
	}

	public static T Extension1<T>(this T a) where T : Foo1Parent
	{
		Debug.Log(string.Format("Extension1<{0}>", typeof(T)));
		return a;
	}

	public static T Extension2<T>(this T a, GameObject b) where T : Foo1Parent
	{
		Debug.Log(string.Format("Extension2<{0}>", typeof(T)), b);
		return a;
	}

	public static void Extension2<T1, T2>(this T1 a, T2 b) where T1 : Foo1Parent where T2 : Foo2Parent
	{
		Debug.Log(string.Format("Extension2<{0},{1}>", typeof(T1), typeof(T2)));
	}

	public static T UnsupportedExtension<T>(this GameObject obj) where T : Component
	{
		return obj.GetComponent<T>();
	}
}
