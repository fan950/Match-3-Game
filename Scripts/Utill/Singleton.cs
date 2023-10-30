using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
{
    #region Singleton
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = Instantiate(Resources.Load("Manager/" + typeof(T).Name) as GameObject);
                obj.name = typeof(T).Name;
                instance = obj.GetComponent<T>();
            }
            return instance;
        }
    }
    #endregion

    public virtual void Awake()
    {
        if (null == instance)
            instance = GetComponent<T>();

        DontDestroyOnLoad(gameObject);
    }
}
