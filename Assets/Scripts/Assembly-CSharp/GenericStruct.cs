using XLua;

//[Hotfix(HotfixFlag.Stateful)]
public struct GenericStruct<T>
{
	private T a;

	public GenericStruct(T a)
	{
		this.a = a;
	}

	public T GetA(int p)
	{
		return a;
	}
}
