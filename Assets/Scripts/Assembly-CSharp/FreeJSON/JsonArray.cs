using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeJSON
{
	public class JsonArray : Json
	{
		private class Parser
		{
			private static Stack<List<string>> splitArrayPool = new Stack<List<string>>();

			private static JsonArray jsonArray = new JsonArray();

			private static StringBuilder stringBuilder = new StringBuilder();

			private static int id = 0;

			public static JsonArray Parse(string json)
			{
				for (int i = 0; i < json.Length; i++)
				{
					char c = json[i];
					if (c == '"')
					{
						i = AppendUntilStringEnd(true, i, json);
					}
					else if (!char.IsWhiteSpace(c))
					{
						stringBuilder.Append(c);
					}
				}
				List<string> list = Split(json);
				jsonArray = new JsonArray();
				for (int j = 0; j < list.Count; j++)
				{
					if (!string.IsNullOrEmpty(list[j]))
					{
						id++;
						jsonArray.values.Add(id.ToString(), list[j]);
					}
				}
				return jsonArray;
			}

			private static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json)
			{
				stringBuilder.Append(json[startIdx]);
				for (int i = startIdx + 1; i < json.Length; i++)
				{
					if (json[i] == '\\')
					{
						if (appendEscapeCharacter)
						{
							stringBuilder.Append(json[i]);
						}
						stringBuilder.Append(json[i + 1]);
						i++;
					}
					else
					{
						if (json[i] == '"')
						{
							stringBuilder.Append(json[i]);
							return i;
						}
						stringBuilder.Append(json[i]);
					}
				}
				return json.Length - 1;
			}

			private static List<string> Split(string json)
			{
				List<string> list = ((splitArrayPool.Count <= 0) ? new List<string>() : splitArrayPool.Pop());
				list.Clear();
				int num = 0;
				stringBuilder.Length = 0;
				for (int i = 1; i < json.Length - 1; i++)
				{
					switch (json[i])
					{
					case '[':
					case '{':
						num++;
						break;
					case ']':
					case '}':
						num--;
						break;
					case '"':
						i = AppendUntilStringEnd(true, i, json);
						continue;
					case ',':
					case ':':
						if (num == 0)
						{
							list.Add(stringBuilder.ToString());
							stringBuilder.Length = 0;
							continue;
						}
						break;
					}
					stringBuilder.Append(json[i]);
				}
				list.Add(stringBuilder.ToString());
				return list;
			}
		}

		private int id;

		public bool HasValue(int index)
		{
			if (values.Count - 1 >= index && values.Values.ElementAt(index) != string.Empty && values.Values.ElementAt(index) != "null")
			{
				return true;
			}
			return false;
		}

		public bool Contains(object value)
		{
			string value2 = ConvertToString(value);
			if (string.IsNullOrEmpty(value2))
			{
				Debug.LogWarning("Json does not support this type");
				return false;
			}
			return values.ContainsValue(value2);
		}

		public void RemoveAt(int index)
		{
			values.Remove(values.Keys.ElementAt(index));
		}

		public bool Remove(object value)
		{
			string text = ConvertToString(value);
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogWarning("Json does not support this type");
				return false;
			}
			bool result = false;
			foreach (KeyValuePair<string, string> value2 in values)
			{
				if (value2.Value == text)
				{
					values.Remove(value2.Key);
					result = true;
				}
			}
			return result;
		}

		public void Add(object value)
		{
			id++;
			AddData(id.ToString(), value);
		}

		public T Get<T>(int index)
		{
			return (T)Get(index, typeof(T));
		}

		public object Get(int index, Type type)
		{
			return GetData(values.Keys.ElementAt(index), type);
		}

		public static JsonArray Parse(string jsonString)
		{
			return Parser.Parse(jsonString);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			foreach (KeyValuePair<string, string> value in values)
			{
				stringBuilder.Append(value.Value);
				stringBuilder.Append(',');
			}
			if (values.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}
	}
}
