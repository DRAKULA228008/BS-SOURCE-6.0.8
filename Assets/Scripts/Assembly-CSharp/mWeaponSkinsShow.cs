using UnityEngine;

public class mWeaponSkinsShow : MonoBehaviour
{
	public GameObject Button;

	private void Start()
	{
		EventManager.AddListener("AccountUpdate", Check);
		Check();
	}

	private void Check()
	{
	}

    public void Load()
    {
        if (!AccountManager.isConnect)
        {
            UIToast.Show(Localization.Get("Connection account"));
            return;
        }
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
        LevelManager.LoadLevel("WeaponSkinsShow");
    }
}
