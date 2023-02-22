using UnityEngine;

[ExecuteInEditMode]
public class LODObject : MonoBehaviour
{
	public bool isEditor;

	public Renderer[] Originals;

	public Renderer[] LODs;

	private bool isLOD;

	private Transform mTransform;

	private int id;

	public nTimer Timer;

	public static bool isScope;

	private static float distance = 15f;

	public static Transform Target;

	private Transform cachedTransform
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

	private void Start()
	{
		if (Target == null)
		{
			Target = Camera.main.transform;
		}
		Timer.In(Random.Range(0.1f, 0.15f), true, CheckLOD);
	}

	private void CheckLOD()
	{
		if (Target == null)
		{
			if (!(Camera.main != null))
			{
				return;
			}
			Target = Camera.main.transform;
		}
		if (Vector3.Distance(cachedTransform.position, Target.position) < ((!isScope) ? distance : 40f))
		{
			if (isLOD)
			{
				isLOD = false;
				for (int i = 0; i < Originals.Length; i++)
				{
					Originals[i].enabled = true;
				}
				for (int j = 0; j < LODs.Length; j++)
				{
					LODs[j].enabled = false;
				}
			}
		}
		else if (!isLOD)
		{
			isLOD = true;
			for (int k = 0; k < Originals.Length; k++)
			{
				Originals[k].enabled = false;
			}
			for (int l = 0; l < LODs.Length; l++)
			{
				LODs[l].enabled = true;
			}
		}
	}

	public static void SetDistance(float dis)
	{
		distance = 10f + dis * 40f;
	}
}
