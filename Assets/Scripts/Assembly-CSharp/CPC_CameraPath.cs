using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPC_CameraPath : MonoBehaviour
{
	public bool useMainCamera = true;

	public Camera selectedCamera;

	public bool lookAtTarget;

	public Transform target;

	public bool playOnAwake;

	public float playOnAwakeTime = 10f;

	public List<CPC_Point> points = new List<CPC_Point>();

	public CPC_Visual visual;

	public bool looped;

	public bool alwaysShow = true;

	public CPC_EAfterLoop afterLoop;

	private int currentWaypointIndex;

	private float currentTimeInWaypoint;

	private float timePerSegment;

	private bool paused;

	private bool playing;

	private void Start()
	{
		if (Camera.main == null)
		{
			Debug.LogError("There is no main camera in the scene!");
		}
		if (useMainCamera)
		{
			selectedCamera = Camera.main;
		}
		else if (selectedCamera == null)
		{
			selectedCamera = Camera.main;
			Debug.LogError("No camera selected for following path, defaulting to main camera");
		}
		if (lookAtTarget && target == null)
		{
			lookAtTarget = false;
			Debug.LogError("No target selected to look at, defaulting to normal rotation");
		}
		foreach (CPC_Point point in points)
		{
			if (point.curveTypeRotation == CPC_ECurveType.EaseInAndOut)
			{
				point.rotationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
			}
			if (point.curveTypeRotation == CPC_ECurveType.Linear)
			{
				point.rotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			}
			if (point.curveTypePosition == CPC_ECurveType.EaseInAndOut)
			{
				point.positionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
			}
			if (point.curveTypePosition == CPC_ECurveType.Linear)
			{
				point.positionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			}
		}
		if (playOnAwake)
		{
			PlayPath(playOnAwakeTime);
		}
	}

	public void PlayPath(float time)
	{
		if (time <= 0f)
		{
			time = 0.001f;
		}
		paused = false;
		playing = true;
		StopAllCoroutines();
		StartCoroutine(FollowPath(time));
	}

	public void StopPath()
	{
		playing = false;
		paused = false;
		StopAllCoroutines();
	}

	public void UpdateTimeInSeconds(float seconds)
	{
		timePerSegment = seconds / (float)((!looped) ? (points.Count - 1) : points.Count);
	}

	public void PausePath()
	{
		paused = true;
		playing = false;
	}

	public void ResumePath()
	{
		if (paused)
		{
			playing = true;
		}
		paused = false;
	}

	public bool IsPaused()
	{
		return paused;
	}

	public bool IsPlaying()
	{
		return playing;
	}

	public int GetCurrentWayPoint()
	{
		return currentWaypointIndex;
	}

	public float GetCurrentTimeInWaypoint()
	{
		return currentTimeInWaypoint;
	}

	public void SetCurrentWayPoint(int value)
	{
		currentWaypointIndex = value;
	}

	public void SetCurrentTimeInWaypoint(float value)
	{
		currentTimeInWaypoint = value;
	}

	public void RefreshTransform()
	{
		selectedCamera.transform.position = GetBezierPosition(currentWaypointIndex, currentTimeInWaypoint);
		if (!lookAtTarget)
		{
			selectedCamera.transform.rotation = GetLerpRotation(currentWaypointIndex, currentTimeInWaypoint);
		}
		else
		{
			selectedCamera.transform.rotation = Quaternion.LookRotation((target.transform.position - selectedCamera.transform.position).normalized);
		}
	}

	private IEnumerator FollowPath(float time)
	{
		UpdateTimeInSeconds(time);
		currentWaypointIndex = 0;
		while (currentWaypointIndex < points.Count)
		{
			currentTimeInWaypoint = 0f;
			while (currentTimeInWaypoint < 1f)
			{
				if (!paused)
				{
					currentTimeInWaypoint += Time.deltaTime / timePerSegment;
					selectedCamera.transform.position = GetBezierPosition(currentWaypointIndex, currentTimeInWaypoint);
					if (!lookAtTarget)
					{
						selectedCamera.transform.rotation = GetLerpRotation(currentWaypointIndex, currentTimeInWaypoint);
					}
					else
					{
						selectedCamera.transform.rotation = Quaternion.LookRotation((target.transform.position - selectedCamera.transform.position).normalized);
					}
				}
				yield return 0;
			}
			currentWaypointIndex++;
			if (currentWaypointIndex == points.Count - 1 && !looped)
			{
				break;
			}
			if (currentWaypointIndex == points.Count && afterLoop == CPC_EAfterLoop.Continue)
			{
				currentWaypointIndex = 0;
			}
		}
		StopPath();
	}

	private int GetNextIndex(int index)
	{
		if (index == points.Count - 1)
		{
			return 0;
		}
		return index + 1;
	}

	private Vector3 GetBezierPosition(int pointIndex, float time)
	{
		float t = points[pointIndex].positionCurve.Evaluate(time);
		int nextIndex = GetNextIndex(pointIndex);
		return Vector3.Lerp(Vector3.Lerp(Vector3.Lerp(points[pointIndex].position, points[pointIndex].position + points[pointIndex].handlenext, t), Vector3.Lerp(points[pointIndex].position + points[pointIndex].handlenext, points[nextIndex].position + points[nextIndex].handleprev, t), t), Vector3.Lerp(Vector3.Lerp(points[pointIndex].position + points[pointIndex].handlenext, points[nextIndex].position + points[nextIndex].handleprev, t), Vector3.Lerp(points[nextIndex].position + points[nextIndex].handleprev, points[nextIndex].position, t), t), t);
	}

	private Quaternion GetLerpRotation(int pointIndex, float time)
	{
		return Quaternion.Lerp(points[pointIndex].rotation, points[GetNextIndex(pointIndex)].rotation, points[pointIndex].rotationCurve.Evaluate(time));
	}
}
