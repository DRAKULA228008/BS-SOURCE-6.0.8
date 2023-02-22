using UnityEngine;

public class nMonoBehaviour : MonoBehaviour
{
	private GameObject mGameObject;

	private Transform mTransform;

	private MeshFilter mMeshFilter;

	private MeshRenderer mMeshRenderer;

	public GameObject cachedGameObject
	{
		get
		{
			if (mGameObject == null)
			{
				mGameObject = base.gameObject;
			}
			return mGameObject;
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (mTransform == null)
			{
				mTransform = base.transform;
			}
			return mTransform;
		}
	}

	public MeshFilter cachedMeshFilter
	{
		get
		{
			if (mMeshFilter == null)
			{
				mMeshFilter = GetComponent<MeshFilter>();
			}
			return mMeshFilter;
		}
	}

	public MeshRenderer cachedMeshRenderer
	{
		get
		{
			if (mMeshRenderer == null)
			{
				mMeshRenderer = GetComponent<MeshRenderer>();
			}
			return mMeshRenderer;
		}
	}

	public virtual void OnUpdate()
	{
	}

	public virtual void OnFixedUpdate()
	{
	}

	public virtual void OnLateUpdate()
	{
	}
}
