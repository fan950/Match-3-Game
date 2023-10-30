using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeFx : MonoBehaviour
{
    [HideInInspector] public PooledObject pooledObject;
    public float fTime = 2.0f;

    private float fActiveTime;

    private void OnEnable()
    {
        fActiveTime = 0.0f;
    }

    private void Start()
    {
        if (pooledObject == null)
            pooledObject = GetComponent<PooledObject>();
    }
    private void Update()
    {
        fActiveTime += Time.deltaTime;
        if (fActiveTime >= fTime)
        {
            pooledObject.pool.ReturnObj(gameObject);
        }
    }
}
