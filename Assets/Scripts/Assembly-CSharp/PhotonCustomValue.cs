using ExitGames.Client.Photon;

internal static class PhotonCustomValue
{
	public static string teamKey = "t";

	public static string levelKey = "l";

	public static string clanKey = "c";

	public static string pingKey = "p";

	public static string deathsKey = "d";

	public static string killsKey = "k";

	public static string deadKey = "de";

	public static string playerIDKey = "pl";

	public static string avatarKey = "a";

	public static string gameModeKey = "g";

	public static string onlyWeaponKey = "o";

	public static string roundStateKey = "r";

	public static string sceneNameKey = "s";

	public static string passwordKey = "p";

	public static string officialServerKey = "of";

	public static string minLevelKey = "ml";

	public static string customMapModes = "c1";

	public static string customMapHash = "c2";

	public static string customMapUrl = "c3";

	public static void SetTeam(this PhotonPlayer player, Team team)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[teamKey] = (byte)team;
		player.SetCustomProperties(hashtable);
	}

	public static Team GetTeam(this PhotonPlayer player)
	{
		object value;
		if (player.CustomProperties.TryGetValue(teamKey, out value))
		{
			return (Team)(byte)value;
		}
		return Team.None;
	}

	public static void SetLevel(this PhotonPlayer player, int level)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[levelKey] = (byte)level;
		player.SetCustomProperties(hashtable);
	}

	public static int GetLevel(this PhotonPlayer player)
	{
		object value;
		if (player.CustomProperties.TryGetValue(levelKey, out value))
		{
			return (byte)value;
		}
		return 1;
	}

	public static void SetClan(this PhotonPlayer player, string clan)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[clanKey] = clan;
		player.SetCustomProperties(hashtable);
	}

	public static string GetClan(this PhotonPlayer player)
	{
		object value;
		if (player.CustomProperties.TryGetValue(clanKey, out value))
		{
			return (string)value;
		}
		return string.Empty;
	}

	public static void SetPlayerID(this PhotonPlayer player, int id)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[playerIDKey] = id;
		player.SetCustomProperties(hashtable);
	}

	public static int GetPlayerID(this PhotonPlayer player)
	{
		object value;
		if (player.CustomProperties.TryGetValue(playerIDKey, out value))
		{
			return (int)value;
		}
		return 0;
	}

	public static void SetAvatarUrl(this PhotonPlayer player, string url)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[avatarKey] = url;
		player.SetCustomProperties(hashtable);
	}

	public static string GetAvatarUrl(this PhotonPlayer player)
	{
		object value;
		if (player.CustomProperties.TryGetValue(avatarKey, out value))
		{
			return (string)value;
		}
		return string.Empty;
	}

	public static void SetGameMode(this Room room, GameMode mode)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[gameModeKey] = (byte)mode;
		room.SetCustomProperties(hashtable);
	}

	public static GameMode GetGameMode(this Room room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(gameModeKey, out value))
		{
			return (GameMode)(byte)value;
		}
		return GameMode.TeamDeathmatch;
	}

	public static void SetOnlyWeapon(this Room room, int weaponID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[onlyWeaponKey] = (byte)weaponID;
		room.SetCustomProperties(hashtable);
	}

	public static int GetOnlyWeapon(this Room room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(onlyWeaponKey, out value))
		{
			return (byte)value;
		}
		return 1;
	}

	public static void SetRoundState(this Room room, RoundState state)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[roundStateKey] = (byte)state;
		room.SetCustomProperties(hashtable);
	}

	public static RoundState GetRoundState(this Room room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(roundStateKey, out value))
		{
			return (RoundState)(byte)value;
		}
		return RoundState.WaitPlayer;
	}

	public static GameMode GetGameMode(this RoomInfo room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(gameModeKey, out value))
		{
			return (GameMode)(byte)value;
		}
		return GameMode.TeamDeathmatch;
	}

	public static string GetSceneName(this RoomInfo room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(sceneNameKey, out value))
		{
			return (string)value;
		}
		return string.Empty;
	}

	public static int GetOnlyWeapon(this RoomInfo room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(onlyWeaponKey, out value))
		{
			return (byte)value;
		}
		return 1;
	}

	public static Hashtable CreateRoomHashtable(this RoomInfo photonNetwork, string password, GameMode mode, bool officialServer)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[passwordKey] = password;
		hashtable[gameModeKey] = (byte)mode;
		hashtable[officialServerKey] = officialServer;
		return hashtable;
	}

	public static string GetPassword(this RoomInfo room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(passwordKey, out value))
		{
			return (string)value;
		}
		return string.Empty;
	}

	public static bool HasPassword(this RoomInfo room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(passwordKey, out value))
		{
			return !string.IsNullOrEmpty((string)value);
		}
		return false;
	}

	public static int GetCustomMapHash(this RoomInfo room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(customMapHash, out value))
		{
			return (int)value;
		}
		return 0;
	}

	public static string GetCustomMapUrl(this RoomInfo room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(customMapUrl, out value))
		{
			return (string)value;
		}
		return string.Empty;
	}

	public static int[] GetCustomMapModes(this RoomInfo room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(customMapModes, out value))
		{
			return (int[])value;
		}
		return new int[0];
	}

	public static bool isCustomMap(this RoomInfo room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(customMapHash, out value))
		{
			return ((int)value != 0) ? true : false;
		}
		return false;
	}

	public static bool isOfficialServer(this RoomInfo room)
	{
		object value;
		if (room != null && room.CustomProperties.TryGetValue(officialServerKey, out value))
		{
			return (bool)value;
		}
		return false;
	}

	public static void SetMinLevel(this Room room, byte level)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[minLevelKey] = level;
		room.SetCustomProperties(hashtable);
	}

	public static byte GetMinLevel(this RoomInfo room)
	{
		object value;
		if (room.CustomProperties.TryGetValue(minLevelKey, out value))
		{
			return (byte)value;
		}
		return 1;
	}
}
