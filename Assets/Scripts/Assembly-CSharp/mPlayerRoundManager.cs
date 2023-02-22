using System;
using UnityEngine;

public class mPlayerRoundManager : MonoBehaviour
{
	public GameObject panel;

	public UILabel modeLabel;

	public UILabel moneyLabel;

	public UILabel xpLabel;

	public UILabel killsLabel;

	public UILabel headshotLabel;

	public UILabel deathsLabel;

	public UILabel timeLabel;

	private static mPlayerRoundManager instance;

	private void Start()
	{
		instance = this;
	}

	public static void Show()
	{
		instance.panel.SetActive(true);
		instance.modeLabel.text = Localization.Get(PlayerRoundManager.GetMode().ToString());
		instance.moneyLabel.text = PlayerRoundManager.GetMoney().ToString();
		instance.xpLabel.text = PlayerRoundManager.GetXP().ToString();
		instance.killsLabel.text = PlayerRoundManager.GetKills().ToString();
		instance.headshotLabel.text = PlayerRoundManager.GetHeadshot().ToString();
		instance.deathsLabel.text = PlayerRoundManager.GetDeaths().ToString();
		instance.timeLabel.text = ConvertTime(PlayerRoundManager.GetTime());
	}

	public void Close()
	{
		panel.SetActive(false);
		PlayerRoundManager.Clear();
		EventManager.Dispatch("AccountUpdate");

    }

    private static string ConvertTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return string.Format("{0:0}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}
