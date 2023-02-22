using UnityEngine;
using XLua;

public class Helloworld : MonoBehaviour
{
	public CryptoString text;

	private LuaEnv luaenv;

	private void Start()
	{
		luaenv = new LuaEnv();
		luaenv.DoString(text);
		luaenv.DoString(luaenv.Global.Get<string>("a"));
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
	//	luaenv.Dispose();
	}
}
