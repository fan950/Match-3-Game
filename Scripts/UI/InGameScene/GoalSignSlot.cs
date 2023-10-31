using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalSignSlot : MonoBehaviour
{
    public Image iconImg;
    public Text countTxt;
    public void Init(Sprite sprite, int count, Vector2 imgSize, bool isAll = false)
    {
        iconImg.rectTransform.sizeDelta = imgSize;
        iconImg.sprite = sprite;
        if (isAll)
            countTxt.text = "All";
        else
            countTxt.text = count.ToString();
    }
}
