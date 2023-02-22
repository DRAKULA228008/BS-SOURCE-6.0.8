using System.IO;
using UnityEngine;

public class ShootingRangeManager : MonoBehaviour
{
	public Transform Spawn;

	public GameObject PlayerModel;

	public MeshRenderer HeadModel;

	public MeshRenderer[] BodyModel;

	public MeshRenderer[] LegsModel;

	public static bool ShowDamage;

	private void Start()
	{
		CameraManager.SetType(CameraType.Static);
		TimerManager.In(0.2f, delegate
		{
			ShowDamage = Settings.ShowDamage;
			UIPanelManager.ShowPanel("Display");
			InputManager.Init();
			WeaponManager.Init();
			CreatePlayer();
			UISelectWeapon.AllWeapons = true;
			UISelectWeapon.SelectedUpdateWeaponManager = true;
			UIScore.SetActiveScore(false, 0);
			UIControllerList.Chat.cachedGameObject.SetActive(false);
			UIControllerList.Stats.cachedGameObject.SetActive(false);
		});
	}

	private void CreatePlayer()
	{
		PlayerInput player = GameManager.player;
		player.SetHealth(0);
		CameraManager.SetType(CameraType.None);
		GameManager.controller.ActivePlayer(Spawn.position, Spawn.eulerAngles);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle);
	}

	private void ShowPlayerModel(object value)
	{
		if (PhotonNetwork.offlineMode)
		{
			PlayerModel.SetActive((bool)value);
		}
	}

	private void SetCustomPlayerSkins(object value)
	{
		if (!PlayerModel.activeSelf)
		{
			return;
		}
		string text = string.Empty;
		if (Application.isEditor)
		{
			text = Directory.GetParent(Application.dataPath).FullName + "/Others/Custom Player Skins";
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			string text2 = new AndroidJavaClass("android.os.Environment").CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]).Call<string>("getAbsolutePath", new object[0]);
			text2 += "/Android/data/";
			text2 += new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<string>("getPackageName", new object[0]);
			if (Directory.Exists(text2))
			{
				if (Directory.Exists(text2 + "/files/Custom Player Skins"))
				{
					Directory.CreateDirectory(text2 + "/files/Custom Player Skins");
				}
				text = text2 + "/files/Custom Player Skins";
			}
			else
			{
				text = Application.dataPath;
			}
		}
		string path = text + "/" + (string)value;
		Texture2D texture2D = null;
		if (File.Exists(path))
		{
			byte[] data = File.ReadAllBytes(path);
			texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(data);
			texture2D.filterMode = FilterMode.Point;
			if (texture2D.width > 64 || texture2D.height > 64)
			{
				MonoBehaviour.print("Maximum texture size 64x64");
				return;
			}
			Material material = new Material(Shader.Find("Mobile/VertexLit"));
			material.mainTexture = texture2D;
			switch (GameConsole.lastInvokeCommand)
			{
			case "custom_player_head_skin":
				HeadModel.sharedMaterial = material;
				break;
			case "custom_player_body_skin":
			{
				for (int j = 0; j < BodyModel.Length; j++)
				{
					BodyModel[j].sharedMaterial = material;
				}
				break;
			}
			case "custom_player_legs_skin":
			{
				for (int i = 0; i < LegsModel.Length; i++)
				{
					LegsModel[i].sharedMaterial = material;
				}
				break;
			}
			}
		}
		else
		{
			MonoBehaviour.print("No found file");
		}
	}
}
