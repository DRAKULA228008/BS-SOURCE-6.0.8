using DG.Tweening;
using UnityEngine;

public class UIDamage : MonoBehaviour
{
	public Transform Arrow;

	public UISprite ArrowSprite;

	public float Duration = 1f;

	public float ArrowAngleOffset;

	[Disabled]
	public Vector3 AttackPosition;

	[Disabled]
	public Transform Player;

	[Disabled]
	public float LookAtAngle;

	private Tweener cachedTweener;

	private static UIDamage instance;

	private void Start()
	{
		instance = this;
	}

	public static void Damage(Vector3 position, Transform playerCamera)
	{
		instance.AttackPosition = position;
		instance.Player = playerCamera;
		instance.UpdateDamage();
		instance.ArrowSprite.alpha = 1f;
		if (instance.cachedTweener == null)
		{
			instance.cachedTweener = DOTween.To(() => instance.ArrowSprite.alpha, delegate(float x)
			{
				instance.ArrowSprite.alpha = x;
			}, 0f, instance.Duration).OnUpdate(instance.UpdateDamage).SetAutoKill(false);
		}
		else
		{
			instance.cachedTweener.ChangeStartValue(instance.ArrowSprite.alpha).Restart();
		}
	}

	private void UpdateDamage()
	{
		Vector3 rhs = AttackPosition - Player.position;
		rhs.y = 0f;
		rhs.Normalize();
		Vector3 forward = Player.forward;
		float num = Vector3.Dot(forward, rhs);
		if (Vector3.Cross(forward, rhs).y > 0f)
		{
			LookAtAngle = (1f - num) * -90f;
		}
		else
		{
			LookAtAngle = (1f - num) * 90f;
		}
		LookAtAngle += ArrowAngleOffset;
		Vector3 localEulerAngles = Arrow.localEulerAngles;
		localEulerAngles.z = LookAtAngle;
		Arrow.localEulerAngles = localEulerAngles;
	}
}
