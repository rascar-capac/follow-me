using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RewardType
{
	Item,
	Quest,
	BoostStats
}

[CreateAssetMenu(fileName = "New Reward", menuName = "Quest/New Reward ", order = 2)]
public class QuestRewardData : ScriptableObject
{
	public RewardType _rewardType;
	public InventoryItemData _rewardItem;
}
