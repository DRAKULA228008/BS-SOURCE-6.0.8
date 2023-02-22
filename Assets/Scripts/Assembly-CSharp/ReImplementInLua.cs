using UnityEngine;
using XLua;

public class ReImplementInLua : MonoBehaviour
{
	private void Start()
	{
		LuaEnv luaEnv = new LuaEnv();
		luaEnv.DoString("\r\n            function test_vector3(title, v1, v2)\r\n               print(title)\r\n               print(v1.x, v1.y, v1.z)\r\n               print(v1, v2)\r\n               print(v1 + v2)\r\n               v1:Set(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z)\r\n               print(v1)\r\n               print(CS.UnityEngine.Vector3.Normalize(v1))\r\n            end\r\n            test_vector3('----before change metatable----', CS.UnityEngine.Vector3(1, 2, 3), CS.UnityEngine.Vector3(7, 8, 9))\r\n\r\n            local get_x, set_x = xlua.genaccessor(0, 8)\r\n            local get_y, set_y = xlua.genaccessor(4, 8)\r\n            local get_z, set_z = xlua.genaccessor(8, 8)\r\n            \r\n            local fields_getters = {\r\n                x = get_x, y = get_y, z = get_z\r\n            }\r\n            local fields_setters = {\r\n                x = set_x, y = set_y, z = set_z\r\n            }\r\n\r\n            local ins_methods = {\r\n                Set = function(o, x, y, z)\r\n                    set_x(o, x)\r\n                    set_y(o, y)\r\n                    set_z(o, z)\r\n                end\r\n            }\r\n\r\n            local mt = {\r\n                __index = function(o, k)\r\n                    --print('__index', k)\r\n                    if ins_methods[k] then return ins_methods[k] end\r\n                    return fields_getters[k] and fields_getters[k](o)\r\n                end,\r\n\r\n                __newindex = function(o, k, v)\r\n                    return fields_setters[k] and fields_setters[k](o, v) or error('no such field ' .. k)\r\n                end,\r\n\r\n                __tostring = function(o)\r\n                    return string.format('vector3 { %f, %f, %f}', o.x, o.y, o.z)\r\n                end,\r\n\r\n                __add = function(a, b)\r\n                    return CS.UnityEngine.Vector3(a.x + b.x, a.y + b.y, a.z + b.z)\r\n                end\r\n            }\r\n\r\n            xlua.setmetatable(CS.UnityEngine.Vector3, mt)\r\n            test_vector3('----after change metatable----', CS.UnityEngine.Vector3(1, 2, 3), CS.UnityEngine.Vector3(7, 8, 9))\r\n        ");
		Debug.Log("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
		luaEnv.DoString("\r\n            local mt = {\r\n                __index = {\r\n                    SwapXY = function(o) --成员函数\r\n                        o.x, o.y = o.y, o.x\r\n                    end\r\n                },\r\n\r\n                __tostring = function(o) --打印格式化函数\r\n                    return string.format('struct { %d, %d}', o.x, o.y)\r\n                end,\r\n            }\r\n\r\n            xlua.setmetatable(CS.PushAsTableStruct, mt)\r\n            \r\n            local PushAsTableStruct = {\r\n                Print = function(o) --静态函数\r\n                    print(o.x, o.y)\r\n                end\r\n            }\r\n\r\n            setmetatable(PushAsTableStruct, {\r\n                __call = function(_, x, y) --构造函数\r\n                    return setmetatable({x = x, y = y}, mt)\r\n                end\r\n            })\r\n            \r\n            xlua.setclass(CS, 'PushAsTableStruct', PushAsTableStruct)\r\n        ");
		PushAsTableStruct value = default(PushAsTableStruct);
		value.x = 100;
		value.y = 200;
		luaEnv.Global.Set("from_cs", value);
		luaEnv.DoString("\r\n            print('--------------from csharp---------------------')\r\n            assert(type(from_cs) == 'table')\r\n            print(from_cs)\r\n            CS.PushAsTableStruct.Print(from_cs)\r\n            from_cs:SwapXY()\r\n            print(from_cs)\r\n\r\n            print('--------------from lua---------------------')\r\n            local from_lua = CS.PushAsTableStruct(4, 5)\r\n            assert(type(from_lua) == 'table')\r\n            print(from_lua)\r\n            CS.PushAsTableStruct.Print(from_lua)\r\n            from_lua:SwapXY()\r\n            print(from_lua)\r\n        ");
		luaEnv.Dispose();
	}

	private void Update()
	{
	}
}
