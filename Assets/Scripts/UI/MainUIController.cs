using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OneB;

namespace Platformer.UI
{
    /// <summary>
    /// A simple controller for switching between UI panels.
    /// </summary>
    public class MainUIController : MonoBehaviour
    {
        public GameObject[] panels;

        public void SetActivePanel(int index)
        {
            for (var i = 0; i < panels.Length; i++)
            {
                var active = i == index;
                var g = panels[i];
                if (g.activeSelf != active) g.SetActive(active);
            }
        }
        void OnEnable()
        {
            SetActivePanel(0);

            //var inboxList = await OnlineManager.Instance.API.Send<InboxList>(new GetInboxListCommand());
            //Debug.Log(inboxList?.Items);
            //var daily_rewards = await OnlineManager.Instance.API.Send<ItemTable>(new GetBlueprintObjectCommand("ItemTable"));
            //Debug.Log(daily_rewards);

            //var playerProfile = await OnlineManager.Instance.API.Send<PlayerProfileRes>(new GetPlayerObjectCommand("Profile"));
            //Debug.Log(playerProfile);
            //var profile = await OnlineManager.Instance.API.Send<PlayerProfileRes>(new PostPlayerObjectCommand<PlayerProfileReq>("Profile", new PlayerProfileReq
            //{
            //    PlayerName = "Harry Potter",
            //    Country = "VN"
            //}));

            //var dailyRewards = await OnlineManager.Instance.API.Send<DailyRewardsCanClaimRewardOutput>(new CallGameScriptCommand("DailyRewards", "CanClaimRewards"));
            //Debug.Log(dailyRewards);

            //var playerData = await OnlineManager.Instance.API.Send<PlayerDataRes>(new GetPlayerObjectCommand("Data"));
            //Debug.Log(playerData);
            //foreach (var item in playerData?.Inventory)
            //{
            //    InventoryManager.Instance.AddItem(item.ItemId, item.Amount);
            //}

        }
    }
}