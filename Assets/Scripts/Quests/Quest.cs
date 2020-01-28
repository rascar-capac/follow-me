using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Quest
{
    [HideInInspector]
    public QuestData Data;

    [HideInInspector]
    public List<Item> ItemsToActivate = new List<Item>();
    [HideInInspector]
    public List<Zone> ZoneToReach = new List<Zone>();

    PlayerInventory PlayerInventory;
    Player Player;
    bool QuestCompleted = false;

    public QuestCompletedEvent onQuestCompleted = new QuestCompletedEvent();

    public Quest(Player player, QuestData data)
    {
        Player = player;
        PlayerInventory = Player.transform.GetComponent<PlayerInventory>();

        Data = data;
        Item it = null;
        if (Data.Items != null)
        {
            Data.Items.ForEach(i =>
            {
                if (i != null)
                {
                    it = i.GetComponent<Item>();
                    it._itemData.IsActivated = false;
                    ItemsToActivate.Add(it);
                }
            });
        }

        if (Data.Zones != null)
            ZoneToReach.AddRange(Data.Zones);

        if (ItemsToActivate != null && ItemsToActivate.Count > 0)
            PlayerInventory.onItemActivated.AddListener(ItemActivated);

        if (ZoneToReach != null && ZoneToReach.Count > 0)
            Player.onZoneEnter.AddListener(ZoneReached);
    }

    void ItemActivated(Item Item)
    {
        Item.ActivateItem();
        if (ItemsToActivate != null && ItemsToActivate.Count > 0 && ItemsToActivate.Exists(It => It == Item))
            ItemsToActivate.Remove(Item);

        CheckQuestFinished();
    }

    void ZoneReached(ZoneInteractable who, Zone zone)
    {
        if (!(who is Player))
            return;

        if (ZoneToReach != null && ZoneToReach.Count > 0 && ZoneToReach.Exists(z => z == zone))
            ZoneToReach.Remove(zone);

        CheckQuestFinished();
    }

    public void CheckQuestFinished() 
    {
        if (ItemsToActivate != null && ItemsToActivate.Count > 0)
            return;

        if (ZoneToReach != null && ZoneToReach.Count > 0)
            return;

        QuestCompleted = true;

        PlayerInventory.onItemActivated.RemoveListener(ItemActivated);
        Player.onZoneEnter.RemoveListener(ZoneReached);

        onQuestCompleted.Invoke(this);
    }
}
public class QuestCompletedEvent : UnityEvent<Quest> { }
