using System;
using ExitGames.Client.Photon;

public static class RoundPlayersData
{
	public class PlayerData
	{
		public PhotonPlayer player;

		public int death;

		public int kills;

		public int ping = 200;

		public bool dead = true;
	}

	private static readonly byte pingKey = 4;

	private static readonly byte updateDataKey = 97;

	private static readonly byte clearKey = 98;

	private static readonly byte allDataKey = 99;

	private static PlayerData localData = new PlayerData();

	private static BetterList<PlayerData> playerList = new BetterList<PlayerData>();

	private static bool init = false;

	private static PhotonDataWrite photonData = new PhotonDataWrite();

	public static void Init()
	{
		if (!init)
		{
			PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
			PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerDisconnected));
			init = true;
		}
	}

	public static void ClearProperties(this PhotonPlayer player)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[PhotonCustomValue.teamKey] = (byte)0;
		player.SetCustomProperties(hashtable);
		if (player.IsLocal)
		{
			localData.death = 0;
			localData.kills = 0;
			localData.dead = true;
			if (PhotonNetwork.room != null)
			{
				photonData.Clear();
				photonData.Write(clearKey);
				PhotonRPC.RPC("UpdateRoundInfo", PhotonTargets.Others, photonData);
			}
		}
	}

	private static void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		PlayerData playerData = new PlayerData();
		playerData.player = player;
		playerList.Add(playerData);
		photonData.Clear();
		photonData.Write(allDataKey);
		photonData.Write((short)localData.death);
		photonData.Write((short)localData.kills);
		photonData.Write((short)localData.ping);
		photonData.Write(localData.dead);
		TimerManager.In(0.5f, delegate
		{
			PhotonRPC.RPC("UpdateRoundInfo", player, photonData);
		});
	}

	private static void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		for (int i = 0; i < playerList.size; i++)
		{
			if (playerList[i].player.ID == player.ID)
			{
				playerList.RemoveAt(i);
				break;
			}
		}
	}

	private static PlayerData GetData(PhotonPlayer player)
	{
		for (int i = 0; i < playerList.size; i++)
		{
			if (playerList[i].player.ID == player.ID)
			{
				return playerList[i];
			}
		}
		return null;
	}

	public static void SetDeaths(this PhotonPlayer player, int deaths)
	{
		if (player.IsLocal)
		{
			localData.death = deaths;
			photonData.Clear();
			photonData.Write(updateDataKey);
			photonData.Write((short)localData.death);
			photonData.Write((short)localData.kills);
			photonData.Write(localData.dead);
			PhotonRPC.RPC("UpdateRoundInfo", PhotonTargets.Others, photonData);
		}
	}

	public static void SetDeaths1(this PhotonPlayer player)
	{
		if (player.IsLocal)
		{
			int deaths = player.GetDeaths();
			deaths++;
			player.SetDeaths(deaths);
		}
	}

	public static int GetDeaths(this PhotonPlayer player)
	{
		if (player.IsLocal)
		{
			return localData.death;
		}
		for (int i = 0; i < playerList.size; i++)
		{
			if (playerList[i].player.ID == player.ID)
			{
				return playerList[i].death;
			}
		}
		return 0;
	}

	public static void SetKills(this PhotonPlayer player, int kills)
	{
		if (player.IsLocal)
		{
			localData.kills = kills;
			photonData.Clear();
			photonData.Write(updateDataKey);
			photonData.Write((short)localData.death);
			photonData.Write((short)localData.kills);
			photonData.Write(localData.dead);
			PhotonRPC.RPC("UpdateRoundInfo", PhotonTargets.Others, photonData);
		}
	}

	public static void SetKills1(this PhotonPlayer player)
	{
		if (player.IsLocal)
		{
			int kills = player.GetKills();
			kills++;
			player.SetKills(kills);
		}
	}

	public static int GetKills(this PhotonPlayer player)
	{
		if (player.IsLocal)
		{
			return localData.kills;
		}
		for (int i = 0; i < playerList.size; i++)
		{
			if (playerList[i].player.ID == player.ID)
			{
				return playerList[i].kills;
			}
		}
		return 0;
	}

	public static void SetDead(this PhotonPlayer player, bool dead)
	{
		if (player.IsLocal)
		{
			localData.dead = dead;
			photonData.Clear();
			photonData.Write(updateDataKey);
			photonData.Write((short)localData.death);
			photonData.Write((short)localData.kills);
			photonData.Write(localData.dead);
			PhotonRPC.RPC("UpdateRoundInfo", PhotonTargets.Others, photonData);
		}
	}

	public static bool GetDead(this PhotonPlayer player)
	{
		if (player.IsLocal)
		{
			return localData.dead;
		}
		for (int i = 0; i < playerList.size; i++)
		{
			if (playerList[i].player.ID == player.ID)
			{
				return playerList[i].dead;
			}
		}
		return false;
	}

	public static void UpdatePing(this PhotonPlayer player)
	{
		if (player.IsLocal)
		{
			localData.ping = PhotonNetwork.GetPing();
			photonData.Clear();
			photonData.Write(pingKey);
			photonData.Write((short)localData.ping);
			PhotonRPC.RPC("UpdateRoundInfo", PhotonTargets.Others, photonData);
		}
	}

	public static int GetPing(this PhotonPlayer player)
	{
		if (player.IsLocal)
		{
			return localData.ping;
		}
		for (int i = 0; i < playerList.size; i++)
		{
			if (playerList[i].player.ID == player.ID)
			{
				return playerList[i].ping;
			}
		}
		return 200;
	}

	[PunRPC]
	public static void UpdateRoundInfo(PhotonMessage message)
	{
		byte b = message.ReadByte();
		if (b == 99)
		{
			PlayerData playerData = new PlayerData();
			playerData.player = message.sender;
			playerData.death = message.ReadShort();
			playerData.kills = message.ReadShort();
			playerData.ping = message.ReadShort();
			playerData.dead = message.ReadBool();
			playerList.Add(playerData);
			return;
		}
		PlayerData data = GetData(message.sender);
		if (data == null)
		{
			data = new PlayerData();
			data.player = message.sender;
			data.death = 0;
			data.kills = 0;
			data.dead = true;
			playerList.Add(data);
			return;
		}
		switch (b)
		{
		case 2:
			data.death = message.ReadShort();
			break;
		case 3:
			data.kills = message.ReadShort();
			break;
		case 4:
			data.ping = message.ReadShort();
			break;
		case 5:
			data.dead = message.ReadBool();
			break;
		case 97:
			data.death = message.ReadShort();
			data.kills = message.ReadShort();
			data.dead = message.ReadBool();
			break;
		case 98:
			data.death = 0;
			data.kills = 0;
			data.dead = true;
			break;
		}
	}
}
