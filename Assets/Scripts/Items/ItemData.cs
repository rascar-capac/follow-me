using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    [Tooltip("Name of the item")]
	public string Name;
    [Tooltip("Description of the item")]
    public string Description;
    [Tooltip("Icon of the item")]
    public Sprite Icon;
    [Tooltip("Base prefab of the item")]
    public GameObject _itemBasePrefab;
    [Tooltip("Activated prefab of the item")]
    public GameObject _itemActivatedPrefab;
    [Tooltip("Item is catchable")]
    public bool IsCatchable = false;
    [Tooltip("Item is Activable")]
    public bool IsActivable = false;
	[HideInInspector]
	public bool IsActivated = false;
}
