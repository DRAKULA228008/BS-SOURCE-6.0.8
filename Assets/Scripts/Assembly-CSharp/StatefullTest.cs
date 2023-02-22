using System;
using UnityEngine;
using XLua;

//[Hotfix(HotfixFlag.Stateful)]
public class StatefullTest
{
	public int AProp { get; set; }

	public int this[string field]
	{
		get
		{
			return 1;
		}
		set
		{
		}
	}

	public event Action<int, double> AEvent;

	public StatefullTest()
	{
	}

	public StatefullTest(int a, int b)
	{
		if (a <= 0)
		{
			Debug.Log("a=" + a);
			if (b <= 0 && a + b <= 0)
			{
				Debug.Log("b=" + b);
			}
		}
	}

	public void Start()
	{
	}

	private void Update()
	{
	}

	public void GenericTest<T>(T a)
	{
	}

	public static void StaticFunc(int a, int b)
	{
	}

	public static void StaticFunc(string a, int b, int c)
	{
	}

	~StatefullTest()
	{
		Debug.Log("~StatefullTest");
	}
}
