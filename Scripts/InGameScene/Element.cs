using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Element : MonoBehaviour
{
    [HideInInspector] public int nPosX;
    [HideInInspector] public int nPosY;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    public int nDownCount;
    public eElementType elementType;
    [HideInInspector] public Animator animator;

    [HideInInspector] public PooledObject pooledObject;
    public void Init(int nPosX, int nPosY)
    {
        this.nPosX = nPosX;
        this.nPosY = nPosY;

        if (pooledObject == null)
            pooledObject = GetComponent<PooledObject>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void Die()
    {
        if (animator != null)
            animator.SetTrigger("Kill");
        else
            DieAction();
    }
    public void DieAction()
    {
        pooledObject.pool.ReturnObj(gameObject);
    }
}
