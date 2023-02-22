using System;
using System.Collections.Generic;
using FreeJSON;
using UnityEngine;

[Serializable]
public class mClanPlayers
{
	private static Dictionary<int, string> cachePlayerInfo = new Dictionary<int, string>();

	public GameObject panel;

	public UIGrid grid;

	public UIPanel statsPanel;

	public UILabel playerStatsNameLabel;

	public UILabel playerStatsIDLabel;

	public UILabel playerStatsLevelLabel;

	public UILabel playerStatsXPLabel;

	public UILabel playerStatsDeathsLabel;

	public UILabel playerStatsKillsLabel;

	public UILabel playerStatsHeadshotLabel;

	public UILabel playerStatsTimeLabel;

	public UILabel playerStatsLastLoginLabel;

	public mClanPlayerElement container;

	private mClanPlayerElement selectPlayer;

	private List<mClanPlayerElement> activeContainers = new List<mClanPlayerElement>();

	private List<mClanPlayerElement> deactiveContainers = new List<mClanPlayerElement>();

	public void Open()
	{
		UpdateList();
	}

	public void DeletePlayer()
	{
		mPopUp.ShowPopup(Localization.Get("Do you want to remove") + ": " + selectPlayer.Name.text, Localization.Get("Delete"), Localization.Get("Yes"), delegate
		{
		}, Localization.Get("No"), delegate
		{
			mPopUp.HideAll();
		});
	}

	public void SelectPlayer(mClanPlayerElement element)
	{
		if (ClanManager.admin != (int)AccountManager.instance.Data.ID || element == null || element.ID == (int)AccountManager.instance.Data.ID)
		{
			return;
		}
		selectPlayer = element;
		mPanelManager.ShowTween(statsPanel.cachedGameObject);
		playerStatsNameLabel.text = element.Name.text;
		playerStatsIDLabel.text = element.ID.ToString();
		playerStatsLevelLabel.text = string.Empty;
		playerStatsXPLabel.text = string.Empty;
		playerStatsDeathsLabel.text = string.Empty;
		playerStatsKillsLabel.text = string.Empty;
		playerStatsHeadshotLabel.text = string.Empty;
		playerStatsTimeLabel.text = string.Empty;
		playerStatsLastLoginLabel.text = string.Empty;
		if (cachePlayerInfo.ContainsKey(element.ID))
		{
			UpdatePlayerInfo(cachePlayerInfo[element.ID]);
			return;
		}
		AccountManager.GetFriendsInfo(element.ID, delegate(string result)
		{
			cachePlayerInfo[element.ID] = result;
			UpdatePlayerInfo(result);
		}, delegate(string error)
		{
			UIToast.Show("Get Friend Info Error: " + error);
		});
	}

	private void UpdatePlayerInfo(string result)
	{
		JsonObject jsonObject = JsonObject.Parse(result);
		playerStatsNameLabel.text = jsonObject.Get<string>("0");
		playerStatsIDLabel.text = jsonObject.Get<string>("1");
		playerStatsLevelLabel.text = jsonObject.Get("2", "1");
		playerStatsXPLabel.text = jsonObject.Get("3", "0");
		playerStatsDeathsLabel.text = jsonObject.Get("4", "0");
		playerStatsKillsLabel.text = jsonObject.Get("5", "0");
		playerStatsHeadshotLabel.text = jsonObject.Get("6", "0");
		playerStatsTimeLabel.text = ConvertTime(jsonObject.Get("7", 0L));
		playerStatsLastLoginLabel.text = ConvertLastLoginTime(jsonObject.Get<long>("8"));
		CryptoPrefs.SetString("Friend_#" + playerStatsIDLabel.text, playerStatsNameLabel.text);
	}

	public void UpdateList()
	{
		DeactiveAll();
		for (int i = 0; i < ClanManager.players.Length; i++)
		{
			GetContainer().SetData(ClanManager.players[i]);
		}
		grid.repositionNow = true;
		UpdatePlayers();
	}

	private mClanPlayerElement GetContainer()
	{
		if (deactiveContainers.Count != 0)
		{
			mClanPlayerElement mClanPlayerElement2 = deactiveContainers[0];
			deactiveContainers.RemoveAt(0);
			mClanPlayerElement2.Widget.cachedGameObject.SetActive(true);
			activeContainers.Add(mClanPlayerElement2);
			return mClanPlayerElement2;
		}
		GameObject gameObject = grid.gameObject.AddChild(container.Widget.cachedGameObject);
		gameObject.SetActive(true);
		mClanPlayerElement component = gameObject.GetComponent<mClanPlayerElement>();
		activeContainers.Add(component);
		return component;
	}

	private void DeactiveAll()
	{
		for (int i = 0; i < activeContainers.Count; i++)
		{
			activeContainers[i].Widget.cachedGameObject.SetActive(false);
			deactiveContainers.Add(activeContainers[i]);
		}
		activeContainers.Clear();
	}

	private void UpdatePlayers()
	{
		List<int> list = new List<int>();
		string empty = string.Empty;
		for (int i = 0; i < ClanManager.players.Length; i++)
		{
			empty = CryptoPrefs.GetString("Friend_#" + ClanManager.players[i], "null");
			if (empty == "null")
			{
				list.Add(ClanManager.players[i]);
			}
		}
		if (list.Count != 0)
		{
			GetPlayersNames(list.ToArray());
		}
	}

	public void GetPlayersNames(int[] ids)
	{
		AccountManager.GetFriendsName(ids, delegate
		{
			UpdateList();
		}, delegate(string error)
		{
			UIToast.Show("Get Friends Name Error: " + error);
		});
	}

	private string ConvertTime(long time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		if (timeSpan.Days * 24 + timeSpan.Hours > 0)
		{
			return timeSpan.Days * 24 + timeSpan.Hours + " " + Localization.Get("h") + ".";
		}
		if (timeSpan.Minutes > 0)
		{
			return timeSpan.Minutes + " " + Localization.Get("m") + ".";
		}
		return timeSpan.Seconds + " " + Localization.Get("s") + ".";
	}

	private string ConvertLastLoginTime(long time)
	{
		time += NTPManager.GetMilliSeconds(DateTime.Now) - NTPManager.GetMilliSeconds(DateTime.UtcNow);
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(time).ToLocalTime();
		return dateTime.Day.ToString("D2") + "/" + dateTime.Month.ToString("D2") + "/" + dateTime.Year + " " + dateTime.Hour.ToString("D2") + ":" + dateTime.Minute.ToString("D2") + ":" + dateTime.Second.ToString("D2");
	}
}
