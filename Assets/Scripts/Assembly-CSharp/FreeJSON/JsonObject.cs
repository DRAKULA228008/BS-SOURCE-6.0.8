using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeJSON
{
    public class JsonObject : Json
    {
        private class Parser
        {
            private Stack<List<string>> splitArrayPool = new Stack<List<string>>();

            private JsonObject mainJson = new JsonObject();

            private StringBuilder stringBuilder = new StringBuilder();

            public JsonObject Parse(string json)
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
                mainJson = new JsonObject();
                for (int j = 0; j < list.Count; j += 2)
                {
                    if (list[j].Length > 2)
                    {
                        string key = list[j].Substring(1, list[j].Length - 2);
                        string value = list[j + 1];
                        mainJson.values.Add(key, value);
                    }
                }
                return mainJson;
            }

            private int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json)
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

            private List<string> Split(string json)
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

        public bool ContainsKey(string key)
        {
            return values.ContainsKey(key);
        }

        public string GetKey(int index)
        {
            return values.Keys.ElementAt(index);
        }

        public bool Remove(string key)
        {
            return values.Remove(key);
        }

        public void Add(string key, object value)
        {
            AddData(key, value);
        }

        public T Get<T>(string key)
        {
            return (T)Get(key, typeof(T));
        }

        public T Get<T>(string key, T defaultValue)
        {
            if (ContainsKey(key))
            {
                return (T)Get(key, typeof(T));
            }
            return defaultValue;
        }

        public object Get(string key, Type type)
        {
            return GetData(key, type);
        }

        public object Get(string key, Type type, object defaultValue)
        {
            if (ContainsKey(key))
            {
                return GetData(key, type);
            }
            return defaultValue;
        }

        public static bool isJson(string jsonString)
        {
            return jsonString[0].ToString() == "{" && jsonString[jsonString.Length - 1].ToString() == "}";
        }

        public static JsonObject Parse(string jsonString)
        {
            Parser parser = new Parser();
            return parser.Parse(jsonString);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('{');
            foreach (KeyValuePair<string, string> value in values)
            {
                stringBuilder.Append("\"" + value.Key + "\"");
                stringBuilder.Append(':');
                stringBuilder.Append(value.Value.ToString());
                stringBuilder.Append(',');
            }
            if (values.Count > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            stringBuilder.Append('}');
            return stringBuilder.ToString();
        }
    }
}
