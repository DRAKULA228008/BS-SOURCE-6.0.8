using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTarget : MonoBehaviour
{
	public Vector3 DefaultPosition;

	public Vector3 ActivePosition;

	public float Duration = 1f;

	public UnityEvent DamageCallback;

	private bool Activated;

	public void SetActive(bool active)
	{
		Activated = active;
		Vector3 endValue = ((!Activated) ? DefaultPosition : ActivePosition);
		base.transform.DOLocalRotate(endValue, Duration);
	}

	public bool GetActive()
	{
		return Activated;
	}

	private void Damage(DamageInfo damageInfo)
	{
		if (Activated)
		{
			DamageCallback.Invoke();
		}
	}
}
