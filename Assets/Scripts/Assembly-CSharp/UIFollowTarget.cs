using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
	public Transform target;

	public Camera uiCamera;

	public UIWidget widget;

	public UISprite sprite;

	public UILabel label;

	private Camera gameCamera;

	private Transform gameCameraTransform;

	public float alphaDistance;

	public float alphaDistance2;

	public Vector3 customPos;

	public Rect minMaxPos = new Rect(0.1f, 0.9f, 0.1f, 0.9f);

	private Vector3 pos;

	private bool isActive;

	private static UIFollowTarget instance;

	private void Start()
	{
		instance = this;
	}

	public static void SetTarget(Transform target, Camera cam, string text, Color color)
	{
		instance.widget.cachedGameObject.SetActive(true);
		instance.target = target;
		instance.gameCamera = cam;
		instance.gameCameraTransform = cam.transform;
		instance.label.text = text;
		instance.sprite.color = color;
		instance.isActive = true;
	}

	public static void Deactive()
	{
		instance.widget.cachedGameObject.SetActive(false);
		instance.isActive = false;
	}

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
		if (target == null)
		{
			isActive = false;
			return;
		}
		pos = gameCamera.WorldToViewportPoint(target.position);
		if (pos.z < 0f)
		{
			pos *= -1f;
		}
		pos.x = Mathf.Clamp(pos.x, minMaxPos.x, minMaxPos.y);
		pos.y = Mathf.Clamp(pos.y, minMaxPos.width, minMaxPos.height);
		widget.cachedTransform.position = uiCamera.ViewportToWorldPoint(pos);
		pos = widget.cachedTransform.localPosition;
		pos.x = Mathf.FloorToInt(pos.x);
		pos.y = Mathf.FloorToInt(pos.y);
		pos.z = 0f;
		widget.cachedTransform.localPosition = pos + customPos;
		sprite.alpha = Mathf.Clamp(Vector3.Distance(target.position, gameCameraTransform.position) * alphaDistance - alphaDistance2, 0f, 1f);
		label.alpha = sprite.alpha;
		widget.UpdateWidget();
		sprite.UpdateWidget();
		label.UpdateWidget();
	}
}
