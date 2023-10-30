using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemSlot : UIBtn
{
    public eItemType itemType;
    private ItemList itemList;

    public Text count;

    public override void Init(Action action)
    {
        base.Init(delegate { itemList.SetAppltItem(itemType); });
    }
    public void SetItemList(ItemList itemList)
    {
        this.itemList = itemList;
        SetCount();
    }

    public void SetCount() 
    {
        switch (itemType)
        {
            case eItemType.Lollipop:
                count.text = itemList.localGameData.nLollipop.ToString();
                break;
            case eItemType.All:
                count.text = itemList.localGameData.nAll.ToString();
                break;
            case eItemType.Switch:
                count.text = itemList.localGameData.nSwitch.ToString();
                break;
            case eItemType.ColorBomb:
                count.text = itemList.localGameData.nColorBomb.ToString();
                break;
        }
    }
}
