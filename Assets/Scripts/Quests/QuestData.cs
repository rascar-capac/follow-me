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
    [Header("List of zone the player must reached to complete the quest")]
    public List<Zone> Zones;
    [Header("List of items the player must activate to complete the quest")]
    public List<Item> Items;
    [Header("List of rewards the player receives when the quest is completed")]
    public List<QuestRewardData> QuestRewardData;
}
