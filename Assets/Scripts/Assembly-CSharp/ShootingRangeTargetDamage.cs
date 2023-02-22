using UnityEngine;

public class ShootingRangeTargetDamage : MonoBehaviour
{
	public PlayerSkinMember Member;

	public ShootingRangeTarget Target;

	private void Damage(DamageInfo damageInfo)
	{
		if (Member == PlayerSkinMember.Face)
		{
			damageInfo.headshot = true;
		}
		damageInfo.damage = WeaponManager.GetMemberDamage(Member, damageInfo.weapon);
		if (Target.GetActive())
		{
			UICrosshair.Hit();
		}
		Target.Damage(damageInfo);
		if (ShootingRangeManager.ShowDamage)
		{
			UIToast.Show(Localization.Get("Damage") + ": " + damageInfo.damage, 2f);
		}
	}
}
