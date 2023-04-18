using System.Collections.Generic;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardsManager : MonoBehaviour
{
    public static DailyRewardsManager Instance;
    public ItemDatabase database;
    public List<RewardSlot> Container = new List<RewardSlot>();
    public Transform ItemContent;
    public GameObject InventoryItem;
    public Button CanClaimRewards;
    private void Awake()
    {
        Instance = this;
    }
    public void GetDailyRewards()
    {
        StartCoroutine(OnlineManager.Instance.GetDailyRewards((DailyRewardsList dailyRewards) =>
        {
            Debug.Log(dailyRewards);
            UpdateList(dailyRewards.Steps);
        }));
    }
    public void ClaimRewards()
    {
        StartCoroutine(OnlineManager.Instance.ClaimDailyRewards((DailyRewardsClaimRewardsOutput dailyRewards) =>
        {
            Debug.Log(dailyRewards);
            UpdateList(dailyRewards.Steps);
        }));
    }
    private void UpdateList(RepeatedField<DailyRewardsSteps> steps)
    {
        Container = new List<RewardSlot>();
        foreach (var item in steps)
        {
            Container.Add(new RewardSlot(database.GetItem(item.ItemId), item));
        }
        ListItems();
    }
    public void ListItems()
    {
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }
        var canClaimItem = false;
        foreach (var item in Container)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemAmount = obj.transform.Find("ItemAmount").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var itemDescription = obj.transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>();
            var itemHighlight = obj.transform.Find("ItemHighlight").GetComponent<Image>();

            itemAmount.text = item.data.Amount.ToString();
            itemName.text = item.data.Claimed ? "Claimed" : item.item.id;
            itemIcon.sprite = item.item.icon;
            itemDescription.text = item.data.Description;
            itemHighlight.enabled = item.data.CanClaim;
            canClaimItem |= item.data.CanClaim;
        }
        CanClaimRewards.interactable = canClaimItem;
    }
}
