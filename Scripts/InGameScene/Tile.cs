using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile : MonoBehaviour
{
    public eTileType tileType;
    public eTileLine tileLine;

    [HideInInspector] public BoxCollider2D boxCollider;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public eTileColor eTileColor;

    [HideInInspector] public int nPosX;
    [HideInInspector] public int nPosY;

    [HideInInspector] public List<Vector3> lisNextPos;

    [HideInInspector] public Animator animator;
    private Element element;
    [HideInInspector] public PooledObject pooledObject;

    public Action moveAction;

    public void Init(int posX, int posY)
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (pooledObject == null)
            pooledObject = GetComponent<PooledObject>();
        if (animator == null)
            animator = GetComponent<Animator>();

        ChangeTile(posX, posY);
        lisNextPos = new List<Vector3>();

        if (tileType == eTileType.Chocolate || tileType == eTileType.Marshmallow)
        {
            boxCollider.enabled = false;
        }
    }

    public void SetColor()
    {
        switch (tileType)
        {
            case eTileType.RedCandy:
            case eTileType.RedCandyHorizontalStriped:
            case eTileType.RedCandyVerticalStriped:
            case eTileType.RedCandyWrapped:
            case eTileType.RedCandy_All:
                eTileColor = eTileColor.Red;
                break;
            case eTileType.GreenCandy:
            case eTileType.GreenCandyHorizontalStriped:
            case eTileType.GreenCandyVerticalStriped:
            case eTileType.GreenCandyWrapped:
            case eTileType.GreenCandy_All:
                eTileColor = eTileColor.Green;
                break;
            case eTileType.BlueCandy:
            case eTileType.BlueCandyHorizontalStriped:
            case eTileType.BlueCandyVerticalStriped:
            case eTileType.BlueCandyWrapped:
            case eTileType.BlueCandy_All:
                eTileColor = eTileColor.Blue;
                break;
            case eTileType.YellowCandy:
            case eTileType.YellowCandyHorizontalStriped:
            case eTileType.YellowCandyVerticalStriped:
            case eTileType.YellowCandyWrapped:
            case eTileType.YellowCandy_All:
                eTileColor = eTileColor.Yellow;
                break;
            case eTileType.PurpleCandy:
            case eTileType.PurpleCandyHorizontalStriped:
            case eTileType.PurpleCandyVerticalStriped:
            case eTileType.PurpleCandyWrapped:
            case eTileType.PurpleCandy_All:
                eTileColor = eTileColor.Purple;
                break;
            case eTileType.OrangeCandy:
            case eTileType.OrangeCandyHorizontalStriped:
            case eTileType.OrangeCandyVerticalStriped:
            case eTileType.OrangeCandyWrapped:
            case eTileType.OrangeCandy_All:
                eTileColor = eTileColor.Orange;
                break;
            default:
                eTileColor = eTileColor.None;
                break;
        }
    }
    public void ChangeTile(int posX, int posY)
    {
        nPosX = posX;
        nPosY = posY;
        SetColor();

        spriteRenderer.sortingOrder = posY;
    }
    public void Move(float fSpeed)
    {
        if (lisNextPos.Count > 0)
        {
            if (transform.position == lisNextPos[0])
            {
                lisNextPos.RemoveAt(0);
                return;
            }
            transform.position = Vector3.MoveTowards(transform.position, lisNextPos[0], Time.deltaTime * fSpeed);
        }
    }

    public void AddNextPos(int nNextX, int nNextY, Vector3 pos)
    {
        if (lisNextPos == null)
            lisNextPos = new List<Vector3>();
        ChangeTile(nNextX, nNextY);
        lisNextPos.Add(pos);
    }
    public int EndPosY()
    {
        return (int)lisNextPos[lisNextPos.Count - 1].y;
    }
    public void Die(Element element = null)
    {
        this.element = element;

        if (animator != null)
            animator.SetTrigger("Kill");
        else
            DieAction();
    }
    public void DieAction()
    {
        pooledObject.pool.ReturnObj(gameObject);
    }

    public void ShowExplosionFx()
    {
        if (element != null)
        {
            var _elementfx = InGameScene.instance.fxPool.GetElementFx(element.elementType);
            _elementfx.transform.position = transform.position;
            element.DieAction();
            element = null;
        }
        else
        {
            var _fx = InGameScene.instance.fxPool.GetExplodeFx(tileType);
            _fx.transform.position = transform.position;
        }
    }
}
