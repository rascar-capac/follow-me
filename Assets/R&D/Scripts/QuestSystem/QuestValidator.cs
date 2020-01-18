using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestValidator : BaseMonoBehaviour
{
	#region Checking Quest
	public void CheckQuestFinish(List<QuestData> questList)
	{
		foreach (QuestData quest in questList)
		{
			switch (quest._questType)
			{
				case QuestType.GotoPosition:
					quest._questFinish = CheckGoToPosition(transform, quest._goalGoToPosition, 2); //enlever le 2
					break;

				default:
					break;
			}
		}
	}

	bool CheckGoToPosition(Transform agent,Transform target, float validationDistance)
	{
		bool hasFinish = false;

		Debug.Log(Vector3.Distance(agent.position, target.position));
		if (Vector3.Distance(agent.position, target.position) <= validationDistance)
			hasFinish = true;

		return hasFinish;
	}
	#endregion


	#region Reward Quest
	public InventoryItemData RewardItem(QuestRewardData questReward)
	{
		return questReward._rewardItem;
	}
	#endregion
}
