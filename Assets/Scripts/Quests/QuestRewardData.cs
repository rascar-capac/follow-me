using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Reward", menuName = "Quest/New Reward ", order = 2)]
public class QuestRewardData : ScriptableObject
{
	[Header("Items data the player will receive on quest completed")]
	public List<ItemData> Items;
	[Header("Quests data to open on quest completed")]
	public List<QuestData> Quests;
}
