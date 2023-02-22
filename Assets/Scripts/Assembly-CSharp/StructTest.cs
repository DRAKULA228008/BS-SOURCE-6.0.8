using UnityEngine;
using XLua;

[Hotfix(HotfixFlag.Stateless)]
public struct StructTest
{
	private GameObject go;

	public StructTest(GameObject go)
	{
		this.go = go;
	}

	public GameObject GetGo(int a, object b)
	{
		return go;
	}
}
