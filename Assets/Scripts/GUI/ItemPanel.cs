using System.Collections.Generic;
using UnityEngine;

public class ItemPanel : MonoBehaviour
{
    public ItemContainer itemContainer;
    public List<InventoryButton> Buttons;

    private void OnEnable()
    {
        Clear();
        Show();
    }
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        SetSourcePanel();
        SetIndex();
        Show();
    }

    private void SetSourcePanel()
    {
        for(int i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].SetItemPanel(this);
        }
    }

    private void LateUpdate()
    {
        if(itemContainer == null) { return; }
        if (itemContainer.isDirty)
        {
            Show();
            itemContainer.isDirty = false;
        }
    }
    private void SetIndex()
    {
        for (int i = 0;  i < Buttons.Count; i++)
        {
            Buttons[i].SetIndex(i);
        }
    }

    public virtual void Show()
    {
        if (itemContainer == null) { return; }
        for (int i = 0; i < itemContainer.slots.Count && i < Buttons.Count; i++)
        {
            if (itemContainer.slots[i].item == null)
            {
                Buttons[i].clean();
            }
            else
            {
                Buttons[i].Set(itemContainer.slots[i]);
            }
        }
    }
    public void Clear()
    {
        for(int i = 0;i < Buttons.Count; i++)
        {
            Buttons[i].clean();
        }
    }
    public void SetInventory(ItemContainer newInventory)
    {
        itemContainer = newInventory;
    }
    public virtual void OnClick(int id)
    {

    }
}
