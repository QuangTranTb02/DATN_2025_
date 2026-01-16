using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemConvertorData
{
    public ItemSlot itemSlot;
    public int timer;

    public ItemConvertorData()
    {
        itemSlot = new ItemSlot();

    }

}
[RequireComponent(typeof(TimeAgent))]
public class ItemConvertorInteract : Interacable, IPersistant
{
    [SerializeField] Item convertableItem;
    [SerializeField] Item productedItem;
    [SerializeField] int productItemCount = 1;

    [SerializeField] int timeToProcess = 5;

    ItemConvertorData data;

    Animator animator;
    TimeAgent timeAgent;
    private void Start()
    {
        if(data == null) 
        { 
            data = new ItemConvertorData();
        }
        animator = GetComponent<Animator>();
        timeAgent = GetComponent<TimeAgent>();

        timeAgent.OnTimeTick += ItemConvertProcess;

        Animate();
    }

    private void ItemConvertProcess(DayTimeController dayTimeController)
    {
        if (data.itemSlot == null) { return; }
        if (data.timer > 0)
        {
            data.timer -= 1;
            if (data.timer <= 0)
            {
                CompleteItemConversion();
            }
        }
    }

    public override void Interact(Player Player)
    {
        if(data.itemSlot.item == null)
        {
            if (GameManager.Instance.DragAndDropController.Check(convertableItem))
            {
                Debug.Log("Start Convertion");
                StartItemProcessing(GameManager.Instance.DragAndDropController.itemSlot);
                return;
            }
            ToolbarController toolbarController = Player.GetComponent<ToolbarController>();
            if(toolbarController == null) { return;}

            ItemSlot itemSlot = toolbarController.GetItemSlot;
            if(itemSlot.item == convertableItem)
            {
                StartItemProcessing(itemSlot);
                return;
            }

        }
        if(data.itemSlot.item != null && data.timer <= 0)
        {
            GameManager.Instance.inventoryContainer.Add(data.itemSlot.item, data.itemSlot.count);
            data.itemSlot.clear();
        }
    }

    private void StartItemProcessing(ItemSlot toProcessing)
    {
        
        data.itemSlot.copy(GameManager.Instance.DragAndDropController.itemSlot);
        data.itemSlot.count = 1;
        if(toProcessing.item.Stackable)
        {
            toProcessing.count -= 1;
            if(toProcessing.count < 0)
            {
                toProcessing.clear();
            }
        }
        else
        {
            toProcessing.clear();
        }
        data.timer = timeToProcess;
        Animate();
    }

    private void Animate()
    {
        animator.SetBool("working", data.timer > 0f);
    }
    private void CompleteItemConversion()
    {
        Animate();

        Debug.Log("Done Convertion");
        data.itemSlot.clear();
        data.itemSlot.set(productedItem, productItemCount);
    }

    public string Read()
    {
        return JsonUtility.ToJson(data);
    }

    public void Load(string jsonString)
    {
       data  = JsonUtility.FromJson<ItemConvertorData>(jsonString);
    }
}
