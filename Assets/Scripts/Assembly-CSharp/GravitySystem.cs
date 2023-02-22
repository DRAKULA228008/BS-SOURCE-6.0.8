using UnityEngine;

public class GravitySystem : MonoBehaviour
{
	public CryptoFloat jumpForce = 0.15f;

	public CryptoFloat jumpForceDamping = 0.1f;

	public CryptoFloat gravity = 0.2f;

	public CryptoFloat gravityRagdoll = 9.8f;

	private void Start()
	{
		TimerManager.In(0.5f, delegate
		{
			vp_FPController fPController = GameManager.player.FPController;
			fPController.PhysicsGravityModifier = (float)gravity;
			fPController.MotorJumpForce = (float)jumpForce;
			fPController.MotorJumpForceDamping = (float)jumpForceDamping;
		});
	}
}
