using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public static class FirebaseMiniJson
{
	private enum TOKEN
	{
		NONE,
		CURLY_OPEN,
		CURLY_CLOSE,
		SQUARED_OPEN,
		SQUARED_CLOSE,
		COLON,
		COMMA,
		STRING,
		NUMBER,
		TRUE,
		FALSE,
		NULL
	}

	private const int BUILDER_CAPACITY = 2000;

	private static int lastErrorIndex = -1;

	private static string lastDecode;

	public static object Deserialize(string json)
	{
		lastDecode = json;
		if (json != null)
		{
			char[] json2 = json.ToCharArray();
			int index = 0;
			bool success = true;
			object result = ParseValue(json2, ref index, ref success);
			if (success)
			{
				lastErrorIndex = -1;
			}
			else
			{
				lastErrorIndex = index;
			}
			return result;
		}
		return null;
	}

	public static string Serialize(object json)
	{
		StringBuilder stringBuilder = new StringBuilder(2000);
		return (!SerializeValue(json, stringBuilder)) ? null : stringBuilder.ToString();
	}

	public static bool LastDecodeSuccessful()
	{
		return lastErrorIndex == -1;
	}

	public static int GetLastErrorIndex()
	{
		return lastErrorIndex;
	}

	public static string GetLastErrorSnippet()
	{
		if (lastErrorIndex == -1)
		{
			return string.Empty;
		}
		int num = lastErrorIndex - 5;
		int num2 = lastErrorIndex + 15;
		if (num < 0)
		{
			num = 0;
		}
		if (num2 >= lastDecode.Length)
		{
			num2 = lastDecode.Length - 1;
		}
		return lastDecode.Substring(num, num2 - num + 1);
	}

	private static Dictionary<string, object> ParseObject(char[] json, ref int index)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		NextToken(json, ref index);
		while (true)
		{
			switch (LookAhead(json, index))
			{
			case TOKEN.NONE:
				return null;
			case TOKEN.COMMA:
				NextToken(json, ref index);
				continue;
			case TOKEN.CURLY_CLOSE:
				NextToken(json, ref index);
				return dictionary;
			}
			string text = ParseString(json, ref index);
			if (text == null)
			{
				return null;
			}
			TOKEN tOKEN = NextToken(json, ref index);
			if (tOKEN != TOKEN.COLON)
			{
				return null;
			}
			bool success = true;
			object value = ParseValue(json, ref index, ref success);
			if (!success)
			{
				return null;
			}
			dictionary[text] = value;
		}
	}

	private static List<object> ParseArray(char[] json, ref int index)
	{
		List<object> list = new List<object>();
		NextToken(json, ref index);
		while (true)
		{
			switch (LookAhead(json, index))
			{
			case TOKEN.NONE:
				return null;
			case TOKEN.COMMA:
				NextToken(json, ref index);
				continue;
			case TOKEN.SQUARED_CLOSE:
				NextToken(json, ref index);
				return list;
			}
			bool success = true;
			object item = ParseValue(json, ref index, ref success);
			if (!success)
			{
				return null;
			}
			list.Add(item);
		}
	}

	private static object ParseValue(char[] json, ref int index, ref bool success)
	{
		switch (LookAhead(json, index))
		{
		case TOKEN.STRING:
			return ParseString(json, ref index);
		case TOKEN.NUMBER:
			return ParseNumber(json, ref index);
		case TOKEN.CURLY_OPEN:
			return ParseObject(json, ref index);
		case TOKEN.SQUARED_OPEN:
			return ParseArray(json, ref index);
		case TOKEN.TRUE:
			NextToken(json, ref index);
			return true;
		case TOKEN.FALSE:
			NextToken(json, ref index);
			return false;
		case TOKEN.NULL:
			NextToken(json, ref index);
			return null;
		default:
			success = false;
			return null;
		}
	}

	private static string ParseString(char[] json, ref int index)
	{
		StringBuilder stringBuilder = new StringBuilder();
		EatWhitespace(json, ref index);
		char c = json[index++];
		bool flag = false;
		while (!flag && index != json.Length)
		{
			c = json[index++];
			switch (c)
			{
			case '"':
				flag = true;
				break;
			case '\\':
				if (index != json.Length)
				{
					switch (json[index++])
					{
					case '"':
						stringBuilder.Append('"');
						continue;
					case '\\':
						stringBuilder.Append('\\');
						continue;
					case '/':
						stringBuilder.Append('/');
						continue;
					case 'b':
						stringBuilder.Append('\b');
						continue;
					case 'f':
						stringBuilder.Append('\f');
						continue;
					case 'n':
						stringBuilder.Append('\n');
						continue;
					case 'r':
						stringBuilder.Append('\r');
						continue;
					case 't':
						stringBuilder.Append('\t');
						continue;
					case 'u':
						break;
					default:
						continue;
					}
					int num = json.Length - index;
					if (num >= 4)
					{
						char[] array = new char[4];
						Array.Copy(json, index, array, 0, 4);
						stringBuilder.AppendFormat(string.Format("&#x{0};", array));
						index += 4;
						continue;
					}
				}
				break;
			default:
				stringBuilder.Append(c);
				continue;
			}
			break;
		}
		if (!flag)
		{
			return null;
		}
		return stringBuilder.ToString();
	}

	private static object ParseNumber(char[] json, ref int index)
	{
		EatWhitespace(json, ref index);
		int lastIndexOfNumber = GetLastIndexOfNumber(json, index);
		int num = lastIndexOfNumber - index + 1;
		char[] array = new char[num];
		Array.Copy(json, index, array, 0, num);
		index = lastIndexOfNumber + 1;
		string text = new string(array);
		if (text.IndexOf('.') == -1)
		{
			return long.Parse(text);
		}
		return double.Parse(text);
	}

	private static int GetLastIndexOfNumber(char[] json, int index)
	{
		int i;
		for (i = index; i < json.Length && "0123456789+-.eE".IndexOf(json[i]) != -1; i++)
		{
		}
		return i - 1;
	}

	private static void EatWhitespace(char[] json, ref int index)
	{
		while (index < json.Length && " \t\n\r".IndexOf(json[index]) != -1)
		{
			index++;
		}
	}

	private static TOKEN LookAhead(char[] json, int index)
	{
		int index2 = index;
		return NextToken(json, ref index2);
	}

	private static TOKEN NextToken(char[] json, ref int index)
	{
		EatWhitespace(json, ref index);
		if (index == json.Length)
		{
			return TOKEN.NONE;
		}
		char c = json[index];
		index++;
		switch (c)
		{
		case '{':
			return TOKEN.CURLY_OPEN;
		case '}':
			return TOKEN.CURLY_CLOSE;
		case '[':
			return TOKEN.SQUARED_OPEN;
		case ']':
			return TOKEN.SQUARED_CLOSE;
		case ',':
			return TOKEN.COMMA;
		case '"':
			return TOKEN.STRING;
		case '-':
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			return TOKEN.NUMBER;
		case ':':
			return TOKEN.COLON;
		default:
		{
			index--;
			int num = json.Length - index;
			if (num >= 5 && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
			{
				index += 5;
				return TOKEN.FALSE;
			}
			if (num >= 4)
			{
				if (json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
				{
					index += 4;
					return TOKEN.TRUE;
				}
				if (json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
				{
					index += 4;
					return TOKEN.NULL;
				}
			}
			return TOKEN.NONE;
		}
		}
	}

	private static bool SerializeObject(IDictionary anObject, StringBuilder builder)
	{
		bool flag = true;
		builder.Append('{');
		foreach (object key in anObject.Keys)
		{
			if (!flag)
			{
				builder.Append(',');
			}
			SerializeString(key.ToString(), builder);
			builder.Append(':');
			if (!SerializeValue(anObject[key], builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append('}');
		return true;
	}

	private static bool SerializeArray(IList anArray, StringBuilder builder)
	{
		builder.Append('[');
		bool flag = true;
		foreach (object item in anArray)
		{
			if (!flag)
			{
				builder.Append(',');
			}
			if (!SerializeValue(item, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append(']');
		return true;
	}

	private static bool SerializeValue(object value, StringBuilder builder)
	{
		if (value == null)
		{
			builder.Append("null");
		}
		else if (value.GetType().IsArray)
		{
			SerializeArray((IList)value, builder);
		}
		else if (value is string)
		{
			SerializeString((string)value, builder);
		}
		else if (value is char)
		{
			SerializeString(Convert.ToString((char)value), builder);
		}
		else if (value is IDictionary)
		{
			SerializeObject((IDictionary)value, builder);
		}
		else if (value is IList)
		{
			SerializeArray((IList)value, builder);
		}
		else if (value is bool)
		{
			builder.Append((!(bool)value) ? "false" : "true");
		}
		else
		{
			if (!value.GetType().IsPrimitive)
			{
				return false;
			}
			SerializeNumber(Convert.ToDouble(value), builder);
		}
		return true;
	}

	private static void SerializeString(string aString, StringBuilder builder)
	{
		builder.Append('"');
		char[] array = aString.ToCharArray();
		char[] array2 = array;
		foreach (char c in array2)
		{
			switch (c)
			{
			case '"':
				builder.Append("\\\"");
				continue;
			case '\\':
				builder.Append("\\\\");
				continue;
			case '\b':
				builder.Append("\\b");
				continue;
			case '\f':
				builder.Append("\\f");
				continue;
			case '\n':
				builder.Append("\\n");
				continue;
			case '\r':
				builder.Append("\\r");
				continue;
			case '\t':
				builder.Append("\\t");
				continue;
			}
			int num = Convert.ToInt32(c);
			if (num >= 32 && num <= 126)
			{
				builder.Append(c);
			}
			else
			{
				builder.Append("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
			}
		}
		builder.Append('"');
	}

	private static void SerializeNumber(double number, StringBuilder builder)
	{
		builder.Append(number.ToString());
	}
}
