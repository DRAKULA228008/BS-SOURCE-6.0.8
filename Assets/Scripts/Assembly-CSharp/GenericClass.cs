using UnityEngine;
using XLua;

[Hotfix(HotfixFlag.Stateless)]
public class GenericClass<T>
{
	private T a;

	public GenericClass(T a)
	{
		this.a = a;
	}

	public void Func1()
	{
		Debug.Log("a=" + a);
	}

	public T Func2()
	{
		return default(T);
	}
}
