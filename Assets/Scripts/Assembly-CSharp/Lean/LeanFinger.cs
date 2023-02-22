using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lean
{
	public class LeanFinger
	{
		public class Snapshot
		{
			public float Age;

			public Vector2 ScreenPosition;

			public Vector3 GetWorldPosition(float distance, Camera camera = null)
			{
				if (camera == null)
				{
					camera = Camera.main;
				}
				if (camera != null)
				{
					Vector3 position = new Vector3(ScreenPosition.x, ScreenPosition.y, distance);
					return camera.ScreenToWorldPoint(position);
				}
				return default(Vector3);
			}
		}

		public int Index;

		public float Age;

		public bool Set;

		public bool HeldSet;

		public bool Tap;

		public int TapCount;

		public Vector2 StartScreenPosition;

		public Vector2 LastScreenPosition;

		public float TotalDeltaMagnitude;

		public bool LastSet;

		public bool LastHeldSet;

		public Vector2 ScreenPosition;

		public bool StartedOverGui;

		public List<Snapshot> Snapshots = new List<Snapshot>();

		private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>();

		private static List<Snapshot> tempSnapshots = new List<Snapshot>();

		public bool IsActive
		{
			get
			{
				return LeanTouch.Fingers.Contains(this);
			}
		}

		public float SnapshotDuration
		{
			get
			{
				if (Snapshots.Count > 0)
				{
					return Age - Snapshots[0].Age;
				}
				return 0f;
			}
		}

		public bool IsOverGui
		{
			get
			{
				EventSystem current = EventSystem.current;
				if (current != null)
				{
					PointerEventData pointerEventData = new PointerEventData(current);
					pointerEventData.position = new Vector2(ScreenPosition.x, ScreenPosition.y);
					tempRaycastResults.Clear();
					current.RaycastAll(pointerEventData, tempRaycastResults);
					return tempRaycastResults.Count > 0;
				}
				return false;
			}
		}

		public bool HeldDown
		{
			get
			{
				return HeldSet && !LastHeldSet;
			}
		}

		public bool HeldUp
		{
			get
			{
				return !HeldSet && LastHeldSet;
			}
		}

		public bool Down
		{
			get
			{
				return Set && !LastSet;
			}
		}

		public bool Up
		{
			get
			{
				return !Set && LastSet;
			}
		}

		public Vector2 LastSnapshotDelta
		{
			get
			{
				int count = Snapshots.Count;
				if (count > 0)
				{
					Snapshot snapshot = Snapshots[count - 1];
					if (snapshot != null)
					{
						return ScreenPosition - snapshot.ScreenPosition;
					}
				}
				return Vector2.zero;
			}
		}

		public Vector2 DeltaScreenPosition
		{
			get
			{
				return ScreenPosition - LastScreenPosition;
			}
		}

		public Vector2 ScaledDeltaScreenPosition
		{
			get
			{
				return DeltaScreenPosition * LeanTouch.ScalingFactor;
			}
		}

		public Vector2 TotalDeltaScreenPosition
		{
			get
			{
				return ScreenPosition - StartScreenPosition;
			}
		}

		public Vector2 ScaledTotalDeltaScreenPosition
		{
			get
			{
				return TotalDeltaScreenPosition * LeanTouch.ScalingFactor;
			}
		}

		public Vector2 SwipeDelta
		{
			get
			{
				if (LeanTouch.Instance != null)
				{
					return GetSnapshotDelta(LeanTouch.Instance.TapThreshold);
				}
				return Vector2.zero;
			}
		}

		public Vector2 ScaledSwipeDelta
		{
			get
			{
				return SwipeDelta * LeanTouch.ScalingFactor;
			}
		}

		public float ScaledTotalDeltaMagnitude
		{
			get
			{
				return TotalDeltaMagnitude * LeanTouch.ScalingFactor;
			}
		}

		public Ray GetRay(Camera camera = null)
		{
			if (camera == null)
			{
				camera = Camera.main;
			}
			if (camera != null)
			{
				return camera.ScreenPointToRay(ScreenPosition);
			}
			return default(Ray);
		}

		public Vector2 GetSnapshotDelta(float deltaTime)
		{
			return ScreenPosition - GetSnapshotScreenPosition(Age - deltaTime);
		}

		public Vector2 GetScaledSnapshotDelta(float deltaTime)
		{
			return GetSnapshotDelta(deltaTime) * LeanTouch.ScalingFactor;
		}

		public Vector2 GetSnapshotScreenPosition(float targetAge)
		{
			int lowerSnapshotIndex = GetLowerSnapshotIndex(targetAge);
			float age = 0f;
			Vector2 screenPosition = default(Vector2);
			GetSnapshot(lowerSnapshotIndex, out age, out screenPosition);
			if (targetAge <= age)
			{
				return screenPosition;
			}
			int index = lowerSnapshotIndex + 1;
			float age2 = 0f;
			Vector2 screenPosition2 = default(Vector2);
			GetSnapshot(index, out age2, out screenPosition2);
			if (targetAge >= age2)
			{
				return screenPosition2;
			}
			return Vector2.Lerp(screenPosition, screenPosition2, Mathf.InverseLerp(age, age2, targetAge));
		}

		public void GetSnapshot(int index, out float age, out Vector2 screenPosition)
		{
			if (index >= 0 && index < Snapshots.Count)
			{
				Snapshot snapshot = Snapshots[index];
				age = snapshot.Age;
				screenPosition = snapshot.ScreenPosition;
			}
			else
			{
				age = Age;
				screenPosition = ScreenPosition;
			}
		}

		public float GetRadians(Vector2 referencePoint)
		{
			return Mathf.Atan2(ScreenPosition.x - referencePoint.x, ScreenPosition.y - referencePoint.y);
		}

		public float GetDegrees(Vector2 referencePoint)
		{
			return GetRadians(referencePoint) * 57.29578f;
		}

		public float GetLastRadians(Vector2 referencePoint)
		{
			return Mathf.Atan2(LastScreenPosition.x - referencePoint.x, LastScreenPosition.y - referencePoint.y);
		}

		public float GetLastDegrees(Vector2 referencePoint)
		{
			return GetLastRadians(referencePoint) * 57.29578f;
		}

		public float GetDeltaRadians(Vector2 referencePoint)
		{
			return GetDeltaRadians(referencePoint, referencePoint);
		}

		public float GetDeltaRadians(Vector2 lastReferencePoint, Vector2 referencePoint)
		{
			float lastRadians = GetLastRadians(lastReferencePoint);
			float radians = GetRadians(referencePoint);
			float num = Mathf.Repeat(lastRadians - radians, (float)Math.PI * 2f);
			if (num > (float)Math.PI)
			{
				num -= (float)Math.PI * 2f;
			}
			return num;
		}

		public float GetDeltaDegrees(Vector2 referencePoint)
		{
			return GetDeltaRadians(referencePoint, referencePoint) * 57.29578f;
		}

		public float GetDeltaDegrees(Vector2 lastReferencePoint, Vector2 referencePoint)
		{
			return GetDeltaRadians(lastReferencePoint, referencePoint) * 57.29578f;
		}

		public float GetLastDistance(Vector2 referencePoint)
		{
			return Vector2.Distance(LastScreenPosition, referencePoint);
		}

		public float GetDistance(Vector2 referencePoint)
		{
			return Vector2.Distance(ScreenPosition, referencePoint);
		}

		public Vector3 GetWorldPosition(float distance, Camera camera = null)
		{
			if (camera == null)
			{
				camera = Camera.main;
			}
			if (camera != null)
			{
				Vector3 position = new Vector3(ScreenPosition.x, ScreenPosition.y, distance);
				return camera.ScreenToWorldPoint(position);
			}
			return default(Vector3);
		}

		public Vector3 GetLastWorldPosition(float distance, Camera camera = null)
		{
			if (camera == null)
			{
				camera = Camera.main;
			}
			if (camera != null)
			{
				Vector3 position = new Vector3(ScreenPosition.x, ScreenPosition.y, distance);
				return camera.ScreenToWorldPoint(position);
			}
			return default(Vector3);
		}

		public Vector3 GetDeltaWorldPosition(float distance, Camera camera = null)
		{
			return GetDeltaWorldPosition(distance, distance, camera);
		}

		public Vector3 GetDeltaWorldPosition(float lastDistance, float distance, Camera camera = null)
		{
			if (camera == null)
			{
				camera = Camera.main;
			}
			if (camera != null)
			{
				return GetWorldPosition(distance, camera) - GetLastWorldPosition(lastDistance, camera);
			}
			return default(Vector3);
		}

		public void Show(Texture texture)
		{
			if (texture != null && Set)
			{
				Rect position = new Rect(0f, 0f, texture.width, texture.height);
				position.center = new Vector2(ScreenPosition.x, (float)Screen.height - ScreenPosition.y);
				GUI.DrawTexture(position, texture);
			}
		}

		public void ClearSnapshots(int count = -1)
		{
			if (count > 0 && count <= Snapshots.Count)
			{
				for (int i = 0; i < count; i++)
				{
					tempSnapshots.Add(Snapshots[i]);
				}
				Snapshots.RemoveRange(0, count);
			}
			else if (count < 0)
			{
				tempSnapshots.AddRange(Snapshots);
				Snapshots.Clear();
			}
		}

		public void RecordSnapshot()
		{
			Snapshot snapshot = null;
			int count = tempSnapshots.Count;
			if (count > 0)
			{
				snapshot = tempSnapshots[count - 1];
				tempSnapshots.RemoveAt(count - 1);
			}
			if (snapshot == null)
			{
				snapshot = new Snapshot();
			}
			snapshot.Age = Age;
			snapshot.ScreenPosition = ScreenPosition;
			Snapshots.Add(snapshot);
		}

		public int GetLowerSnapshotIndex(float targetAge)
		{
			int count = Snapshots.Count;
			if (count > 0)
			{
				float age = Snapshots[0].Age;
				if (targetAge > age)
				{
					for (int i = 1; i < count; i++)
					{
						if (Snapshots[i].Age > targetAge)
						{
							return i - 1;
						}
					}
					return count - 1;
				}
			}
			return 0;
		}
	}
}
