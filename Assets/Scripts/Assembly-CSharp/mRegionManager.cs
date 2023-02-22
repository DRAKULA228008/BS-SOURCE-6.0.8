using System.Collections;
using UnityEngine;

public class mRegionManager : MonoBehaviour
{
	public UILabel[] labels;

	public bool actived;

	public void Check()
	{
		if (!actived)
		{
			actived = true;
			StartCoroutine(CheckCoroutine());
		}
	}

	private IEnumerator CheckCoroutine()
	{
		PhotonPingManager pingManager = new PhotonPingManager();
		foreach (Region region2 in PhotonNetwork.networkingPeer.AvailableRegions)
		{
			StartCoroutine(pingManager.PingSocket(region2));
		}
		while (!pingManager.Done)
		{
			Debug.Log(pingManager.Done);
			yield return new WaitForSeconds(0.1f);
		}
		foreach (Region region in PhotonNetwork.networkingPeer.AvailableRegions)
		{
			switch (region.Code)
			{
			case CloudRegionCode.eu:
				labels[0].text = region.Ping + "ms";
				break;
			case CloudRegionCode.us:
				labels[1].text = region.Ping + "ms";
				break;
			case CloudRegionCode.kr:
				labels[2].text = region.Ping + "ms";
				break;
			case CloudRegionCode.sa:
				labels[3].text = region.Ping + "ms";
				break;
			case CloudRegionCode.@in:
				labels[4].text = region.Ping + "ms";
				break;
			case CloudRegionCode.au:
				labels[5].text = region.Ping + "ms";
				break;
			}
		}
		actived = false;
	}
}
