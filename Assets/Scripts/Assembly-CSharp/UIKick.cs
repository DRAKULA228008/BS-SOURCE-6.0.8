using System;
using System.Collections.Generic;
using FreeJSON;
using Photon;
using UnityEngine;

public class UIKick : Photon.MonoBehaviour
{
	private float lastKickTime;

	private Dictionary<string, List<string>> kickList = new Dictionary<string, List<string>>();

	private List<string> kicked = new List<string>();

	private void Start()
	{
		PhotonRPC.AddMessage("PhotonKickPlayer", PhotonKickPlayer);
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
	}

	private void OnDisable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (kicked.Contains(playerConnect.GetPlayerID().ToString()) || string.IsNullOrEmpty(playerConnect.GetPlayerID().ToString()))
		{
			PhotonNetwork.CloseConnection(playerConnect);
			return;
		}
		TimerManager.In(1f, delegate
		{
			JsonObject jsonObject = new JsonObject();
			if (kickList.Count > 0)
			{
				jsonObject.Add("1", kickList);
			}
			if (kicked.Count > 0)
			{
				jsonObject.Add("2", kicked);
			}
			if (jsonObject.Length > 0)
			{
				PhotonDataWrite data = PhotonRPC.GetData();
				data.Write(3);
				data.Write(jsonObject.ToString());
				PhotonRPC.RPC("PhotonKickPlayer", playerConnect, data);
			}
		});
	}

	public void Kick()
	{
		if (lastKickTime > Time.time)
		{
			UIToast.Show(ConvertTime(lastKickTime - Time.time));
			return;
		}
		if (kickList.ContainsKey(UIPlayerStatistics.SelectPlayer.GetPlayerID().ToString()) && kickList[UIPlayerStatistics.SelectPlayer.GetPlayerID().ToString()].Contains(PhotonNetwork.player.GetPlayerID().ToString()))
		{
			UIToast.Show("Error");
			return;
		}
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write(1);
		data.Write(UIPlayerStatistics.SelectPlayer.ID);
		PhotonRPC.RPC("PhotonKickPlayer", PhotonTargets.All, data);
		lastKickTime = Time.time + 300f;
		UIToast.Show("Ok");
	}

	[PunRPC]
	private void PhotonKickPlayer(PhotonMessage message)
	{
		if (message.ReadInt() == 1)
		{
			PhotonPlayer player = PhotonPlayer.Find(message.ReadInt());
			if (player == null)
			{
				return;
			}
			if (kickList.ContainsKey(player.GetPlayerID().ToString()))
			{
				kickList[player.GetPlayerID().ToString()].Add(message.sender.GetPlayerID().ToString());
			}
			else
			{
				kickList[player.GetPlayerID().ToString()] = new List<string> { message.sender.GetPlayerID().ToString() };
			}
			if (!PhotonNetwork.isMasterClient)
			{
				return;
			}
			int num = kickList[player.GetPlayerID().ToString()].Count * 100;
			if (num / PhotonNetwork.room.PlayerCount >= 60)
			{
				PhotonDataWrite data = PhotonRPC.GetData();
				data.Write(2);
				data.Write(player.ID);
				PhotonRPC.RPC("PhotonKickPlayer", PhotonTargets.All, data);
				TimerManager.In(1.5f, delegate
				{
					PhotonNetwork.CloseConnection(player);
				});
			}
		}
		else if (message.ReadInt() == 2)
		{
			PhotonPlayer photonPlayer = PhotonPlayer.Find(message.ReadInt());
			if (photonPlayer != null)
			{
				kicked.Add(photonPlayer.GetPlayerID().ToString());
				if (PhotonNetwork.player.ID == photonPlayer.ID)
				{
					GameManager.leaveRoomMessage = Localization.Get("You kicked from the server");
				}
			}
		}
		else if (message.ReadInt() == 3)
		{
			JsonObject jsonObject = JsonObject.Parse(message.ReadString());
			if (jsonObject.ContainsKey("1"))
			{
				kickList = jsonObject.Get<Dictionary<string, List<string>>>("1");
			}
			if (jsonObject.ContainsKey("2"))
			{
				kicked = jsonObject.Get<List<string>>("2");
			}
		}
	}

	private static string ConvertTime(float time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		return string.Format("{0:0}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
	}
}
