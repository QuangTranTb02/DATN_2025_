using UnityEngine;

public class Trading : MonoBehaviour
{
    [SerializeField] GameObject storePanel;
    [SerializeField] GameObject inventoryPanel;

    [SerializeField] GameObject toolbarPanel;
    Store store;
    Currence money;

    ItemStorePanel itemStorePanel;
    [SerializeField] ItemContainer playerInventory;
    [SerializeField] ItemPanel inventoryItemPanel;
    private void Awake()
    {
        money = GetComponent<Currence>();
        itemStorePanel = storePanel.GetComponent<ItemStorePanel>();
    }
    public void BeginTrading(Store store)
    {

        this.store = store;

        itemStorePanel.SetInventory(store.storeContent);

        storePanel.SetActive(true);
        inventoryPanel.SetActive(true);
        toolbarPanel.SetActive(false);
    }
    public void StopTrading()
    {
        store = null;
        storePanel.SetActive(false);
        inventoryPanel.SetActive(false);
        toolbarPanel.SetActive(true);
    }
    public void SellItem()
    {
        if (GameManager.Instance.DragAndDropController.CheckForSale() == true)
        {
            ItemSlot itemToSell = GameManager.Instance.DragAndDropController.itemSlot;
            int moneyGain = itemToSell.item.Stackable == true ?
                (int)(itemToSell.item.price * itemToSell.count * store.buyFromPlayerMultip):
                (int)(itemToSell.item.price * store.buyFromPlayerMultip);
            money.Add(moneyGain);
            itemToSell.clear();
            GameManager.Instance.DragAndDropController.UpdateIcon();
        }

    }

    internal void BuyItem(int id)
    {
        Item itemToNuy = store.storeContent.slots[id].item;
        int totolPrice = (int)(itemToNuy.price * store.sellFromPlayerMultip);

        if(money.Check(totolPrice) == true)
        {
            money.Decrease(totolPrice);
            playerInventory.Add(itemToNuy);
            inventoryItemPanel.Show();
        }

    }
}
