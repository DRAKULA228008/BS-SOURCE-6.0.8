using System.Collections.Generic;
using UnityEngine;

public class UIPlayerList : MonoBehaviour
{
	public UIGrid Grid;

	public GameObject Element;

	public UILabel PlayerNameLabel;

	public UILabel LevelLabel;

	public UILabel KillsLabel;

	public UILabel DeathsLabel;

	public UILabel PingLabel;

	private List<Transform> PlayerList = new List<Transform>();

	private List<Transform> PlayerListPool = new List<Transform>();

	public void Active()
	{
		PlayerNameLabel.gameObject.SetActive(false);
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		for (int i = 0; i < playerList.Length; i++)
		{
			Transform transform = GetGameObject().transform;
			transform.name = playerList[i].UserId;
			transform.GetComponent<UILabel>().text = playerList[i].UserId;
		}
		Grid.repositionNow = true;
	}

	public void SelectPlayer(Transform playerTransform)
	{
		PhotonPlayer player = GetPlayer(playerTransform.name);
		if (player != null)
		{
			PlayerNameLabel.gameObject.SetActive(true);
			if (player.GetTeam() == Team.Blue)
			{
				PlayerNameLabel.text = "[00c5ff]" + player.UserId;
			}
			else if (player.GetTeam() == Team.Red)
			{
				PlayerNameLabel.text = "[ff0000]" + player.UserId;
			}
			KillsLabel.text = Localization.Get("Kills") + ": " + player.GetKills();
			DeathsLabel.text = Localization.Get("Deaths") + ": " + player.GetDeaths();
			PingLabel.text = Localization.Get("Ping") + ": " + player.GetPing();
		}
	}

	private PhotonPlayer GetPlayer(string playerName)
	{
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			if (PhotonNetwork.playerList[i].UserId == playerName)
			{
				return PhotonNetwork.playerList[i];
			}
		}
		return null;
	}

	private GameObject GetGameObject()
	{
		GameObject gameObject;
		if (PlayerListPool.Count != 0)
		{
			gameObject = PlayerListPool[0].gameObject;
			PlayerListPool.RemoveAt(0);
		}
		else
		{
			gameObject = Grid.gameObject.AddChild(Element);
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void ClearList()
	{
		if (PlayerList.Count != 0)
		{
			for (int i = 0; i < PlayerList.Count; i++)
			{
				PlayerList[i].gameObject.SetActive(false);
				PlayerListPool.Add(PlayerList[i]);
			}
			PlayerList.Clear();
		}
	}
}
