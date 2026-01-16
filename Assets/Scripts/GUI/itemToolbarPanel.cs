using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemToolbarPanel : ItemPanel
{
    [SerializeField] ToolbarController toolbarController;

    int currentSelectedTool;

    private void Start()
    {
        Init();
        toolbarController.OnChange += HightLight;
        HightLight(0);
    }
    public override void OnClick(int id)
    {
        toolbarController.set(id);
        ToolbarController.Instance.UpdateHighlightIcon(id);
        HightLight(id);
    }
    public void HightLight(int id)
    {
        Buttons[currentSelectedTool].Hightight(false);
        currentSelectedTool = id;
        Buttons[currentSelectedTool].Hightight(true);

    }
    public override void Show()
    {
        base.Show();
        toolbarController.UpdateHighlightIcon();
    }
}
