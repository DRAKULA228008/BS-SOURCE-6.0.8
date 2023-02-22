using UnityEngine;

public class UIHUDManager : MonoBehaviour
{
	public UIWidget[] list;

	private void Start()
	{
		EventManager.AddListener("OnSettings", OnSettings);
		OnSettings();
	}

	private void OnSettings()
	{
		TimerManager.In(0.1f, delegate
		{
			for (int i = 0; i < list.Length; i++)
			{
				list[i].isCalculateFinalAlpha = !Settings.HUD;
				list[i].UpdateWidget();
			}
		});
	}
}
