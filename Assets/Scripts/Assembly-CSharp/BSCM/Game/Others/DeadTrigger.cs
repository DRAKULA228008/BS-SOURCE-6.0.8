using UnityEngine;

namespace BSCM.Game.Others
{
	public class DeadTrigger : MonoBehaviour
	{
		private void Start()
		{
			base.gameObject.AddComponent<DeadTrigger>();
		}
	}
}
