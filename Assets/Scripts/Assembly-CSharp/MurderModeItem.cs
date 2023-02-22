using UnityEngine;

public class MurderModeItem : MonoBehaviour
{
	public enum ItemList
	{
		Weapon,
		Clue
	}

	public int ID;

	public ItemList Item;

	public bool Active;

	private void OnTriggerEnter(Collider other)
	{
		if (GameManager.roundState != RoundState.EndRound && other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (!(component != null))
			{
			}
		}
	}
}
