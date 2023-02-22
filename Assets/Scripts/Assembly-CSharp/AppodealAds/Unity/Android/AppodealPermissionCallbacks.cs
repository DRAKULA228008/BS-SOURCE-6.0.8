using AppodealAds.Unity.Common;
using UnityEngine;

namespace AppodealAds.Unity.Android
{
	public class AppodealPermissionCallbacks : AndroidJavaProxy
	{
		private IPermissionGrantedListener listener;

		internal AppodealPermissionCallbacks(IPermissionGrantedListener listener)
			: base("com.appodeal.ads.utils.PermissionsHelper$AppodealPermissionCallbacks")
		{
			this.listener = listener;
		}

		private void writeExternalStorageResponse(int result)
		{
			listener.writeExternalStorageResponse(result);
		}

		private void accessCoarseLocationResponse(int result)
		{
			listener.accessCoarseLocationResponse(result);
		}
	}
}
