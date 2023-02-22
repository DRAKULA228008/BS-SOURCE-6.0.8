using System;
using UnityEngine;

[Serializable]
public class mClanCreate
{
    public GameObject panel;

    public UIInput tag;

    public UIInput name;

    public void OnSubmit()
    {
        UIInput current = UIInput.current;
        if (current == tag)
        {
            tag.value = mChangeName.UpdateSymbols(tag.value, true);
            if (tag.value.Length > 4 || tag.value.Length < 1)
            {
                tag.value = Utils.NameGenerator(3);
            }
        }
        if (current == name)
        {
            name.value = mChangeName.UpdateSymbols(name.value, true);
            if (name.value.Length > 15 || name.value.Length < 2)
            {
                name.value = Utils.NameGenerator(UnityEngine.Random.Range(5, 12));
            }
        }
    }

    public void CreateClan(Action complete, Action<string> error)
    {
        if (AccountManager.GetGold() < 250)
        {
            UIToast.Show(Localization.Get("Not enough money"));
            return;
        }
        tag.value = mChangeName.UpdateSymbols(tag.value, true);
        name.value = mChangeName.UpdateSymbols(name.value, true);
        if (tag.value.Length <= 4 && tag.value.Length >= 1)
        {
            AccountManager.SetClan(tag.value);
            AccountManager.SetClanFirebase(tag.value, null, null);
            AccountManager.UpdateGold(AccountManager.GetGold() - 250, null, null);
            AccountManager.SetGold(AccountManager.GetGold() - 250);
            EventManager.Dispatch("AccountUpdate");
            mPanelManager.Hide();
            mPanelManager.Show("Menu", true);
        }
    }
}
