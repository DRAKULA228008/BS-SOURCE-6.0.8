using UnityEngine;

public class PlayerAIDamage : MonoBehaviour
{
	public PlayerSkinMember Member;

	public PlayerAI playerAI;

	private void Damage(DamageInfo damageInfo)
	{
		if (Member == PlayerSkinMember.Face)
		{
			damageInfo.headshot = true;
		}
		damageInfo.damage = WeaponManager.GetMemberDamage(Member, damageInfo.weapon);
		playerAI.Damage(damageInfo);
	}
}
