using UnityEngine;

public class mMapCamera : MonoBehaviour
{
	public float Speed;

	public float Amplitude;

	public MeshAtlas[] AK47;

	public MeshAtlas[] M4A1;

	public MeshAtlas[] AWP;

	public MeshAtlas[] Galil;

	public MeshAtlas[] Axe;

	public MeshAtlas[] Katana;

	private Transform mTransform;

	private Vector3 StartPosition;

	private void Start()
	{
		mTransform = base.transform;
		StartPosition = mTransform.position;
		TimerManager.In(0.1f, delegate
		{
			string spriteName = "1-" + WeaponManager.GetRandomWeaponSkin(1).ID;
			for (int i = 0; i < AK47.Length; i++)
			{
				AK47[i].spriteName = spriteName;
			}
			spriteName = "5-" + WeaponManager.GetRandomWeaponSkin(5).ID;
			for (int j = 0; j < M4A1.Length; j++)
			{
				M4A1[j].spriteName = spriteName;
			}
			spriteName = "8-" + WeaponManager.GetRandomWeaponSkin(8).ID;
			for (int k = 0; k < AWP.Length; k++)
			{
				AWP[k].spriteName = spriteName;
			}
			spriteName = "28-" + WeaponManager.GetRandomWeaponSkin(28).ID;
			for (int l = 0; l < Galil.Length; l++)
			{
				Galil[l].spriteName = spriteName;
			}
			spriteName = "10-" + WeaponManager.GetRandomWeaponSkin(10).ID;
			for (int m = 0; m < Axe.Length; m++)
			{
				Axe[m].spriteName = spriteName;
			}
			spriteName = "22-" + WeaponManager.GetRandomWeaponSkin(22).ID;
			for (int n = 0; n < Katana.Length; n++)
			{
				Katana[n].spriteName = spriteName;
			}
		});
	}

	private void LateUpdate()
	{
		mTransform.position = StartPosition + Vector3.forward * Mathf.Cos(Time.time * Speed) * Amplitude;
	}
}
