using System;
using UnityEngine;
using XLua;

[LuaCallCSharp(GenFlag.No)]
public class LuaBehaviour : MonoBehaviour
{
	internal const float GCInterval = 1f;

	public TextAsset luaScript;

	public Injection[] injections;

	internal static LuaEnv luaEnv = new LuaEnv();

	internal static float lastGCTime = 0f;

	private Action luaStart;

	private Action luaUpdate;

	private Action luaOnDestroy;

	private LuaTable scriptEnv;

	private void Awake()
	{
		scriptEnv = luaEnv.NewTable();
		LuaTable luaTable = luaEnv.NewTable();
		luaTable.Set("__index", luaEnv.Global);
		scriptEnv.SetMetaTable(luaTable);
		luaTable.Dispose();
		scriptEnv.Set("self", this);
		Injection[] array = injections;
		foreach (Injection injection in array)
		{
			scriptEnv.Set(injection.name, injection.value);
		}
		luaEnv.DoString(luaScript.text, "LuaBehaviour", scriptEnv);
		Action action = scriptEnv.Get<Action>("awake");
		scriptEnv.Get<string, Action>("start", out luaStart);
		scriptEnv.Get<string, Action>("update", out luaUpdate);
		scriptEnv.Get<string, Action>("ondestroy", out luaOnDestroy);
		if (action != null)
		{
			action();
		}
	}

	private void Start()
	{
		if (luaStart != null)
		{
			luaStart();
		}
	}

	private void Update()
	{
		if (luaUpdate != null)
		{
			luaUpdate();
		}
		if (Time.time - lastGCTime > 1f)
		{
			luaEnv.Tick();
			lastGCTime = Time.time;
		}
	}

	private void OnDestroy()
	{
		if (luaOnDestroy != null)
		{
			luaOnDestroy();
		}
		luaOnDestroy = null;
		luaUpdate = null;
		luaStart = null;
		scriptEnv.Dispose();
		injections = null;
	}
}
