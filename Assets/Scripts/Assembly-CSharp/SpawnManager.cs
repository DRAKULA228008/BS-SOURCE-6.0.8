using System.Collections.Generic;
using FreeJSON;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	public SpawnPoint blue;

	public SpawnPoint red;

	public SpawnPoint[] random;

	public SpawnPoint cameraStatic;

	private static SpawnManager instance;

	private void Awake()
	{
		instance = this;
	}

	public static SpawnPoint GetTeamSpawn()
	{
		return GetTeamSpawn(GameManager.team);
	}

	public static SpawnPoint GetCameraStatic()
	{
		return instance.cameraStatic;
	}

	public static SpawnPoint GetTeamSpawn(Team team)
	{
		switch (team)
		{
		case Team.Blue:
			return instance.blue;
		case Team.Red:
			return instance.red;
		default:
			return null;
		}
	}

	public static SpawnPoint GetSpawn(int index)
	{
		return instance.random[index];
	}

	public static SpawnPoint GetRandomSpawn()
	{
		return instance.random[Random.Range(0, instance.random.Length)];
	}

	public static SpawnPoint GetPlayerIDSpawn()
	{
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		for (int i = 0; i < playerList.Length; i++)
		{
			list.Add(playerList[i]);
		}
		list.Sort(SortPlayerID);
		int iD = PhotonNetwork.player.ID;
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].ID == iD)
			{
				return instance.random[j];
			}
		}
		return instance.random[0];
	}

	private static int SortPlayerID(PhotonPlayer a, PhotonPlayer b)
	{
		return a.ID.CompareTo(b.ID);
	}

	public static void SetCustomMapData(JsonObject json)
	{
		JsonObject jsonObject = json.Get<JsonObject>("BlueSpawn");
		instance.blue.cachedTransform.position = jsonObject.Get<Vector3>("position");
		instance.blue.cachedTransform.rotation = jsonObject.Get<Quaternion>("rotation");
		instance.blue.cachedTransform.localScale = jsonObject.Get<Vector3>("localScale");
		jsonObject = json.Get<JsonObject>("RedSpawn");
		instance.red.cachedTransform.position = jsonObject.Get<Vector3>("position");
		instance.red.cachedTransform.rotation = jsonObject.Get<Quaternion>("rotation");
		instance.red.cachedTransform.localScale = jsonObject.Get<Vector3>("localScale");
	}
}
