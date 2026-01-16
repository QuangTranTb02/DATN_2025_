using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootContainerInteract : Interacable,IPersistant
{
    [SerializeField] GameObject OpenChest;
    [SerializeField] GameObject CloseChest;
    [SerializeField] bool Opened;
    [SerializeField] AudioClip onOpenAudio;
    [SerializeField] ItemContainer itemContainer;


    private void Start()
    {
        if (itemContainer == null)
        {
            Init();
        }
    }

    private void Init()
    {
            itemContainer = (ItemContainer)ScriptableObject.CreateInstance(typeof(ItemContainer));
            itemContainer.Init();
    }

    public override void Interact(Player player)
    {
        if(Opened == false)
        {
            Open(player);
        }
        else
        {
            Close(player);
        }
    }
    public void Open(Player Player)
    {
        Opened = true;
        OpenChest.SetActive(true);
        CloseChest.SetActive(false);

        AudioManager.Instance.Play(onOpenAudio);
        Player.GetComponent<ItemContainerInteractController>().Open(itemContainer, transform);
    }
    public void Close(Player Player)
    {
        Opened = false;
        OpenChest.SetActive(false);
        CloseChest.SetActive(true);

        AudioManager.Instance.Play(onOpenAudio);

        Player.GetComponent<ItemContainerInteractController>().Close();
    }
    [Serializable]
    public class SaveLootItemData
    {
        public int itemId;
        public int count;

        public SaveLootItemData(int id, int count)
        {
            this.itemId = id;   
            this.count = count;
        }
    }
    [Serializable]
    public class ToSave
    {
        public List<SaveLootItemData> itemDatas;
        public ToSave()
        {
            itemDatas = new List<SaveLootItemData>();
        }
    }
    public string Read()
    {
        ToSave toSave = new ToSave();
        for(int i = 0; i < itemContainer.slots.Count; i++)
        {
            if (itemContainer.slots[i].item == null)
            {
                toSave.itemDatas.Add(new SaveLootItemData(-1, 0));
            }
            else
            {
                toSave.itemDatas.Add(new SaveLootItemData(
                    itemContainer.slots[i].item.id,
                    itemContainer.slots[i].count));
            }
        }
        return JsonUtility.ToJson(toSave);
    }

    public void Load(string jsonString)
    {
        if(jsonString == "" || jsonString == "{}" || jsonString == null) { return; }
        if(jsonString != null) { Init(); }
        ToSave toLoad = JsonUtility.FromJson<ToSave>(jsonString);
        for(int i = 0; i < toLoad.itemDatas.Count; i++)
        {
            if (toLoad.itemDatas[i].itemId == -1)
            {
                itemContainer.slots[i].clear();
            }
            else
            {
                itemContainer.slots[i].item = GameManager.Instance.itemDB.items[toLoad.itemDatas[i].itemId];
                itemContainer.slots[i].count = toLoad.itemDatas[i].count;
            }
        }
    }
}
