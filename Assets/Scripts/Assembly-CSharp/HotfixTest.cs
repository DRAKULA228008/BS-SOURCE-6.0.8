using UnityEngine;
using XLua;

[Hotfix(HotfixFlag.Stateless)]
public class HotfixTest : MonoBehaviour
{
	private LuaEnv luaenv = new LuaEnv();

	public int tick;

	private void Start()
	{
	}

	private void Update()
	{
		if (++tick % 50 == 0)
		{
			Debug.Log(">>>>>>>>Update in C#, tick = " + tick);
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f, 10f, 300f, 80f), "Hotfix"))
		{
			luaenv.DoString("\r\n                xlua.hotfix(CS.HotfixTest, 'Update', function(self)\r\n                    self.tick = self.tick + 1\r\n                    if (self.tick % 50) == 0 then\r\n                        print('<<<<<<<<Update in lua, tick = ' .. self.tick)\r\n                    end\r\n                end)\r\n            ");
		}
		string text = "在运行该示例之前，请细致阅读xLua文档，并执行以下步骤：\r\n\r\n1.宏定义：添加 HOTFIX_ENABLE 到 'Edit > Project Settings > Player > Other Settings > Scripting Define Symbols'。\r\n（注意：各平台需要分别设置）\r\n\r\n2.生成代码：执行 'XLua > Generate Code' 菜单，等待Unity编译完成。\r\n\r\n3.注入：执行 'XLua > Hotfix Inject In Editor' 菜单。注入成功会打印 'hotfix inject finish!' 或者 'had injected!' 。";
		string text2 = "Read documents carefully before you run this example, then follow the steps below:\r\n\r\n1. Define: Add 'HOTFIX_ENABLE' to 'Edit > Project Settings > Player > Other Settings > Scripting Define Symbols'.\r\n(Note: Each platform needs to set this respectively)\r\n\r\n2.Generate Code: Execute menu 'XLua > Generate Code', wait for Unity's compilation.\r\n\r\n\r\n3.Inject: Execute menu 'XLua > Hotfix Inject In Editor'.There should be 'hotfix inject finish!' or 'had injected!' print in the Console if the Injection is successful.";
		GUIStyle textArea = GUI.skin.textArea;
		textArea.normal.textColor = Color.red;
		textArea.fontSize = 16;
		GUI.TextArea(new Rect(10f, 100f, 500f, 290f), text, textArea);
		GUI.TextArea(new Rect(10f, 400f, 500f, 290f), text2, textArea);
	}
}
