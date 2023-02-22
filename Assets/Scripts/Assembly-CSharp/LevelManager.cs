using System.Collections.Generic;
using FreeJSON;
using UnityEngine;

public class LevelManager
{
    public static bool customScene = false;

    private static Dictionary<GameMode, List<string>> gameModeScenes = new Dictionary<GameMode, List<string>>();

    private static List<string> scenes = new List<string>();

    public static void Init()
    {
        if (gameModeScenes.Count != 0)
        {
            return;
        }
        TextAsset textAsset = Resources.Load("Others/SceneManager") as TextAsset;
        JsonArray jsonArray = JsonArray.Parse(Utils.XOR(textAsset.text));
        for (int i = 0; i < jsonArray.Length; i++)
        {
            GameMode key = jsonArray.Get<JsonObject>(i).Get<GameMode>("GameMode");
            List<string> list = new List<string>();
            JsonArray jsonArray2 = jsonArray.Get<JsonObject>(i).Get<JsonArray>("Scenes");
            for (int j = 0; j < jsonArray2.Length; j++)
            {
                list.Add(jsonArray2.Get<string>(j));
                if (!scenes.Contains(jsonArray2.Get<string>(j)))
                {
                    scenes.Add(jsonArray2.Get<string>(j));
                }
            }
            gameModeScenes.Add(key, list);
        }
    }

    public static List<string> GetGameModeScenes(GameMode mode)
    {
        Init();
        return gameModeScenes[mode];
    }

    public static List<string> GetAllScenes()
    {
        Init();
        return scenes;
    }

    public static bool HasScene(string scene)
    {
        Init();
        return scenes.Contains(scene);
    }

    public static string GetNextScene(GameMode mode)
    {
        return GetNextScene(mode, GetSceneName());
    }

    public static string GetNextScene(GameMode mode, string scene)
    {
        Init();
        if (customScene)
        {
            return scene;
        }
        List<string> list = gameModeScenes[mode];
        for (int i = 0; i < list.Count; i++)
        {
            if (scene == list[i])
            {
                if (list.Count - 1 == i)
                {
                    return list[0];
                }
                return list[i + 1];
            }
        }
        return string.Empty;
    }

    public static bool HasSceneInGameMode(GameMode mode)
    {
        return HasSceneInGameMode(mode, GetSceneName());
    }

    public static bool HasSceneInGameMode(GameMode mode, string scene)
    {
        List<string> list = GetGameModeScenes(mode);
        return list.Contains(scene);
    }

    public static string GetSceneName()
    {
        if (customScene)
        {
            return Application.loadedLevelName;
        }
        else
        {
            return Application.loadedLevelName;
        }
    }

    private static string GetEncryptSceneName(string name)
    {
        string text = Utils.XOR(name, Utils.test, true);
        return text.Replace("/", "#");
    }

    public static void LoadLevel(string name)
    {
        if (customScene)
        {
            Application.LoadLevel(name);
        }
        else
        {
            Application.LoadLevel(name);
        }
    }
}
