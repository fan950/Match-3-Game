using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UIBtn : MonoBehaviour
{
    private Button button;

    private const string sPath= "Sounds/UI/Button";
    public virtual void Init(Action action)
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();

        if (action != null)
            button.onClick.AddListener(delegate 
            { 
                action();

                SoundsManager.Instance.Play(sPath);
            });
    }

}
