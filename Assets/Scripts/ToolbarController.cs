using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarController : Singleton<ToolbarController>
{
    [SerializeField] int ToolbarSize = 12;
    int SelectedTool;
    [SerializeField] IconHighLight IconHighLight;
    public Action<int> OnChange;
    internal void set(int id)
    {
        SelectedTool = id;
    }
    public ItemSlot GetItemSlot
    {
        get
        {
            return GameManager.Instance.inventoryContainer.slots[SelectedTool];
        }
    }
    public Item GetItem
    {
        get
        {
            return GameManager.Instance.inventoryContainer.slots[SelectedTool].item;
        }
    }
    private void Start()
    {
        OnChange += UpdateHighlightIcon;
        UpdateHighlightIcon(SelectedTool);
    }
    private void Update()
    {
        //lăn con lăn để chuyển selection item
        float delta = Input.mouseScrollDelta.y;
        if(delta != 0)
        {
            if(delta > 0 )
            {
                SelectedTool += 1;
                SelectedTool = (SelectedTool >= ToolbarSize) ? 0 : SelectedTool;
            }
            else
            {
                SelectedTool -= 1;
                SelectedTool = (SelectedTool < 0) ? ToolbarSize - 1 : SelectedTool;
            }
            OnChange?.Invoke(SelectedTool);
        }
    }
    public void UpdateHighlightIcon(int id = 0)
    {
        Item item = GetItem;
        if(item == null) {
            IconHighLight.Show = false;
            return; }
        IconHighLight.Show = item.iconHightlight;

        if(item.iconHightlight)
        {
            IconHighLight.Set(item.Icon);
        }
    }
}
