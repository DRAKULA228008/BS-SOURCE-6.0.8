using UnityEngine;

namespace BSCM.Game.Others
{
	public class Teleport : MonoBehaviour
	{
		public Vector3 to;

		private void Start()
		{
			TriggerTeleport triggerTeleport = base.gameObject.AddComponent<TriggerTeleport>();
			triggerTeleport.Position = to;
		}
	}
}
