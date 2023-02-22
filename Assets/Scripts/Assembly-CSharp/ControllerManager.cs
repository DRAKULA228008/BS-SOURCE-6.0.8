using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class ControllerManager : Photon.MonoBehaviour, IPunObservable
{
    [HideInInspector]
    public PlayerInput playerInput;

    [HideInInspector]
    public PlayerSkin playerSkin;

    private Vector3 PlayerPositionWidth = new Vector3(nValue.int0, 0f - nValue.float008, nValue.int0);

    private int falsePositives;

    private byte pGroup;

    private bool isPool;

    public static List<ControllerManager> ControllerList = new List<ControllerManager>();

    public static event Action<int, int, int> SetWeaponEvent;

    public static event Action<int, bool> SetDeadEvent;

    public static event Action<int, Team> SetTeamEvent;

    public static event Action<int, byte> SetHealthEvent;

    public static event Action<int> SetFireEvent;

    public static event Action<int> SetReloadEvent;

    private void Awake()
    {
        base.photonView.AddPunObservable(this);
        base.photonView.AddMessage("PhotonConnected", PhotonConnected);
        base.photonView.AddMessage("PhotonSetWeapon", PhotonSetWeapon);
        base.photonView.AddMessage("PhotonFireWeapon", PhotonFireWeapon);
        base.photonView.AddMessage("PhotonReloadWeapon", PhotonReloadWeapon);
        base.photonView.AddMessage("PhotonActivePlayer", PhotonActivePlayer);
        base.photonView.AddMessage("PhotonDeactivePlayer", PhotonDeactivePlayer);
        base.photonView.AddMessage("PhotonSetPosition", PhotonSetPosition);
        base.photonView.AddMessage("PhotonSetRotation", PhotonSetRotation);
        base.photonView.AddMessage("PhotonSpawnPlayer", PhotonSpawnPlayer);
        base.photonView.AddMessage("PhotonDamage", PhotonDamage);
        base.photonView.AddMessage("PhotonSetHealth", PhotonSetHealth);
        base.photonView.AddMessage("PhotonSetTeam", PhotonSetTeam);
        base.photonView.AddMessage("PhotonUpdateFireStatValue", PhotonUpdateFireStatValue);
        base.name = base.photonView.owner.UserId;
        if (base.photonView.isMine)
        {
            Transform transform = Utils.AddChild(GameSettings.instance.PlayerController, base.transform).transform;
            playerInput = transform.GetComponent<PlayerInput>();
            playerInput.Controller = this;
        }
        else
        {
            Transform transform2 = Utils.AddChild(GameSettings.instance.PlayerSkin, base.transform).transform;
            playerSkin = transform2.GetComponent<PlayerSkin>();
            playerSkin.PlayerRagdoll = Utils.AddChild(GameSettings.instance.PlayerRagdoll, base.transform, new Vector3(nValue.int0, 2000f, base.photonView.ownerId)).GetComponent<PlayerSkinRagdoll>();
            playerSkin.PlayerRagdoll.defaultPosition = new Vector3(nValue.int0, 2000f, base.photonView.ownerId);
        }
    }

    private void OnEnable()
    {
        if (!base.photonView.isMine)
        {
            ControllerList.Add(this);
        }
        PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
        if (isPool)
        {
            base.name = base.photonView.owner.UserId;
            playerSkin.Start();
        }
    }

    private void OnDisable()
    {
        isPool = true;
        if (!base.photonView.isMine)
        {
            ControllerList.Remove(this);
        }
        PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
        if (playerSkin != null)
        {
            playerSkin.OnDefault();
        }
    }

    private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
    {
        if (base.photonView.isMine)
        {
            bool activeSelf = playerInput.gameObject.activeSelf;
            Team playerTeam = playerInput.PlayerTeam;
            int num = AccountManager.GetPlayerSkinSelected(BodyParts.Head);
            int num2 = AccountManager.GetPlayerSkinSelected(BodyParts.Body);
            int num3 = AccountManager.GetPlayerSkinSelected(BodyParts.Legs);
            if (playerTeam == Team.Red && playerInput.Zombie)
            {
                num = 99;
                num2 = 99;
                num3 = 99;
            }
            byte value = (byte)(int)playerInput.Health;
            PlayerWeapons.PlayerWeaponData selectedWeaponData = playerInput.PlayerWeapon.GetSelectedWeaponData();
            int num4 = selectedWeaponData.ID;
            int num5 = selectedWeaponData.Skin;
            int value2 = selectedWeaponData.FireStat;
            byte[] value3 = AccountWeaponStickers.Serialize(num4, num5);
            pGroup = base.photonView.group;
            base.photonView.group = 0;
            PhotonDataWrite data = base.photonView.GetData();
            data.Write(activeSelf);
            data.Write(value);
            data.Write((byte)num);
            data.Write((byte)num2);
            data.Write((byte)num3);
            data.Write((byte)playerTeam);
            data.Write((byte)num4);
            data.Write((byte)num5);
            data.Write(value2);
            data.Write(value3);
            base.photonView.RPC("PhotonConnected", playerConnect, data);
            base.photonView.group = pGroup;
        }
    }

    [PunRPC]
    private void PhotonConnected(PhotonMessage message)
    {
        bool flag = message.ReadBool();
        byte health = message.ReadByte();
        byte head = message.ReadByte();
        byte body = message.ReadByte();
        byte legs = message.ReadByte();
        byte playerTeam = message.ReadByte();
        byte weaponID = message.ReadByte();
        byte skinID = message.ReadByte();
        int fireStat = message.ReadInt();
        byte[] stickers = message.ReadBytes();
        playerSkin.PlayerTeam = (Team)playerTeam;
        playerSkin.SetSkin(head, body, legs);
        playerSkin.Health = health;
        if (flag)
        {
            PhotonActivePlayer(Vector3.zero, Vector3.zero);
            playerSkin.SetWeapon(WeaponManager.GetWeaponData(weaponID), skinID, fireStat, stickers);
        }
        Invoke("UpdateAvatar", UnityEngine.Random.value * 2f);
    }

    private void UpdateAvatar()
    {
        if (Settings.ShowAvatars && base.photonView != null)
        {
            AvatarManager.Load(base.photonView.owner.GetAvatarUrl());
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream)
    {
        if (stream.isWriting)
        {
            stream.Write(playerInput.PlayerTransform.position + PlayerPositionWidth);
            stream.Write(playerInput.PlayerTransform.rotation);
            stream.Write((!(playerInput.MoveAxis.y < 0f)) ? playerInput.MoveAxis.magnitude : (0f - playerInput.MoveAxis.magnitude));
            stream.Write(playerInput.mCharacterController.isGrounded);
            stream.Write(playerInput.RotateCamera);
        }
        else
        {
            playerSkin.PhotonPosition = stream.ReadVector3();
            playerSkin.PhotonRotation = stream.ReadQuaternion();
            playerSkin.SetMove(stream.ReadFloat());
            playerSkin.SetGrounded(stream.ReadBool());
            playerSkin.SetRotate(stream.ReadFloat());
        }
    }

    public void SetWeapon(PlayerWeapons.PlayerWeaponData weaponData)
    {
        nProfiler.BeginSample("ControllerManager.SetWeapon");
        int num = weaponData.ID;
        int num2 = weaponData.Skin;
        int value = weaponData.FireStat;
        byte[] value2 = AccountWeaponStickers.Serialize(num, num2);
        pGroup = base.photonView.group;
        base.photonView.group = 0;
        PhotonDataWrite data = base.photonView.GetData();
        data.Write((byte)num);
        data.Write((byte)num2);
        data.Write(value);
        data.Write(value2);
        base.photonView.RPC("PhotonSetWeapon", PhotonTargets.Others, data);
        base.photonView.group = pGroup;
        nProfiler.EndSample();
    }

    [PunRPC]
    private void PhotonSetWeapon(PhotonMessage message)
    {
        byte b = message.ReadByte();
        byte b2 = message.ReadByte();
        int fireStat = message.ReadInt();
        byte[] stickers = message.ReadBytes();
        nProfiler.BeginSample("ControllerManager.PhotonSetWeapon");
        playerSkin.SetWeapon(WeaponManager.GetWeaponData(b), b2, fireStat, stickers);
        if (ControllerManager.SetWeaponEvent != null)
        {
            ControllerManager.SetWeaponEvent(base.photonView.ownerId, b, b2);
        }
        nProfiler.EndSample();
    }

    public void FireWeapon(DecalInfo decalInfo)
    {
        nProfiler.BeginSample("ControllerManager.FireWeapon");
        PhotonDataWrite data = base.photonView.GetData();
        data.Write(decalInfo.ToArray());
        base.photonView.RPC("PhotonFireWeapon", PhotonTargets.Others, data);
        DecalsManager.FireWeapon(decalInfo);
        nProfiler.EndSample();
    }

    [PunRPC]
    private void PhotonFireWeapon(PhotonMessage message)
    {
        nProfiler.BeginSample("ControllerManager.PhotonFireWeapon");
        playerSkin.Fire(DecalInfo.SetData(message.ReadBytes()));
        if (ControllerManager.SetFireEvent != null)
        {
            ControllerManager.SetFireEvent(base.photonView.ownerId);
        }
        nProfiler.EndSample();
    }

    public void ReloadWeapon()
    {
        nProfiler.BeginSample("ControllerManager.ReloadWeapon");
        base.photonView.RPC("PhotonReloadWeapon", PhotonTargets.Others);
        nProfiler.EndSample();
    }

    [PunRPC]
    public void PhotonReloadWeapon(PhotonMessage message)
    {
        nProfiler.BeginSample("ControllerManager.PhotonReloadWeapon");
        playerSkin.Reload();
        if (ControllerManager.SetReloadEvent != null)
        {
            ControllerManager.SetReloadEvent(base.photonView.ownerId);
        }
        nProfiler.EndSample();
    }

    public void ActivePlayer(Vector3 pos, Vector3 rot)
    {
        nProfiler.BeginSample("ControllerManager.ActivePlayer");
        if (base.photonView.isMine)
        {
            playerInput.FPController.Activate();
            playerInput.FPController.Stop();
            playerInput.FPController.SetPosition(pos);
            playerInput.FPCamera.SetRotation(rot);
            PhotonNetwork.player.SetDead(false);
        }
        pGroup = base.photonView.group;
        base.photonView.group = 0;
        PhotonDataWrite data = base.photonView.GetData();
        data.Write(pos);
        data.Write(rot);
        base.photonView.RPC("PhotonActivePlayer", PhotonTargets.Others, data);
        base.photonView.group = pGroup;
        nProfiler.EndSample();
    }

    [PunRPC]
    private void PhotonActivePlayer(PhotonMessage message)
    {
        PhotonActivePlayer(message.ReadVector3(), message.ReadVector3());
    }

    private void PhotonActivePlayer(Vector3 pos, Vector3 rot)
    {
        nProfiler.BeginSample("ControllerManager.PhotonActivePlayer");
        if (!base.photonView.isMine)
        {
            playerSkin.PlayerRagdoll.Deactive();
            playerSkin.SetPosition(pos);
            playerSkin.SetRotation(rot);
            playerSkin.Dead = false;
            playerSkin.PlayerRoot.SetActive(true);
            playerSkin.StartDamageTime();
            playerSkin.UpdateSkin();
            if (ControllerManager.SetDeadEvent != null)
            {
                ControllerManager.SetDeadEvent(base.photonView.ownerId, false);
            }
        }
        nProfiler.EndSample();
    }

    public void DeactivePlayer()
    {
        DeactivePlayer(Vector3.zero, false);
    }

    public void DeactivePlayer(Vector3 force)
    {
        DeactivePlayer(force, false);
    }

    public void DeactivePlayer(Vector3 force, bool headshot)
    {
        nProfiler.BeginSample("ControllerManager.DeactivePlayer");
        if (base.photonView.isMine)
        {
            playerInput.FPController.Deactivate();
            PhotonNetwork.player.SetDead(true);
        }
        pGroup = base.photonView.group;
        base.photonView.group = 0;
        PhotonDataWrite data = base.photonView.GetData();
        data.Write(force);
        data.Write(headshot);
        base.photonView.RPC("PhotonDeactivePlayer", PhotonTargets.Others, data);
        base.photonView.group = pGroup;
        nProfiler.EndSample();
    }

    [PunRPC]
    public void PhotonDeactivePlayer(PhotonMessage message)
    {
        nProfiler.BeginSample("ControllerManager.PhotonDeactivePlayer");
        Vector3 force = message.ReadVector3();
        bool head = message.ReadBool();
        if (!base.photonView.isMine)
        {
            playerSkin.ActiveRagdoll(force, head);
            playerSkin.Dead = true;
            UIDeathScreen.ClearGivenDamage(base.photonView.ownerId);
            if (ControllerManager.SetDeadEvent != null)
            {
                ControllerManager.SetDeadEvent(base.photonView.ownerId, true);
            }
        }
        nProfiler.EndSample();
    }

    public void SetPosition(Vector3 position)
    {
        nProfiler.BeginSample("ControllerManager.SetPosition");
        PhotonDataWrite data = base.photonView.GetData();
        data.Write(position);
        base.photonView.RPC("PhotonSetPosition", PhotonTargets.All, data);
        nProfiler.EndSample();
    }

    [PunRPC]
    private void PhotonSetPosition(PhotonMessage message)
    {
        nProfiler.BeginSample("ControllerManager.PhotonSetPosition");
        Vector3 position = message.ReadVector3();
        if (base.photonView.isMine)
        {
            playerInput.FPController.Stop();
            playerInput.FPController.SetPosition(position);
        }
        else
        {
            playerSkin.SetPosition(position);
        }
        nProfiler.EndSample();
    }

    public void SetRotation(Vector3 rotation)
    {
        nProfiler.BeginSample("ControllerManager.SetRotation");
        PhotonDataWrite data = base.photonView.GetData();
        data.Write(rotation);
        base.photonView.RPC("PhotonSetRotation", PhotonTargets.All, data);
        nProfiler.EndSample();
    }

    [PunRPC]
    private void PhotonSetRotation(PhotonMessage message)
    {
        nProfiler.BeginSample("ControllerManager.PhotonSetRotation");
        Vector3 vector = message.ReadVector3();
        if (base.photonView.isMine)
        {
            playerInput.FPCamera.SetRotation(vector);
        }
        else
        {
            playerSkin.SetRotation(vector);
        }
        nProfiler.EndSample();
    }

    public void SpawnPlayer(Vector3 position, Vector3 rotation)
    {
        nProfiler.BeginSample("ControllerManager.SpawnPlayer");
        PhotonDataWrite data = base.photonView.GetData();
        data.Write(position);
        data.Write(rotation);
        base.photonView.RPC("PhotonSpawnPlayer", PhotonTargets.All, data);
        nProfiler.EndSample();
    }

    [PunRPC]
    private void PhotonSpawnPlayer(PhotonMessage message)
    {
        nProfiler.BeginSample("ControllerManager.PhotonSpawnPlayer");
        Vector3 position = message.ReadVector3();
        Vector3 vector = message.ReadVector3();
        if (base.photonView.isMine)
        {
            playerInput.FPController.Stop();
            playerInput.FPController.SetPosition(position);
            playerInput.FPCamera.SetRotation(vector);
        }
        else
        {
            playerSkin.SetPosition(position);
            playerSkin.SetRotation(vector);
        }
        nProfiler.EndSample();
    }

    public void Damage(DamageInfo damageInfo)
    {
        nProfiler.BeginSample("ControllerManager.Damage");
        pGroup = base.photonView.group;
        base.photonView.group = 0;
        PhotonDataWrite data = base.photonView.GetData();
        data.Write(damageInfo.Deserialize());
        base.photonView.RPC("PhotonDamage", base.photonView.owner, data);
        base.photonView.group = pGroup;
        UIDeathScreen.AddGivenDamage(base.photonView.ownerId, damageInfo.damage);
        nProfiler.EndSample();
    }

    [PunRPC]
    private void PhotonDamage(PhotonMessage message)
    {
        nProfiler.BeginSample("ControllerManager.PhotonDamage");
        DamageInfo damageInfo = DamageInfo.Serialize(message.ReadBytes());
        if (message.timestamp + (double)nValue.float05 > PhotonNetwork.time)
        {
            UIDeathScreen.AddTakenDamage(message.sender.ID, damageInfo.damage);
            playerInput.Damage(damageInfo);
            if (falsePositives >= nValue.int0)
            {
                falsePositives--;
            }
        }
        else
        {
            falsePositives++;
            if (falsePositives >= nValue.int10)
            {
                PhotonNetwork.LeaveRoom();
            }
        }
        nProfiler.EndSample();
    }

    public void SetHealth(byte health)
    {
        nProfiler.BeginSample("ControllerManager.SetHealth");
        PhotonDataWrite data = base.photonView.GetData();
        data.Write(health);
        base.photonView.RPC("PhotonSetHealth", PhotonTargets.All, data);
        nProfiler.EndSample();
    }

    [PunRPC]
    private void PhotonSetHealth(PhotonMessage message)
    {
        nProfiler.BeginSample("ControllerManager.PhotonSetHealth");
        byte b = message.ReadByte();
        if (!base.photonView.isMine)
        {
            playerSkin.Health = b;
            if (ControllerManager.SetHealthEvent != null)
            {
                ControllerManager.SetHealthEvent(base.photonView.ownerId, b);
            }
        }
        nProfiler.EndSample();
    }

    public void SetTeam(Team team)
    {
        base.photonView.owner.SetTeam(team);
        byte value = (byte)AccountManager.GetPlayerSkinSelected(BodyParts.Head);
        byte value2 = (byte)AccountManager.GetPlayerSkinSelected(BodyParts.Body);
        byte value3 = (byte)AccountManager.GetPlayerSkinSelected(BodyParts.Legs);
        if (team == Team.Red && playerInput.Zombie)
        {
            value = 99;
            value2 = 99;
            value3 = 99;
        }
        pGroup = base.photonView.group;
        base.photonView.group = 0;
        PhotonDataWrite data = base.photonView.GetData();
        data.Write((byte)team);
        data.Write(value);
        data.Write(value2);
        data.Write(value3);
        base.photonView.RPC("PhotonSetTeam", PhotonTargets.All, data);
        base.photonView.group = pGroup;
    }

    [PunRPC]
    private void PhotonSetTeam(PhotonMessage message)
    {
        byte b = message.ReadByte();
        byte head = message.ReadByte();
        byte body = message.ReadByte();
        byte legs = message.ReadByte();
        if (base.photonView.isMine)
        {
            playerInput.PlayerTeam = (Team)b;
            return;
        }
        playerSkin.PlayerTeam = (Team)b;
        playerSkin.SetSkin(head, body, legs);
        playerSkin.UpdateSkin();
        if (ControllerManager.SetTeamEvent != null)
        {
            ControllerManager.SetTeamEvent(base.photonView.ownerId, (Team)b);
        }
    }

    public void UpdateFireStatValue(int weaponID)
    {
        PhotonDataWrite data = base.photonView.GetData();
        data.Write((byte)weaponID);
        base.photonView.RPC("PhotonUpdateFireStatValue", PhotonTargets.Others, data);
    }

    [PunRPC]
    private void PhotonUpdateFireStatValue(PhotonMessage message)
    {
        byte b = message.ReadByte();
        if (playerSkin.SelectWeapon != null && playerSkin.SelectWeapon.Data.weapon == b)
        {
            playerSkin.SelectWeapon.UpdateFireStat1();
        }
    }

    public static ControllerManager FindController(int id)
    {
        for (int i = 0; i < ControllerList.Count; i++)
        {
            if (ControllerList[i].photonView.ownerId == id)
            {
                return ControllerList[i];
            }
        }
        return null;
    }
}
