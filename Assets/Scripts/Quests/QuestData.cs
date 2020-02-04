using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;



//[System.Serializable]
public class QuestData : BaseMonoBehaviour
{
	[Header("The title of the quest")]
    public string QuestTitle;
    [Header("The text that describes the quest")]
    public string QuestDescription;
    [Header("The text shown when quest is completed")]
    public string QuestCompletedDescription;
    [Header("List of zone the player must reached to complete the quest")]
    public List<Zone> Zones;
    [Header("List of zones the tribe must reached to complete the quest")]
    public List<Zone> TribeZones;
    [Header("List of items the player must activate to complete the quest")]
    public List<GameObject> Items;
    [Header("The tribe must have 100% Energy to be completed")]
    public bool TribeCharged = false;
    //[Header("List of rewards the player receives when the quest is completed")]
    //public List<QuestRewardData> QuestRewardData;

    [Header("Items data the player will receive on quest completed")]
    public List<ItemData> ItemsReward;
    [Header("Quests data to open on quest completed")]
    public List<QuestData> QuestsReward;
    [Header("Zone to activate on quest completed")]
    public List<ZoneApparition> ZonesReward;
    [Header("Docility point reward")]
    public int DocilityPointsReward = 0;
}
public enum QuestType
{
    GotoPosition,
    ActivatesItems
}
[System.Serializable]
public struct ZoneApparition
{
    public bool Instantiate;
    public Zone Zone;
    public GameObject PositionToAppear;
}