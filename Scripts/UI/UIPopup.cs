using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIPopup : MonoBehaviour
{
    protected Action action;
    protected RectTransform rectTransform;

    protected string sOpenSoundsPath = "Sounds/UI/PopupOpen";
    protected string sCloseSoundsPath = "Sounds/UI/PopupClose";
    public virtual void Init(Transform parent)
    {
        rectTransform = GetComponent<RectTransform>();

        transform.SetParent(parent);
        transform.localScale = Vector3.one;
        rectTransform.anchoredPosition3D = Vector3.zero;
    }
    public virtual void Open(Action action = null)
    {
        this.action = action;
        gameObject.SetActive(true);

        SoundsManager.Instance.Play(sOpenSoundsPath);
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);

        SoundsManager.Instance.Play(sCloseSoundsPath);
    }

}
