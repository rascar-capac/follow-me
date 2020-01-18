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

	protected override void Start()
	{
		base.Start();

		_playerQuestValidator = GetComponent<QuestValidator>();
		_playerInventory = GetComponent<PlayerInventory>();
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
		for (int i = 0; i < _questsPlayer.Count; i++)
		{
			if (_questsPlayer[i]._questFinish == true)
			{
				foreach (QuestRewardData reward in _questsPlayer[i]._questRewards)
				{
					switch (reward._rewardType)
					{
						case RewardType.Item:
							_playerInventory._playerInventory.Add(reward._rewardItem);
							break;

						case RewardType.Quest:
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
	}
}
