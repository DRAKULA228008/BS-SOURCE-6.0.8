using System;
using UnityEngine;

public class TrapButton : MonoBehaviour
{
	public Team PlayerTeam = Team.Red;

	[Range(1f, 30f)]
	public int Key;

	public KeyCode Keycode;

	public MeshRenderer ButtonRenderer;

	public Transform ButtonRedBlock;

	[Header("Weapon Settings")]
	public bool Weapon;

	[SelectedWeapon(WeaponType.Rifle)]
	public int SelectWeapon;

	private bool isTrigger;

	private bool isClickButton;

	private bool Active = true;

	private void Start()
	{
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
		EventManager.AddListener<byte>("DeathRunClickButton", DeactiveButton);
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
		if (name == "Fire" && GameManager.roundState == RoundState.PlayRound && isTrigger && !isClickButton && ButtonRenderer.isVisible)
		{
			ClickButton();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!isClickButton && other.CompareTag("Player") && PhotonNetwork.player.GetTeam() == PlayerTeam)
		{
			isTrigger = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!isClickButton && other.CompareTag("Player") && PhotonNetwork.player.GetTeam() == PlayerTeam)
		{
			isTrigger = false;
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
				ButtonRedBlock.localPosition = Vector3.zero;
			}
		});
	}

	private void ClickButton()
	{
		if (!Active)
		{
			return;
		}
		DeathRun.ClickButton((byte)Key);
		isClickButton = true;
		isTrigger = false;
		if (Weapon)
		{
			GameManager.player.PlayerWeapon.CanFire = false;
			TimerManager.In(nValue.float005, delegate
			{
				WeaponManager.SetSelectWeapon(WeaponType.Rifle, SelectWeapon);
				GameManager.player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle);
				GameManager.player.PlayerWeapon.CanFire = true;
			});
		}
	}

	public void DeactiveButton(byte button)
	{
		if (button == Key)
		{
			DeactiveButton();
			EventManager.Dispatch("Button" + Key);
		}
	}

	public void DeactiveButton()
	{
		isClickButton = true;
		isTrigger = false;
		if (ButtonRedBlock != null)
		{
			ButtonRedBlock.localPosition = Vector3.down * nValue.float005;
		}
	}

	[ContextMenu("Click Button")]
	private void GetClickButton()
	{
		ClickButton();
	}
}
