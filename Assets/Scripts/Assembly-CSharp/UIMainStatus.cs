using UnityEngine;

public class UIMainStatus : MonoBehaviour
{
	public UILabel label;

	private static UIMainStatus instance;

	private void Start()
	{
		instance = this;
		PhotonRPC.AddMessage("PhotonShow", PhotonShow);
	}

	public static void Add(string text)
	{
		Add(text, false, 2f, string.Empty);
	}

	public static void Add(string text, bool local)
	{
		Add(text, local, 2f, string.Empty);
	}

	public static void Add(string text, bool local, float duration)
	{
		Add(text, local, duration, string.Empty);
	}

	public static void Add(string text, bool local, float duration, string localize)
	{
		if (local)
		{
			Show(text, duration);
			return;
		}
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write(text);
		data.Write(duration);
		data.Write(localize);
		PhotonRPC.RPC("PhotonShow", PhotonTargets.All, data);
	}

	[PunRPC]
	private void PhotonShow(PhotonMessage message)
	{
		string text = message.ReadString();
		float duration = message.ReadFloat();
		string text2 = message.ReadString();
		if (string.IsNullOrEmpty(text2))
		{
			Show(text, duration);
			return;
		}
		text2 = Localization.Get(text2);
		text = text.Replace("[@]", text2);
		Show(text, duration);
	}

	public static void Show(string text)
	{
		Show(text, 5f);
	}

	public static void Show(string text, float duration)
	{
		instance.label.text = text;
		TimerManager.Cancel("MainStatus");
		TimerManager.In("MainStatus", duration, instance.Clear);
	}

	private void Clear()
	{
		instance.label.text = string.Empty;
	}
}
