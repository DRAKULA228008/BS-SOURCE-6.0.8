using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon;

// Token: 0x0200053B RID: 1339
public static class PhotonClassesManager
{
	// Token: 0x06002BCA RID: 11210 RVA: 0x0001DC7B File Offset: 0x0001BE7B
	public static void Add(PunBehaviour behaviour)
	{
		PhotonClassesManager.behaviourList.Add(behaviour);
		PhotonClassesManager.count = PhotonClassesManager.behaviourList.Count;
	}

	// Token: 0x06002BCB RID: 11211 RVA: 0x000DC450 File Offset: 0x000DA650
	public static void SendMessage(PhotonNetworkingMessage method, params object[] parameters)
	{
		PhotonClassesManager.behaviourList.RemoveAll((PunBehaviour x) => x == null);
		PhotonClassesManager.count = PhotonClassesManager.behaviourList.Count;
		switch (method)
		{
		case PhotonNetworkingMessage.OnConnectedToPhoton:
			for (int i = 0; i < PhotonClassesManager.count; i++)
			{
				PhotonClassesManager.behaviourList[i].OnConnectedToPhoton();
			}
			break;
		case PhotonNetworkingMessage.OnLeftRoom:
			for (int j = 0; j < PhotonClassesManager.count; j++)
			{
				PhotonClassesManager.behaviourList[j].OnLeftRoom();
			}
			break;
		case PhotonNetworkingMessage.OnMasterClientSwitched:
			for (int k = 0; k < PhotonClassesManager.count; k++)
			{
				PhotonClassesManager.behaviourList[k].OnMasterClientSwitched((PhotonPlayer)parameters[0]);
			}
			break;
		case PhotonNetworkingMessage.OnPhotonCreateRoomFailed:
			for (int l = 0; l < PhotonClassesManager.count; l++)
			{
				PhotonClassesManager.behaviourList[l].OnPhotonCreateRoomFailed(parameters);
			}
			break;
		case PhotonNetworkingMessage.OnPhotonJoinRoomFailed:
			for (int m = 0; m < PhotonClassesManager.count; m++)
			{
				PhotonClassesManager.behaviourList[m].OnPhotonJoinRoomFailed(parameters);
			}
			break;
		case PhotonNetworkingMessage.OnCreatedRoom:
			for (int n = 0; n < PhotonClassesManager.count; n++)
			{
				PhotonClassesManager.behaviourList[n].OnCreatedRoom();
			}
			break;
		case PhotonNetworkingMessage.OnJoinedLobby:
			for (int num = 0; num < PhotonClassesManager.count; num++)
			{
				PhotonClassesManager.behaviourList[num].OnJoinedLobby();
			}
			break;
		case PhotonNetworkingMessage.OnLeftLobby:
			for (int num2 = 0; num2 < PhotonClassesManager.count; num2++)
			{
				PhotonClassesManager.behaviourList[num2].OnLeftLobby();
			}
			break;
		case PhotonNetworkingMessage.OnDisconnectedFromPhoton:
			for (int num3 = 0; num3 < PhotonClassesManager.count; num3++)
			{
				PhotonClassesManager.behaviourList[num3].OnDisconnectedFromPhoton();
			}
			break;
		case PhotonNetworkingMessage.OnConnectionFail:
			for (int num4 = 0; num4 < PhotonClassesManager.count; num4++)
			{
				PhotonClassesManager.behaviourList[num4].OnConnectionFail((DisconnectCause)((int)parameters[0]));
			}
			break;
		case PhotonNetworkingMessage.OnFailedToConnectToPhoton:
			for (int num5 = 0; num5 < PhotonClassesManager.count; num5++)
			{
				PhotonClassesManager.behaviourList[num5].OnFailedToConnectToPhoton((DisconnectCause)((int)parameters[0]));
			}
			break;
		case PhotonNetworkingMessage.OnReceivedRoomListUpdate:
			for (int num6 = 0; num6 < PhotonClassesManager.count; num6++)
			{
				PhotonClassesManager.behaviourList[num6].OnReceivedRoomListUpdate();
			}
			break;
		case PhotonNetworkingMessage.OnJoinedRoom:
			for (int num7 = 0; num7 < PhotonClassesManager.count; num7++)
			{
				PhotonClassesManager.behaviourList[num7].OnJoinedRoom();
			}
			break;
		case PhotonNetworkingMessage.OnPhotonPlayerConnected:
			for (int num8 = 0; num8 < PhotonClassesManager.count; num8++)
			{
				PhotonClassesManager.behaviourList[num8].OnPhotonPlayerConnected((PhotonPlayer)parameters[0]);
			}
			break;
		case PhotonNetworkingMessage.OnPhotonPlayerDisconnected:
			for (int num9 = 0; num9 < PhotonClassesManager.count; num9++)
			{
				PhotonClassesManager.behaviourList[num9].OnPhotonPlayerDisconnected((PhotonPlayer)parameters[0]);
			}
			break;
		case PhotonNetworkingMessage.OnPhotonRandomJoinFailed:
			for (int num10 = 0; num10 < PhotonClassesManager.count; num10++)
			{
				PhotonClassesManager.behaviourList[num10].OnPhotonRandomJoinFailed(parameters);
			}
			break;
		case PhotonNetworkingMessage.OnConnectedToMaster:
			for (int num11 = 0; num11 < PhotonClassesManager.count; num11++)
			{
				PhotonClassesManager.behaviourList[num11].OnConnectedToMaster();
			}
			break;
		case PhotonNetworkingMessage.OnPhotonInstantiate:
			for (int num12 = 0; num12 < PhotonClassesManager.count; num12++)
			{
				PhotonClassesManager.behaviourList[num12].OnPhotonInstantiate((PhotonMessageInfo)parameters[0]);
			}
			break;
		case PhotonNetworkingMessage.OnPhotonMaxCccuReached:
			for (int num13 = 0; num13 < PhotonClassesManager.count; num13++)
			{
				PhotonClassesManager.behaviourList[num13].OnPhotonMaxCccuReached();
			}
			break;
		case PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged:
			for (int num14 = 0; num14 < PhotonClassesManager.count; num14++)
			{
				PhotonClassesManager.behaviourList[num14].OnPhotonCustomRoomPropertiesChanged((Hashtable)parameters[0]);
			}
			break;
		case PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged:
			for (int num15 = 0; num15 < PhotonClassesManager.count; num15++)
			{
				PhotonClassesManager.behaviourList[num15].OnPhotonPlayerPropertiesChanged(parameters);
			}
			break;
		case PhotonNetworkingMessage.OnUpdatedFriendList:
			for (int num16 = 0; num16 < PhotonClassesManager.count; num16++)
			{
				PhotonClassesManager.behaviourList[num16].OnUpdatedFriendList();
			}
			break;
		case PhotonNetworkingMessage.OnCustomAuthenticationFailed:
			for (int num17 = 0; num17 < PhotonClassesManager.count; num17++)
			{
				PhotonClassesManager.behaviourList[num17].OnCustomAuthenticationFailed((string)parameters[0]);
			}
			break;
		//case PhotonNetworkingMessage.OnCustomAuthenticationResponse:
		//	for (int num18 = 0; num18 < PhotonClassesManager.count; num18++)
		//	{
		//		PhotonClassesManager.behaviourList[num18].OnCustomAuthenticationResponse((Dictionary<string, object>)parameters[0]);
		//	}
		//	break;
		case PhotonNetworkingMessage.OnWebRpcResponse:
			for (int num19 = 0; num19 < PhotonClassesManager.count; num19++)
			{
				PhotonClassesManager.behaviourList[num19].OnWebRpcResponse((OperationResponse)parameters[0]);
			}
			break;
		case PhotonNetworkingMessage.OnOwnershipRequest:
			for (int num20 = 0; num20 < PhotonClassesManager.count; num20++)
			{
				PhotonClassesManager.behaviourList[num20].OnOwnershipRequest(parameters);
			}
			break;
		case PhotonNetworkingMessage.OnLobbyStatisticsUpdate:
			for (int num21 = 0; num21 < PhotonClassesManager.count; num21++)
			{
				PhotonClassesManager.behaviourList[num21].OnLobbyStatisticsUpdate();
			}
			break;
		//case PhotonNetworkingMessage.OnOwnershipTransfered:
		//	for (int num22 = 0; num22 < PhotonClassesManager.count; num22++)
		//	{
		//		PhotonClassesManager.behaviourList[num22].OnOwnershipTransfered(parameters);
		//	}
		//	break;
		}
	}

	// Token: 0x04001BE6 RID: 7142
	private static List<PunBehaviour> behaviourList = new List<PunBehaviour>();

	// Token: 0x04001BE7 RID: 7143
	private static int count = 0;
}
