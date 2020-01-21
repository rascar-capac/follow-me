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
	public string _questTitle;
	public string _questDescription;
	public bool _questAccess;
	public bool _questFinish = false;
	public List<QuestRewardData> _questRewards;
	public QuestType _questType;

	[ShowIf("_questType", QuestType.GotoPosition)]
	public Transform _goalGoToPosition; // Transformer en List et Changer logique QuestValidator/CheckQuestFinish()
	// Ajouter un float distanceMinimum

	[ShowIf("_questType", QuestType.ActivatesItems)]
	public List<Item> _itemsToActivate;
}
