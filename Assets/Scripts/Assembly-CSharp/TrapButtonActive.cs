using UnityEngine;

public class TrapButtonActive : MonoBehaviour
{
	[Range(1f, 30f)]
	public int Key;

	public TrapButton[] Buttons;

	private void Start()
	{
		EventManager.AddListener<byte>("DeathRunClickButton", DeactiveButtons);
	}

	private void DeactiveButtons(byte button)
	{
		if (button == Key)
		{
			for (int i = 0; i < Buttons.Length; i++)
			{
				Buttons[i].DeactiveButton();
			}
		}
	}
}
