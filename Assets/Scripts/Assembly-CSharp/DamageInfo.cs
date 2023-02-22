using ExitGames.Client.Photon;
using UnityEngine;

public struct DamageInfo
{
	public int damage;

	public Vector3 position;

	public Team team;

	public int weapon;

	public int weaponSkin;

	public int player;

	public bool headshot;

	public bool otherPlayer
	{
		get
		{
			return player != -1 && player != PhotonNetwork.player.ID;
		}
	}

	public void Set(int d, Vector3 p, Team t, int w, int ws, int pl, bool h)
	{
		damage = d;
		position = p;
		team = t;
		weapon = w;
		weaponSkin = ws;
		player = pl;
		headshot = h;
	}

	public static DamageInfo Get()
	{
		return default(DamageInfo);
	}

	public static DamageInfo Get(int damage, Vector3 position, Team team, int weapon, int weaponSkin, int player, bool headshot)
	{
		DamageInfo result = default(DamageInfo);
		result.damage = damage;
		result.position = position;
		result.team = team;
		result.weapon = weapon;
		result.weaponSkin = weaponSkin;
		result.player = player;
		result.headshot = headshot;
		return result;
	}

	public byte[] Deserialize()
	{
		byte[] array = new byte[25];
		int targetOffset = 0;
		byte b = (byte)Random.Range(1, 255);
		Protocol.Serialize(position.x, array, ref targetOffset);
		Protocol.Serialize(position.y, array, ref targetOffset);
		Protocol.Serialize(position.z, array, ref targetOffset);
		Protocol.Serialize(damage, array, ref targetOffset);
		array[16] = (byte)team;
		array[17] = (byte)weapon;
		array[18] = b;
		array[19] = (byte)weaponSkin;
		array[20] = (byte)(headshot ? 1u : 0u);
		targetOffset += 5;
		Protocol.Serialize(player, array, ref targetOffset);
		for (int i = 0; i < array.Length; i++)
		{
			if (i != 18)
			{
				array[i] = (byte)(array[i] ^ b);
			}
		}
		return array;
	}

	public static DamageInfo Serialize(byte[] bytes)
	{
		DamageInfo result = Get();
		int offset = 0;
		byte b = bytes[18];
		for (int i = 0; i < bytes.Length; i++)
		{
			if (i != 18)
			{
				bytes[i] = (byte)(bytes[i] ^ b);
			}
		}
		Protocol.Deserialize(out result.position.x, bytes, ref offset);
		Protocol.Deserialize(out result.position.y, bytes, ref offset);
		Protocol.Deserialize(out result.position.z, bytes, ref offset);
		Protocol.Deserialize(out result.damage, bytes, ref offset);
		result.team = (Team)bytes[16];
		result.weapon = bytes[17];
		result.weaponSkin = bytes[19];
		result.headshot = ((bytes[20] == 1) ? true : false);
		offset += 5;
		Protocol.Deserialize(out result.player, bytes, ref offset);
		return result;
	}
}
