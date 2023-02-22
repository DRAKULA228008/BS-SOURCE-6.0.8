using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class UIGameConsole : MonoBehaviour
{
	public enum Value
	{
		None,
		Int,
		Float,
		Bool,
		String,
		Vector2,
		Vector3,
		Color
	}

	public class CommandData
	{
		public Value value;

		public MethodInfo method;
	}

	public UIInput input;

	public UITextList textList;

	private static Dictionary<string, CommandData> commands;

	private List<string> commandsContains = new List<string>();

	private int selectCommandsIndex;

	private List<string> commandsHistory = new List<string>();

	private string text = string.Empty;

	private bool showGUI;

	private static UIGameConsole instance;

	public static string lastInvokeCommand;

	private void Start()
	{
		GetCommands();
	}

	public void OnSubmit()
	{
		string value = input.value;
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		value = value.Replace("\n", string.Empty);
		Log(">" + value);
		commandsHistory.Add(value);
		if (value == "list")
		{
			if (commands.Count != 0)
			{
				value = string.Empty;
				Log("/////////////////////////////////////////////////");
				for (int i = 0; i < commands.Count; i++)
				{
					Log(commands.Keys.ElementAt(i));
				}
				Log("/////////////////////////////////////////////////");
			}
		}
		else
		{
			OnCommand();
		}
	}

	public void OnClear()
	{
		textList.Clear();
	}

	private void GetCommands()
	{
		if (commands != null)
		{
			return;
		}
		commands = new Dictionary<string, CommandData>();
		MethodInfo[] methods = typeof(ConsoleCommands).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
		for (int i = 0; i < methods.Length; i++)
		{
			ConsoleAttribute consoleAttribute = (ConsoleAttribute)methods[i].GetCustomAttributes(typeof(ConsoleAttribute), true)[0];
			for (int j = 0; j < consoleAttribute.commands.Length; j++)
			{
				CommandData commandData = new CommandData();
				commandData.method = methods[i];
				ParameterInfo[] parameters = commandData.method.GetParameters();
				if (parameters != null && parameters.Length == 1)
				{
					commandData.value = GetValueType(parameters[0].ParameterType);
				}
				commands.Add(consoleAttribute.commands[j], commandData);
			}
		}
	}

	private static Value GetValueType(Type type)
	{
		if (type == typeof(int))
		{
			return Value.Int;
		}
		if (type == typeof(float))
		{
			return Value.Float;
		}
		if (type == typeof(bool))
		{
			return Value.Bool;
		}
		if (type == typeof(string))
		{
			return Value.String;
		}
		if (type == typeof(Vector2))
		{
			return Value.Vector2;
		}
		if (type == typeof(Vector3))
		{
			return Value.Vector3;
		}
		if (type == typeof(Color))
		{
			return Value.Color;
		}
		return Value.None;
	}

	private void OnCommand()
	{
		string[] array = text.Split(new char[1] { ' ' }, 2);
		if (array.Length < 3 && commands.ContainsKey(array[0]))
		{
			switch (commands[array[0]].value)
			{
			case Value.None:
				if (array.Length == 1)
				{
					lastInvokeCommand = array[0];
					commands[array[0]].method.Invoke(this, null);
				}
				else
				{
					LogError("Command error");
				}
				break;
			case Value.Int:
			case Value.Float:
			case Value.Bool:
			case Value.String:
			case Value.Vector2:
			case Value.Vector3:
			case Value.Color:
			{
				object result;
				if (array.Length == 2 && Parse(array[1], out result, commands[array[0]].value))
				{
					lastInvokeCommand = array[0];
					commands[array[0]].method.Invoke(true, new object[1] { result });
				}
				else
				{
					LogError("Command error");
				}
				break;
			}
			}
		}
		else
		{
			LogError("Command not found: " + array[0]);
		}
		text = string.Empty;
	}

	private bool Contains(string t, string t2)
	{
		if (t.Length > t2.Length)
		{
			return false;
		}
		for (int i = 0; i < t.Length; i++)
		{
			if (t[i] != t2[i])
			{
				return false;
			}
		}
		return true;
	}

	private string Format(Value valueType)
	{
		switch (valueType)
		{
		case Value.Int:
			return "[int]";
		case Value.Float:
			return "[float]";
		case Value.Bool:
			return "[bool]";
		case Value.String:
			return "[string]";
		case Value.Vector2:
			return "[vector2]";
		case Value.Vector3:
			return "[vector3]";
		case Value.Color:
			return "[color]";
		default:
			return string.Empty;
		}
	}

	private bool Parse(string value, out object result, Value valueType)
	{
		switch (valueType)
		{
		case Value.Int:
		{
			int result7 = 0;
			if (int.TryParse(value, out result7))
			{
				result = result7;
				return true;
			}
			break;
		}
		case Value.Float:
		{
			float result8 = 0f;
			value = value.Replace(",", ".");
			if (float.TryParse(value, out result8))
			{
				result = result8;
				return true;
			}
			break;
		}
		case Value.Bool:
		{
			bool result6 = false;
			if (value == "1")
			{
				result = true;
				return true;
			}
			if (value == "0")
			{
				result = false;
				return true;
			}
			if (bool.TryParse(value, out result6))
			{
				result = result6;
				return true;
			}
			break;
		}
		case Value.String:
			result = value;
			return true;
		case Value.Vector2:
		{
			string[] array2 = value.Split(' ');
			if (array2.Length == 2)
			{
				float result9 = 0f;
				float result10;
				if (float.TryParse(array2[0], out result10) && float.TryParse(array2[1], out result9))
				{
					result = new Vector2(result10, result9);
					return true;
				}
			}
			break;
		}
		case Value.Vector3:
		{
			string[] array3 = value.Split(' ');
			if (array3.Length == 3)
			{
				float result11 = 0f;
				float result12;
				float result13;
				if (float.TryParse(array3[0], out result12) && float.TryParse(array3[1], out result13) && float.TryParse(array3[2], out result11))
				{
					result = new Vector3(result12, result13, result11);
					return true;
				}
			}
			break;
		}
		case Value.Color:
		{
			string[] array = value.Split(' ');
			if (array.Length == 4)
			{
				byte result2 = 0;
				byte result3;
				byte result4;
				byte result5;
				if (byte.TryParse(array[0], out result3) && byte.TryParse(array[1], out result4) && byte.TryParse(array[2], out result5) && byte.TryParse(array[3], out result2))
				{
					result = new Color32(result3, result4, result5, result2);
					return true;
				}
			}
			break;
		}
		}
		result = null;
		return false;
	}

	private void UpdateArrowCommand()
	{
		if (text != " " && commandsContains.Count != 0)
		{
			if (selectCommandsIndex > commandsContains.Count - 1)
			{
				selectCommandsIndex = 0;
			}
			else if (selectCommandsIndex < 0)
			{
				selectCommandsIndex = commandsContains.Count - 1;
			}
			text = commandsContains[selectCommandsIndex];
		}
		else if (commandsHistory.Count != 0)
		{
			if (selectCommandsIndex > commandsHistory.Count - 1)
			{
				selectCommandsIndex = 0;
			}
			else if (selectCommandsIndex < 0)
			{
				selectCommandsIndex = commandsHistory.Count - 1;
			}
			text = commandsHistory[selectCommandsIndex];
		}
	}

	public static void Log(string message)
	{
		ConsoleManager.Log(message);
	}

	public static void LogWarning(string message)
	{
		ConsoleManager.LogWarning(message);
	}

	public static void LogError(string message)
	{
		ConsoleManager.LogError(message);
	}

	public static void ShowConsole(bool show)
	{
		instance.showGUI = show;
	}
}
