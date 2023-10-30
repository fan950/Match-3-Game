using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;

    private Stack<GameObject> stackSaveObj = new Stack<GameObject>();
    public ObjectPool Init(int nCount = 15)
    {
        for (var i = 0; i < nCount; ++i)
        {
            GameObject obj = CreateObj();
            obj.SetActive(false);
            stackSaveObj.Push(obj);
        }
        return this;
    }

    private GameObject CreateObj()
    {
        GameObject _obj = Instantiate(prefab);
        PooledObject _poolObj = _obj.GetComponent<PooledObject>();
        if (_poolObj == null)
        {
            _poolObj = _obj.AddComponent<PooledObject>();
        }
        _poolObj.pool = this;
        _obj.transform.SetParent(transform);
        _obj.transform.localScale = Vector3.one;
        return _obj;
    }

    public GameObject GetObj()
    {
        GameObject _obj = null;
        if (stackSaveObj.Count > 0)
            _obj = stackSaveObj.Pop();
        else
            _obj = CreateObj();
        _obj.SetActive(true);
        return _obj;

    }

    public void ReturnObj(GameObject obj)
    {
        obj.SetActive(false);

        if (!stackSaveObj.Contains(obj))
        {
            stackSaveObj.Push(obj);
        }
        obj.transform.parent = transform;
    }
}
