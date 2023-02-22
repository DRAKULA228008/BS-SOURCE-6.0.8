using UnityEngine;

public class PlayerSkinDamage : MonoBehaviour
{
	public PlayerSkinMember Member;

	public PlayerSkin playerSkin;

	private Transform mTrans;

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = base.transform;
			}
			return mTrans;
		}
	}

	public void Damage(DamageInfo damageInfo)
	{
		if (Member == PlayerSkinMember.Face)
		{
			damageInfo.headshot = true;
		}
		damageInfo.damage = WeaponManager.GetMemberDamage(Member, damageInfo.weapon);
		playerSkin.Damage(damageInfo);
	}
}
