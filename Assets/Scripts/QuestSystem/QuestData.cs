using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum QuestType
{
	GotoPosition
}

[CreateAssetMenu(fileName = "Quest name", menuName = "Quest/New Quest", order = 1)]
public class QuestData : ScriptableObject
{
	public string _questTitle;
	public string _questDescription;
	public bool _questAccess;
	public bool _questFinish = false;
	public QuestType _questType;
	[ShowIf("_questType", QuestType.GotoPosition)]
	public Transform _goalGoToPosition; // Transformer en List et Changer logique sQuestValidator/CheckQuestFinish()
	// Ajouter un float distanceMinimum
	public List<QuestRewardData> _questRewards;
}
