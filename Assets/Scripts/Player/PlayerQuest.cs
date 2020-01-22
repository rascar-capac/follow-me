using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(QuestValidator))]
[RequireComponent(typeof(PlayerInventory))]
public class PlayerQuest : Singleton<PlayerQuest>
{
	public List<QuestData> _questsPlayer = new List<QuestData>();
	QuestValidator _playerQuestValidator;

	PlayerInventory _playerInventory;

	UIManager _refUIManager;

	protected override void Start()
	{
		base.Start();

		_playerQuestValidator = GetComponent<QuestValidator>();
		_playerInventory = GetComponent<PlayerInventory>();

		_refUIManager = FindObjectOfType<UIManager>();
	}

	private void Update()
	{
		if(_questsPlayer.Count > 0)
		{
			_playerQuestValidator.CheckQuestFinish(_questsPlayer);
			PlayerQuestRewarding();
		}
	}

	void PlayerQuestRewarding()
	{
		string alertMessageQuestFinish = "";

		for (int i = 0; i < _questsPlayer.Count; i++)
		{
			if (_questsPlayer[i]._questFinish == true)
			{
				alertMessageQuestFinish += "Quête '" + _questsPlayer[i]._questTitle + "' est finie! \n";
				foreach (QuestRewardData reward in _questsPlayer[i]._questRewards)
				{
					switch (reward._rewardType)
					{
						case RewardType.Item:
							foreach (ItemData item in reward._rewardsItem)
							{
								_playerInventory._playerInventory.Add(item);
								alertMessageQuestFinish += "Récompense recue: " + item._itemName + "\n";
							}
							break;

						case RewardType.Quest:
							foreach (QuestData newQuest in reward._rewardsQuest)
							{
								_questsPlayer.Add(newQuest);
								alertMessageQuestFinish += "Nouvelle quête recue: " + newQuest._questTitle + "\n";
							}
							break;

						case RewardType.BoostStats:
							break;

						default:
							break;
					}
				}
				_questsPlayer.Remove(_questsPlayer[i]);
			}
		}	
		if (alertMessageQuestFinish != "")
			_refUIManager.AlertMessage(alertMessageQuestFinish, 8);
	}
}
