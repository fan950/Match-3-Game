using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UITilePool : MonoBehaviour
{
    public RectTransform canvas;
    public RectTransform targetPos;

    [SerializeField] private ObjectPool UITile_Pool;
    private Dictionary<GameObject, Image> dicImage = new Dictionary<GameObject, Image>();

    private const float fMoveSpeed = 1300;
    private const float fSizeUpSpeed = 6;
    private const float fSizeDownSpeed = 2;

    private Action action;
    public void Init()
    {
        UITile_Pool.Init();
    }

    public void CallUITile(bool isTile,Tile tile, Element element, Action action)
    {
        this.action = action;
        GameObject _obj = UITile_Pool.GetObj();
        GameObject _tempObj = null;
        if (!dicImage.ContainsKey(_obj))
        {
            dicImage.Add(_obj, _obj.GetComponent<Image>());
        }

        if (isTile)
        {
            dicImage[_obj].sprite = tile.spriteRenderer.sprite;
            _tempObj = tile.gameObject;
        }
        else 
        {
            dicImage[_obj].sprite = element.spriteRenderer.sprite;
            _tempObj = element.gameObject;
        }

        Vector3 targetPositoin = _tempObj.transform.position;     
        Vector3 screenPositoin = Camera.main.WorldToScreenPoint(targetPositoin);
        Vector2 screenPositoin2 = new Vector2(screenPositoin.x, screenPositoin.y);
        Vector2 anchoredPositoin;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas, screenPositoin2, Camera.main, out anchoredPositoin);
        dicImage[_obj].rectTransform.anchoredPosition = anchoredPositoin;

        StartCoroutine(MoveTile(_obj));
    }

    public IEnumerator MoveTile(GameObject obj)
    {
        float _fSize = 1;
        while (true)
        {
            yield return null;

            _fSize += Time.deltaTime * fSizeUpSpeed;
            obj.transform.localScale = new Vector3(_fSize, _fSize, _fSize);

            if (obj.transform.localScale.x >= 2f)
            {
                break;
            }
        }

        _fSize = obj.transform.localScale.x;

        while (true)
        {
            yield return null;

            _fSize -= Time.deltaTime * fSizeDownSpeed;
            obj.transform.localScale = new Vector3(_fSize, _fSize, _fSize);

            if (obj.transform.localScale.x > 1.0f)
            {
                obj.transform.localScale = new Vector3(_fSize, _fSize, _fSize);
            }
            else
                obj.transform.localScale = Vector3.one;

            dicImage[obj].rectTransform.anchoredPosition = Vector2.MoveTowards(dicImage[obj].rectTransform.anchoredPosition, targetPos.anchoredPosition, fMoveSpeed * Time.deltaTime);

            if (dicImage[obj].rectTransform.anchoredPosition == targetPos.anchoredPosition)
            {
                if (action != null)
                    action();

                UITile_Pool.ReturnObj(obj);
                break;
            }
        }
    }
}
