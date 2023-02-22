using UnityEngine;

public class CameraDead : MonoBehaviour
{
	public CameraManager cameraManager;

	public Rigidbody cameraRigidbody;

	public Transform cameraTransform;

	public BoxCollider cameraBoxCollider;

	private void Start()
	{
		cameraRigidbody.detectCollisions = false;
	}

	public void Active(object[] parameters)
	{
		Active((Vector3)parameters[0], (Vector3)parameters[1], (Vector3)parameters[2]);
	}

	public void Active(Vector3 position, Vector3 rotation, Vector3 force)
	{
		cameraTransform.gameObject.SetActive(true);
		cameraBoxCollider.isTrigger = false;
		cameraRigidbody.isKinematic = false;
		cameraRigidbody.detectCollisions = true;
		cameraTransform.position = position;
		cameraTransform.eulerAngles = rotation;
		cameraRigidbody.velocity = Vector3.zero;
		cameraRigidbody.AddForce(force);
		cameraRigidbody.AddRelativeForce(force);
		LODObject.Target = cameraTransform;
		SkyboxManager.GetCameraParent().localEulerAngles = Vector3.zero;
	}

	public void Deactive()
	{
		if (CameraManager.type == CameraType.Dead)
		{
			cameraRigidbody.isKinematic = true;
			cameraRigidbody.detectCollisions = false;
			cameraBoxCollider.isTrigger = true;
			cameraTransform.gameObject.SetActive(false);
		}
	}

	public void OnUpdate()
	{
		if (CameraManager.type == CameraType.Dead)
		{
			SkyboxManager.GetCamera().rotation = cameraTransform.rotation;
		}
	}
}
