using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComboGroup : MonoBehaviour
{
    private List<GameObject> lisUnActive=new List<GameObject>();

    private Dictionary<GameObject, UICombo> dicImg = new Dictionary<GameObject, UICombo>();

    private const int nMax = 5;
    public void Init()
    {
        for (int i = 0; i < nMax; ++i)
        {
            GameObject _obj = Instantiate(Resources.Load("UI/InGame/UICombo") as GameObject);
            dicImg.Add(_obj, _obj.GetComponent<UICombo>());

            _obj.transform.SetParent(transform);
            dicImg[_obj].Init(UnActiveCombo);
            _obj.SetActive(false);

            lisUnActive.Add(_obj);
        }
    }
    public void UnActiveCombo(GameObject obj)
    {
        lisUnActive.Add(obj);
    }
    public void SetCombo(int nCount)
    {
        GameObject _obj = lisUnActive[0];
        lisUnActive.RemoveAt(0);

        Sprite sprite = null;
        switch (nCount)
        {
            case 1:
                sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, "Good");
                break;
            case 2:
                sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, "Super");
                break;
            default:
                sprite = UIManager.Instance.GetSprite(eAtlasType.InGame_UI, "Yummy");
                break;
        }
        dicImg[_obj].SetImg(sprite);
        dicImg[_obj].gameObject.SetActive(true);
    }
}
