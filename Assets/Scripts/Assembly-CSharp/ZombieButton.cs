using System;
using UnityEngine;

public class ZombieButton : MonoBehaviour
{
	public Team PlayerTeam = Team.Blue;

	[Range(1f, 20f)]
	public int Button = 1;

	public KeyCode Keycode;

	public MeshRenderer ButtonRenderer;

	public Transform ButtonRedBlock;

	private bool isTrigger;

	private bool isClickButton;

	private bool Active = true;

	private void Start()
	{
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
		EventManager.AddListener<byte>("ZombieClickButton", DeactiveButton);
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
	}

	private void GetButtonDown(string name)
	{
		if (name == "Fire" && isTrigger && !isClickButton && ButtonRenderer.isVisible)
		{
			ClickButton();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!isClickButton && other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && component.PlayerTeam == PlayerTeam)
			{
				isTrigger = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!isClickButton && other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && component.PlayerTeam == PlayerTeam)
			{
				isTrigger = false;
			}
		}
	}

	private void StartRound()
	{
		isClickButton = false;
		isTrigger = false;
		Active = false;
		TimerManager.In(nValue.float02, delegate
		{
			Active = true;
			if (ButtonRedBlock != null)
			{
				ButtonRedBlock.localPosition = new Vector3(0f - nValue.float01, nValue.float02, nValue.int0);
			}
		});
	}

	private void ClickButton()
	{
		if (Active && GameManager.roundState == RoundState.PlayRound)
		{
			ZombieMode.ClickButton((byte)Button);
			isClickButton = true;
			isTrigger = false;
		}
	}

	public void DeactiveButton(byte button)
	{
		if (button == Button)
		{
			isClickButton = true;
			isTrigger = false;
			EventManager.Dispatch("Button" + Button);
			if (ButtonRedBlock != null)
			{
				ButtonRedBlock.localPosition = new Vector3(-0.1f, 0.198f, -0.02f);
			}
		}
	}

	[ContextMenu("Click Button")]
	private void GetClickButton()
	{
		ClickButton();
	}
}
