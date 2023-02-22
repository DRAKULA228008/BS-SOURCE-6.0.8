using System;
using UnityEngine;

[Serializable]
public class CPC_Point
{
	public Vector3 position;

	public Quaternion rotation;

	public Vector3 handleprev;

	public Vector3 handlenext;

	public CPC_ECurveType curveTypeRotation;

	public AnimationCurve rotationCurve;

	public CPC_ECurveType curveTypePosition;

	public AnimationCurve positionCurve;

	public bool chained;

	public CPC_Point(Vector3 pos, Quaternion rot)
	{
		position = pos;
		rotation = rot;
		handleprev = Vector3.back;
		handlenext = Vector3.forward;
		curveTypeRotation = CPC_ECurveType.EaseInAndOut;
		rotationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		curveTypePosition = CPC_ECurveType.Linear;
		positionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		chained = true;
	}
}
