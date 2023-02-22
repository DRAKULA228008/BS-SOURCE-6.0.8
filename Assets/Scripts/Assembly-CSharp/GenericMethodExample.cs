using UnityEngine;
using XLua;

public class GenericMethodExample : MonoBehaviour
{
	private const string script = "\n        local foo1 = CS.Foo1Child()\n        local foo2 = CS.Foo2Child()\n\n        local obj = CS.UnityEngine.GameObject()\n        foo1:PlainExtension()\n        foo1:Extension1()\n        foo1:Extension2(obj) -- overload1\n        foo1:Extension2(foo2) -- overload2\n        \n        local foo = CS.Foo()\n        foo:Test1(foo1)\n        foo:Test2(foo1,foo2,obj)\n";

	private LuaEnv env;

	private void Start()
	{
		env = new LuaEnv();
		env.DoString("\n        local foo1 = CS.Foo1Child()\n        local foo2 = CS.Foo2Child()\n\n        local obj = CS.UnityEngine.GameObject()\n        foo1:PlainExtension()\n        foo1:Extension1()\n        foo1:Extension2(obj) -- overload1\n        foo1:Extension2(foo2) -- overload2\n        \n        local foo = CS.Foo()\n        foo:Test1(foo1)\n        foo:Test2(foo1,foo2,obj)\n");
	}

	private void Update()
	{
		if (env != null)
		{
			env.Tick();
		}
	}

	private void OnDestroy()
	{
		env.Dispose();
	}
}
