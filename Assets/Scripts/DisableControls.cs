using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableControls : MonoBehaviour
{
    PlayerController playerController;
    ToolPlayerController toolPlayerController;
    InventoryController inventoryController;
    ToolbarController toolbarController;
    ItemContainerInteractController itemContainerInteractController;
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        toolPlayerController = GetComponent<ToolPlayerController>();
        inventoryController = GetComponent<InventoryController>();
        toolbarController = GetComponent<ToolbarController>();
        itemContainerInteractController = GetComponent<ItemContainerInteractController>();
    }
    public void DisableControl()
    {
        playerController.enabled = false;
        toolPlayerController.enabled = false;
        inventoryController.enabled = false;
        toolbarController.enabled = false;
        itemContainerInteractController.enabled = false;
    }
    public void EnableControl()
    {
        playerController.enabled = true;
        toolPlayerController.enabled = true;
        inventoryController.enabled = true;
        toolbarController.enabled = true;
        itemContainerInteractController.enabled = true;
    }

}
