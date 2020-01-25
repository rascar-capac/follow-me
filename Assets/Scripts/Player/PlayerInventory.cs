using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]

public class PlayerInventory : BaseMonoBehaviour
{
	[Header("Player Inventory")]
	public List<ItemData> _playerInventory;

    [Header("Pick-up item layer")]
    public LayerMask ItemLayer;

    public List<Item> ItemsToActivate;
    [HideInInspector]
    public List<Item> dynamicItems;
    //public GameObject LeftArm;
    //public GameObject LeftHand;
    //public GameObject RightArm;
    //public GameObject RightHand;

    Tribe tribe;

    bool AllowPickup = true;
    bool AllowActivate = true;
	Camera _mainCamera;

    Item LeftHandItem;
    Item RightHandItem;

    public ItemActivatedEvent onItemActivated = new ItemActivatedEvent();
    public UnityEvent onQuestStoneFinished = new UnityEvent();
    float xPixels = 300;
    float yPixels = 300;
    Vector3 leftHandItemPosition;
    Vector3 rightHandItemPosition;
    Quaternion leftHandItemRotation;
    Quaternion rightHandItemRotation;

    protected override void Start()
	{
		base.Start();
		_mainCamera = CameraManager.I._MainCamera;
        tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();

		InputManager.I.onPickUpKeyPressed.AddListener(PickUpItem);
        InputManager.I.onActivateItemKeyPressed.AddListener(InteractItem);

        UIManager.I.onToolsInventoryClosedEvent.AddListener(() => { AllowPickup = true; AllowActivate = true; });
        UIManager.I.onToolsInventoryOpenedEvent.AddListener(() => { AllowPickup = false; AllowActivate = false; });

        ToolsInventoryManager.I.onToolSelected.AddListener(PutItemInHand);

        dynamicItems = new List<Item>();
        dynamicItems.AddRange(ItemsToActivate);
        foreach (Item it in dynamicItems)
        {
            it._itemData.IsActivated = false;
        }

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
		if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hitInfo, GameManager.I._data.PlayerItemDistanceInteraction, ItemLayer))
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

        if (!AmbiantManager.I.IsUsableNow(GameManager.I._data.StonesActivationUsable))
        {
            UIManager.I.AlertMessage("Unable to activate the stone now.");
            return;
        }

		RaycastHit hitInfo;
		if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hitInfo, GameManager.I._data.PlayerItemDistanceInteraction, ItemLayer))
        {
            Item it = hitInfo.transform.GetComponentInParent<Item>();
            ItemData data = it._itemData;

            if (!(it.InZone && tribe.InZones.Exists(z => z == it.InZone)))
            {
                UIManager.I.AlertMessage("Your tribe must be here to activate this Item.");
                return;
            }

			if (data.IsActivable && !data.IsActivated && data._itemActivatedPrefab != null)
			{
				onItemActivated?.Invoke(it);

				it.ActivateItem();
                dynamicItems.Remove(it);
                if (dynamicItems.Count <= 0)
                {
                    UIManager.I.AlertMessage("All stones has been activated !");
                    onQuestStoneFinished.Invoke();  
                }
			}
		}
    }

    public void PutItemInHand(GameObject o, Hand hand)
    {
        leftHandItemPosition = CameraManager.I._MainCamera.ScreenToWorldPoint(new Vector3(xPixels, yPixels, CameraManager.I._MainCamera.nearClipPlane * 3));
        leftHandItemRotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, CameraManager.I._MainCamera.transform.position - leftHandItemPosition);

        rightHandItemPosition = CameraManager.I._MainCamera.ScreenToWorldPoint(new Vector3(Screen.width - xPixels, yPixels, CameraManager.I._MainCamera.nearClipPlane * 3));
        rightHandItemRotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, CameraManager.I._MainCamera.transform.position - rightHandItemPosition);

        if (hand == Hand.Left && RightHandItem && RightHandItem._itemData._itemName == o.GetComponent<Item>()._itemData._itemName)
        {
            SwapHands();
            return;
        }
        if (hand == Hand.Right && LeftHandItem && LeftHandItem._itemData._itemName == o.GetComponent<Item>()._itemData._itemName)
        {
            SwapHands();
            return;
        }

        GameObject newone = Instantiate(o);
        newone.transform.SetParent(CameraManager.I._MainCamera.transform);

        if (hand == Hand.Left)
        {
            if (LeftHandItem)
                Destroy(LeftHandItem.gameObject);
            LeftHandItem = newone.GetComponent<Item>();
            LeftHandItem.IsEnabled = true;

            LeftHandItem.transform.position = leftHandItemPosition;
            LeftHandItem.transform.rotation = leftHandItemRotation;
        }
        else
        {
            if (RightHandItem)
                Destroy(RightHandItem.gameObject);
            RightHandItem = newone.GetComponent<Item>();
            RightHandItem.IsEnabled = true;
            
            RightHandItem.transform.position = rightHandItemPosition;
            RightHandItem.transform.rotation = rightHandItemRotation;
        }
    }

    public void SwapHands() {
        (LeftHandItem, RightHandItem) = (RightHandItem, LeftHandItem);
        if (RightHandItem)
        {
            RightHandItem.transform.position = rightHandItemPosition;
            RightHandItem.transform.rotation = rightHandItemRotation;
        }
        if (LeftHandItem)
        { 
            LeftHandItem.transform.position = leftHandItemPosition;
            LeftHandItem.transform.rotation = leftHandItemRotation;
        }
    }
}
public class ItemActivatedEvent : UnityEvent<Item> { }
public enum Hand
{
    Left,
    Right
}