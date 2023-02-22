using ExitGames.Client.Photon;

public struct PhotonEventData
{
    public readonly byte id;

    public readonly PhotonEventTag tag;

    public readonly object[] parameters;

    public readonly int senderID;

    public readonly double timestamp;

    public PhotonEventData(Hashtable rpcEvent, int sender)
    {
        if (rpcEvent.ContainsKey((byte)1))
        {
            tag = (PhotonEventTag)(byte)rpcEvent[(byte)1];
        }
        else
        {
            tag = PhotonEventTag.None;
        }
        if (rpcEvent.ContainsKey((byte)2))
        {
            id = (byte)rpcEvent[(byte)2];
        }
        else
        {
            id = 0;
        }
        if (rpcEvent.ContainsKey((byte)3))
        {
            uint num = (uint)(int)rpcEvent[(byte)3];
            double num2 = num;
            timestamp = num2 / 1000.0;
        }
        else
        {
            timestamp = -1.0;
        }
        if (rpcEvent.ContainsKey((byte)4))
        {
            parameters = (object[])rpcEvent[(byte)4];
        }
        else
        {
            parameters = new object[0];
        }
        senderID = sender;
    }
}
