using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : BaseMonoBehaviour
{
	[Header("Place Item Data")]
	public ItemData _itemData;

	GameObject _currentItemPrefabDisplay;
	//public bool ItemIsActivated = false; // Déplacer dans _ItemData

	protected override void Start()
	{
		base.Start();

		//// Set-up IsActivated + Instantiate base item.
		//if (_itemData != null)
		//{
		//	_itemData.IsActivated = false;
		//	_currentItemPrefabDisplay = Instantiate(_itemData._itemBasePrefab, transform.position, Quaternion.identity, transform);
		//}
		//else
		//	Debug.LogError("Place an ItemData on Item component");
	}

	// If an item is activated...
	public void ActivateItem()
	{
		_itemData.IsActivated = true;
        //_currentItemPrefabDisplay.GetComponent<MeshFilter>().mesh = _itemData._itemActivatedMesh;
        Destroy(_currentItemPrefabDisplay);
        _currentItemPrefabDisplay = Instantiate(_itemData._itemActivatedPrefab, transform.position, Quaternion.identity, transform);
        UIManager.I.AlertMessage($"{_itemData._itemName} has been activated...");
    }
}
