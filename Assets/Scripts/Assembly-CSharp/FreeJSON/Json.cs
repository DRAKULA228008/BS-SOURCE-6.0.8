using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FreeJSON
{
	public class Json
	{
		public static List<JsonConverter> Converters = new List<JsonConverter>();

		protected Dictionary<string, string> values = new Dictionary<string, string>();

		public int Length
		{
			get
			{
				return values.Count;
			}
		}

		private bool ContainsKey(string key)
		{
			return values.ContainsKey(key);
		}

		protected void AddData(string key, object value)
		{
			string value2 = ConvertToString(value);
			if (string.IsNullOrEmpty(value2))
			{
				Debug.LogWarning("Json does not support this type: " + value.GetType().Name);
			}
			else
			{
				values[key] = value2;
			}
		}

		protected T GetData<T>(string key)
		{
			return (T)GetData(key, typeof(T));
		}

		protected object GetData(string key, Type type)
		{
			return ConvertToType(key, type);
		}

		protected string ConvertToString(object value)
		{
			if (value == null)
			{
				return "null";
			}
			for (int i = 0; i < Converters.Count; i++)
			{
				if (Converters[i].CanConvert(value.GetType()))
				{
					return Converters[i].Serialize(value);
				}
			}
			if (value is byte || value is sbyte)
			{
				return value.ToString();
			}
			if (value is short || value is ushort)
			{
				return value.ToString();
			}
			if (value is int || value is uint)
			{
				return value.ToString();
			}
			if (value is long || value is ulong)
			{
				return value.ToString();
			}
			if (value is float)
			{
				return value.ToString();
			}
			if (value is double)
			{
				return value.ToString();
			}
			if (value is decimal)
			{
				return value.ToString();
			}
			if (value is char)
			{
				return "\"" + value.ToString() + "\"";
			}
			if (value is string)
			{
				return "\"" + value.ToString() + "\"";
			}
			if (value is bool)
			{
				return value.ToString().ToLower();
			}
			if (value is Enum)
			{
				return value.ToString();
			}
			if (value is Vector2)
			{
				JsonObject jsonObject = new JsonObject();
				Vector2 vector = (Vector2)value;
				jsonObject.Add("x", vector.x);
				jsonObject.Add("y", vector.y);
				return jsonObject.ToString();
			}
			if (value is Vector3)
			{
				JsonObject jsonObject2 = new JsonObject();
				Vector3 vector2 = (Vector3)value;
				jsonObject2.Add("x", vector2.x);
				jsonObject2.Add("y", vector2.y);
				jsonObject2.Add("z", vector2.z);
				return jsonObject2.ToString();
			}
			if (value is Vector4)
			{
				JsonObject jsonObject3 = new JsonObject();
				Vector4 vector3 = (Vector4)value;
				jsonObject3.Add("x", vector3.x);
				jsonObject3.Add("y", vector3.y);
				jsonObject3.Add("z", vector3.z);
				jsonObject3.Add("w", vector3.w);
				return jsonObject3.ToString();
			}
			if (value is Quaternion)
			{
				JsonObject jsonObject4 = new JsonObject();
				Quaternion quaternion = (Quaternion)value;
				jsonObject4.Add("x", quaternion.x);
				jsonObject4.Add("y", quaternion.y);
				jsonObject4.Add("z", quaternion.z);
				jsonObject4.Add("w", quaternion.w);
				return jsonObject4.ToString();
			}
			if (value is Color)
			{
				JsonObject jsonObject5 = new JsonObject();
				Color color = (Color)value;
				jsonObject5.Add("r", color.r);
				jsonObject5.Add("g", color.g);
				jsonObject5.Add("b", color.b);
				jsonObject5.Add("a", color.a);
				return jsonObject5.ToString();
			}
			if (value is Color32)
			{
				JsonObject jsonObject6 = new JsonObject();
				Color32 color2 = (Color32)value;
				jsonObject6.Add("r", color2.r);
				jsonObject6.Add("g", color2.g);
				jsonObject6.Add("b", color2.b);
				jsonObject6.Add("a", color2.a);
				return jsonObject6.ToString();
			}
			if (value is Rect)
			{
				JsonObject jsonObject7 = new JsonObject();
				Rect rect = (Rect)value;
				jsonObject7.Add("x", rect.x);
				jsonObject7.Add("y", rect.y);
				jsonObject7.Add("width", rect.width);
				jsonObject7.Add("height", rect.height);
				return jsonObject7.ToString();
			}
			if (value is IList)
			{
				JsonArray jsonArray = new JsonArray();
				IList list = value as IList;
				for (int j = 0; j < list.Count; j++)
				{
					jsonArray.Add(list[j]);
				}
				return jsonArray.ToString();
			}
			if (value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(Dictionary<, >))
			{
				if (value.GetType().GetGenericArguments()[0] != typeof(string))
				{
					Debug.LogWarning("Json does not support this type");
					return "{}";
				}
				JsonObject jsonObject8 = new JsonObject();
				IDictionary dictionary = value as IDictionary;
				foreach (object key in dictionary.Keys)
				{
					jsonObject8.Add((string)key, dictionary[key]);
				}
				return jsonObject8.ToString();
			}
			if (value is JsonArray)
			{
				return value.ToString();
			}
			if (value is JsonObject)
			{
				return value.ToString();
			}
			if (value.GetType().IsClass)
			{
				return JsonConvert.Serialize(value);
			}
			if (value is CryptoInt)
			{
				return value.ToString();
			}
			if (value is CryptoInt2)
			{
				return value.ToString();
			}
			if (value is CryptoFloat)
			{
				return value.ToString();
			}
			if (value is CryptoString)
			{
				return "\"" + value.ToString() + "\"";
			}
			if (value is CryptoBool)
			{
				return value.ToString().ToLower();
			}
			return string.Empty;
		}

		protected object ConvertToType(string key, Type type)
		{
			for (int i = 0; i < Converters.Count; i++)
			{
				if (Converters[i].CanConvert(type))
				{
					if (ContainsKey(key))
					{
						return Converters[i].Deserialize(type, values[key]);
					}
					return null;
				}
			}
			if (type == typeof(byte))
			{
				byte result = 0;
				if (ContainsKey(key))
				{
					byte.TryParse(values[key], out result);
				}
				return result;
			}
			if (type == typeof(sbyte))
			{
				sbyte result2 = 0;
				if (ContainsKey(key))
				{
					sbyte.TryParse(values[key], out result2);
				}
				return result2;
			}
			if (type == typeof(short))
			{
				short result3 = 0;
				if (ContainsKey(key))
				{
					short.TryParse(values[key], out result3);
				}
				return result3;
			}
			if (type == typeof(ushort))
			{
				ushort result4 = 0;
				if (ContainsKey(key))
				{
					ushort.TryParse(values[key], out result4);
				}
				return result4;
			}
			if (type == typeof(int))
			{
				int result5 = 0;
				if (ContainsKey(key))
				{
					int.TryParse(values[key], out result5);
				}
				return result5;
			}
			if (type == typeof(uint))
			{
				uint result6 = 0u;
				if (ContainsKey(key))
				{
					uint.TryParse(values[key], out result6);
				}
				return result6;
			}
			if (type == typeof(long))
			{
				long result7 = 0L;
				if (ContainsKey(key))
				{
					string text = values[key];
					text = text.Replace("\"", string.Empty);
					long.TryParse(text, out result7);
				}
				return result7;
			}
			if (type == typeof(ulong))
			{
				ulong result8 = 0uL;
				if (ContainsKey(key))
				{
					ulong.TryParse(values[key], out result8);
				}
				return result8;
			}
			if (type == typeof(float))
			{
				float result9 = 0f;
				if (ContainsKey(key))
				{
					float.TryParse(values[key], out result9);
				}
				return result9;
			}
			if (type == typeof(double))
			{
				double result10 = 0.0;
				if (ContainsKey(key))
				{
					double.TryParse(values[key], out result10);
				}
				return result10;
			}
			if (type == typeof(decimal))
			{
				decimal result11 = 0m;
				if (ContainsKey(key))
				{
					decimal.TryParse(values[key], out result11);
				}
				return result11;
			}
			if (type == typeof(char))
			{
				string empty = string.Empty;
				char result12 = '\0';
				if (ContainsKey(key))
				{
					empty = values[key];
					if (empty[0] == '"')
					{
						empty = empty.Remove(0, 1);
					}
					if (empty[empty.Length - 1] == '"')
					{
						empty = empty.Remove(empty.Length - 1, 1);
					}
					char.TryParse(empty, out result12);
				}
				return result12;
			}
			if (type == typeof(string))
			{
				string text2 = string.Empty;
				if (ContainsKey(key))
				{
					text2 = values[key];
					if (text2[0] == '"')
					{
						text2 = text2.Remove(0, 1);
					}
					if (text2[text2.Length - 1] == '"')
					{
						text2 = text2.Remove(text2.Length - 1, 1);
					}
				}
				return text2;
			}
			if (type == typeof(bool))
			{
				bool result13 = false;
				if (ContainsKey(key))
				{
					if (values[key] == "1")
					{
						result13 = true;
					}
					else
					{
						bool.TryParse(values[key], out result13);
					}
				}
				return result13;
			}
			if (type.IsEnum)
			{
				if (ContainsKey(key))
				{
					return (Enum)Enum.Parse(type, GetData<string>(key));
				}
				return (Enum)Enum.ToObject(type, 0);
			}
			if (type == typeof(Vector2))
			{
				Vector2 vector = default(Vector2);
				if (ContainsKey(key))
				{
					JsonObject data = GetData<JsonObject>(key);
					vector.x = data.Get<float>("x");
					vector.y = data.Get<float>("y");
				}
				return vector;
			}
			if (type == typeof(Vector3))
			{
				Vector3 vector2 = default(Vector3);
				if (ContainsKey(key))
				{
					JsonObject data2 = GetData<JsonObject>(key);
					vector2.x = data2.Get<float>("x");
					vector2.y = data2.Get<float>("y");
					vector2.z = data2.Get<float>("z");
				}
				return vector2;
			}
			if (type == typeof(Vector4))
			{
				Vector4 vector3 = default(Vector4);
				if (ContainsKey(key))
				{
					JsonObject data3 = GetData<JsonObject>(key);
					vector3.x = data3.Get<float>("x");
					vector3.y = data3.Get<float>("y");
					vector3.z = data3.Get<float>("z");
					vector3.w = data3.Get<float>("w");
				}
				return vector3;
			}
			if (type == typeof(Quaternion))
			{
				Quaternion quaternion = default(Quaternion);
				if (ContainsKey(key))
				{
					JsonObject data4 = GetData<JsonObject>(key);
					quaternion.x = data4.Get<float>("x");
					quaternion.y = data4.Get<float>("y");
					quaternion.z = data4.Get<float>("z");
					quaternion.w = data4.Get<float>("w");
				}
				return quaternion;
			}
			if (type == typeof(Color))
			{
				Color color = default(Color);
				if (ContainsKey(key))
				{
					JsonObject data5 = GetData<JsonObject>(key);
					color.r = data5.Get<float>("r");
					color.g = data5.Get<float>("g");
					color.b = data5.Get<float>("b");
					color.a = data5.Get<float>("a");
				}
				return color;
			}
			if (type == typeof(Color32))
			{
				Color32 color2 = default(Color32);
				if (ContainsKey(key))
				{
					JsonObject data6 = GetData<JsonObject>(key);
					color2.r = data6.Get<byte>("r");
					color2.g = data6.Get<byte>("g");
					color2.b = data6.Get<byte>("b");
					color2.a = data6.Get<byte>("a");
				}
				return color2;
			}
			if (type == typeof(Rect))
			{
				Rect rect = default(Rect);
				if (ContainsKey(key))
				{
					JsonObject data7 = GetData<JsonObject>(key);
					rect.x = data7.Get<float>("x");
					rect.y = data7.Get<float>("y");
					rect.width = data7.Get<float>("width");
					rect.height = data7.Get<float>("height");
				}
				return rect;
			}
			if (type.IsArray)
			{
				Array array;
				if (ContainsKey(key))
				{
					JsonArray jsonArray = (JsonArray)GetData(key, typeof(JsonArray));
					array = Array.CreateInstance(type.GetElementType(), jsonArray.Length);
					for (int j = 0; j < jsonArray.Length; j++)
					{
						array.SetValue(jsonArray.Get(j, type.GetElementType()), j);
					}
				}
				else
				{
					array = Array.CreateInstance(type.GetElementType(), 0);
				}
				return array;
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
			{
				IList list = null;
				if (ContainsKey(key))
				{
					JsonArray jsonArray2 = (JsonArray)GetData(key, typeof(JsonArray));
					Type type2 = type.GetGenericArguments()[0];
					list = (IList)type.GetConstructor(new Type[1] { typeof(int) }).Invoke(new object[1] { jsonArray2.Length });
					for (int k = 0; k < jsonArray2.Length; k++)
					{
						list.Add(jsonArray2.Get(k, type2));
					}
				}
				else
				{
					list = (IList)type.GetConstructor(new Type[1] { typeof(int) }).Invoke(new object[1] { 0 });
				}
				return list;
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<, >))
			{
				IDictionary dictionary = null;
				if (ContainsKey(key))
				{
					JsonObject jsonObject = (JsonObject)GetData(key, typeof(JsonObject));
					Type type3 = type.GetGenericArguments()[0];
					Type type4 = type.GetGenericArguments()[1];
					if (type3 != typeof(string))
					{
						Debug.LogWarning("Json does not support this type");
						return "{}";
					}
					dictionary = (IDictionary)type.GetConstructor(new Type[1] { typeof(int) }).Invoke(new object[1] { jsonObject.Length });
					for (int l = 0; l < jsonObject.Length; l++)
					{
						string key2 = jsonObject.values.Keys.ElementAt(l);
						object value = jsonObject.Get(key2, type4);
						dictionary.Add(key2, value);
					}
				}
				else
				{
					dictionary = (IDictionary)type.GetConstructor(new Type[1] { typeof(int) }).Invoke(new object[1] { 0 });
				}
				return dictionary;
			}
			if (type == typeof(JsonArray))
			{
				if (ContainsKey(key))
				{
					return JsonArray.Parse(values[key]);
				}
				return new JsonArray();
			}
			if (type == typeof(JsonObject))
			{
				if (ContainsKey(key))
				{
					return JsonObject.Parse(values[key]);
				}
				return new JsonObject();
			}
			if (type.IsClass)
			{
				if (ContainsKey(key))
				{
					JsonObject data8 = GetData<JsonObject>(key);
					return JsonConvert.Deserialize(type, data8.ToString());
				}
				return Activator.CreateInstance(type);
			}
			Debug.LogWarning("Json does not support this type");
			return null;
		}
	}
}
