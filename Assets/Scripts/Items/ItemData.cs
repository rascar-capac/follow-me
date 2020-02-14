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
    [Tooltip("Activated material of the item")]
    public Material _ItemActivatedMaterial;
    [Tooltip("Item to drop here")]
    public ItemData _itemToDrop;
    [Tooltip("Position to drop")]
    public GameObject _PositionToDrop;
    [Tooltip("Item is catchable")]
    public bool IsCatchable = false;
    [Tooltip("Item is swapable")]
    public bool IsSwapable = false;
    [Tooltip("Item is activable")]
    public bool IsActivable = false;
	[HideInInspector]
	public bool IsActivated = false;
	[Tooltip("Base mesh of item")]
	public Mesh MeshBase;
	[Tooltip("Base material of item")]
	public Material MaterialBase;
	[Tooltip("Base mesh of item")]
	public Mesh MeshActivated;
	[Tooltip("Base material of item")]
	public Material MaterialActivated;
}
