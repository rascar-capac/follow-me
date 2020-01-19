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
					quest._questFinish = CheckGoToPosition(transform, quest._goalGoToPosition, 25); //enlever le 25 et remplacer par le float distanceMinimum de QuestData
					break;

				default:
					break;
			}
		}
	}

	bool CheckGoToPosition(Transform agent,Transform target, float validationDistance)
	{
		bool hasFinish = false;

		if (Vector2.Distance(new Vector2(agent.position.x, agent.position.z), new Vector2(target.position.x, target.position.z)) <= validationDistance)
			hasFinish = true;

		return hasFinish;
	}
	#endregion
}
