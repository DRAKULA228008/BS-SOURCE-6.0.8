using UnityEngine;

public class CameraStatic : MonoBehaviour
{
	public CameraManager cameraManager;

	private Transform cameraTransform;

	public void Active()
	{
		cameraTransform = cameraManager.cameraTransform;
		cameraTransform.gameObject.SetActive(true);
		cameraTransform.position = cameraManager.cameraStaticPoint.position;
		cameraTransform.rotation = cameraManager.cameraStaticPoint.rotation;
		LODObject.Target = cameraTransform;
		SkyboxManager.GetCameraParent().localEulerAngles = Vector3.zero;
		SkyboxManager.GetCamera().rotation = cameraTransform.rotation;
	}

	public void Deactive()
	{
		if (CameraManager.type == CameraType.Static)
		{
			cameraTransform = cameraManager.cameraTransform;
			cameraTransform.gameObject.SetActive(false);
		}
	}
}
