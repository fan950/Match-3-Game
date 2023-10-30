using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyPopup : UIPopup
{
    public void AniAction() 
    {
        InGameScene.instance.isPuase = false;
        Close();
    }
}
