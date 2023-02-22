using UnityEngine;

public class UINameManager : MonoBehaviour
{
	private static bool showPlayerName = true;

	public Camera m_Camera;

	private UILabel label;

	private string hitName;

	private Ray ray;

	private Vector3 cameraPoint = new Vector3(0.5f, 0.5f, 0f);

	private static Color32 blueColor = new Color32(0, 181, byte.MaxValue, byte.MaxValue);

	private static Color32 redColor = new Color32(byte.MaxValue, 0, 80, byte.MaxValue);

	private void Start()
	{
		label = UIElements.Get<UILabel>("NameLabel");
		showPlayerName = GameConsole.Load("show_player_name", true);
	}

	private void OnEnable()
	{
		TimerManager.In("NameTimer", 0.2f, -1, 0.2f, UpdateName);
	}

	private void OnDisable()
	{
		TimerManager.Cancel("NameTimer");
		if (label != null)
		{
			label.text = string.Empty;
		}
	}

	private void UpdateName()
	{
		if (!showPlayerName)
		{
			return;
		}
		ray = m_Camera.ViewportPointToRay(cameraPoint);
		if (nRaycast.RaycastName(ray, 100f))
		{
			hitName = nRaycast.container.playerSkin.Controller.photonView.owner.UserId;
			if (nRaycast.container.playerSkin.PlayerTeam == Team.Blue)
			{
				label.color = blueColor;
			}
			else
			{
				label.color = redColor;
			}
			label.text = hitName;
		}
		else
		{
			label.text = string.Empty;
		}
	}
}
