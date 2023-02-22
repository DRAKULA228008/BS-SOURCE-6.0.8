using UnityEngine;
using XLua;

public class InvokeLua : MonoBehaviour
{
	[CSharpCallLua]
	public interface ICalc
	{
		int Mult { get; set; }

		int Add(int a, int b);
	}

	[CSharpCallLua]
	public delegate ICalc CalcNew(int mult, params string[] args);

	private string script = "\r\n                local calc_mt = {\r\n                    __index = {\r\n                        Add = function(self, a, b)\r\n                            return (a + b) * self.Mult\r\n                        end\r\n                    }\r\n                }\r\n\r\n                Calc = {\r\n\t                New = function (mult, ...)\r\n                        print(...)\r\n                        return setmetatable({Mult = mult}, calc_mt)\r\n                    end\r\n                }\r\n\t        ";

	private void Start()
	{
		LuaEnv luaEnv = new LuaEnv();
		Test(luaEnv);
		luaEnv.Dispose();
	}

	private void Test(LuaEnv luaenv)
	{
		luaenv.DoString(script);
		CalcNew inPath = luaenv.Global.GetInPath<CalcNew>("Calc.New");
		ICalc calc = inPath(10, "hi", "john");
		Debug.Log("sum(*10) =" + calc.Add(1, 2));
		calc.Mult = 100;
		Debug.Log("sum(*100)=" + calc.Add(1, 2));
	}

	private void Update()
	{
	}
}
