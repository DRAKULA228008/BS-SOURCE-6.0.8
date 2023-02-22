using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEngine;

public sealed class vp_ComponentPreset
{
	private enum ReadMode
	{
		Normal,
		LineComment,
		BlockComment
	}

	private class Field
	{
		public RuntimeFieldHandle FieldHandle;

		public object Args;

		public Field(RuntimeFieldHandle fieldHandle, object args)
		{
			FieldHandle = fieldHandle;
			Args = args;
		}
	}

	private static string m_FullPath;

	private static int m_LineNumber;

	private static Type m_Type;

	public static bool LogErrors = true;

	private static ReadMode m_ReadMode;

	private Type m_ComponentType;

	private List<Field> m_Fields = new List<Field>();

	public Type ComponentType
	{
		get
		{
			return m_ComponentType;
		}
	}

	public static string Save(Component component, string fullPath)
	{
		vp_ComponentPreset vp_ComponentPreset2 = new vp_ComponentPreset();
		vp_ComponentPreset2.InitFromComponent(component);
		return Save(vp_ComponentPreset2, fullPath);
	}

	public static string Save(vp_ComponentPreset savePreset, string fullPath, bool isDifference = false)
	{
		m_FullPath = fullPath;
		bool logErrors = LogErrors;
		LogErrors = false;
		vp_ComponentPreset vp_ComponentPreset2 = new vp_ComponentPreset();
		vp_ComponentPreset2.LoadTextStream(m_FullPath);
		LogErrors = logErrors;
		if (vp_ComponentPreset2 != null)
		{
			if (vp_ComponentPreset2.m_ComponentType != null)
			{
				if (vp_ComponentPreset2.ComponentType != savePreset.ComponentType)
				{
					return "'" + ExtractFilenameFromPath(m_FullPath) + "' has the WRONG component type: " + vp_ComponentPreset2.ComponentType.ToString() + ".\n\nDo you want to replace it with a " + savePreset.ComponentType.ToString() + "?";
				}
				if (File.Exists(m_FullPath))
				{
					if (isDifference)
					{
						return "This will update '" + ExtractFilenameFromPath(m_FullPath) + "' with only the values modified since pressing Play or setting a state.\n\nContinue?";
					}
					return "'" + ExtractFilenameFromPath(m_FullPath) + "' already exists.\n\nDo you want to replace it?";
				}
			}
			if (File.Exists(m_FullPath))
			{
				return "'" + ExtractFilenameFromPath(m_FullPath) + "' has an UNKNOWN component type.\n\nDo you want to replace it?";
			}
		}
		ClearTextFile();
		Append("///////////////////////////////////////////////////////////");
		Append("// Component Preset Script");
		Append("///////////////////////////////////////////////////////////\n");
		Append("ComponentType " + savePreset.ComponentType.Name);
		foreach (Field field in savePreset.m_Fields)
		{
			string text = string.Empty;
			string empty = string.Empty;
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(field.FieldHandle);
			if (fieldFromHandle.FieldType == typeof(float) || fieldFromHandle.FieldType == typeof(CryptoFloat))
			{
				empty = string.Format("{0:0.#######}", (float)field.Args);
			}
			else if (fieldFromHandle.FieldType == typeof(Vector4) || fieldFromHandle.FieldType == typeof(CryptoVector4))
			{
				Vector4 vector = (Vector4)field.Args;
				empty = string.Format("{0:0.#######}", vector.x) + " " + string.Format("{0:0.#######}", vector.y) + " " + string.Format("{0:0.#######}", vector.z) + " " + string.Format("{0:0.#######}", vector.w);
			}
			else if (fieldFromHandle.FieldType == typeof(Vector3) || fieldFromHandle.FieldType == typeof(CryptoVector3))
			{
				Vector3 vector2 = (Vector3)field.Args;
				empty = string.Format("{0:0.#######}", vector2.x) + " " + string.Format("{0:0.#######}", vector2.y) + " " + string.Format("{0:0.#######}", vector2.z);
			}
			else if (fieldFromHandle.FieldType == typeof(Vector2) || fieldFromHandle.FieldType == typeof(CryptoVector2))
			{
				Vector2 vector3 = (Vector2)field.Args;
				empty = string.Format("{0:0.#######}", vector3.x) + " " + string.Format("{0:0.#######}", vector3.y);
			}
			else if (fieldFromHandle.FieldType == typeof(int) || fieldFromHandle.FieldType == typeof(CryptoInt))
			{
				empty = ((int)field.Args).ToString();
			}
			else if (fieldFromHandle.FieldType == typeof(bool) || fieldFromHandle.FieldType == typeof(CryptoBool))
			{
				empty = ((bool)field.Args).ToString();
			}
			else if (fieldFromHandle.FieldType == typeof(string) || fieldFromHandle.FieldType == typeof(CryptoString))
			{
				empty = (string)field.Args;
			}
			else
			{
				text = "//";
				empty = "<NOTE: Type '" + fieldFromHandle.FieldType.Name.ToString() + "' can't be saved to preset.>";
			}
			if (!string.IsNullOrEmpty(empty) && fieldFromHandle.Name != "Persist")
			{
				Append(text + fieldFromHandle.Name + " " + empty);
			}
		}
		return null;
	}

	public static string SaveDifference(vp_ComponentPreset initialStatePreset, Component modifiedComponent, string fullPath, vp_ComponentPreset diskPreset)
	{
		if (initialStatePreset.ComponentType != modifiedComponent.GetType())
		{
			Error("Tried to save difference between different type components in 'SaveDifference'");
			return null;
		}
		vp_ComponentPreset vp_ComponentPreset2 = new vp_ComponentPreset();
		vp_ComponentPreset2.InitFromComponent(modifiedComponent);
		vp_ComponentPreset vp_ComponentPreset3 = new vp_ComponentPreset();
		vp_ComponentPreset3.m_ComponentType = vp_ComponentPreset2.ComponentType;
		for (int i = 0; i < vp_ComponentPreset2.m_Fields.Count; i++)
		{
			if (!initialStatePreset.m_Fields[i].Args.Equals(vp_ComponentPreset2.m_Fields[i].Args))
			{
				vp_ComponentPreset3.m_Fields.Add(vp_ComponentPreset2.m_Fields[i]);
			}
		}
		foreach (Field field in diskPreset.m_Fields)
		{
			bool flag = true;
			foreach (Field field2 in vp_ComponentPreset3.m_Fields)
			{
				if (field.FieldHandle == field2.FieldHandle)
				{
					flag = false;
				}
			}
			bool flag2 = false;
			foreach (Field field3 in vp_ComponentPreset2.m_Fields)
			{
				if (field.FieldHandle == field3.FieldHandle)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				flag = false;
			}
			if (flag)
			{
				vp_ComponentPreset3.m_Fields.Add(field);
			}
		}
		return Save(vp_ComponentPreset3, fullPath, true);
	}

	public void InitFromComponent(Component component)
	{
		m_ComponentType = component.GetType();
		m_Fields.Clear();
		FieldInfo[] fields = component.GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.IsPublic && (fieldInfo.FieldType == typeof(float) || fieldInfo.FieldType == typeof(Vector4) || fieldInfo.FieldType == typeof(Vector3) || fieldInfo.FieldType == typeof(Vector2) || fieldInfo.FieldType == typeof(int) || fieldInfo.FieldType == typeof(bool) || fieldInfo.FieldType == typeof(string) || fieldInfo.FieldType == typeof(CryptoInt) || fieldInfo.FieldType == typeof(CryptoBool) || fieldInfo.FieldType == typeof(CryptoFloat) || fieldInfo.FieldType == typeof(CryptoString) || fieldInfo.FieldType == typeof(CryptoVector2) || fieldInfo.FieldType == typeof(CryptoVector3) || fieldInfo.FieldType == typeof(CryptoVector4)))
			{
				m_Fields.Add(new Field(fieldInfo.FieldHandle, fieldInfo.GetValue(component)));
			}
		}
	}

	public static vp_ComponentPreset CreateFromComponent(Component component)
	{
		vp_ComponentPreset vp_ComponentPreset2 = new vp_ComponentPreset();
		vp_ComponentPreset2.m_ComponentType = component.GetType();
		FieldInfo[] fields = component.GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.IsPublic && (fieldInfo.FieldType == typeof(float) || fieldInfo.FieldType == typeof(Vector4) || fieldInfo.FieldType == typeof(Vector3) || fieldInfo.FieldType == typeof(Vector2) || fieldInfo.FieldType == typeof(int) || fieldInfo.FieldType == typeof(bool) || fieldInfo.FieldType == typeof(string) || fieldInfo.FieldType == typeof(CryptoInt) || fieldInfo.FieldType == typeof(CryptoBool) || fieldInfo.FieldType == typeof(CryptoFloat) || fieldInfo.FieldType == typeof(CryptoString) || fieldInfo.FieldType == typeof(CryptoVector2) || fieldInfo.FieldType == typeof(CryptoVector3) || fieldInfo.FieldType == typeof(CryptoVector4)))
			{
				vp_ComponentPreset2.m_Fields.Add(new Field(fieldInfo.FieldHandle, fieldInfo.GetValue(component)));
			}
		}
		return vp_ComponentPreset2;
	}

	public bool LoadTextStream(string fullPath)
	{
		m_FullPath = fullPath;
		FileInfo fileInfo = null;
		TextReader textReader = null;
		fileInfo = new FileInfo(m_FullPath);
		if (fileInfo != null && fileInfo.Exists)
		{
			textReader = fileInfo.OpenText();
			List<string> list = new List<string>();
			string item;
			while ((item = textReader.ReadLine()) != null)
			{
				list.Add(item);
			}
			textReader.Close();
			if (list == null)
			{
				Error("Preset is empty. '" + m_FullPath + "'");
				return false;
			}
			ParseLines(list);
			return true;
		}
		Error("Failed to read file. '" + m_FullPath + "'");
		return false;
	}

	public static bool Load(Component component, string fullPath)
	{
		vp_ComponentPreset vp_ComponentPreset2 = new vp_ComponentPreset();
		vp_ComponentPreset2.LoadTextStream(fullPath);
		return Apply(component, vp_ComponentPreset2);
	}

	public bool LoadFromResources(string resourcePath)
	{
		m_FullPath = resourcePath;
		TextAsset textAsset = Resources.Load(m_FullPath) as TextAsset;
		if (textAsset == null)
		{
			Error("Failed to read file. '" + m_FullPath + "'");
			return false;
		}
		return LoadFromTextAsset(textAsset);
	}

	public static vp_ComponentPreset LoadFromResources(Component component, string resourcePath)
	{
		vp_ComponentPreset vp_ComponentPreset2 = new vp_ComponentPreset();
		vp_ComponentPreset2.LoadFromResources(resourcePath);
		Apply(component, vp_ComponentPreset2);
		return vp_ComponentPreset2;
	}

	public bool LoadFromTextAsset(TextAsset file)
	{
		m_FullPath = file.name;
		List<string> list = new List<string>();
		string[] array = file.text.Split('\n');
		string[] array2 = array;
		foreach (string item in array2)
		{
			list.Add(item);
		}
		if (list == null)
		{
			Error("Preset is empty. '" + m_FullPath + "'");
			return false;
		}
		ParseLines(list);
		return true;
	}

	public static vp_ComponentPreset LoadFromTextAsset(Component component, TextAsset file)
	{
		vp_ComponentPreset vp_ComponentPreset2 = new vp_ComponentPreset();
		vp_ComponentPreset2.LoadFromTextAsset(file);
		Apply(component, vp_ComponentPreset2);
		return vp_ComponentPreset2;
	}

	private static void Append(string str)
	{
		str = str.Replace("\n", Environment.NewLine);
		StreamWriter streamWriter = null;
		try
		{
			streamWriter = new StreamWriter(m_FullPath, true);
			streamWriter.WriteLine(str);
			if (streamWriter != null)
			{
				streamWriter.Close();
			}
		}
		catch
		{
			Error("Failed to write to file: '" + m_FullPath + "'");
		}
		if (streamWriter != null)
		{
			streamWriter.Close();
		}
	}

	private static void ClearTextFile()
	{
		StreamWriter streamWriter = null;
		try
		{
			streamWriter = new StreamWriter(m_FullPath, false);
			if (streamWriter != null)
			{
				streamWriter.Close();
			}
		}
		catch
		{
			Error("Failed to clear file: '" + m_FullPath + "'");
		}
		if (streamWriter != null)
		{
			streamWriter.Close();
		}
	}

	private void ParseLines(List<string> lines)
	{
		m_LineNumber = 0;
		foreach (string line in lines)
		{
			m_LineNumber++;
			string text = RemoveComments(line);
			if (string.IsNullOrEmpty(text) || Parse(text))
			{
				continue;
			}
			return;
		}
		m_LineNumber = 0;
	}

	private bool Parse(string line)
	{
		line = line.Trim();
		if (string.IsNullOrEmpty(line))
		{
			return true;
		}
		string[] array = line.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim();
		}
		if (m_ComponentType == null)
		{
			if (array[0] == "ComponentType" && array.Length == 2)
			{
				m_Type = Type.GetType(array[1]);
				if (m_Type == null)
				{
					PresetError("No such ComponentType: '" + array[1] + "'");
					return false;
				}
				m_ComponentType = m_Type;
				return true;
			}
			PresetError("Unknown ComponentType.");
			return false;
		}
		FieldInfo fieldInfo = null;
		FieldInfo[] fields = m_Type.GetFields();
		foreach (FieldInfo fieldInfo2 in fields)
		{
			if (fieldInfo2.Name == array[0])
			{
				fieldInfo = fieldInfo2;
			}
		}
		if (fieldInfo == null)
		{
			if (array[0] != "ComponentType")
			{
				PresetError("'" + m_Type.Name + "' has no such field: '" + array[0] + "'");
			}
			return true;
		}
		Field item = new Field(fieldInfo.FieldHandle, TokensToObject(fieldInfo, array));
		m_Fields.Add(item);
		return true;
	}

	public static bool Apply(Component component, vp_ComponentPreset preset)
	{
		if (preset == null)
		{
			Error("Tried to apply a preset that was null in '" + vp_Utility.GetErrorLocation() + "'");
			return false;
		}
		if (preset.m_ComponentType == null)
		{
			Error("Preset ComponentType was null in '" + vp_Utility.GetErrorLocation() + "'");
			return false;
		}
		if (component == null)
		{
			Error("Component was null when attempting to apply preset in '" + vp_Utility.GetErrorLocation() + "'");
			return false;
		}
		if (component.GetType() != preset.m_ComponentType)
		{
			string text = string.Concat("a '", preset.m_ComponentType, "' preset");
			if (preset.m_ComponentType == null)
			{
				text = "an unknown preset type";
			}
			Error("Tried to apply " + text + " to a '" + component.GetType().ToString() + "' component in '" + vp_Utility.GetErrorLocation() + "'");
			return false;
		}
		foreach (Field field in preset.m_Fields)
		{
			FieldInfo[] fields = component.GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.FieldHandle == field.FieldHandle)
				{
					fieldInfo.SetValue(component, field.Args);
				}
			}
		}
		return true;
	}

	public static Type GetFileType(string fullPath)
	{
		bool logErrors = LogErrors;
		LogErrors = false;
		vp_ComponentPreset vp_ComponentPreset2 = new vp_ComponentPreset();
		vp_ComponentPreset2.LoadTextStream(fullPath);
		LogErrors = logErrors;
		if (vp_ComponentPreset2 != null && vp_ComponentPreset2.m_ComponentType != null)
		{
			return vp_ComponentPreset2.m_ComponentType;
		}
		return null;
	}

	public static Type GetFileTypeFromAsset(TextAsset asset)
	{
		bool logErrors = LogErrors;
		LogErrors = false;
		vp_ComponentPreset vp_ComponentPreset2 = new vp_ComponentPreset();
		vp_ComponentPreset2.LoadFromTextAsset(asset);
		LogErrors = logErrors;
		if (vp_ComponentPreset2 != null && vp_ComponentPreset2.m_ComponentType != null)
		{
			return vp_ComponentPreset2.m_ComponentType;
		}
		return null;
	}

	private static object TokensToObject(FieldInfo field, string[] tokens)
	{
		if (field.FieldType == typeof(CryptoFloat))
		{
			return (CryptoFloat)ArgsToFloat(tokens);
		}
		if (field.FieldType == typeof(CryptoVector4))
		{
			return (CryptoVector4)ArgsToVector4(tokens);
		}
		if (field.FieldType == typeof(CryptoVector3))
		{
			return (CryptoVector3)ArgsToVector3(tokens);
		}
		if (field.FieldType == typeof(CryptoVector2))
		{
			return (CryptoVector2)ArgsToVector2(tokens);
		}
		if (field.FieldType == typeof(CryptoInt))
		{
			return (CryptoInt)ArgsToInt(tokens);
		}
		if (field.FieldType == typeof(CryptoBool))
		{
			return (CryptoBool)ArgsToBool(tokens);
		}
		if (field.FieldType == typeof(CryptoString))
		{
			return (CryptoString)ArgsToString(tokens);
		}
		if (field.FieldType == typeof(float))
		{
			return ArgsToFloat(tokens);
		}
		if (field.FieldType == typeof(Vector4))
		{
			return ArgsToVector4(tokens);
		}
		if (field.FieldType == typeof(Vector3))
		{
			return ArgsToVector3(tokens);
		}
		if (field.FieldType == typeof(Vector2))
		{
			return ArgsToVector2(tokens);
		}
		if (field.FieldType == typeof(int))
		{
			return ArgsToInt(tokens);
		}
		if (field.FieldType == typeof(bool))
		{
			return ArgsToBool(tokens);
		}
		if (field.FieldType == typeof(string) || field.FieldType == typeof(CryptoString))
		{
			return ArgsToString(tokens);
		}
		return null;
	}

	private static string RemoveComments(string str)
	{
		string text = string.Empty;
		for (int i = 0; i < str.Length; i++)
		{
			switch (m_ReadMode)
			{
			case ReadMode.Normal:
				if (str[i] == '/' && str[i + 1] == '*')
				{
					m_ReadMode = ReadMode.BlockComment;
					i++;
				}
				else if (str[i] == '/' && str[i + 1] == '/')
				{
					m_ReadMode = ReadMode.LineComment;
					i++;
				}
				else
				{
					text += str[i];
				}
				break;
			case ReadMode.LineComment:
				if (i == str.Length - 1)
				{
					m_ReadMode = ReadMode.Normal;
				}
				break;
			case ReadMode.BlockComment:
				if (str[i] == '*' && str[i + 1] == '/')
				{
					m_ReadMode = ReadMode.Normal;
					i++;
				}
				break;
			}
		}
		return text;
	}

	private static Vector4 ArgsToVector4(string[] args)
	{
		//Discarded unreachable code: IL_00c5
		if (args.Length - 1 != 4)
		{
			PresetError("Wrong number of fields for '" + args[0] + "'");
			return Vector4.zero;
		}
		try
		{
			return new Vector4(Convert.ToSingle(args[1], CultureInfo.InvariantCulture), Convert.ToSingle(args[2], CultureInfo.InvariantCulture), Convert.ToSingle(args[3], CultureInfo.InvariantCulture), Convert.ToSingle(args[4], CultureInfo.InvariantCulture));
		}
		catch
		{
			PresetError("Illegal value: '" + args[1] + ", " + args[2] + ", " + args[3] + ", " + args[4] + "'");
			return Vector4.zero;
		}
	}

	private static Vector3 ArgsToVector3(string[] args)
	{
		//Discarded unreachable code: IL_00a9
		if (args.Length - 1 != 3)
		{
			PresetError("Wrong number of fields for '" + args[0] + "'");
			return Vector3.zero;
		}
		try
		{
			return new Vector3(Convert.ToSingle(args[1], CultureInfo.InvariantCulture), Convert.ToSingle(args[2], CultureInfo.InvariantCulture), Convert.ToSingle(args[3], CultureInfo.InvariantCulture));
		}
		catch
		{
			PresetError("Illegal value: '" + args[1] + ", " + args[2] + ", " + args[3] + "'");
			return Vector3.zero;
		}
	}

	private static Vector2 ArgsToVector2(string[] args)
	{
		//Discarded unreachable code: IL_008e
		if (args.Length - 1 != 2)
		{
			PresetError("Wrong number of fields for '" + args[0] + "'");
			return Vector2.zero;
		}
		try
		{
			return new Vector2(Convert.ToSingle(args[1], CultureInfo.InvariantCulture), Convert.ToSingle(args[2], CultureInfo.InvariantCulture));
		}
		catch
		{
			PresetError("Illegal value: '" + args[1] + ", " + args[2] + "'");
			return Vector2.zero;
		}
	}

	private static float ArgsToFloat(string[] args)
	{
		//Discarded unreachable code: IL_005e
		if (args.Length - 1 != 1)
		{
			PresetError("Wrong number of fields for '" + args[0] + "'");
			return 0f;
		}
		try
		{
			return Convert.ToSingle(args[1], CultureInfo.InvariantCulture);
		}
		catch
		{
			PresetError("Illegal value: '" + args[1] + "'");
			return 0f;
		}
	}

	private static int ArgsToInt(string[] args)
	{
		//Discarded unreachable code: IL_0056
		if (args.Length - 1 != 1)
		{
			PresetError("Wrong number of fields for '" + args[0] + "'");
			return 0;
		}
		try
		{
			return Convert.ToInt32(args[1], CultureInfo.InvariantCulture);
		}
		catch
		{
			PresetError("Illegal value: '" + args[1] + "'");
			return 0;
		}
	}

	private static bool ArgsToBool(string[] args)
	{
		if (args.Length - 1 != 1)
		{
			PresetError("Wrong number of fields for '" + args[0] + "'");
			return false;
		}
		if (args[1].ToLower() == "true")
		{
			return true;
		}
		if (args[1].ToLower() == "false")
		{
			return false;
		}
		PresetError("Illegal value: '" + args[1] + "'");
		return false;
	}

	private static string ArgsToString(string[] args)
	{
		string text = string.Empty;
		for (int i = 1; i < args.Length; i++)
		{
			text += args[i];
			if (i < args.Length - 1)
			{
				text += " ";
			}
		}
		return text;
	}

	public Type GetFieldType(string fieldName)
	{
		Type result = null;
		foreach (Field field in m_Fields)
		{
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(field.FieldHandle);
			if (fieldFromHandle.Name == fieldName)
			{
				result = fieldFromHandle.FieldType;
			}
		}
		return result;
	}

	public object GetFieldValue(string fieldName)
	{
		object result = null;
		foreach (Field field in m_Fields)
		{
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(field.FieldHandle);
			if (fieldFromHandle.Name == fieldName)
			{
				result = field.Args;
			}
		}
		return result;
	}

	public static string ExtractFilenameFromPath(string path)
	{
		int num = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
		if (num == -1)
		{
			return path;
		}
		if (num == path.Length - 1)
		{
			return string.Empty;
		}
		return path.Substring(num + 1, path.Length - num - 1);
	}

	private static void PresetError(string message)
	{
		if (LogErrors)
		{
			Debug.LogError("Preset Error: " + m_FullPath + " (at " + m_LineNumber + ") " + message);
		}
	}

	private static void Error(string message)
	{
		if (LogErrors)
		{
			Debug.LogError("Error: " + message);
		}
	}
}
