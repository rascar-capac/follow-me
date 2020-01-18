using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
	public QuestType _questType;
	public Transform _goalGoToPosition;
	public bool _questFinish = false;
	public List<QuestRewardData> _questRewards;
}
