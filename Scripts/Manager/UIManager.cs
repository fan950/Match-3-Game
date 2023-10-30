using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<string, UIPopup> dicUIPopup = new Dictionary<string, UIPopup>();
    private Dictionary<eAtlasType, SpriteAtlas> dicSpriteAtlas = new Dictionary<eAtlasType, SpriteAtlas>();
    public override void Awake()
    {
        base.Awake();
        SetSprite();
    }
    public UIPopup OpenPopup(string sPath, Transform parent)
    {
        if (!dicUIPopup.ContainsKey(sPath))
        {
            GameObject _obj = Instantiate(Resources.Load(sPath)) as GameObject;

            UIPopup _uIPopup = _obj.GetComponent<UIPopup>();
            _uIPopup.Init(parent);

            dicUIPopup.Add(sPath, _uIPopup);
        }
        return dicUIPopup[sPath];
    }

    public void UIPopupClear()
    {
        foreach (KeyValuePair<string, UIPopup> popup in dicUIPopup)
        {
            Destroy(popup.Value.gameObject);
        }
        dicUIPopup.Clear();
    }


    #region Atlas
    public void SetSprite()
    {
        string str = string.Empty;
        dicSpriteAtlas.Clear();
        eAtlasType eAtlas = eAtlasType.None;
        for (int i = 1; i < (int)eAtlasType.Max; ++i)
        {
            eAtlas = (eAtlasType)i;
            SpriteAtlas spriteAtlas = Resources.Load<SpriteAtlas>("Atlas/" + eAtlas.ToString());

            if (spriteAtlas == null)
            {
                Debug.LogError("아틀란스 없음");
                return;
            }

            dicSpriteAtlas.Add(eAtlas, spriteAtlas);
        }
    }

    public Sprite GetSprite(eAtlasType key, string name)
    {
        return dicSpriteAtlas[key].GetSprite(name);
    }
    #endregion
}
