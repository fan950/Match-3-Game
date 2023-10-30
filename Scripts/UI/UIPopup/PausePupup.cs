using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PausePupup : UIPopup
{
    public UIBtn retryBtn;
    public UIBtn continueBtn;
    public UIBtn homeBtn;

    public override void Init(Transform parent)
    {
        base.Init(parent);

        retryBtn.Init(delegate
        {
            GameManager.Instance.NextScene(eScene.InGameScene);
        });
        homeBtn.Init(delegate
        {
            GameManager.Instance.NextScene(eScene.LevelScene);
        });
        continueBtn.Init(delegate
        {
            Close();
        });
    }
    public override void Close()
    {
        base.Close();
        InGameScene.instance.isPuase = false;
    }
}
