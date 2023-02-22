using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class GameConsole : MonoBehaviour
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

	private KeyCode key = KeyCode.Escape;

	private GUIStyle style = new GUIStyle();

	private GUIStyle styleButton = new GUIStyle();

	private GUIStyle styleButton2 = new GUIStyle();

	private GUIStyle styleText = new GUIStyle();

	private Texture2D background;

	private Texture2D background2;

	private Vector2 scroll;

	private static Dictionary<string, CommandData> commands;

	private List<string> commandsContains = new List<string>();

	private int selectCommandsIndex;

	private List<string> commandsHistory = new List<string>();

	private string text = string.Empty;

	private bool showGUI;

	private float dpi;

	private Rect[] rects;

	private static GameConsole instance;

	public static string lastInvokeCommand;

	public static bool actived
	{
		set
		{
			if (value)
			{
				if (instance != null)
				{
					return;
				}
				GameObject gameObject = new GameObject("GameConsole");
				gameObject.AddComponent<GameConsole>();
				if (!ConsoleManager.isCreated)
				{
					gameObject.AddComponent<ConsoleManager>();
				}
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
			else if (instance != null)
			{
				UnityEngine.Object.Destroy(instance.gameObject);
			}
		}
	}

	private void Awake()
	{
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		rects = new Rect[6];
		rects[0] = xRect(2f, 2f, 96f, 88f);
		rects[1] = xRect(2f, 91f, 40f, 8f);
		rects[2] = xRect(57f, 91f, 20f, 8f);
		rects[3] = xRect(78f, 91f, 20f, 8f);
		rects[4] = xRect(43f, 91f, 6f, 8f);
		rects[5] = xRect(50f, 91f, 6f, 8f);
		background = CreateTexture2D(new Color32(0, 0, 0, 180));
		background2 = CreateTexture2D(new Color32(100, 100, 100, byte.MaxValue));
		int num = (int)(13f * (float)Screen.width / 800f);
		style.normal.textColor = Color.white;
		style.fontSize = num;
		style.padding.left = 4;
		styleButton.normal.textColor = Color.white;
		styleButton.normal.background = background;
		styleButton.fontSize = num;
		styleButton.padding.left = 4;
		styleButton.alignment = TextAnchor.MiddleCenter;
		styleText.normal.textColor = Color.white;
		styleText.normal.background = background;
		styleText.fontSize = num;
		styleText.padding.left = 4;
		styleText.alignment = TextAnchor.MiddleLeft;
		styleButton2.normal.textColor = Color.white;
		styleButton2.normal.background = background2;
		styleButton2.fontSize = num - 1;
		styleButton2.padding.left = 4;
		styleButton2.contentOffset = new Vector2(15f, 0f);
		styleButton2.alignment = TextAnchor.MiddleLeft;
		dpi = Screen.dpi / 100f;
		if (dpi == 0f)
		{
			dpi = 1.6f;
		}
	}

	public static void Save(string command, int value)
	{
		PlayerPrefs.SetInt("console:" + command, value);
	}

	public static void Save(string command, float value)
	{
		PlayerPrefs.SetFloat("console:" + command, value);
	}

	public static void Save(string command, bool value)
	{
		PlayerPrefs.SetInt("console:" + command, value ? 1 : 0);
	}

	public static void Save(string command, Color32 value)
	{
		string value2 = value.r + "|" + value.g + "|" + value.b + "|" + value.a;
		PlayerPrefs.SetString("console:" + command, value2);
	}

	public static T Load<T>(string command)
	{
		return Load(command, default(T));
	}

	public static T Load<T>(string command, T defaultValue)
	{
		if (typeof(T) == typeof(int))
		{
			return (T)(object)PlayerPrefs.GetInt("console:" + command, (int)(object)defaultValue);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)PlayerPrefs.GetFloat("console:" + command, (float)(object)defaultValue);
		}
		if (typeof(T) == typeof(bool))
		{
			return (T)(object)((PlayerPrefs.GetInt("console:" + command, ((bool)(object)defaultValue) ? 1 : 0) == 1) ? true : false);
		}
		if (typeof(T) == typeof(Color32))
		{
			Color32 color = (Color32)(object)defaultValue;
			string @string = PlayerPrefs.GetString("console:" + command, color.r + "|" + color.g + "|" + color.b + "|" + color.a);
			string[] array = @string.Split("|"[0]);
			color.r = byte.Parse(array[0]);
			color.g = byte.Parse(array[1]);
			color.b = byte.Parse(array[2]);
			color.a = byte.Parse(array[3]);
			return (T)(object)color;
		}
		return defaultValue;
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

	private void Update()
	{
		if (Input.GetKeyDown(key))
		{
			showGUI = !showGUI;
		}
		if (Input.touchCount == 1)
		{
			Touch touch = Input.touches[0];
			if (touch.phase == TouchPhase.Moved)
			{
				scroll += touch.deltaPosition / dpi;
			}
		}
	}

	private void OnGUI()
	{
		if (!showGUI)
		{
			return;
		}
		GUI.DrawTextureWithTexCoords(rects[0], background, new Rect(0f, 0f, 1f, 1f), true);
		GUILayout.BeginArea(rects[0]);
		scroll = GUILayout.BeginScrollView(scroll);
		GUILayout.Space(2f);
		for (int i = 0; i < ConsoleManager.list.Count; i++)
		{
			GUILayout.Label(ConsoleManager.list[i], style);
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		GUI.changed = false;
		text = GUI.TextField(rects[1], text, 300, styleText);
		if (GUI.changed)
		{
			selectCommandsIndex = 0;
			text = text.Replace("\n", string.Empty);
			commandsContains.Clear();
			if (text != string.Empty)
			{
				for (int j = 0; j < commands.Count; j++)
				{
					if (Contains(text, commands.Keys.ElementAt(j)))
					{
						commandsContains.Add(commands.Keys.ElementAt(j));
					}
				}
			}
		}
		for (int k = 0; k < commandsContains.Count; k++)
		{
			if (GUI.Button(xRect(2.5f, 85 - k * 6, 30f, 6f), commandsContains[k] + " " + Format(commands[commandsContains[k]].value), styleButton2))
			{
				text = commandsContains[k];
				commandsContains.Clear();
				break;
			}
		}
		if (GUI.Button(rects[4], "<<", styleButton))
		{
			selectCommandsIndex--;
			UpdateArrowCommand();
		}
		if (GUI.Button(rects[5], ">>", styleButton))
		{
			selectCommandsIndex++;
			UpdateArrowCommand();
		}
		if (GUI.Button(rects[2], "Submit", styleButton) && !string.IsNullOrEmpty(text))
		{
			text = text.Replace("\n", string.Empty);
			Log(">" + text);
			commandsHistory.Add(text);
			if (text == "list")
			{
				if (commands.Count != 0)
				{
					text = string.Empty;
					Log("/////////////////////////////////////////////////");
					for (int l = 0; l < commands.Count; l++)
					{
						Log(commands.Keys.ElementAt(l));
					}
					Log("/////////////////////////////////////////////////");
				}
			}
			else
			{
				OnCommand();
			}
		}
		if (GUI.Button(rects[3], "Clear", styleButton))
		{
			ConsoleManager.list.Clear();
		}
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

	private Rect xRect(Rect rect)
	{
		return xRect(rect.x, rect.y, rect.width, rect.height);
	}

	private Rect xRect(float x, float y, float width, float height)
	{
		x = (float)Screen.width * x / 100f;
		y = (float)Screen.height * y / 100f;
		width = (float)Screen.width * width / 100f;
		height = (float)Screen.height * height / 100f;
		return new Rect(x, y, width, height);
	}

	private Texture2D CreateTexture2D(Color color)
	{
		Texture2D texture2D = new Texture2D(2, 2);
		for (int i = 0; i < texture2D.width; i++)
		{
			for (int j = 0; j < texture2D.height; j++)
			{
				texture2D.SetPixel(i, j, color);
			}
		}
		texture2D.Apply();
		return texture2D;
	}
}
