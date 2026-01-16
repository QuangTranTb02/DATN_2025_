using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> 
{
    public PlayerController m_PlayerController;
    public ItemContainer inventoryContainer;
    public ItemDragAndDropController DragAndDropController;
    public DayTimeController DayTimeController;
    public DialogueSystem dialogueSystem;
    public ItemList itemDB;
    public OnScreenMessageSystem messageSystem;
    public SenceTint screenTint;
    //public DataManager dataManager;
    public GameObject FishingMiniGame;
}
