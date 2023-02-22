using UnityEngine;
using XLua;

[Hotfix(HotfixFlag.Stateless)]
public class HotfixCalc
{
	public int Add(int a, int b)
	{
		return a - b;
	}

	public Vector3 Add(Vector3 a, Vector3 b)
	{
		return a - b;
	}

	public int TestOut(int a, out double b, ref string c)
	{
		b = a + 2;
		c = "wrong version";
		return a + 3;
	}

	public int TestOut(int a, out double b, ref string c, GameObject go)
	{
		return TestOut(a, out b, ref c);
	}

	public T Test1<T>()
	{
		return default(T);
	}

	public T1 Test2<T1, T2, T3>(T1 a, out T2 b, ref T3 c)
	{
		b = default(T2);
		return a;
	}

	public static int Test3<T>(T a)
	{
		return 0;
	}

	public static void Test4<T>(T a)
	{
	}

	public void Test5<T>(int a, params T[] arg)
	{
	}
}
