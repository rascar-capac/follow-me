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

    //public GameObject LeftArm;
    //public GameObject LeftHand;
    //public GameObject RightArm;
    //public GameObject RightHand;


    bool AllowPickup = true;
    bool AllowActivate = true;
	Camera _mainCamera;

    Item LeftHandItem;
    Item RightHandItem;

    public ItemActivatedEvent onItemActivated = new ItemActivatedEvent();
    float xPixels = 200;
    float yPixels = 200;
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

        //Vector3 pos = CameraManager.I._MainCamera.ScreenToWorldPoint(new Vector3(xpixels, ypixels, CameraManager.I._MainCamera.nearClipPlane * 1.5f));
        //pos.z -= LeftArm.transform.GetChild(0).localScale.y / 1.5f ;
        //LeftArm.transform.position = pos;
        //pos = CameraManager.I._MainCamera.ScreenToWorldPoint(new Vector3(Screen.width - xpixels, ypixels, CameraManager.I._MainCamera.nearClipPlane * 1.5f));
        //pos.z -= RightArm.transform.GetChild(0).localScale.y / 1.5f;
        //RightArm.transform.position = pos;

        //LeftHand = LeftArm.transform.Find("Arm").transform.Find("Hand").gameObject;
        //RightHand = RightArm.transform.Find("Arm").transform.Find("Hand").gameObject;
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
				onItemActivated?.Invoke(it);

				// Impossible d'utiliser ActivateItem() sans le Set-Up de _currentItemPrefabDisplay dans Item.cs
				// C'est ennuyant, je n'ai pas trouver la solution.
				//it.ActivateItem(); 

			}
		}
    }

    public void PutItemInHand(GameObject o, Hand hand)
    {
        GameObject newone = Instantiate(o);
        newone.transform.SetParent(CameraManager.I._MainCamera.transform);

        Vector3 position = Vector3.zero;

        if (hand == Hand.Left)
        {
            position = CameraManager.I._MainCamera.ScreenToWorldPoint(new Vector3(xPixels, yPixels, CameraManager.I._MainCamera.nearClipPlane * 3));
            //position = LeftHand.transform.position + new Vector3(0, 0.1f, 0);
            if (LeftHandItem)
                Destroy(LeftHandItem.gameObject);
            LeftHandItem = newone.GetComponent<Item>();
        }
        else
        {
            position = CameraManager.I._MainCamera.ScreenToWorldPoint(new Vector3(Screen.width - xPixels, yPixels, CameraManager.I._MainCamera.nearClipPlane * 3));
            //position = LeftHand.transform.position + new Vector3(0, 0.1f, 0);
            if (RightHandItem)
                Destroy(RightHandItem.gameObject);
            RightHandItem = newone.GetComponent<Item>();
        }

        Quaternion rotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, position-CameraManager.I._MainCamera.transform.position);

        newone.transform.position = position;
        newone.transform.rotation = rotation;
    }
}
public class ItemActivatedEvent : UnityEvent<Item> { }
public enum Hand
{
    Left,
    Right
}