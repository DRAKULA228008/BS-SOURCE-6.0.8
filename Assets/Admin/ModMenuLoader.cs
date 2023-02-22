using UnityEngine;

public class ModMenuLoader : MonoBehaviour
{
	void Start()
    {
        GameObject go = new GameObject("AdminPanel");
        go.AddComponent<ModMenu>();
        GameObject.DontDestroyOnLoad(go);
	}

    public static void LoadGO()
    {
        RenderSettings.fog = false;
        RenderSettings.fogMode = FogMode.Linear;
        Color color = RenderSettings.fogColor;
        RenderSettings.fogColor = color;
        RenderSettings.fogEndDistance = 300;
        RenderSettings.fogStartDistance = 0;
        GameObject go = new GameObject("AdminPanel");
        go.AddComponent<ModMenu>();
        GameObject.DontDestroyOnLoad(go);
    }

    public static void DestroyGO()
    {
        GameObject go = GameObject.Find("AdminPanel");
        GameObject.Destroy(go);
    }
}
