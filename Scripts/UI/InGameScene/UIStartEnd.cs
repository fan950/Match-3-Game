using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStartEnd : MonoBehaviour
{
    public Animator startendAni;

    private Canvas canvas;
    public void Init(Canvas canvas) 
    {
        this.canvas = canvas;
        startendAni = GetComponent<Animator>();

        canvas.sortingOrder = 10;
        startendAni.SetTrigger("Start");
    }

    public void ActiveObj(bool isActive) 
    {
        canvas.sortingOrder = 10;
        gameObject.SetActive(isActive);
    }

    public void EndAni() 
    {
        canvas.sortingOrder = -1;
        gameObject.SetActive(false);
    }
}
