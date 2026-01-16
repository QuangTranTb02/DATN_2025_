using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ItemSlot
{
    public Item item;
    public int count;

    public void copy(ItemSlot slot)
    {
        item = slot.item;
        count = slot.count;
    }
    public void set(Item item, int count)
    {
        this.item = item;
        this.count = count;

    }
    public void clear()
    {
        item = null;
        count = 0;
    }
}
[CreateAssetMenu(menuName = "Data/ItemContainer")]
public class ItemContainer : ScriptableObject
{
    public List<ItemSlot> slots;

    public bool isDirty;
    internal void Init()
    {
        slots = new List<ItemSlot>();
        for(int i = 0; i < 36; i++)
        {
            slots.Add(new ItemSlot());
        }
    }
    public void Add(Item item, int Count = 1)
    {
        isDirty = true;
        if(item.Stackable == true)
        {
            ItemSlot itemslot = slots.Find(x => x.item == item);
            if(itemslot != null)
            {
                itemslot.count += Count;
            }
            else
            {
                itemslot = slots.Find(x => x.item == null);
                if(itemslot != null)
                {
                    itemslot.item = item;
                    itemslot.count = Count;
                }
            }
        }
        else
        {
            //thêm item không thể stack
            ItemSlot itemslot = slots.Find(x => x.item == null);
            if(itemslot != null)
            {
                itemslot.item = item;
            }
        }
    }
    public void Remove(Item item, int count = 1)
    {
        isDirty = true;
        if(item.Stackable)
        {
            ItemSlot itemSlot =  slots.Find(x => x.item == item);
            if (itemSlot == null) return;
            itemSlot.count -= count;
            if(itemSlot.count < 0)
            {
                itemSlot.clear();
            }
        }
        else
        {
            while(count > 0)
            {
                count -= 1;
                ItemSlot itemSlot = slots.Find(x => x.item == item);
                if (itemSlot == null) break;
                itemSlot.clear();
            }
        }
    }

    internal bool CheckFreeSpace()
    {
        for(int i = 0; i< slots.Count; i++)
        {
            if (slots[i].item == null)
                return true;
        }
        return false;
    }

    internal bool CheckItem(ItemSlot checkingItem)
    {
        ItemSlot itemSlot = slots.Find(x => x.item == checkingItem.item);
        if (itemSlot == null) return false;
        if(checkingItem.item.Stackable) return itemSlot.count >= checkingItem.count;

        return true;
    }

    
}
