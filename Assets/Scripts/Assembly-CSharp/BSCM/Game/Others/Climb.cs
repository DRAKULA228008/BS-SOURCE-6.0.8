using UnityEngine;

namespace BSCM.Game.Others
{
	public class Climb : MonoBehaviour
	{
		private void Start()
		{
			base.gameObject.AddComponent<ClimbSystem>();
		}
	}
}
