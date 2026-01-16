using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStorePanel : ItemPanel
{
    [SerializeField] Trading trading;
    public override void OnClick(int id)
    {
        if(GameManager.Instance.DragAndDropController.itemSlot.item == null) 
        {
            BuyItem(id);
        }
        else
        {
            SellItem();
        }
        Show();
    }

    private void BuyItem(int id)
    {
        trading.BuyItem(id);
    }

    private void SellItem()
    {
        trading.SellItem();
    }
}
