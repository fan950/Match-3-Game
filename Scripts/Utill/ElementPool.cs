using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementPool : MonoBehaviour
{
    [SerializeField] private ObjectPool honey_Pool;
    [SerializeField] private ObjectPool ice_Pool;
    [SerializeField] private ObjectPool syrup1_Pool;
    [SerializeField] private ObjectPool syrup2_Pool;

    private Dictionary<eElementType, ObjectPool> dicElement = new Dictionary<eElementType, ObjectPool>();
    private Dictionary<GameObject, Element> dicSaveElement = new Dictionary<GameObject, Element>();

    public void Init()
    {
        dicElement.Add(eElementType.Honey, honey_Pool.Init());
        dicElement.Add(eElementType.Ice, ice_Pool.Init());
        dicElement.Add(eElementType.Syrup1, syrup1_Pool.Init());
        dicElement.Add(eElementType.Syrup2, syrup2_Pool.Init());
    }

    public Element GetElement(eElementType elementType)
    {
        if (elementType == eElementType.None)
            return null;

        GameObject _obj = dicElement[elementType].GetObj();

        if (!dicSaveElement.ContainsKey(_obj))
        {
            dicSaveElement.Add(_obj, _obj.GetComponent<Element>());
        }

        return dicSaveElement[_obj];
    }
}
