using System;
using UnityEngine;
using XLua;

public class HotfixTest2 : MonoBehaviour
{
	private void Start()
	{
		LuaEnv luaEnv = new LuaEnv();
		HotfixCalc hotfixCalc = new HotfixCalc();
		NoHotfixCalc noHotfixCalc = new NoHotfixCalc();
		int num = 100000000;
		DateTime now = DateTime.Now;
		for (int i = 0; i < num; i++)
		{
			hotfixCalc.Add(2, 1);
		}
		double totalMilliseconds = (DateTime.Now - now).TotalMilliseconds;
		Debug.Log("Hotfix using:" + totalMilliseconds);
		now = DateTime.Now;
		for (int j = 0; j < num; j++)
		{
			noHotfixCalc.Add(2, 1);
		}
		double totalMilliseconds2 = (DateTime.Now - now).TotalMilliseconds;
		Debug.Log("No Hotfix using:" + totalMilliseconds2);
		Debug.Log("drop:" + (totalMilliseconds - totalMilliseconds2) / totalMilliseconds);
		Debug.Log("Before Fix: 2 + 1 = " + hotfixCalc.Add(2, 1));
		Debug.Log("Before Fix: Vector3(2, 3, 4) + Vector3(1, 2, 3) = " + hotfixCalc.Add(new Vector3(2f, 3f, 4f), new Vector3(1f, 2f, 3f)));
		luaEnv.DoString("\r\n            xlua.hotfix(CS.HotfixCalc, 'Add', function(self, a, b)\r\n                return a + b\r\n            end)\r\n        ");
		Debug.Log("After Fix: 2 + 1 = " + hotfixCalc.Add(2, 1));
		Debug.Log("After Fix: Vector3(2, 3, 4) + Vector3(1, 2, 3) = " + hotfixCalc.Add(new Vector3(2f, 3f, 4f), new Vector3(1f, 2f, 3f)));
		string c = "hehe";
		double b;
		int num2 = hotfixCalc.TestOut(100, out b, ref c);
		Debug.Log("ret = " + num2 + ", num = " + b + ", str = " + c);
		luaEnv.DoString("\r\n            xlua.hotfix(CS.HotfixCalc, 'TestOut', function(self, a, c, go)\r\n                    print('TestOut', self, a, c, go)\r\n                    if go then error('test error') end\r\n                    return a + 10, a + 20, 'right version'\r\n                end)\r\n        ");
		c = "hehe";
		num2 = hotfixCalc.TestOut(100, out b, ref c);
		Debug.Log("ret = " + num2 + ", num = " + b + ", str = " + c);
		luaEnv.DoString("\r\n            xlua.hotfix(CS.HotfixCalc, {\r\n                 Test1 = function(self)\r\n                    print('Test1', self)\r\n                    return 1\r\n                 end;\r\n                 Test2 = function(self, a, b)\r\n                     print('Test1', self, a, b)\r\n                     return a + 10, 1024, b\r\n                 end;\r\n                 Test3 = function(a)\r\n                    print(a)\r\n                    return 10\r\n                 end;\r\n                 Test4 = function(a)\r\n                    print(a)\r\n                 end;\r\n                 Test5 = function(self, a, ...)\r\n                    print('Test4', self, a, ...)\r\n                 end\r\n            })\r\n        ");
		int num3 = hotfixCalc.Test1<int>();
		double b2 = hotfixCalc.Test1<double>();
		Debug.Log("r1:" + num3 + ",r2:" + b2);
		string c2 = "heihei";
		int num4 = hotfixCalc.Test2<int, double, string>(num3, out b2, ref c2);
		Debug.Log("r1:" + num3 + ",r2:" + b2 + ",r3:" + num4 + ",ss:" + c2);
		num4 = HotfixCalc.Test3("test3");
		num4 = HotfixCalc.Test3(2);
		num4 = HotfixCalc.Test3(this);
		Debug.Log("r3:" + num4);
		HotfixCalc.Test4(this);
		HotfixCalc.Test4(2);
		hotfixCalc.Test5<string>(10, "a", "b", "c");
		hotfixCalc.Test5<int>(10, 1, 3, 5);
		Debug.Log("----------------------before------------------------");
		TestStateful();
		GC.Collect();
		GC.WaitForPendingFinalizers();
		luaEnv.DoString("\r\n            xlua.hotfix(CS.StatefullTest, {\r\n                ['.ctor'] = function(csobj)\r\n                    return {evt = {}, start = 0}\r\n                end;\r\n                set_AProp = function(self, v)\r\n                    print('set_AProp', v)\r\n                    self.AProp = v\r\n                end;\r\n                get_AProp = function(self)\r\n                    return self.AProp\r\n                end;\r\n                get_Item = function(self, k)\r\n                    print('get_Item', k)\r\n                    return 1024\r\n                end;\r\n                set_Item = function(self, k, v)\r\n                    print('set_Item', k, v)\r\n                end;\r\n                add_AEvent = function(self, cb)\r\n                    print('add_AEvent', cb)\r\n                    table.insert(self.evt, cb)\r\n                end;\r\n                remove_AEvent = function(self, cb)\r\n                   print('remove_AEvent', cb)\r\n                   for i, v in ipairs(self.evt) do\r\n                       if v == cb then\r\n                           table.remove(self.evt, i)\r\n                           break\r\n                       end\r\n                   end\r\n                end;\r\n                Start = function(self)\r\n                    print('Start')\r\n                    for _, cb in ipairs(self.evt) do\r\n                        cb(self.start, 2)\r\n                    end\r\n                    self.start = self.start + 1\r\n                end;\r\n                StaticFunc = function(a, b, c)\r\n                   print(a, b, c)\r\n                end;\r\n                GenericTest = function(self, a)\r\n                   print(self, a)\r\n                end;\r\n                Finalize = function(self)\r\n                   print('Finalize', self)\r\n                end\r\n           })\r\n        ");
		Debug.Log("----------------------after------------------------");
		TestStateful();
		luaEnv.FullGc();
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GenericClass<double> genericClass = new GenericClass<double>(1.1);
		genericClass.Func1();
		Debug.Log(genericClass.Func2());
		luaEnv.DoString("\r\n            xlua.hotfix(CS['GenericClass`1[System.Double]'], {\r\n                ['.ctor'] = function(obj, a)\r\n                    print('GenericClass<double>', obj, a)\r\n                end;\r\n                Func1 = function(obj)\r\n                    print('GenericClass<double>.Func1', obj)\r\n                end;\r\n                Func2 = function(obj)\r\n                    print('GenericClass<double>.Func2', obj)\r\n                    return 1314\r\n                end\r\n            })\r\n        ");
		genericClass = new GenericClass<double>(1.1);
		genericClass.Func1();
		Debug.Log(genericClass.Func2());
		InnerTypeTest innerTypeTest = new InnerTypeTest();
		innerTypeTest.Foo();
		luaEnv.DoString("\r\n            xlua.hotfix(CS.InnerTypeTest, 'Bar', function(obj)\r\n                    print('lua Bar', obj)\r\n                    return {x = 10, y = 20}\r\n                end)\r\n        ");
		innerTypeTest.Foo();
		StructTest structTest = new StructTest(base.gameObject);
		Debug.Log("go=" + structTest.GetGo(123, "john"));
		luaEnv.DoString("\r\n            xlua.hotfix(CS.StructTest, 'GetGo', function(self, a, b)\r\n                    print('GetGo', self, a, b)\r\n                    return nil\r\n                end)\r\n        ");
		Debug.Log("go=" + structTest.GetGo(123, "john"));
		GenericStruct<int> genericStruct = new GenericStruct<int>(1);
		Debug.Log("gs.GetA()=" + genericStruct.GetA(123));
		luaEnv.DoString("\r\n            xlua.hotfix(CS['GenericStruct`1[System.Int32]'], 'GetA', function(self, a)\r\n                    print('GetA',self, a)\r\n                    return 789\r\n                end)\r\n        ");
		Debug.Log("gs.GetA()=" + genericStruct.GetA(123));
		try
		{
			hotfixCalc.TestOut(100, out b, ref c, base.gameObject);
		}
		catch (LuaException ex)
		{
			Debug.Log("throw in lua an catch in c# ok, e.Message:" + ex.Message);
		}
	}

	private void TestStateful()
	{
		StatefullTest statefullTest = new StatefullTest();
		statefullTest.AProp = 10;
		Debug.Log("sft.AProp:" + statefullTest.AProp);
		statefullTest["1"] = 1;
		Debug.Log("sft['1']:" + statefullTest["1"]);
		Action<int, double> value = delegate(int a, double b)
		{
			Debug.Log("a:" + a + ",b:" + b);
		};
		statefullTest.AEvent += value;
		statefullTest.Start();
		statefullTest.Start();
		statefullTest.AEvent -= value;
		statefullTest.Start();
		StatefullTest.StaticFunc(1, 2);
		StatefullTest.StaticFunc("e", 3, 4);
		statefullTest.GenericTest(1);
		statefullTest.GenericTest("hehe");
	}

	private void Update()
	{
	}
}
