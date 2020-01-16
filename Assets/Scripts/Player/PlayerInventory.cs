using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Singleton<PlayerInventory>
{
	public List<ScriptableObject> _playerInventory;

	public bool PickUpItem(ScriptableObject _itemFound)
	{
		_playerInventory.Add(_itemFound);

		return true;
	}
}
