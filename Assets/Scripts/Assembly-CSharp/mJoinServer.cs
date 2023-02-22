using System;
using BestHTTP;
using BSCM;

public class mJoinServer
{
	public static RoomInfo room;

	public static Action onBack;

	public static void Join()
	{
		if (string.IsNullOrEmpty(room.GetPassword()))
		{
			if (room.PlayerCount == room.MaxPlayers)
			{
				OnLoadCustomMap(delegate
				{
					mPopUp.ShowPopup(Localization.Get("Do you want to queue up this server?"), Localization.Get("Queue"), Localization.Get("Yes"), delegate
					{
						mPhotonSettings.QueueServer(room);
					}, Localization.Get("No"), delegate
					{
						onBack();
					});
				});
			}
			else
			{
				OnLoadCustomMap(delegate
				{
					mPhotonSettings.JoinServer(room);
				});
			}
		}
		else if (room.isOfficialServer())
		{
			if (room.PlayerCount == room.MaxPlayers)
			{
				mPopUp.ShowPopup(Localization.Get("Do you want to queue up this server?"), Localization.Get("Queue"), Localization.Get("Yes"), delegate
				{
					mPhotonSettings.QueueServer(room);
				}, Localization.Get("No"), onBack);
			}
			else
			{
				mPhotonSettings.JoinServer(room);
			}
		}
		else
		{
			mPopUp.ShowInput(string.Empty, Localization.Get("Password"), 4, UIInput.KeyboardType.NumberPad, null, null, "Ok", delegate
			{
				OnPassword();
			}, Localization.Get("Back"), delegate
			{
				onBack();
			});
		}
	}

	private static void OnPassword()
	{
		if (room.GetPassword() == mPopUp.GetInputText())
		{
			if (room.PlayerCount == room.MaxPlayers)
			{
				OnLoadCustomMap(delegate
				{
					mPopUp.ShowPopup(Localization.Get("Do you want to queue up this server?"), Localization.Get("Queue"), Localization.Get("Yes"), delegate
					{
						mPhotonSettings.QueueServer(room);
					}, Localization.Get("No"), onBack);
				});
			}
			else
			{
				OnLoadCustomMap(delegate
				{
					mPhotonSettings.JoinServer(room);
				});
			}
		}
		else
		{
			UIToast.Show(Localization.Get("Password is incorrect"));
		}
	}

	private static void OnLoadCustomMap(Action callback)
	{
		if (!room.isCustomMap())
		{
			LevelManager.customScene = false;
			if (callback != null)
			{
				callback();
			}
			return;
		}
		int hash = Manager.GetBundleHash(room.GetSceneName());
		if (hash != 0 && hash == room.GetCustomMapHash())
		{
			Manager.LoadBundle(Manager.GetBundlePath(room.GetSceneName()));
			LevelManager.customScene = true;
			if (callback != null)
			{
				callback();
			}
			return;
		}
		mPopUp.ShowPopup(Localization.Get("Do you really want to download a custom map?"), Localization.Get("Map"), Localization.Get("Yes"), delegate
		{
			Uri uri = new Uri("https://drive.google.com/uc?export=download&id=" + room.GetCustomMapUrl());
			HTTPRequest request = new HTTPRequest(uri, delegate(HTTPRequest req, HTTPResponse res)
			{
				if (res.IsSuccess)
				{
					string path = Manager.SaveBundle(room.GetSceneName(), room.GetCustomMapModes(), room.GetCustomMapHash(), room.GetCustomMapUrl(), res.Data);
					hash = Manager.GetBundleHash(room.GetSceneName());
					if (hash != 0 && hash == room.GetCustomMapHash())
					{
						Manager.LoadBundle(path);
						LevelManager.customScene = true;
						if (callback != null)
						{
							callback();
						}
					}
					else
					{
						LevelManager.customScene = false;
						UIToast.Show(Localization.Get("Error"));
						onBack();
					}
				}
				else
				{
					LevelManager.customScene = false;
					UIToast.Show(Localization.Get("Error") + ": " + res.Message);
					onBack();
				}
			});
			request.OnProgress = OnDownloadProgress;
			request.Send();
			mPopUp.ShowPopup(Localization.Get("Please wait") + "...", Localization.Get("Map"), Localization.Get("Exit"), delegate
			{
				request.Abort();
				LevelManager.customScene = false;
				onBack();
			});
		}, Localization.Get("No"), delegate
		{
			onBack();
		});
	}

	private static void OnDownloadProgress(HTTPRequest request, long downloaded, long length)
	{
		mPopUp.SetPopupText(Localization.Get("Please wait") + "... (" + downloaded + "/" + length + " bytes]");
	}
}
