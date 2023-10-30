using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIAniAction : MonoBehaviour
{
    private Animator animator;
    private Action action;

    private string sSoundsPath;
    public void Init(Action action,string sSoundsPath)
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        this.action = action;

        this.sSoundsPath = sSoundsPath;
    }

    public void AniAction()
    {
        action();
    }

    public void PlaySounds() 
    {
        SoundsManager.Instance.Play(sSoundsPath);
    }
}
