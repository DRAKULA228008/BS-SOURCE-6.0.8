using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[ExecuteInEditMode]
public class CryptoBoxCollider : MonoBehaviour
{
	public CryptoVector3 center;

	public CryptoVector3 size;

	private BoxCollider mCollider;

	public BoxCollider cachedBoxCollider
	{
		get
		{
			if (mCollider == null)
			{
				mCollider = GetComponent<BoxCollider>();
			}
			return mCollider;
		}
	}

	private void OnEnable()
	{
		Check();
	}

	private void OnDisable()
	{
		Check();
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			Check();
		}
	}

	private void Check()
	{
		if (cachedBoxCollider.size != size)
		{
			CheckManager.Detected();
		}
		if (cachedBoxCollider.center != center)
		{
			CheckManager.Detected();
		}
	}
}
