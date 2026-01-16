using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipePanel : ItemPanel
{
    [SerializeField] RecipeList recipeList;
    [SerializeField] Crafting crafting;
    public override void Show()
    {
        for(int i = 0; i < Buttons.Count && i < recipeList.recipes.Count; i++) 
        {
            Buttons[i].Set(recipeList.recipes[i].outPut);
        }
    }
    public override void OnClick(int id)
    {
        if(id >= recipeList.recipes.Count)
        {
            return;
        }
        crafting.Craft(recipeList.recipes[id]);
    }
}
