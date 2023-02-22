using System;
using System.Collections.Generic;
using FreeJSON;
using Prime31;
using UnityEngine;

// Token: 0x0200035C RID: 860
public class InAppManager : MonoBehaviour
{
    // Token: 0x06001DA8 RID: 7592 RVA: 0x000140F7 File Offset: 0x000122F7
    private void Start()
    {

    }

    // Token: 0x06001DA9 RID: 7593 RVA: 0x000A079C File Offset: 0x0009E99C
    private void OnEnable()
    {

    }

    // Token: 0x06001DAA RID: 7594 RVA: 0x000A0820 File Offset: 0x0009EA20
    private void OnDisable()
    {

    }

    // Token: 0x06001DAB RID: 7595 RVA: 0x00014104 File Offset: 0x00012304
    public static void Init()
    {

    }

    // Token: 0x06001DAC RID: 7596 RVA: 0x00014120 File Offset: 0x00012320
    private void billingSupportedEvent()
    {
        this.UpdateQueryInventory();
    }

    // Token: 0x06001DAD RID: 7597 RVA: 0x000A08A4 File Offset: 0x0009EAA4
    private void UpdateQueryInventory()
    {

    }

    // Token: 0x06001DAE RID: 7598 RVA: 0x00014128 File Offset: 0x00012328
    private void billingNotSupportedEvent(string error)
    {
        MonoBehaviour.print("billingNotSupportedEvent: " + error);
    }

    // Token: 0x06001DAF RID: 7599 RVA: 0x000A08E0 File Offset: 0x0009EAE0
    public static string GetPrice(string sku)
    {
        return string.Empty;
    }

    // Token: 0x06001DB3 RID: 7603 RVA: 0x00014142 File Offset: 0x00012342
    private void purchaseFailedEvent(string error, int response)
    {
        UIToast.Show(Localization.Get("Error") + ": " + error);
    }

    // Token: 0x06001DB4 RID: 7604 RVA: 0x0001415E File Offset: 0x0001235E
    private void Failed()
    {
        mPopUp.ShowPopup(Localization.Get("An error occurred while syncing the data"), Localization.Get("Error"), Localization.Get("Refresh"), delegate ()
        {
            AccountManager.UpdateData(new Action(this.Failed));
        });
    }

    // Token: 0x06001DB5 RID: 7605 RVA: 0x0001418F File Offset: 0x0001238F
    public static void Consume(string sku)
    {

    }

    public static void Purchase(string sku)
    {

    }

    // Token: 0x040011D3 RID: 4563
    private static InAppManager instance;
}
