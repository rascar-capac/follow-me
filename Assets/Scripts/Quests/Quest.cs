using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Quest
{
    [HideInInspector]
    public QuestData Data;

    List<Item> ItemsToActivate;
    List<Zone> ZoneToReach;

    PlayerInventory PlayerInventory;
    Player Player;
    bool QuestCompleted = false;

    public QuestCompletedEvent onQuestCompleted = new QuestCompletedEvent();

    public Quest(Player player, QuestData data)
    {
        Player = player;
        PlayerInventory = Player.transform.GetComponent<PlayerInventory>();

        Data = data;

        ItemsToActivate = Data.Items;
        ZoneToReach = Data.Zones;

        if (ItemsToActivate != null && ItemsToActivate.Count > 0)
            PlayerInventory.onItemActivated.AddListener(ItemActivated);

        if (ZoneToReach != null && ZoneToReach.Count > 0)
            Player.onZoneEnter.AddListener(ZoneReached);
    }

    void ItemActivated(Item Item)
    {
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

    void CheckQuestFinished() 
    {
        if (ItemsToActivate != null && ItemsToActivate.Count > 0)
            return;

        if (ZoneToReach != null && ZoneToReach.Count > 0)
            return;

        QuestCompleted = true;
        onQuestCompleted.Invoke(this);
    }
}
public class QuestCompletedEvent : UnityEvent<Quest> { }
