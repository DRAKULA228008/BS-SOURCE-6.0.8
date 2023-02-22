using UnityEngine;

public class ScoreLevelManager : MonoBehaviour
{
	public TextMesh label;

	private void Start()
	{
		EventManager.AddListener("UpdateScore", UpdateScore);
		UpdateScore();
	}

	private void UpdateScore()
	{
		label.text = string.Concat(GameManager.redScore, ":", GameManager.blueScore);
	}
}
