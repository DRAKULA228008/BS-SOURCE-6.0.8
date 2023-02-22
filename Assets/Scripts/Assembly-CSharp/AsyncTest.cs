using UnityEngine;
using XLua;

public class AsyncTest : MonoBehaviour
{
	private LuaEnv luaenv;

	private void Start()
	{
		luaenv = new LuaEnv();
		luaenv.DoString("require 'async_test'");
	}

	private void Update()
	{
		if (luaenv != null)
		{
			luaenv.Tick();
		}
	}
}
