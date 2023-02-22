using UnityEngine;

public class UIPlayerStatisticsElement : MonoBehaviour
{
	public UILabel PlayerNameLabel;

	public UITexture AvatarTexture;

	public UILabel LevelLabel;

	public UILabel ClanTagLabel;

	public UILabel KillsLabel;

	public UILabel DeathsLabel;

	public UILabel PingLabel;

	public UIWidget Widget;

	public UIDragScrollView DragScroll;

	private static Color32 AdminColor = new Color32(60, 181, 232, byte.MaxValue);

	private static Color32 LocalPlayerColor = Color.green;

	public PhotonPlayer PlayerInfo;

	private int Timer;

	private Transform mTransform;

	public Transform cachedTransform
	{
		get
		{
			if (mTransform == null)
			{
				mTransform = base.transform;
			}
			return mTransform;
		}
	}

	private void OnDisable()
	{
		TimerManager.Cancel(Timer);
	}

	public void SetData(PhotonPlayer playerInfo)
	{
		PlayerInfo = playerInfo;
		string text = playerInfo.GetClan().ToUpper();
		ClanTagLabel.text = text;
		if (Settings.ShowAvatars)
		{
			AvatarTexture.mainTexture = AvatarManager.Get(playerInfo.GetAvatarUrl());
		}
		else
		{
			AvatarTexture.mainTexture = GameSettings.instance.NoAvatarTexture;
		}
		PlayerNameLabel.text = UpdatePlayerName(playerInfo);
		LevelLabel.text = StringCache.Get(playerInfo.GetLevel());
		KillsLabel.text = StringCache.Get(PlayerInfo.GetKills());
		DeathsLabel.text = StringCache.Get(PlayerInfo.GetDeaths());
		PingLabel.text = StringCache.Get(PlayerInfo.GetPing());
		base.name = PlayerNameLabel.text;
		if (playerInfo.GetDead())
		{
			Widget.alpha = 0.5f;
		}
		else
		{
			Widget.alpha = 1f;
		}
		Widget.UpdateWidget();
		if (playerInfo.IsLocal)
		{
			PlayerNameLabel.color = LocalPlayerColor;
			LevelLabel.color = LocalPlayerColor;
			KillsLabel.color = LocalPlayerColor;
			DeathsLabel.color = LocalPlayerColor;
			PingLabel.color = LocalPlayerColor;
		}
		else if (playerInfo.IsMasterClient)
		{
			PlayerNameLabel.color = AdminColor;
			LevelLabel.color = AdminColor;
			KillsLabel.color = AdminColor;
			DeathsLabel.color = AdminColor;
			PingLabel.color = AdminColor;
		}
		else
		{
			PlayerNameLabel.color = Color.white;
			LevelLabel.color = Color.white;
			KillsLabel.color = Color.white;
			DeathsLabel.color = Color.white;
			PingLabel.color = Color.white;
		}
		if (!TimerManager.IsActive(Timer))
		{
			Timer = TimerManager.In(3f, -1, 3f, UpdateData);
		}
	}

	private string UpdatePlayerName(PhotonPlayer player)
	{
		if ((PhotonNetwork.room.GetGameMode() == GameMode.Bomb || PhotonNetwork.room.GetGameMode() == GameMode.Bomb2) && PhotonNetwork.player.GetTeam() != Team.Blue && BombManager.GetPlayerBombID() != -1 && player.ID == BombManager.GetPlayerBombID())
		{
			return player.UserId + "   " + UIStatus.GetSpecialSymbol(98);
		}
		return player.UserId;
	}

	private void UpdateData()
	{
		SetData(PlayerInfo);
	}
}
