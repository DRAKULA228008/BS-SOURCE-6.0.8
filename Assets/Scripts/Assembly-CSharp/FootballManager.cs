using UnityEngine;

public class FootballManager : MonoBehaviour
{
	public GameObject Ball;

	public Vector3 StartBallPosition;

	private static FootballManager instance;

	private void Awake()
	{
		instance = this;
	}

	public static void StartRound()
	{
		instance.Ball.SetActive(true);
		if (PhotonNetwork.isMasterClient)
		{
			instance.Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
			instance.Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
		instance.Ball.transform.position = instance.StartBallPosition;
	}

	public static void FinishRound()
	{
		instance.Ball.SetActive(false);
		if (PhotonNetwork.isMasterClient)
		{
			instance.Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
			instance.Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
		instance.Ball.transform.position = instance.StartBallPosition;
	}
}
