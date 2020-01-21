using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestValidator : BaseMonoBehaviour
{
	#region Checking Quest
	// Ne pas appliquer toute les frames
	public void CheckQuestFinish(List<QuestData> questList)
	{
		foreach (QuestData quest in questList)
		{
			switch (quest._questType)
			{
				case QuestType.GotoPosition:
					quest._questFinish = CheckGoToPosition(transform, quest._goalGoToPosition, 25); //enlever le 25 et remplacer par le float distanceMinimum de QuestData
					break;

				case QuestType.ActivatesItems:
					quest._questFinish = CheckActivatedItems(quest._itemsToActivate, quest);
					break;

				default:
					break;
			}
		}
	}

	bool CheckGoToPosition(Transform agent,Transform target, float validationDistance)
	{
		bool isFinish = false;

		if (Vector2.Distance(new Vector2(agent.position.x, agent.position.z), new Vector2(target.position.x, target.position.z)) <= validationDistance)
			isFinish = true;

		return isFinish;
	}

	bool CheckActivatedItems(List<Item> itemsToCheck, QuestData quest)
	{
		// is true by default...
		bool isFinish = true;

		// but is false if an item is not activated
		if (itemsToCheck != null)
		{
			foreach (Item item in itemsToCheck)
				if (item._itemData.IsActivated == false)
					isFinish = false;
		}
		else
			Debug.LogError("Aucun items à vérifier dans " + quest.name);

		return isFinish;
	}
	#endregion
}
