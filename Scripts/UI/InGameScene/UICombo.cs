using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UICombo : MonoBehaviour
{
    private Image comboImg;
    public Action<GameObject> action;
    public void Init(Action<GameObject> action)
    {
        if (comboImg == null)
            comboImg = GetComponent<Image>();

        this.action = action;

        transform.localScale = Vector3.one;
        comboImg.rectTransform.anchoredPosition3D = Vector3.zero;
    }

    public void SetImg(Sprite sprite)
    {
        comboImg.sprite = sprite;
    }

    public void AniAction()
    {
        action(gameObject);
        gameObject.SetActive(false);
    }
}
