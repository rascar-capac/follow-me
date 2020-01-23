using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : BaseMonoBehaviour
{
	[Header("Player Inventory")]
	public List<ItemData> _playerInventory;

	[Header("Pick-up Range")]
	public float _pickUpRange = 5;

    [Header("Pick-up item layer")]
    public LayerMask ItemLayer;

    bool AllowPickup = true;
    bool AllowActivate = true;

	Camera _mainCamera;

    public ItemActivatedEvent onItemActivated = new ItemActivatedEvent();

    protected override void Start()
	{
		base.Start();
		_mainCamera = CameraManager.I._MainCamera;
		InputManager.I.onPickUpKeyPressed.AddListener(PickUpItem);
        InputManager.I.onActivateItemKeyPressed.AddListener(InteractItem);
        UIManager.I.onToolsInventoryClosedEvent.AddListener(() => { AllowPickup = true; AllowActivate = true; });
        UIManager.I.onToolsInventoryOpenedEvent.AddListener(() => { AllowPickup = false; AllowActivate = false; });
    }

    void PickUpItem()
	{
        if (!AllowPickup)
            return;

		RaycastHit hitInfo;
		if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hitInfo, _pickUpRange, ItemLayer))
        {
            Item it = hitInfo.transform.GetComponent<Item>();
            if (it._itemData.IsCatchable)
            {
                _playerInventory.Add(it._itemData);
                Destroy(hitInfo.transform.gameObject);
            }
		}
	}

    void InteractItem()
    {
		Debug.Log("Dans InteractItem()");
		if (!AllowActivate)
            return;

		RaycastHit hitInfo;
		if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hitInfo, _pickUpRange, ItemLayer))
        {
            Item it = hitInfo.transform.GetComponentInParent<Item>();
            ItemData data = it._itemData;

			//if (data.IsActivable && !it.ItemIsActivated && data._itemActivatedPrefab != null) // Modification car déplacement du bool "Is Activated" directement dans ItemData
			if (data.IsActivable && !data.IsActivated && data._itemActivatedPrefab != null)
			{
				//onItemActivated?.Invoke(it);

				// Impossible d'utiliser ActivateItem() sans le Set-Up de _currentItemPrefabDisplay dans Item.cs
				// C'est ennuyant, je n'ai pas trouver la solution.
				//it.ActivateItem(); 

			}
		}
    }
}
public class ItemActivatedEvent : UnityEvent<Item> { }