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

	Camera _mainCamera;
	RaycastHit _hitInfo = new RaycastHit();

    public ItemActivatedEvent onItemActivated = new ItemActivatedEvent();

    protected override void Start()
	{
		base.Start();
		_mainCamera = CameraManager.I._MainCamera;
		InputManager.I.onPickUpKeyPressed.AddListener(PickUpItem);
        InputManager.I.onActivateItemKeyPressed.AddListener(ActivateItem);
    }

    void PickUpItem()
	{
		if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out _hitInfo, _pickUpRange, ItemLayer))
        {
            Item it = _hitInfo.transform.GetComponent<Item>();
            if (it._itemData.IsCatchable)
            {
                _playerInventory.Add(it._itemData);
                Destroy(_hitInfo.transform.gameObject);
            }
		}
	}
    void ActivateItem()
    {
        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out _hitInfo, _pickUpRange, ItemLayer))
        {
            Item it = _hitInfo.transform.GetComponent<Item>();
            ItemData data = it._itemData;
            if (data.IsActivable && !it.ItemIsActivated && data.ActivatedPrefab != null)
            {
                onItemActivated?.Invoke(it);
                //GameObject activated = Instantiate(it._itemData.ActivatedPrefab, _hitInfo.transform.position, _hitInfo.transform.rotation, _hitInfo.transform.parent);
                //Destroy(_hitInfo.transform.gameObject);
            }
        }
    }
    //[Header("Debug Mode")]
    //public bool _debugMode = false;
    //public Color _debugColor = Color.yellow;

    //private void OnDrawGizmos()
    //{
    //	if (_debugMode == true && _mainCamera != null)
    //	{
    //		Gizmos.color = _debugColor;
    //		Gizmos.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward);
    //	}
    //}
}
public class ItemActivatedEvent : UnityEvent<Item> { }