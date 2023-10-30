using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClearPopup : UIPopup
{
    public UIAniAction clearPro;
    public UIAniAction bonusTime;

    private Action bonisAction;

    private const string sClearSoundsPath = "Sounds/UI/WinStarPop";
    private const string sbonusSoundsPath = "Sounds/UI/BuyPopButton";
    public override void Init(Transform parent)
    {
        base.Init(parent);

        clearPro.Init(delegate
        {
            clearPro.gameObject.SetActive(false);
            bonusTime.gameObject.SetActive(true);
        }, sClearSoundsPath);

        bonusTime.Init(delegate
        {
            bonisAction();
            Close();
        }, sbonusSoundsPath);

        clearPro.gameObject.SetActive(false);
        bonusTime.gameObject.SetActive(false);
    }

    public void SetPopupAction(Action bonus)
    {
        bonisAction = bonus;
    }
    public override void Open(Action action = null)
    {
        base.Open();
        gameObject.SetActive(true);
        clearPro.gameObject.SetActive(true);
    }

    public override void Close()
    {
        base.Close();

        clearPro.gameObject.SetActive(false);
        bonusTime.gameObject.SetActive(false);
    }
}
