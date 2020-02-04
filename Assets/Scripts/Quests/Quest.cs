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
    [HideInInspector]
    public List<Zone> TribeZoneToReach = new List<Zone>();

    PlayerInventory PlayerInventory;
    Player Player;
    Tribe Tribe;
    bool QuestCompleted = false;

    public QuestCompletedEvent onQuestCompleted = new QuestCompletedEvent();

    public Quest(Player player, QuestData data)
    {
        Player = player;
        PlayerInventory = Player.transform.GetComponent<PlayerInventory>();
        Tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();

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
                    it.gameObject.SetActive(true);
                    ItemsToActivate.Add(it);
                }
            });
        }

        if (Data.Zones != null)
            ZoneToReach.AddRange(Data.Zones);

        if (Data.TribeZones != null)
            TribeZoneToReach.AddRange(Data.TribeZones);

        if (ItemsToActivate != null && ItemsToActivate.Count > 0)
        {
            PlayerInventory.onItemPickedUp.AddListener(ItemActivated);
            PlayerInventory.onItemActivated.AddListener(ItemActivated);
        }

        if (ZoneToReach != null && ZoneToReach.Count > 0)
            Player.onZoneEnter.AddListener(ZoneReached);

        if (TribeZoneToReach != null && TribeZoneToReach.Count > 0)
            Tribe.onZoneEnter.AddListener(ZoneReached);

        if (Data.TribeCharged)
            Tribe.onEnergyFull.AddListener(() => CheckQuestFinished());
    }

    void ItemActivated(Item Item)
    {
        if (ItemsToActivate != null && ItemsToActivate.Count > 0 && ItemsToActivate.Exists(It => It == Item))
        {
            if (Item._itemData.IsActivable)
                Item.ActivateItem();
            ItemsToActivate.Remove(Item);
        }

        CheckQuestFinished();
    }

    void ZoneReached(ZoneInteractable who, Zone zone)
    {
        if (who is Player)
        {
            if (ZoneToReach != null && ZoneToReach.Count > 0 && ZoneToReach.Exists(z => z == zone))
                ZoneToReach.Remove(zone);
        }
        else if (who is Tribe)
        {
            if (TribeZoneToReach != null && TribeZoneToReach.Count > 0 && TribeZoneToReach.Exists(z => z == zone))
                TribeZoneToReach.Remove(zone);
        }
        CheckQuestFinished();
    }

    public void CheckQuestFinished() 
    {
        if (ItemsToActivate != null && ItemsToActivate.Count > 0)
            return;

        if (ZoneToReach != null && ZoneToReach.Count > 0)
            return;

        if (TribeZoneToReach != null && TribeZoneToReach.Count > 0)
            return;

        if (Data.TribeCharged && Tribe.Energy < GameManager.I._data.InitialTribeEnergy)
            return;

        QuestCompleted = true;

        PlayerInventory.onItemActivated.RemoveListener(ItemActivated);
        PlayerInventory.onItemPickedUp.RemoveListener(ItemActivated);
        Player.onZoneEnter.RemoveListener(ZoneReached);
        Tribe.onZoneEnter.RemoveListener(ZoneReached);

        onQuestCompleted.Invoke(this);
    }
}
public class QuestCompletedEvent : UnityEvent<Quest> { }
