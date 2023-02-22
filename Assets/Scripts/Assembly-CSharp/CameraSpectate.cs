using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpectate : MonoBehaviour
{
	public CameraManager cameraManager;

	public float distance;

	public float distanceMin = 0.5f;

	public float distanceMax = 3f;

	public float speedRotate = 2.5f;

	private int index;

	private Transform target;

	private Vector2 rotate;

	private Vector3 negDistance = Vector3.zero;

	private Ray ray = default(Ray);

	private RaycastHit raycastHit;

	private Transform cameraTransform;

	private void Start()
	{
		cameraTransform = cameraManager.cameraTransform;
	}

	public void Active()
	{
		Active(-1);
	}

	public void Active(object[] parameters)
	{
		if (parameters != null && parameters.Length > 0)
		{
			Active((int)parameters[0]);
		}
		else
		{
			Active(-1);
		}
	}

	public void Active(int playerID)
	{
		cameraTransform.gameObject.SetActive(true);
		UpdateSelectPlayer(playerID);
		LODObject.Target = cameraTransform;
		SkyboxManager.GetCameraParent().localEulerAngles = Vector3.zero;
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
		InputManager.GetAxisEvent = (InputManager.AxisDelegate)Delegate.Combine(InputManager.GetAxisEvent, new InputManager.AxisDelegate(GetAxis));
	}

	public void Deactive()
	{
		if (CameraManager.type == CameraType.Spectate)
		{
			if (cameraTransform != null)
			{
				cameraTransform.gameObject.SetActive(false);
			}
			InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
			InputManager.GetAxisEvent = (InputManager.AxisDelegate)Delegate.Remove(InputManager.GetAxisEvent, new InputManager.AxisDelegate(GetAxis));
		}
	}

	private void GetButtonDown(string name)
	{
		if (CameraManager.type == CameraType.Spectate && name == "Fire")
		{
			UpdateSelectPlayer();
		}
	}

	private void GetAxis(string name, float value)
	{
		if (CameraManager.type == CameraType.Spectate)
		{
			switch (name)
			{
			case "Mouse X":
				rotate.x += value * speedRotate * distance;
				break;
			case "Mouse Y":
				rotate.y -= value * speedRotate;
				break;
			}
		}
	}

	public void OnUpdate()
	{
		if (CameraManager.type == CameraType.Spectate && !(target == null))
		{
			distance = distanceMax;
			rotate.y = ClampAngle(rotate.y, -20f, 80f);
			Quaternion quaternion = Quaternion.Euler(rotate.y, rotate.x, 0f);
			ray.origin = target.position;
			ray.direction = (cameraTransform.position - target.position).normalized;
			if (Physics.SphereCast(ray.origin, 0.25f, ray.direction, out raycastHit, distance))
			{
				distance = Mathf.Clamp(raycastHit.distance, distanceMin, distanceMax);
			}
			negDistance.z = 0f - distance;
			Vector3 position = quaternion * negDistance + target.position;
			cameraTransform.position = position;
			cameraTransform.rotation = quaternion;
			SkyboxManager.GetCamera().rotation = quaternion;
		}
	}

	public void UpdateSelectPlayer()
	{
		UpdateSelectPlayer(-1);
	}

	public void UpdateSelectPlayer(int playerID)
	{
		if (CameraManager.type != CameraType.Spectate)
		{
			return;
		}
		if (playerID != -1)
		{
			for (int i = 0; i < ControllerManager.ControllerList.Count; i++)
			{
				ControllerManager controllerManager = ControllerManager.ControllerList[i];
				if (controllerManager.photonView.ownerId == playerID)
				{
					if (controllerManager.playerSkin != null && controllerManager.playerSkin.isPlayerActive)
					{
						target = controllerManager.playerSkin.PlayerSpectatePoint;
						cameraManager.OnSelectPlayer(playerID);
						return;
					}
					break;
				}
			}
		}
		List<PlayerSkin> list = new List<PlayerSkin>();
		for (int j = 0; j < ControllerManager.ControllerList.Count; j++)
		{
			ControllerManager controllerManager = ControllerManager.ControllerList[j];
			if (controllerManager.playerSkin != null && controllerManager.playerSkin.isPlayerActive)
			{
				list.Add(controllerManager.playerSkin);
			}
		}
		if (CameraManager.Team)
		{
			Team team = PhotonNetwork.player.GetTeam();
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].PlayerTeam != team)
				{
					list.RemoveAt(k);
					k--;
				}
			}
			index++;
			if (index > list.Count - 1)
			{
				index = 0;
			}
			if (list.Count != 0)
			{
				target = list[index].PlayerSpectatePoint;
				cameraManager.OnSelectPlayer(list[index].Controller.photonView.ownerId);
			}
			else if (PhotonNetwork.player.GetTeam() != 0)
			{
				CameraManager.SetType(CameraType.Static);
				cameraManager.OnSelectPlayer(-1);
			}
			list = null;
		}
		else
		{
			index++;
			if (index > list.Count - 1)
			{
				index = 0;
			}
			if (list.Count != 0)
			{
				target = list[index].PlayerSpectatePoint;
				cameraManager.OnSelectPlayer(list[index].Controller.photonView.ownerId);
			}
			else if (PhotonNetwork.player.GetTeam() != 0)
			{
				CameraManager.SetType(CameraType.Static);
				cameraManager.OnSelectPlayer(-1);
			}
		}
	}

	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
