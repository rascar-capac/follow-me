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

    public GameObject LeftHand;
    public GameObject RightHand;

    [Header("Mutlply camera near plane position by this for hands z position")]

    bool AllowPickup = true;
    bool AllowActivate = true;
	Camera _mainCamera;

    Item LeftHandItem;
    Item RightHandItem;

    public ItemActivatedEvent onItemActivated = new ItemActivatedEvent();
    float xpixels = 10;
    float ypixels = 10;
    protected override void Start()
	{
		base.Start();
		_mainCamera = CameraManager.I._MainCamera;
		InputManager.I.onPickUpKeyPressed.AddListener(PickUpItem);
        InputManager.I.onActivateItemKeyPressed.AddListener(InteractItem);

        UIManager.I.onToolsInventoryClosedEvent.AddListener(() => { AllowPickup = true; AllowActivate = true; });
        UIManager.I.onToolsInventoryOpenedEvent.AddListener(() => { AllowPickup = false; AllowActivate = false; });

        ToolsInventoryManager.I.onToolSelected.AddListener(PutItemInHand);
        Debug.Log(CameraManager.I._MainCamera.nearClipPlane);

        Vector3 pos = CameraManager.I._MainCamera.ScreenToWorldPoint(new Vector3(xpixels, ypixels, CameraManager.I._MainCamera.nearClipPlane * 1.5f));
        pos.z -= LeftHand.transform.GetChild(0).localScale.y / 1.5f ;
        LeftHand.transform.position = pos;
        pos = CameraManager.I._MainCamera.ScreenToWorldPoint(new Vector3(Screen.width - xpixels, ypixels, CameraManager.I._MainCamera.nearClipPlane * 1.5f));
        pos.z -= RightHand.transform.GetChild(0).localScale.y / 1.5f;
        RightHand.transform.position = pos;
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
				it.ActivateItem();
            }
        }
    }

    public void PutItemInHand(GameObject o, Hand hand)
    {
        Quaternion rotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, -CameraManager.I._MainCamera.transform.forward);
        
        if (hand == Hand.Left)
        {
            if (LeftHandItem)
                Destroy(LeftHandItem);
            GameObject newone = Instantiate(o, LeftHand.transform.position + new Vector3(0, 0.5f, 0), rotation, LeftHand.transform);
            LeftHandItem = newone.GetComponent<Item>();
        }
        else
        {
            if (LeftHandItem)
                Destroy(LeftHandItem);
            GameObject newone = Instantiate(o, RightHand.transform.position + new Vector3(0, 0.5f, 0), rotation, RightHand.transform);
            RightHandItem = newone.GetComponent<Item>();
        }
    }
}
public class ItemActivatedEvent : UnityEvent<Item> { }
public enum Hand
{
    Left,
    Right
}