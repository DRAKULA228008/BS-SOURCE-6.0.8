using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public static class PhotonEvent
{
    public struct EventData
    {
        public byte id;

        public PhotonEventTag tag;

        public Delegate callback;

        public EventData(byte i, PhotonEventTag t, EventCallback c)
        {
            id = i;
            tag = t;
            callback = c;
        }
    }

    public delegate void EventCallback(PhotonEventData data);

    private static byte code = 20;

    private static List<EventData> eventList = new List<EventData>();

    private static int count = 0;

    private static bool init = false;

    public static void AddListener(byte id, EventCallback callback)
    {
        if (id != 0)
        {
            AddListener(PhotonEventTag.None, id, callback);
        }
    }

    public static void AddListener(PhotonEventTag tag, EventCallback callback)
    {
        if (tag != 0)
        {
            AddListener(tag, 0, callback);
        }
    }

    private static void AddListener(PhotonEventTag tag, byte id, EventCallback callback)
    {
        if (!init)
        {
            init = true;
            PhotonNetwork.OnEventCall = (PhotonNetwork.EventCallback)Delegate.Combine(PhotonNetwork.OnEventCall, new PhotonNetwork.EventCallback(OnEventHandler));
        }
        EventData item = new EventData(id, tag, callback);
        eventList.Add(item);
    }

    public static void Clear()
    {
        eventList.Clear();
    }

    public static void RPC(PhotonEventTag tag, PhotonPlayer player, params object[] parameters)
    {
        if (tag != 0)
        {
            RPC(tag, 0, false, PhotonTargets.Others, new PhotonPlayer[1] { player }, parameters);
        }
    }

    public static void RPC(PhotonEventTag tag, PhotonPlayer[] players, params object[] parameters)
    {
        if (tag != 0)
        {
            RPC(tag, 0, false, PhotonTargets.Others, players, parameters);
        }
    }

    public static void RPC(PhotonEventTag tag, bool sendTimestamp, PhotonPlayer player, params object[] parameters)
    {
        if (tag != 0)
        {
            RPC(tag, 0, sendTimestamp, PhotonTargets.Others, new PhotonPlayer[1] { player }, parameters);
        }
    }

    public static void RPC(PhotonEventTag tag, bool sendTimestamp, PhotonPlayer[] players, params object[] parameters)
    {
        if (tag != 0)
        {
            RPC(tag, 0, sendTimestamp, PhotonTargets.Others, players, parameters);
        }
    }

    public static void RPC(PhotonEventTag tag, PhotonTargets target, params object[] parameters)
    {
        if (tag != 0)
        {
            RPC(tag, 0, false, target, null, parameters);
        }
    }

    public static void RPC(PhotonEventTag tag, bool sendTimestamp, PhotonTargets target, params object[] parameters)
    {
        if (tag != 0)
        {
            RPC(tag, 0, sendTimestamp, target, null, parameters);
        }
    }

    public static void RPC(byte id, PhotonPlayer player, params object[] parameters)
    {
        if (id != 0)
        {
            RPC(PhotonEventTag.None, id, false, PhotonTargets.Others, new PhotonPlayer[1] { player }, parameters);
        }
    }

    public static void RPC(byte id, PhotonPlayer[] players, params object[] parameters)
    {
        if (id != 0)
        {
            RPC(PhotonEventTag.None, id, false, PhotonTargets.Others, players, parameters);
        }
    }

    public static void RPC(byte id, bool sendTimestamp, PhotonPlayer player, params object[] parameters)
    {
        if (id != 0)
        {
            RPC(PhotonEventTag.None, id, sendTimestamp, PhotonTargets.Others, new PhotonPlayer[1] { player }, parameters);
        }
    }

    public static void RPC(byte id, bool sendTimestamp, PhotonPlayer[] players, params object[] parameters)
    {
        if (id != 0)
        {
            RPC(PhotonEventTag.None, id, sendTimestamp, PhotonTargets.Others, players, parameters);
        }
    }

    public static void RPC(byte id, PhotonTargets target, params object[] parameters)
    {
        if (id != 0)
        {
            RPC(PhotonEventTag.None, id, false, target, null, parameters);
        }
    }

    public static void RPC(byte id, bool sendTimestamp, PhotonTargets target, params object[] parameters)
    {
        if (id != 0)
        {
            RPC(PhotonEventTag.None, id, sendTimestamp, target, null, parameters);
        }
    }

    private static void RPC(PhotonEventTag tag, byte id, bool sendTimestamp, PhotonTargets target, PhotonPlayer[] players, params object[] parameters)
    {
        Hashtable hashtable = new Hashtable();
        if (tag != 0)
        {
            hashtable.Add((byte)1, (byte)tag);
        }
        if (id != 0)
        {
            hashtable.Add((byte)2, id);
        }
        if (sendTimestamp)
        {
            hashtable.Add((byte)3, PhotonNetwork.ServerTimestamp);
        }
        if (parameters != null && parameters.Length > 0)
        {
            hashtable.Add((byte)4, parameters);
        }
        if (players != null)
        {
            if (players.Length == 1)
            {
                if (PhotonNetwork.player.ID == players[0].ID)
                {
                    Invoke(hashtable, players[0].ID);
                    return;
                }
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                raiseEventOptions.TargetActors = new int[1] { players[0].ID };
                RaiseEventOptions options = raiseEventOptions;
                PhotonNetwork.RaiseEvent(code, hashtable, true, options);
            }
            else if (players.Length > 1)
            {
                int[] array = new int[players.Length];
                for (int i = 0; i < players.Length; i++)
                {
                    array[i] = players[i].ID;
                }
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                raiseEventOptions.TargetActors = array;
                RaiseEventOptions options2 = raiseEventOptions;
                PhotonNetwork.RaiseEvent(code, hashtable, true, options2);
            }
            return;
        }
        switch (target)
        {
            case PhotonTargets.All:
                PhotonNetwork.RaiseEvent(code, hashtable, true, null);
                Invoke(hashtable, PhotonNetwork.player.ID);
                break;
            case PhotonTargets.Others:
                PhotonNetwork.RaiseEvent(code, hashtable, true, null);
                break;
            case PhotonTargets.AllBuffered:
                {
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                    raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;
                    RaiseEventOptions options7 = raiseEventOptions;
                    PhotonNetwork.RaiseEvent(code, hashtable, true, options7);
                    Invoke(hashtable, PhotonNetwork.player.ID);
                    break;
                }
            case PhotonTargets.OthersBuffered:
                {
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                    raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;
                    RaiseEventOptions options6 = raiseEventOptions;
                    PhotonNetwork.RaiseEvent(code, hashtable, true, options6);
                    break;
                }
            case PhotonTargets.MasterClient:
                {
                    if (PhotonNetwork.isMasterClient)
                    {
                        Invoke(hashtable, PhotonNetwork.player.ID);
                        break;
                    }
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                    raiseEventOptions.Receivers = ReceiverGroup.MasterClient;
                    RaiseEventOptions options5 = raiseEventOptions;
                    PhotonNetwork.RaiseEvent(code, hashtable, true, options5);
                    break;
                }
            case PhotonTargets.AllViaServer:
                {
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                    raiseEventOptions.Receivers = ReceiverGroup.All;
                    RaiseEventOptions options4 = raiseEventOptions;
                    PhotonNetwork.RaiseEvent(code, hashtable, true, options4);
                    break;
                }
            case PhotonTargets.AllBufferedViaServer:
                {
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                    raiseEventOptions.Receivers = ReceiverGroup.All;
                    raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;
                    RaiseEventOptions options3 = raiseEventOptions;
                    PhotonNetwork.RaiseEvent(code, hashtable, true, options3);
                    break;
                }
            default:
                Debug.LogError("Unsupported target enum: " + target);
                break;
        }
    }

    private static void OnEventHandler(byte eventCode, object content, int senderID)
    {
        if (code == eventCode)
        {
            Invoke((Hashtable)content, senderID);
        }
    }

    private static void Invoke(Hashtable rpcEvent, int senderID)
    {
        PhotonEventData data = new PhotonEventData(rpcEvent, senderID);
        eventList.RemoveAll((EventData x) => (object)x.callback == null);
        count = eventList.Count;
        for (int i = 0; i < count; i++)
        {
            if ((eventList[i].tag == data.tag && data.tag != 0) || (eventList[i].id == data.id && data.id > 0))
            {
                EventCallback eventCallback = eventList[i].callback as EventCallback;
                eventCallback(data);
            }
        }
    }
}
