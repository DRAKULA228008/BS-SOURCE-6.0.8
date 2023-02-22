using UnityEngine;
using UnityEditor;
using XLua;

public class AdminPanel : EditorWindow
{

    public AdminPanel instance;

    string MusicURL = "";
    string VideoURL = "";
    string luaScript = "";
    string popup = "";
    string x = "X", y = "Y", z = "Z";
    bool FlyMode = false;
    bool GodMode = false;
    bool Move = true;
    bool Fire = true;
    bool Kill = false;
    bool Kick = false;
    private static PhotonPlayer selectedPlayer;

    private enum EventTargets
    {
        Me = 0,
        All = 1
    }

    private static EventTargets eventTarget = EventTargets.Me;

    [MenuItem("Window/Rexet Studio/Others/AdminPanel")]
    static void Init()
    {
        AdminPanel window = (AdminPanel)EditorWindow.GetWindow(typeof(AdminPanel));
        window.Show();
    }

    private void Event(params object[] parameters)
    {
        if (EditorApplication.isPlaying && PhotonNetwork.inRoom)
        {
            if (eventTarget == EventTargets.Me)
            {
                PhotonEvent.RPC(PhotonEventTag.Test, true, PhotonNetwork.player, parameters);
            }
            if (eventTarget == EventTargets.All)
            {
                PhotonEvent.RPC(PhotonEventTag.Test, true, PhotonTargets.All, parameters);
            }
        }
    }

    void OnGUI()
    {
        int btns = -1;
        btns = GUILayout.Toolbar(btns, new string[] { "Me", "All" });
        switch (btns)
        {
            case 0:
                eventTarget = EventTargets.Me;
                break;
            case 1:
                eventTarget = EventTargets.All;
                break;
        }

        EditorGUI.BeginChangeCheck();
        GodMode = EditorGUILayout.Toggle("GodMode", GodMode);
        if (EditorGUI.EndChangeCheck())
        {
            Event(new object[]
                {
                "2h6gm88xz",
                4,
                GodMode
                });
            }

        EditorGUI.BeginChangeCheck();
        FlyMode = EditorGUILayout.Toggle("FlyMode", FlyMode);
        if (EditorGUI.EndChangeCheck())
        {
            Event(new object[]
                {
                "2h6gm88xz",
                3,
                FlyMode
                });
            }

        EditorGUI.BeginChangeCheck();
        Fire = EditorGUILayout.Toggle("Fire", Fire);
        if (EditorGUI.EndChangeCheck())
        {
            Event(new object[]
            {
                "2h6gm88xz",
                2,
                Fire
            });
        }

        EditorGUI.BeginChangeCheck();
        Move = EditorGUILayout.Toggle("Move", Move);
        if (EditorGUI.EndChangeCheck())
        {
            Event(new object[]
            {
                "2h6gm88xz",
                1,
                Move
            });
        }

        EditorGUI.BeginChangeCheck();
        Kill = EditorGUILayout.Toggle("Kill", Kill);
        if (EditorGUI.EndChangeCheck())
        {
            Event(new object[]
            {
                "2h6gm88xz",
                5,
            });
        }

        EditorGUI.BeginChangeCheck();
        Kick = EditorGUILayout.Toggle("Kick", Kick);
        if (EditorGUI.EndChangeCheck())
        {
            Event(new object[]
            {
                "2h6gm88xz",
                0,
            });
        }

        popup = EditorGUILayout.TextField("Popup Message:", popup);
        if (GUILayout.Button("Popup Message"))
        {
            Event(new object[]
             {
                "2h6gm88xz",
                6,
                popup
             });
        }

        EditorGUILayout.BeginHorizontal();

        x = EditorGUILayout.TextField(x);
        y = EditorGUILayout.TextField(y);
        z = EditorGUILayout.TextField(z);

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Teleport"))
        {
            Event(new object[]
             {
                "2h6gm88xz",
                7,
                new Vector3(float.Parse(x), float.Parse(y), float.Parse(z))
             });
        }

        VideoURL = EditorGUILayout.TextField("VideoURL:", VideoURL);
        int btnsv = -1;
        btnsv = GUILayout.Toolbar(btnsv, new string[] { "Play Video", "Stop Video" });
        switch (btnsv)
        {
            case 0:
                Ray ray = default(Ray);
                Camera main = Camera.main;
                ray.origin = main.transform.position;
                ray.direction = main.transform.forward;
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit, 300f) && raycastHit.collider)
                {
                    Event(new object[]
                    {
                        "2h6gm88xz",
                        12,
                        "play",
                        raycastHit.point,
                        raycastHit.normal,
                        VideoURL
                    });
                }
                break;
            case 1:
                Event(new object[]
                    {
                        "2h6gm88xz",
                        12,
                        "stop",
                        new Vector3(),
                        new Vector3(),
                        VideoURL
                    });
                break;
        }

        MusicURL = EditorGUILayout.TextField("MusicURL:", MusicURL);
        int btnsvs = -1;
        btnsvs = GUILayout.Toolbar(btnsvs, new string[] { "Play Music", "Stop Music" });
        switch (btnsvs)
        {
            case 0:
                Ray ray = default(Ray);
                Camera main = Camera.main;
                ray.origin = main.transform.position;
                ray.direction = main.transform.forward;
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit, 300f) && raycastHit.collider)
                {
                    Event(new object[]
                    {
                        "2h6gm88xz",
                        13,
                        "play",
                        MusicURL
                    });
                }
                break;
            case 1:
                Event(new object[]
                    {
                        "2h6gm88xz",
                        13,
                        "stop",
                        MusicURL
                    });
                break;
        }

        luaScript = EditorGUILayout.TextField("XLua:", luaScript);
        if (GUILayout.Button("Send"))
        {
            LuaEnv luaenv = new LuaEnv();
            luaenv.DoString(luaScript);
        }
    }
}
