using System;
using Photon;
using UnityEngine;

public class sTweenTime : Photon.MonoBehaviour
{
	private static float delay;

	public static float time
	{
		get
		{
			return delay + Time.timeSinceLevelLoad;
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("PhotonSendTime", PhotonSendTime);
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
	}

	private void OnDisable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			TimerManager.In(1.5f, delegate
			{
				PhotonDataWrite data = base.photonView.GetData();
				data.Write(time);
				base.photonView.RPC("PhotonSendTime", playerConnect, data);
			});
		}
	}

	[PunRPC]
	private void PhotonSendTime(PhotonMessage message)
	{
		delay = message.ReadFloat();
		delay += (float)(PhotonNetwork.time - message.timestamp);
	}
}
