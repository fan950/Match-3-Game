using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    [HideInInspector] public eItemType itemType;
    [HideInInspector] public LocalGameData localGameData;
    public List<ItemSlot> lisItemSlot;

    public void Init(LocalGameData localGameData)
    {
        this.localGameData = localGameData;
        for (int i = 0; i < lisItemSlot.Count; ++i)
        {
            lisItemSlot[i].SetItemList(this);
            lisItemSlot[i].Init(null);
        }
    }

    public void SetAppltItem(eItemType itemType)
    {
        this.itemType = itemType;
        InGameScene.instance.ApplyItemMode();
    }
    public void SetCount()
    {
        for (int i = 0; i < lisItemSlot.Count; ++i)
        {
            lisItemSlot[i].SetCount();
        }
    }
}
