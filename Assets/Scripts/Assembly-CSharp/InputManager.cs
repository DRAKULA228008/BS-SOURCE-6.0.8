using UnityEngine;

public class InputManager : MonoBehaviour
{
	public struct ButtonData
	{
		public string name;

		public KeyCode key;
	}

	public struct AxisData
	{
		public string name;

		public bool raw;
	}

	public delegate void ButtonDelegate(string key);

	public delegate void AxisDelegate(string key, float value);

	private static BetterList<ButtonData> buttons = new BetterList<ButtonData>();

	private static BetterList<AxisData> axis = new BetterList<AxisData>();

	public static ButtonDelegate GetButtonDownEvent;

	public static ButtonDelegate GetButtonEvent;

	public static ButtonDelegate GetButtonUpEvent;

	public static AxisDelegate GetAxisEvent;

	private static InputManager instance;

	private void Start()
	{
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void AddButton(string name, KeyCode key)
	{
		ButtonData item = default(ButtonData);
		item.name = name;
		item.key = key;
		buttons.Add(item);
	}

	private void AddAxis(string name, bool raw)
	{
		AxisData item = default(AxisData);
		item.name = name;
		item.raw = raw;
		axis.Add(item);
	}

	public static void Init()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("InputManager");
			gameObject.AddComponent<InputManager>();
		}
	}

	public static void SetButtonDown(string name)
	{
		if (GetButtonDownEvent != null)
		{
			GetButtonDownEvent(name);
		}
	}

	public static void SetButtonUp(string name)
	{
		if (GetButtonUpEvent != null)
		{
			GetButtonUpEvent(name);
		}
	}

	public static void SetAxis(string name, float value)
	{
		if (GetAxisEvent != null)
		{
			GetAxisEvent(name, value);
		}
	}
}
