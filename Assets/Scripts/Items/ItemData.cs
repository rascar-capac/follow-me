using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    [Tooltip("Name of the item")]
	public string _itemName;
    [Tooltip("Description of the item")]
    public string _itemDescription;
    [Tooltip("Icon of the item")]
    public Sprite _itemIcon;
    [Tooltip("Prefab of the item")]
    public GameObject _itemPrefab;
    [Tooltip("Item is catchable")]
    public bool IsCatchable = false;
    [Tooltip("Item is Activable")]
    public bool IsActivable = false;
    [Tooltip("Activated prefab of the item")]
    public GameObject ActivatedPrefab;
}
