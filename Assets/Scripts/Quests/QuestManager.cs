using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    public List<QuestData> QuestsData;
    List<Quest> Quests;

    Player player;
    PlayerInventory playerInventory;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        playerInventory = player.GetComponent<PlayerInventory>();
        InitQuests();
    }

    void InitQuests()
    {
        if (QuestsData == null)
            return;

        Quests = new List<Quest>();
        for (int i = 0; i < QuestsData.Count; i++)
        {
            LoadQuest(QuestsData[i]);
        }
        if (Quests.Count > 0)
            StartChrono(2, () => { UIManager.I.AlertMessage(Quests[0].Data.QuestDescription); });
    }

    void LoadQuest(QuestData questdata)
    {
        Quest quest = new Quest(player, questdata);
        Quests.Add(quest);
        quest.onQuestCompleted.AddListener(QuestCompleted);
    }

    void QuestCompleted(Quest quest)
    {
        UIManager.I.AlertMessage(quest.Data.QuestCompletedDescription);

        if (quest.Data.ItemsReward != null)
        {
            foreach (ItemData it in quest.Data.ItemsReward)
            {
                playerInventory.AddItemToInventory(it);
            }
        }
        if (quest.Data.QuestsReward != null)
        {
            foreach (QuestData questdata in quest.Data.QuestsReward)
            {
                LoadQuest(questdata);
                UIManager.I.AlertMessage(questdata.QuestDescription);
            }
        }
        if (quest.Data.ZonesReward != null)
        {
            foreach (ZoneApparition zone in quest.Data.ZonesReward)
            {
                GameObject newZone = Instantiate(zone.Zone.gameObject);
                newZone.transform.position = zone.PositionToAppear.transform.position;
                newZone.transform.rotation = Quaternion.identity;
            }
        }
    }
}
