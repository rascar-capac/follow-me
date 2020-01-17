using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : Singleton<PlayerInventory>
{
	[Header("Player Inventory")]
	public List<InventoryItemData> _playerInventory;

	[Header("Pick-up Range")]
	public float _pickUpRange = 5;

	//[Header("Debug Mode")]
	//public bool _debugMode = false;
	//public Color _debugColor = Color.yellow;

	Camera _mainCamera;
	RaycastHit _hitInfo = new RaycastHit();

	protected override void Start()
	{
		base.Start();
		_mainCamera = CameraManager.I._MainCamera;
		InputManager.I.onPickUpKeyPressed.AddListener(PickUpItem);
	}

	void PickUpItem()
	{
		if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out _hitInfo, _pickUpRange))
			if (_hitInfo.transform.gameObject.layer == 10)
			{
				_playerInventory.Add(_hitInfo.transform.GetComponent<InventoryItem>()._itemData);
				Destroy(_hitInfo.transform.gameObject);
			}
	}

	//private void OnDrawGizmos()
	//{
	//	if (_debugMode == true && _mainCamera != null)
	//	{
	//		Gizmos.color = _debugColor;
	//		Gizmos.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward);
	//	}
	//}
}
