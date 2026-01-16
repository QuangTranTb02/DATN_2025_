using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    [SerializeField] ItemContainer inventory;
    public void Craft(CraftingRecipe recipe)
    {
        if(inventory.CheckFreeSpace() == false )
        {
            Debug.Log("Hết chỗ trong item");
            return;
        }

        for (int i = 0; i < recipe.elements.Count; i++)
        {
            if(inventory.CheckItem(recipe.elements[i]) == false)
            {
                Debug.Log("Khong du nguyen lieu");
                return;
            }
        }
        for(int i = 0; i < recipe.elements.Count; i++)
        {
            inventory.Remove(recipe.elements[i].item, recipe.elements[i].count);
        }
        inventory.Add(recipe.outPut.item, recipe.outPut.count);
    }
}