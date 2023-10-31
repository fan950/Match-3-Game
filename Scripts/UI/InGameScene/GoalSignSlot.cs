using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalSignSlot : MonoBehaviour
{
    public Image iconImg;
    public Text countTxt;
    public void Init(Sprite sprite, int count, float fSize = 1.0f, bool isAll = false)
    {
        transform.localScale = new Vector3(fSize, fSize, fSize);
        iconImg.sprite = sprite;
        if (isAll)
            countTxt.text = "All";
        else
            countTxt.text = count.ToString();
    }
}
