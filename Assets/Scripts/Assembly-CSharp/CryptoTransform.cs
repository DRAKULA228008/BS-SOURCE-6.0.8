using UnityEngine;

public static class CryptoTransform
{
	private static Vector3 vZero = new Vector3(nValue.int0, nValue.int0, nValue.int0);

	private static Vector3 vOne = new Vector3(nValue.int1, nValue.int1, nValue.int1);

	public static void HackDetector(this Transform target, nVector position, nVector rotation, nVector scale)
	{
		if (target.hasChanged)
		{
			if (Detected(target.localPosition, position))
			{
				CheckManager.Detected();
			}
			else if (Detected(target.localEulerAngles, rotation))
			{
				CheckManager.Detected();
			}
			else if (Detected(target.localScale, scale))
			{
				CheckManager.Detected();
			}
		}
	}

	private static bool Detected(Vector3 v, nVector a)
	{
		switch (a)
		{
		case nVector.Zero:
			return v != vZero;
		case nVector.One:
			return v != vOne;
		default:
			return false;
		}
	}
}
