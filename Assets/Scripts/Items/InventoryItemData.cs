using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class InventoryItemData : ScriptableObject
{
	public string _itemName;
	public string _itemDescription;
	public Sprite _itemIcon;
	public GameObject _itemPrefab;

	//public bool _itemObtained;
	//public int _itemCount;
}
