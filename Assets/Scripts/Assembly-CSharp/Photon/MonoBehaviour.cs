using System;
using UnityEngine;

namespace Photon
{
	public class MonoBehaviour : UnityEngine.MonoBehaviour
	{
		private PhotonView pvCache;

		public PhotonView photonView
		{
			get
			{
				if (pvCache == null)
				{
					pvCache = PhotonView.Get(this);
				}
				return pvCache;
			}
		}

		[Obsolete("Use a photonView")]
		public new PhotonView networkView
		{
			get
			{
				Debug.LogWarning("Why are you still using networkView? should be PhotonView?");
				return PhotonView.Get(this);
			}
		}
	}
}
