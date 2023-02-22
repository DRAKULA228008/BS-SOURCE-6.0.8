using Photon;
using UnityEngine;

public class ZombieWavesSpawnManager : Photon.MonoBehaviour
{
	public int count = 5;

	public int maxInScene = 2;

	public int alive;

	public int deads;

	public Transform[] spawns;

	public GameObject zombiePrefab;

	private void Start()
	{
		base.photonView.AddMessage("PhotonCreateZombie", PhotonCreateZombie);
		TimerManager.In(1f, -1, 1f, CheckScene);
		PlayerAI.deadEvent += DeadZombie;
		PlayerAI.startEvent += StartZombie;
	}

	private void CheckScene()
	{
		if (count - deads > alive && PlayerAI.list.Count < maxInScene)
		{
			PhotonNetwork.InstantiateSceneObject("Player/PlayerAI", spawns[Random.Range(0, spawns.Length)].position, Quaternion.identity, 0, null);
		}
	}

	private void StartZombie(PlayerAI ai)
	{
		ai.dead = false;
		ai.health = 1000;
		alive++;
	}

	private void DeadZombie(PlayerAI ai)
	{
		deads++;
		alive--;
		if (count <= deads && alive == 0)
		{
			UIToast.Show("Pause 10 sec");
			TimerManager.In(10f, delegate
			{
				count += 5;
				maxInScene++;
				deads = 0;
			});
		}
	}

	private void CreateZombie()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write((byte)Random.Range(0, spawns.Length));
			base.photonView.RPC("PhotonCreateZombie", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void PhotonCreateZombie(PhotonMessage message)
	{
		byte b = message.ReadByte();
		PoolManager.Spawn("Zombie", zombiePrefab, spawns[b].position, Quaternion.identity.eulerAngles);
	}
}
