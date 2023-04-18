using System.Collections.Generic;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public ItemDatabase database;
    public List<InventorySlot> Container = new List<InventorySlot>();
    public Transform ItemContent;
    public GameObject InventoryItem;

    private void Awake()
    {
        Instance = this;
    }
    public void GetInventory()
    {
        StartCoroutine(OnlineManager.Instance.GetInventory((RepeatedField<PlayerData.Types.Inventory> inventories) =>
        {
            Container = new List<InventorySlot>();
            foreach (var item in inventories)
            {
                AddItem(item.ItemId, item.Amount);
            }
            ListItems();
        }));
    }
    public void AddItem(string itemId, uint amount)
    {
        var item = database.GetItem(itemId);
        AddItem(item, amount);
    }
    public void AddItem(Item item, uint amount)
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].item == item)
            {
                Container[i].AddAmount(amount);
                return;
            }
        }

        Container.Add(new InventorySlot(item, amount));

    }

    public void ListItems()
    {
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in Container)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemAmount = obj.transform.Find("ItemAmount").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();

            itemAmount.text = item.amount.ToString();
            itemName.text = item.item.id;
            itemIcon.sprite = item.item.icon;
        }
    }
}
