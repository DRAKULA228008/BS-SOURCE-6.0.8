using UnityEngine;
using XLua;

[Hotfix(HotfixFlag.Stateless)]
public class InnerTypeTest
{
	private struct _InnerStruct
	{
		public int x;

		public int y;
	}

	public void Foo()
	{
		_InnerStruct innerStruct = Bar();
		Debug.Log("{x=" + innerStruct.x + ",y= " + innerStruct.y + "}");
	}

	private _InnerStruct Bar()
	{
		_InnerStruct result = default(_InnerStruct);
		result.x = 1;
		result.y = 2;
		return result;
	}
}
