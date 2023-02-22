using UnityEngine;

namespace BSCM.Game.Others
{
	public class Water : MonoBehaviour
	{
		public bool free;

		private void Start()
		{
			base.gameObject.AddComponent<WaterSystem>().freeGravity = free;
		}
	}
}
