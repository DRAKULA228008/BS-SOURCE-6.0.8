using System;
using UnityEngine;

public class ChunkCube : MonoBehaviour
{
	public Transform cube;

	public float distance;

	public float minDistance;

	public Transform player;

	private RaycastHit hit;

	private Ray ray = default(Ray);

	private Vector3 pos;

	private bool isHit;

	private bool isActive;

	private void Start()
	{
		TimerManager.In(0.2f, delegate
		{
			player = GameManager.player.FPCamera.Transform;
		});
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
		if (name == "Fire" && isHit && (int)GameManager.player.PlayerWeapon.KnifeData.ID == 47)
		{
			ChunkManager.AddCube((int)pos.x, (int)pos.y, (int)pos.z, 1);
		}
	}

	private void Update()
	{
		if (!isActive && GameManager.player.PlayerWeapon.SelectedWeapon == WeaponType.Knife && (int)GameManager.player.PlayerWeapon.KnifeData.ID == 47)
		{
			isActive = true;
		}
		else if (isActive && GameManager.player.PlayerWeapon.SelectedWeapon != WeaponType.Knife)
		{
			isActive = false;
			cube.position = Vector3.down * 10f;
		}
		if (!isActive || Time.frameCount % 2 != 1)
		{
			return;
		}
		ray.origin = player.position;
		ray.direction = player.forward;
		if (Physics.Raycast(ray, out hit, distance))
		{
			if (hit.distance >= minDistance)
			{
				if (hit.transform.CompareTag("ChunkBlock"))
				{
					pos = ray.GetPoint(hit.distance - 0.5f);
					pos = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
					cube.position = pos;
					isHit = true;
				}
			}
			else
			{
				cube.position = Vector3.down * 10f;
				isHit = false;
			}
		}
		else
		{
			cube.position = Vector3.down * 10f;
			isHit = false;
		}
	}
}
