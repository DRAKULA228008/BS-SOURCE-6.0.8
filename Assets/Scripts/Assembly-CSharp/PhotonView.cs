using System.Collections.Generic;
using Photon;
using UnityEngine;

[AddComponentMenu("Photon Networking/Photon View &v")]
public class PhotonView : Photon.MonoBehaviour
{
	public delegate void MessageDelegate(PhotonMessage message);

	public int ownerId;

	public byte group;

	protected internal bool mixedModeIsReliable;

	public IPunObservable[] punObservables = new IPunObservable[0];

	public Dictionary<string, MessageDelegate> messages = new Dictionary<string, MessageDelegate>();

	private PhotonDataWrite data = new PhotonDataWrite();

	private PhotonMessage messageData = new PhotonMessage();

	public bool OwnerShipWasTransfered;

	public int prefixBackup = -1;

	internal object[] instantiationDataField;

	protected internal BetterList<byte> lastOnSerializeDataSent = new BetterList<byte>();

	protected internal object[] lastOnSerializeDataReceived;

	public ViewSynchronization synchronization;

	public OwnershipOption ownershipTransfer;

	[SerializeField]
	private int viewIdField;

	public int instantiationId;

	public int currentMasterID = -1;

	protected internal bool didAwake;

	[SerializeField]
	protected internal bool isRuntimeInstantiated;

	protected internal bool removedFromLocalViewList;

	public int prefix
	{
		get
		{
			if (prefixBackup == -1 && PhotonNetwork.networkingPeer != null)
			{
				prefixBackup = PhotonNetwork.networkingPeer.currentLevelPrefix;
			}
			return prefixBackup;
		}
		set
		{
			prefixBackup = value;
		}
	}

	public object[] instantiationData
	{
		get
		{
			if (!didAwake)
			{
				instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(instantiationId);
			}
			return instantiationDataField;
		}
		set
		{
			instantiationDataField = value;
		}
	}

	public int viewID
	{
		get
		{
			return viewIdField;
		}
		set
		{
			bool flag = didAwake && viewIdField == 0;
			ownerId = value / PhotonNetwork.MAX_VIEW_IDS;
			viewIdField = value;
			if (flag)
			{
				PhotonNetwork.networkingPeer.RegisterPhotonView(this);
			}
		}
	}

	public bool isSceneView
	{
		get
		{
			return CreatorActorNr == 0;
		}
	}

	public PhotonPlayer owner
	{
		get
		{
			return PhotonPlayer.Find(ownerId);
		}
	}

	public int OwnerActorNr
	{
		get
		{
			return ownerId;
		}
	}

	public bool isOwnerActive
	{
		get
		{
			return ownerId != 0 && PhotonNetwork.networkingPeer.mActors.ContainsKey(ownerId);
		}
	}

	public int CreatorActorNr
	{
		get
		{
			return viewIdField / PhotonNetwork.MAX_VIEW_IDS;
		}
	}

	public bool isMine
	{
		get
		{
			return ownerId == PhotonNetwork.player.ID || (!isOwnerActive && PhotonNetwork.isMasterClient);
		}
	}

	protected internal void Awake()
	{
		if (viewID != 0)
		{
			PhotonNetwork.networkingPeer.RegisterPhotonView(this);
			instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(instantiationId);
		}
		didAwake = true;
	}

	public void RequestOwnership()
	{
		PhotonNetwork.networkingPeer.RequestOwnership(viewID, ownerId);
	}

	public void TransferOwnership(PhotonPlayer newOwner)
	{
		TransferOwnership(newOwner.ID);
	}

	public void TransferOwnership(int newOwnerId)
	{
		PhotonNetwork.networkingPeer.TransferOwnership(viewID, newOwnerId);
		ownerId = newOwnerId;
	}

	public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		if (CreatorActorNr == 0 && !OwnerShipWasTransfered && (currentMasterID == -1 || ownerId == currentMasterID))
		{
			ownerId = newMasterClient.ID;
		}
		currentMasterID = newMasterClient.ID;
	}

	protected internal void OnDestroy()
	{
		if (!removedFromLocalViewList)
		{
			bool flag = PhotonNetwork.networkingPeer.LocalCleanPhotonView(this);
			bool flag2 = false;
			flag2 = Application.isLoadingLevel;
			if (flag && !flag2 && instantiationId > 0 && !PhotonHandler.AppQuits && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				Debug.Log("PUN-instantiated '" + base.gameObject.name + "' got destroyed by engine. This is OK when loading levels. Otherwise use: PhotonNetwork.Destroy().");
			}
		}
	}

	public void SerializeView(PhotonStream stream)
	{
		for (int i = 0; i < punObservables.Length; i++)
		{
			punObservables[i].OnPhotonSerializeView(stream);
		}
	}

	public void DeserializeView(PhotonStream stream)
	{
		for (int i = 0; i < punObservables.Length; i++)
		{
			punObservables[i].OnPhotonSerializeView(stream);
		}
	}

	public void RPC(string methodName, PhotonTargets target)
	{
		PhotonNetwork.RPC(this, methodName, target, false, null);
	}

	public void RPC(string methodName, PhotonTargets target, byte[] data)
	{
		PhotonNetwork.RPC(this, methodName, target, false, data);
	}

	public void RPC(string methodName, PhotonTargets target, PhotonDataWrite data)
	{
		PhotonNetwork.RPC(this, methodName, target, false, data.ToArray());
	}

	public void RpcSecure(string methodName, PhotonTargets target, bool encrypt, byte[] data)
	{
		PhotonNetwork.RPC(this, methodName, target, encrypt, data);
	}

	public void RpcSecure(string methodName, PhotonTargets target, bool encrypt, PhotonDataWrite data)
	{
		PhotonNetwork.RPC(this, methodName, target, encrypt, data.ToArray());
	}

	public void RpcSecure(string methodName, PhotonTargets target, bool encrypt)
	{
		PhotonNetwork.RPC(this, methodName, target, encrypt, null);
	}

	public void RPC(string methodName, PhotonPlayer targetPlayer, byte[] data)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, false, data);
	}

	public void RPC(string methodName, PhotonPlayer targetPlayer, PhotonDataWrite data)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, false, data.ToArray());
	}

	public void RPC(string methodName, PhotonPlayer targetPlayer)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, false, null);
	}

	public void RpcSecure(string methodName, PhotonPlayer targetPlayer, bool encrypt, byte[] data)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, encrypt, data);
	}

	public void RpcSecure(string methodName, PhotonPlayer targetPlayer, bool encrypt)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, encrypt, null);
	}

	public static PhotonView Get(Component component)
	{
		return component.GetComponent<PhotonView>();
	}

	public static PhotonView Get(GameObject gameObj)
	{
		return gameObj.GetComponent<PhotonView>();
	}

	public static PhotonView Find(int viewID)
	{
		return PhotonNetwork.networkingPeer.GetPhotonView(viewID);
	}

	public override string ToString()
	{
		return string.Format("View ({3}){0} on {1} {2}", viewID, (!(base.gameObject != null)) ? "GO==null" : base.gameObject.name, (!isSceneView) ? string.Empty : "(scene)", prefix);
	}

	public void AddPunObservable(IPunObservable observable)
	{
		IPunObservable[] array = new IPunObservable[punObservables.Length + 1];
		for (int i = 0; i < punObservables.Length; i++)
		{
			array[i] = punObservables[i];
		}
		array[array.Length - 1] = observable;
		punObservables = array;
	}

	public void AddMessage(string methodName, MessageDelegate callback)
	{
		messages.Add(methodName, callback);
	}

	public PhotonDataWrite GetData()
	{
		data.Clear();
		return data;
	}

	public void InvokeMessage(string methodName, byte[] data, int senderID, int sendTime)
	{
		nProfiler.BeginSample("PhotonView.InvokeMessage");
		if (!messages.ContainsKey(methodName))
		{
			Debug.Log("No Find Method: " + methodName);
			return;
		}
		messageData.SetData(data, senderID, sendTime);
		messages[methodName](messageData);
		nProfiler.EndSample();
	}
}
