using UnityEngine;

public class TriggerTeleport : MonoBehaviour
{
	public Transform Target;

	public Vector3 Position;

	public SpawnPoint TargetElement;

	private BoxCollider boxCollider;

	private Bounds bounds;

	private void Start()
	{
		base.gameObject.layer = 2;
		boxCollider = GetComponent<BoxCollider>();
		if (!(boxCollider == null))
		{
			bounds = boxCollider.bounds;
			if (LevelManager.customScene)
			{
				Target = base.transform.Find("Finish");
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (!(component == null) && bounds.Intersects(component.mCharacterController.bounds))
		{
			if (Position != Vector3.zero)
			{
				component.Controller.SetPosition(Position);
			}
			else if (Target != null)
			{
				component.Controller.SetPosition(Target.position);
				component.FPCamera.SetRotation(Target.eulerAngles);
			}
			else
			{
				component.Controller.SetPosition(TargetElement.spawnPosition);
			}
		}
	}
}
