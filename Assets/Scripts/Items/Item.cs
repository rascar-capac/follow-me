using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectEnabledEvent : UnityEvent<Item, bool> { }
public class Item : BaseMonoBehaviour
{
	[Header("Place Item Data")]
	public ItemData _itemData;

    public ObjectEnabledEvent onObjectEnableChange = new ObjectEnabledEvent();
    private bool _IsEnabled = false;
    public bool IsEnabled
    {
        get
        {
            return _IsEnabled;
        }
        set
        {
            _IsEnabled = value;
            Init();
            onObjectEnableChange.Invoke(this, _IsEnabled);
        }
    }

    public Zone InZone;

	GameObject _currentItemPrefabDisplay;
	//public bool ItemIsActivated = false; // Déplacer dans _ItemData

	protected override void Start()
	{
		base.Start();
        InZone = transform.GetComponentInParent<Zone>();
	}

	// If an item is activated...
	public void ActivateItem()
	{
		_itemData.IsActivated = true;
        if (_itemData._itemActivatedPrefab)
        {
            Destroy(_currentItemPrefabDisplay);
            _currentItemPrefabDisplay = Instantiate(_itemData._itemActivatedPrefab, transform.position, Quaternion.identity, transform);
        }
        UIManager.I.AlertMessage($"{_itemData._itemName} has been activated...");
    }

}
