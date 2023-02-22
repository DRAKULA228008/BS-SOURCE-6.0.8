using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[ExecuteInEditMode]
public class CryptoCapsuleCollider : MonoBehaviour
{
	public CryptoVector3 center;

	public CryptoFloat radius;

	public CryptoFloat height;

	private CapsuleCollider mCollider;

	public CapsuleCollider cachedCapsuleCollider
	{
		get
		{
			if (mCollider == null)
			{
				mCollider = GetComponent<CapsuleCollider>();
			}
			return mCollider;
		}
	}

	private void OnDisable()
	{
		Check();
	}

	private void OnApplicationFocus(bool focus)
	{
		Check();
	}

	private void Check()
	{
		if (cachedCapsuleCollider.center != center || cachedCapsuleCollider.radius != (float)radius || cachedCapsuleCollider.height != (float)height)
		{
			CheckManager.Detected();
		}
	}
}
