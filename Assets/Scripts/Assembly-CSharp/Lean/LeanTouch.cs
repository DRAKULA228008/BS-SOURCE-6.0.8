using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lean
{
	[ExecuteInEditMode]
	[AddComponentMenu("Lean/Touch")]
	public class LeanTouch : MonoBehaviour
	{
		public static LeanTouch Instance;

		public static List<LeanFinger> Fingers = new List<LeanFinger>();

		public static List<LeanFinger> InactiveFingers = new List<LeanFinger>();

		public static Vector2 DragDelta;

		public static Vector2 SoloDragDelta;

		public static Vector2 MultiDragDelta;

		public static float TwistDegrees;

		public static float TwistRadians;

		public static float PinchScale = 1f;

		public static Action<LeanFinger> OnFingerDown;

		public static Action<LeanFinger> OnFingerSet;

		public static Action<LeanFinger> OnFingerUp;

		public static Action<LeanFinger> OnFingerDrag;

		public static Action<LeanFinger> OnFingerTap;

		public static Action<LeanFinger> OnFingerSwipe;

		public static Action<LeanFinger> OnFingerHeldDown;

		public static Action<LeanFinger> OnFingerHeldSet;

		public static Action<LeanFinger> OnFingerHeldUp;

		public static Action<int> OnMultiTap;

		public static Action<Vector2> OnDrag;

		public static Action<Vector2> OnSoloDrag;

		public static Action<Vector2> OnMultiDrag;

		public static Action<float> OnPinch;

		public static Action<float> OnTwistDegrees;

		public static Action<float> OnTwistRadians;

		public float TapThreshold = 0.5f;

		public float SwipeThreshold = 50f;

		public float HeldThreshold = 1f;

		public int ReferenceDpi = 200;

		public bool RecordFingers = true;

		public float RecordThreshold = 5f;

		public float RecordLimit;

		public bool SimulateMultiFingers = true;

		public KeyCode PinchTwistKey = KeyCode.LeftControl;

		public KeyCode MultiDragKey = KeyCode.LeftAlt;

		public Texture2D FingerTexture;

		private static int highestMouseButton = 7;

		private int lastFingerCount;

		private float multiFingerTime;

		private int multiFingerCount;

		public static float ScalingFactor
		{
			get
			{
				float result = 1f;
				int num = 200;
				if (Instance != null)
				{
					num = Instance.ReferenceDpi;
				}
				if (Screen.dpi > 0f && num > 0)
				{
					result = Mathf.Sqrt(num) / Mathf.Sqrt(Screen.dpi);
				}
				return result;
			}
		}

		public static Vector2 ScaledDragDelta
		{
			get
			{
				return DragDelta * ScalingFactor;
			}
		}

		public static Vector2 ScaledSoloDragDelta
		{
			get
			{
				return SoloDragDelta * ScalingFactor;
			}
		}

		public static Vector2 ScaledMultiDragDelta
		{
			get
			{
				return MultiDragDelta * ScalingFactor;
			}
		}

		public static bool AnyMouseButtonSet
		{
			get
			{
				for (int i = 0; i < highestMouseButton; i++)
				{
					if (Input.GetMouseButton(i))
					{
						return true;
					}
				}
				return false;
			}
		}

		public static bool GuiInUse
		{
			get
			{
				if (GUIUtility.hotControl > 0)
				{
					return true;
				}
				for (int num = Fingers.Count - 1; num >= 0; num--)
				{
					if (Fingers[num].IsOverGui)
					{
						return true;
					}
				}
				return false;
			}
		}

		public static Vector2 GetCenterOfFingers()
		{
			Vector2 zero = Vector2.zero;
			int count = Fingers.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					zero += Fingers[i].ScreenPosition;
				}
				zero /= (float)count;
			}
			return zero;
		}

		public static Vector2 GetLastCenterOfFingers()
		{
			Vector2 zero = Vector2.zero;
			int count = Fingers.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					zero += Fingers[i].LastScreenPosition;
				}
				zero /= (float)count;
			}
			return zero;
		}

		public static float GetAverageFingerDistance(Vector2 referencePoint)
		{
			float num = 0f;
			int count = Fingers.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					num += Fingers[i].GetDistance(referencePoint);
				}
				num /= (float)count;
			}
			return num;
		}

		public static float GetLastAverageFingerDistance(Vector2 referencePoint)
		{
			float num = 0f;
			int count = Fingers.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					num += Fingers[i].GetLastDistance(referencePoint);
				}
				num /= (float)count;
			}
			return num;
		}

		public static void MoveObject(Transform transform, Vector2 deltaPosition, Camera camera = null)
		{
			if (transform != null && (deltaPosition.x != 0f || deltaPosition.y != 0f))
			{
				RectTransform rectTransform = transform as RectTransform;
				if (rectTransform != null)
				{
					rectTransform.anchoredPosition += deltaPosition;
				}
				else
				{
					transform.position = MoveObject(transform.position, deltaPosition, camera);
				}
			}
		}

		public static Vector3 MoveObject(Vector3 worldPosition, Vector2 deltaPosition, Camera camera = null)
		{
			if (camera == null)
			{
				camera = Camera.main;
			}
			if (camera != null)
			{
				Vector3 position = camera.WorldToScreenPoint(worldPosition);
				position += (Vector3)deltaPosition;
				worldPosition = camera.ScreenToWorldPoint(position);
			}
			return worldPosition;
		}

		public static void RotateObject(Transform transform, float deltaRotation, Camera camera = null)
		{
			if (transform != null && deltaRotation != 0f)
			{
				transform.rotation = RotateObject(transform.rotation, deltaRotation, camera);
			}
		}

		public static Quaternion RotateObject(Quaternion worldRotation, float deltaRotation, Camera camera = null)
		{
			if (deltaRotation != 0f)
			{
				if (camera == null)
				{
					camera = Camera.main;
				}
				if (camera != null)
				{
					worldRotation = Quaternion.AngleAxis(deltaRotation, camera.transform.forward) * worldRotation;
				}
			}
			return worldRotation;
		}

		public static void ScaleObject(Transform transform, float scale)
		{
			if (transform != null && scale != 1f)
			{
				transform.localScale *= scale;
			}
		}

		public static void ScaleObject(Transform transform, Vector3 min, Vector3 max, float scale)
		{
			if (transform != null && scale != 1f)
			{
				Vector3 value = transform.localScale * scale;
				value = (transform.localScale = Clamp(value, min, max));
			}
		}

		private static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
		{
			value.x = Mathf.Clamp(value.x, min.x, max.x);
			value.y = Mathf.Clamp(value.y, min.y, max.y);
			value.z = Mathf.Clamp(value.z, min.z, max.z);
			return value;
		}

		public static void ScaleObjectRelative(Transform transform, float scale, Vector2 referencePoint, Camera camera = null)
		{
			if (transform != null && scale != 1f)
			{
				if (camera == null)
				{
					camera = Camera.main;
				}
				if (camera != null)
				{
					Vector3 position = camera.WorldToScreenPoint(transform.position);
					position.x = referencePoint.x + (position.x - referencePoint.x) * scale;
					position.y = referencePoint.y + (position.y - referencePoint.y) * scale;
					transform.position = camera.ScreenToWorldPoint(position);
					transform.localScale *= scale;
				}
			}
		}

		public static void RotateObjectRelative(Transform transform, float deltaRotation, Vector2 referencePoint, Camera camera = null)
		{
			if (transform != null && deltaRotation != 0f)
			{
				if (camera == null)
				{
					camera = Camera.main;
				}
				if (camera != null)
				{
					transform.RotateAround(camera.ScreenToWorldPoint(referencePoint), camera.transform.forward, deltaRotation);
				}
			}
		}

		protected virtual void OnEnable()
		{
			if (Instance != null && Instance != this)
			{
				Debug.LogWarning("Your scene already contains a " + typeof(LeanTouch).Name + ", destroying the old one...");
				UnityEngine.Object.DestroyImmediate(Instance.gameObject);
			}
			Instance = this;
		}

		protected virtual void Update()
		{
			UpdateAllInputs();
		}

		protected virtual void OnGUI()
		{
			if (FingerTexture != null && Input.touchCount == 0 && Fingers.Count > 1)
			{
				for (int num = Fingers.Count - 1; num >= 0; num--)
				{
					Fingers[num].Show(FingerTexture);
				}
			}
		}

		private void UpdateAllInputs()
		{
			UpdateFingers();
			UpdateMultiTap();
			UpdateGestures();
			UpdateEvents();
		}

		private void UpdateFingers()
		{
			for (int num = InactiveFingers.Count - 1; num >= 0; num--)
			{
				InactiveFingers[num].Age += Time.unscaledDeltaTime;
			}
			for (int num2 = Fingers.Count - 1; num2 >= 0; num2--)
			{
				LeanFinger leanFinger = Fingers[num2];
				if (leanFinger.Up)
				{
					Fingers.RemoveAt(num2);
					InactiveFingers.Add(leanFinger);
					leanFinger.Age = 0f;
					leanFinger.ClearSnapshots();
				}
				else
				{
					leanFinger.LastSet = leanFinger.Set;
					leanFinger.LastHeldSet = leanFinger.HeldSet;
					leanFinger.LastScreenPosition = leanFinger.ScreenPosition;
					leanFinger.Set = false;
					leanFinger.HeldSet = false;
					leanFinger.Tap = false;
				}
			}
			if (Input.touchCount > 0)
			{
				for (int i = 0; i < Input.touchCount; i++)
				{
					Touch touch = Input.GetTouch(i);
					AddFinger(touch.fingerId, touch.position);
				}
			}
			else if (AnyMouseButtonSet)
			{
				Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
				Vector2 vector = Input.mousePosition;
				if (rect.Contains(vector))
				{
					AddFinger(0, vector);
					if (SimulateMultiFingers)
					{
						if (Input.GetKey(PinchTwistKey))
						{
							Vector2 vector2 = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
							AddFinger(1, vector2 - (vector - vector2));
						}
						else if (Input.GetKey(MultiDragKey))
						{
							AddFinger(1, vector);
						}
					}
				}
			}
			for (int num3 = Fingers.Count - 1; num3 >= 0; num3--)
			{
				LeanFinger leanFinger2 = Fingers[num3];
				if (leanFinger2.Up)
				{
					if (leanFinger2.Age <= TapThreshold)
					{
						leanFinger2.Tap = true;
						leanFinger2.TapCount++;
					}
					else
					{
						leanFinger2.TapCount = 0;
					}
				}
				else if (!leanFinger2.Down)
				{
					leanFinger2.Age += Time.unscaledDeltaTime;
					if (leanFinger2.Age >= HeldThreshold)
					{
						leanFinger2.HeldSet = true;
					}
				}
				leanFinger2.TotalDeltaMagnitude += leanFinger2.DeltaScreenPosition.magnitude;
			}
		}

		private void UpdateMultiTap()
		{
			int count = Fingers.Count;
			if (count >= 1)
			{
				multiFingerTime += Time.unscaledDeltaTime;
				multiFingerCount = Mathf.Max(multiFingerCount, count);
				if (lastFingerCount == 0)
				{
					multiFingerTime = 0f;
					multiFingerCount = 0;
				}
			}
			if (count == 0 && lastFingerCount > 0 && multiFingerTime <= TapThreshold && OnMultiTap != null)
			{
				OnMultiTap(multiFingerCount);
			}
			lastFingerCount = count;
		}

		private void UpdateGestures()
		{
			int count = Fingers.Count;
			DragDelta = Vector3.zero;
			SoloDragDelta = Vector2.zero;
			MultiDragDelta = Vector2.zero;
			PinchScale = 1f;
			TwistRadians = 0f;
			TwistDegrees = 0f;
			if (count <= 0)
			{
				return;
			}
			for (int i = 0; i < count; i++)
			{
				DragDelta += Fingers[i].DeltaScreenPosition;
			}
			if (count == 1)
			{
				SoloDragDelta = Fingers[0].DeltaScreenPosition;
			}
			else
			{
				Vector2 lastCenterOfFingers = GetLastCenterOfFingers();
				Vector2 centerOfFingers = GetCenterOfFingers();
				float lastAverageFingerDistance = GetLastAverageFingerDistance(lastCenterOfFingers);
				float averageFingerDistance = GetAverageFingerDistance(centerOfFingers);
				if (lastAverageFingerDistance > 0f && averageFingerDistance > 0f)
				{
					PinchScale = averageFingerDistance / lastAverageFingerDistance;
				}
				for (int j = 0; j < count; j++)
				{
					TwistRadians += Fingers[j].GetDeltaRadians(lastCenterOfFingers, centerOfFingers);
					TwistDegrees += Fingers[j].GetDeltaDegrees(lastCenterOfFingers, centerOfFingers);
				}
				for (int k = 0; k < count; k++)
				{
					MultiDragDelta += Fingers[k].DeltaScreenPosition;
				}
			}
			TwistRadians /= count;
			TwistDegrees /= count;
			DragDelta /= (float)count;
			MultiDragDelta /= (float)count;
		}

		private void UpdateEvents()
		{
			for (int i = 0; i < Fingers.Count; i++)
			{
				LeanFinger leanFinger = Fingers[i];
				if (leanFinger.Down && OnFingerDown != null)
				{
					OnFingerDown(leanFinger);
				}
				if (leanFinger.Set && OnFingerSet != null)
				{
					OnFingerSet(leanFinger);
				}
				if (leanFinger.Up && OnFingerUp != null)
				{
					OnFingerUp(leanFinger);
				}
				if (leanFinger.Tap && OnFingerTap != null)
				{
					OnFingerTap(leanFinger);
				}
				if (leanFinger.HeldDown && OnFingerHeldDown != null)
				{
					OnFingerHeldDown(leanFinger);
				}
				if (leanFinger.HeldSet && OnFingerHeldSet != null)
				{
					OnFingerHeldSet(leanFinger);
				}
				if (leanFinger.HeldUp && OnFingerHeldUp != null)
				{
					OnFingerHeldUp(leanFinger);
				}
				if (leanFinger.DeltaScreenPosition != Vector2.zero && OnFingerDrag != null)
				{
					OnFingerDrag(leanFinger);
				}
				if (leanFinger.Up && leanFinger.GetScaledSnapshotDelta(TapThreshold).magnitude >= SwipeThreshold && OnFingerSwipe != null)
				{
					OnFingerSwipe(leanFinger);
				}
			}
			if (DragDelta != Vector2.zero && OnDrag != null)
			{
				OnDrag(DragDelta);
			}
			if (SoloDragDelta != Vector2.zero && OnSoloDrag != null)
			{
				OnSoloDrag(SoloDragDelta);
			}
			if (MultiDragDelta != Vector2.zero && OnMultiDrag != null)
			{
				OnMultiDrag(MultiDragDelta);
			}
			if (PinchScale != 1f && OnPinch != null)
			{
				OnPinch(PinchScale);
			}
			if (TwistDegrees != 0f)
			{
				if (OnTwistDegrees != null)
				{
					OnTwistDegrees(TwistDegrees);
				}
				if (OnTwistRadians != null)
				{
					OnTwistRadians(TwistRadians);
				}
			}
		}

		private void AddFinger(int index, Vector2 screenPosition)
		{
			LeanFinger leanFinger = Fingers.Find((LeanFinger t) => t.Index == index);
			if (leanFinger == null)
			{
				int num = InactiveFingers.FindIndex((LeanFinger t) => t.Index == index);
				if (num >= 0)
				{
					leanFinger = InactiveFingers[num];
					InactiveFingers.RemoveAt(num);
					if (leanFinger.Age > TapThreshold)
					{
						leanFinger.TapCount = 0;
					}
					leanFinger.Age = 0f;
					leanFinger.LastSet = false;
					leanFinger.Set = false;
					leanFinger.LastHeldSet = false;
					leanFinger.HeldSet = false;
					leanFinger.Tap = false;
				}
				else
				{
					leanFinger = new LeanFinger();
					leanFinger.Index = index;
				}
				leanFinger.StartScreenPosition = screenPosition;
				leanFinger.LastScreenPosition = screenPosition;
				leanFinger.ScreenPosition = screenPosition;
				leanFinger.StartedOverGui = leanFinger.IsOverGui;
				leanFinger.TotalDeltaMagnitude = 0f;
				Fingers.Add(leanFinger);
			}
			leanFinger.Set = true;
			leanFinger.ScreenPosition = screenPosition;
			if (!RecordFingers)
			{
				return;
			}
			if (RecordLimit > 0f && leanFinger.SnapshotDuration > RecordLimit)
			{
				int lowerSnapshotIndex = leanFinger.GetLowerSnapshotIndex(leanFinger.Age - RecordLimit);
				leanFinger.ClearSnapshots(lowerSnapshotIndex);
			}
			if (RecordThreshold > 0f)
			{
				if (leanFinger.Snapshots.Count == 0 || leanFinger.LastSnapshotDelta.magnitude >= RecordThreshold)
				{
					leanFinger.RecordSnapshot();
				}
			}
			else
			{
				leanFinger.RecordSnapshot();
			}
		}
	}
}
