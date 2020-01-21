using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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
	[ShowIf("_rewardType", RewardType.Item)]
	public List<ItemData> _rewardsItem;
	[ShowIf("_rewardType", RewardType.Quest)]
	public List<QuestData> _rewardsQuest;
}
