using UnityEngine;
using XLua;

public class CoroutineTest : MonoBehaviour
{
	public string text;

	private LuaEnv luaenv;

	private void Start()
	{
		luaenv = new LuaEnv();
		luaenv.DoString(text);
	}

	private void Update()
	{
		if (luaenv != null)
		{
			luaenv.Tick();
		}
	}

	private void OnDestroy()
	{
		luaenv.Dispose();
	}
}
