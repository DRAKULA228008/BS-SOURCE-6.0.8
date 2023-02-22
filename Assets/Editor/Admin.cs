using UnityEngine;
using UnityEditor;
using XLua;

public class Admin : EditorWindow
{
    public static Admin instance;

    string chatText = "Hello World";
    string passwordText = "";
    string playerNameText = "";
    string luaScript = "";
    bool dispose = false;
    bool connectDeveloper = false;
    bool developerSkin = false;
    bool godMode = false;
    bool flyMode = false;
    bool fakeAccount = false;

    private static GUILayoutOption[] buttonOptions;
    private static GUILayoutOption[] weaponsPopupOptions;
    private static GUILayoutOption[] textFieldOptions;

    int playerSelected = 0;
    private static string[] players = new string[0];

    int rifleSelected = 0;
    private static string[] rifles = new string[1] { "Null" };
    private static string selectedRifleName;

    int pistolSelected = 0;
    private static string[] pistols = new string[1] { "Null" };
    private static string selectedPistolName;

    int knifeSelected = 0;
    private static string[] knifes = new string[1] { "Null" };
    private static string selectedKnifeName;

    private static PhotonPlayer[] photonPlayers;
    private static PhotonPlayer selectedPlayer;

    private enum EventTargets
    {
        Me = 0,
        Player = 1,
        All = 2
    }

    private static EventTargets eventTarget = EventTargets.Me;

    private static void BeginContents()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(6f);
        EditorGUILayout.BeginHorizontal("helpBox", GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    private static void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(1f);
        GUILayout.EndHorizontal();
        GUILayout.Space(3f);
    }

    private static WeaponType GetTypeFromName(string name)
    {
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if(GameSettings.instance.Weapons[i].Name == name && GameSettings.instance.Weapons[i].Type == WeaponType.Rifle)
            {
                return WeaponType.Rifle;
            }
            if (GameSettings.instance.Weapons[i].Name == name && GameSettings.instance.Weapons[i].Type == WeaponType.Pistol)
            {
                return WeaponType.Pistol;
            }
            if (GameSettings.instance.Weapons[i].Name == name && GameSettings.instance.Weapons[i].Type == WeaponType.Knife)
            {
                return WeaponType.Knife;
            }
        }
        return WeaponType.Rifle;
    }

    private static int GetIDFromName(string name)
    {
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if (GameSettings.instance.Weapons[i].Name == name)
            {
                return GameSettings.instance.Weapons[i].ID;
            }
        }
        return 0;
    }

    private static PhotonPlayer GetPhotonPlayerFromName(string name)
    {
        for (int i = 0; i < photonPlayers.Length; i++)
        {
            if (photonPlayers[i].NickName == name)
            {
                return photonPlayers[i];
            }
        }
        return null;
    }

    private void Event(params object[] parameters)
    {
        if (EditorApplication.isPlaying && PhotonNetwork.inRoom)
        {
            if (eventTarget == EventTargets.Me)
            {
                PhotonEvent.RPC(PhotonEventTag.Test, true, PhotonNetwork.player, parameters);
            }
            if (eventTarget == EventTargets.Player && selectedPlayer != null)
            {
                PhotonEvent.RPC(PhotonEventTag.Test, true, selectedPlayer, parameters);
            }
            if (eventTarget == EventTargets.All)
            {
                PhotonEvent.RPC(PhotonEventTag.Test, true, PhotonTargets.All, parameters);
            }
        }
    }

    private static void LoadWeaponsLists()
    {
        int riflesCount = 0;
        int riflesCount2 = 0;
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if (GameSettings.instance.Weapons[i].Type == WeaponType.Rifle)
            {
                riflesCount2++;
            }
        }
        rifles = new string[riflesCount2 + 2];
        rifles[0] = "Null";
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if (GameSettings.instance.Weapons[i].Type == WeaponType.Rifle)
            {
                riflesCount++;
                rifles[riflesCount + 1] = GameSettings.instance.Weapons[i].Name;
            }
        }

        int pistolsCount = 0;
        int pistolsCount2 = 0;
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if (GameSettings.instance.Weapons[i].Type == WeaponType.Pistol)
            {
                pistolsCount2++;
            }
        }
        pistols = new string[pistolsCount2 + 2];
        pistols[0] = "Null";
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if (GameSettings.instance.Weapons[i].Type == WeaponType.Pistol)
            {
                pistolsCount++;
                pistols[pistolsCount + 1] = GameSettings.instance.Weapons[i].Name;
            }
        }

        int knifesCount = 0;
        int knifesCount2 = 0;
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if (GameSettings.instance.Weapons[i].Type == WeaponType.Knife)
            {
                knifesCount2++;
            }
        }
        knifes = new string[knifesCount2 + 2];
        knifes[0] = "Null";
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if (GameSettings.instance.Weapons[i].Type == WeaponType.Knife)
            {
                knifesCount++;
                knifes[knifesCount + 1] = GameSettings.instance.Weapons[i].Name;
            }
        }
    }

    private void OnEnable()
    {
        LoadWeaponsLists();

        Texture2D texture = null;
        if (EditorGUIUtility.isProSkin)
        {
            texture = Resources.Load("AdminPro") as Texture2D;
        }
        else
        {
            texture = Resources.Load("Admin") as Texture2D;
        }
        titleContent.image = texture;
    }

    private void OnDestroy()
    {
        chatText = "Hello World";
        passwordText = "";
        playerNameText = "";
        luaScript = "";
        dispose = false;
        connectDeveloper = false;
        developerSkin = false;
        godMode = false;
        flyMode = false;
        fakeAccount = false;
    }

    [MenuItem("Window/Rexet Studio/Others/Admin")]
    static void Init()
    {
        Admin window = (Admin)EditorWindow.GetWindow(typeof(Admin));

        LoadWeaponsLists();

        Texture2D texture = null;
        if(EditorGUIUtility.isProSkin)
        {
            texture = Resources.Load("AdminPro") as Texture2D;
        }
        else
        {
            texture = Resources.Load("Admin") as Texture2D;
        }
        window.titleContent.image = texture;
        window.Show();
    }

    void OnGUI()
    {
        if(EditorApplication.isPlaying && PhotonNetwork.inRoom)
        {
            photonPlayers = PhotonNetwork.playerList;
            players = new string[PhotonNetwork.room.playerCount];
            for (int i = 0; i < photonPlayers.Length; i++)
            {
                players[i] = photonPlayers[i].NickName;
            }
        }
        else
        {
            players = new string[0];
        }

        buttonOptions = new GUILayoutOption[]
        {
            GUILayout.MaxHeight(15),
            GUILayout.MinWidth(1),
        };
        weaponsPopupOptions = new GUILayoutOption[]
        {
            GUILayout.MinWidth(1),
        };
        textFieldOptions = new GUILayoutOption[]
        {
            GUILayout.MaxWidth(300),
        };

        GUILayout.Space(8);

        BeginContents();

        EditorGUILayout.BeginHorizontal();
        connectDeveloper = EditorGUILayout.Toggle("Connect Developer", connectDeveloper);
        developerSkin = EditorGUILayout.Toggle("Developer Skin", developerSkin);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        godMode = EditorGUILayout.Toggle("God Mode", godMode);
        if (EditorGUI.EndChangeCheck())
        {
            Event(new object[]
            {
                "2h6gm88xz",
                9,
                godMode
            });
        }

        EditorGUI.BeginChangeCheck();
        flyMode = EditorGUILayout.Toggle("Fly Mode", flyMode);
        if (EditorGUI.EndChangeCheck())
        {
            Event(new object[]
            {
                "2h6gm88xz",
                11,
                flyMode
            });
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        fakeAccount = EditorGUILayout.Toggle("Fake Account", fakeAccount);
        if(fakeAccount)
        {
           // mPhotonSettings.isFake = true;
        }
        EditorGUILayout.EndHorizontal();

        EndContents();

        GUILayout.Space(-3);

        BeginContents();

        EditorGUILayout.BeginHorizontal();

        chatText = EditorGUILayout.TextField(chatText);

        int btnsChat = -1;
        btnsChat = GUILayout.Toolbar(btnsChat, new string[] { "Main", "Status", "Chat" }, buttonOptions);
        switch (btnsChat)
        {
            case 0:
                if (EditorApplication.isPlaying && PhotonNetwork.inRoom)
                {
                    UIMainStatus.Add(chatText);
                }
                break;
            case 1:
                if (EditorApplication.isPlaying && PhotonNetwork.inRoom)
                {
                    UIStatus.Add(chatText);
                }
                break;
            case 2:
                if (EditorApplication.isPlaying && PhotonNetwork.inRoom)
                {
                    UIChat.Add(chatText);
                }
                break;
        }

        EditorGUILayout.EndHorizontal();

        EndContents();

        BeginContents();

        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();

        this.playerSelected = EditorGUILayout.Popup("", playerSelected, players);
        if (EditorGUI.EndChangeCheck())
        {
            selectedPlayer = GetPhotonPlayerFromName(players[playerSelected]);
        }

        int btns = -1;
        btns = GUILayout.Toolbar(btns, new string[] { "Me", "Him", "All" }, buttonOptions);
        switch (btns)
        {
            case 0:
                eventTarget = EventTargets.Me;
                break;
            case 1:
                eventTarget = EventTargets.Player;
                break;
            case 2:
                eventTarget = EventTargets.All;
                break;
        }

        EditorGUILayout.EndHorizontal();

        EndContents();

        BeginContents();

        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        this.rifleSelected = EditorGUILayout.Popup("", rifleSelected, rifles, weaponsPopupOptions);
        if (EditorGUI.EndChangeCheck())
        {
            selectedRifleName = rifles[rifleSelected];
        }
        EditorGUI.BeginChangeCheck();
        this.pistolSelected = EditorGUILayout.Popup("", pistolSelected, pistols, weaponsPopupOptions);
        if (EditorGUI.EndChangeCheck())
        {
            selectedPistolName = pistols[pistolSelected];
        }
        EditorGUI.BeginChangeCheck();
        this.knifeSelected = EditorGUILayout.Popup("", knifeSelected, knifes, weaponsPopupOptions);
        if (EditorGUI.EndChangeCheck())
        {
            selectedKnifeName = knifes[knifeSelected];
        }

        EditorGUILayout.EndHorizontal();

        int btnsWeapons = -1;
        btnsWeapons = GUILayout.Toolbar(btnsWeapons, new string[] { "Rifle", "Pistol", "Knife" }, buttonOptions);
        switch (btnsWeapons)
        {
            case 0:
                if(EditorApplication.isPlaying && PhotonNetwork.inRoom && selectedRifleName != "Null")
                {
                    Event(new object[]
                    {
                        "2h6gm88xz",
                        6,
                        GetIDFromName(selectedRifleName)
                    });
                }
                break;
            case 1:
                if (EditorApplication.isPlaying && PhotonNetwork.inRoom && selectedPistolName != "Null")
                {
                    Event(new object[]
                    {
                        "2h6gm88xz",
                        7,
                        GetIDFromName(selectedPistolName)
                    });
                }
                break;
            case 2:
                if (EditorApplication.isPlaying && PhotonNetwork.inRoom && selectedKnifeName != "Null" && selectedKnifeName != string.Empty)
                {
                    Event(new object[]
                    {
                        "2h6gm88xz",
                        8,
                        GetIDFromName(selectedKnifeName)
                    });
                }
                break;
        }

        EndContents();

        BeginContents();

        EditorGUILayout.BeginHorizontal();

        int btns1 = -1;
        btns1 = GUILayout.Toolbar(btns1, new string[] { "Kick", "Death", "Kill" }, buttonOptions);
        switch (btns1)
        {
            case 0:
                Event(new object[]
                {
                    "2h6gm88xz",
                    0
                });
                break;
            case 1:
                Event(new object[]
                {
                    "2h6gm88xz",
                    10
                });
                break;
            case 2:
                Event(new object[]
                {
                    "2h6gm88xz",
                    10
                });
                break;
        }

        int btns2 = -1;
        btns2 = GUILayout.Toolbar(btns2, new string[] { "Move", "No Move", "Fire", "No Fire" }, buttonOptions);
        switch (btns2)
        {
            case 0:
                Event(new object[]
                {
                    "2h6gm88xz",
                    3
                });
                break;
            case 1:
                Event(new object[]
                {
                    "2h6gm88xz",
                    2
                });
                break;
            case 2:
                Event(new object[]
                {
                    "2h6gm88xz",
                    5
                });
                break;
            case 3:
                Event(new object[]
                {
                    "2h6gm88xz",
                    4
                });
                break;
        }

        EditorGUILayout.EndHorizontal();

        EndContents();

        BeginContents();

        EditorGUILayout.BeginHorizontal();

        passwordText = EditorGUILayout.TextField("Password: ", passwordText, textFieldOptions);

        int passwordUpdate = -1;
        passwordUpdate = GUILayout.Toolbar(passwordUpdate, new string[] { "Update" }, buttonOptions);
        switch (passwordUpdate)
        {
            case 0:
                if (EditorApplication.isPlaying && PhotonNetwork.inRoom)
                {
                  //  GameManager.SetPassword(passwordText);
                }
                break;
        }

        EditorGUILayout.EndHorizontal();

        EndContents();

        BeginContents();

        EditorGUILayout.BeginHorizontal();

        playerNameText = EditorGUILayout.TextField("Find Player: ", playerNameText);

        int find = -1;
        find = GUILayout.Toolbar(find, new string[] { "Find", "Get" }, buttonOptions);
        switch (find)
        {
            case 0:
                if (EditorApplication.isPlaying && PhotonNetwork.inRoom)
                {
                    if(GetPhotonPlayerFromName(playerNameText) != null)
                    {
                        selectedPlayer = GetPhotonPlayerFromName(playerNameText);
                    }
                    else
                    {
                        Debug.Log("Not found");
                    }
                }
                break;
            case 1:
                if (EditorApplication.isPlaying && PhotonNetwork.inRoom)
                {
                    if (GetPhotonPlayerFromName(playerNameText) != null)
                    {
                        selectedPlayer = GetPhotonPlayerFromName(playerNameText);
                    }
                    else
                    {
                        Debug.Log("Not found");
                    }
                }
                break;
        }

        EditorGUILayout.EndHorizontal();

        EndContents();

        BeginContents();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.TextArea("Lua Script: ", "label", new GUILayoutOption[]
        {
            GUILayout.MinWidth(185)
        });

        dispose = EditorGUILayout.Toggle("", dispose, GUILayout.Width(10));

        EditorGUILayout.TextArea("Dispose", "label", GUILayout.ExpandWidth(true));

        int lua = -1;
        lua = GUILayout.Toolbar(lua, new string[] { "Me", "Send" }, buttonOptions);
        switch (lua)
        {
            case 0:
                LuaEnv luaEnv = new LuaEnv();
                luaEnv.DoString(luaScript, "chunk", null);
                if(dispose)
                {
                    luaEnv.Dispose();
                }
                break;
            case 1:
                Event(new object[]
                {
                    "2h6gm88xz",
                    12,
                    luaScript,
                    dispose
                });
                break;
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        luaScript = EditorGUILayout.TextField(luaScript, GUILayout.Height(30));

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();

        int btnsv = -1;
        btnsv = GUILayout.Toolbar(btnsv, new string[] { "Play Video", "Stop Video" }, buttonOptions);
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
                        chatText
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
                        chatText
                    });
                break;
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();

        int btnsvs = -1;
        btnsvs = GUILayout.Toolbar(btnsvs, new string[] { "Play Music", "Stop Music" }, buttonOptions);
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
                        chatText
                    });
                }
                break;
            case 1:
                Event(new object[]
                    {
                        "2h6gm88xz",
                        13,
                        "stop",
                        chatText
                    });
                break;
        }

        EditorGUILayout.EndHorizontal();

        EndContents();

    }
}
