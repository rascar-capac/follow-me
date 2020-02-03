using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum QuestType
{
	GotoPosition,
	ActivatesItems
}

[CreateAssetMenu(fileName = "Quest name", menuName = "Quest/New Quest", order = 1)]
public class QuestData : ScriptableObject
{
	[Header("The title of the quest")]
    public string QuestTitle;
    [Header("The text that describes the quest")]
    public string QuestDescription;
    [Header("The text shown when quest is completed")]
    public string QuestCompletedDescription;
    [Header("List of zone the player must reached to complete the quest")]
    public List<string> ZonesNames;
    [Header("List of zone the player must reached to complete the quest")]
    public List<Zone> Zones;
    [Header("List of zones the tribe must reached to complete the quest")]
    public List<Zone> TribeZones;
    [Header("List of items the player must activate to complete the quest")]
    public List<GameObject> Items;
    [Header("The tribe must have 100% Energy to be completed")]
    public bool TribeCharged = false;
    [Header("List of rewards the player receives when the quest is completed")]
    public List<QuestRewardData> QuestRewardData;

}
