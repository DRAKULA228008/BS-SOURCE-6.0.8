using System;
using UnityEngine;

public class UIOthers : MonoBehaviour
{
	public UICamera cam;

	public UIGrid buttonGrid;

	public GameObject weaponButton;

	public static event Action pauseEvent;

	private void Start()
	{
		EventManager.AddListener("OnSettings", OnSettings);
		OnSettings();
		UICamera.clickUIInput = false;
		cam.useMouse = false;
		cam.useKeyboard = false;
		cam.useController = false;
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
		PhotonNetwork.onLeftRoom = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onLeftRoom, new PhotonNetwork.VoidDelegate(OnLeftRoom));
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
		PhotonNetwork.onLeftRoom = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onLeftRoom, new PhotonNetwork.VoidDelegate(OnLeftRoom));
	}

	private void GetButtonDown(string name)
	{
		if (!(name != "Pause"))
		{
			UIPanelManager.ShowPanel("Pause");
			if (UIOthers.pauseEvent != null)
			{
				UIOthers.pauseEvent();
			}
			if (!GameManager.changeWeapons)
			{
				weaponButton.SetActive(false);
			}
			buttonGrid.Reposition();
		}
	}

	private void OnSettings()
	{
		UIElements.Get<UIPanel>("DisplayPanel").alpha = ((!Settings.HUD) ? 0f : 1f);
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	private void OnLeftRoom()
	{
		PlayerRoundManager.Show();
		LevelManager.customScene = false;
		PhotonNetwork.leavingRoom = false;
		LevelManager.LoadLevel("Menu");
        PhotonEvent.Clear();
    }
}
