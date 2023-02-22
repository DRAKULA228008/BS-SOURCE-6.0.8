using System;
using UnityEngine;

public class CryptoTransformList : MonoBehaviour
{
	public enum Axis
	{
		Off,
		Zero,
		One
	}

	[Serializable]
	public class TransformData
	{
		public Transform target;

		public Axis Position;

		public Axis Rotation;

		public Axis Scale = Axis.One;
	}

	public TransformData[] transforms = new TransformData[0];

	private static Vector3 vZero = new Vector3(nValue.int0, nValue.int0, nValue.int0);

	private static Vector3 vOne = new Vector3(nValue.int1, nValue.int1, nValue.int1);

	private void OnEnable()
	{
		CheckTransform();
	}

	public void CheckTransform()
	{
		for (int i = 0; i < transforms.Length; i++)
		{
			if (!(transforms[i].target == null) && transforms[i].target.hasChanged)
			{
				if (Detected(transforms[i].target.localPosition, transforms[i].Position))
				{
					CheckManager.Detected();
				}
				else if (Detected(transforms[i].target.localEulerAngles, transforms[i].Rotation))
				{
					CheckManager.Detected();
				}
				else if (Detected(transforms[i].target.localScale, transforms[i].Scale))
				{
					CheckManager.Detected();
				}
			}
		}
	}

	private bool Detected(Vector3 v, Axis a)
	{
		switch (a)
		{
		case Axis.Zero:
			return v != vZero;
		case Axis.One:
			return v != vOne;
		default:
			return false;
		}
	}
}
