using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]

public class PlayerInventory : BaseMonoBehaviour
{
    List<ItemData> _playerInventory = new List<ItemData>();

    [Header("Pick-up / Activate item layer")]
    public LayerMask ItemLayer;

    Tribe tribe;
    Player player;

    bool AllowPickup = true;
    bool AllowActivate = true;
	Camera _mainCamera;

    Item LeftHandItem;
    Item RightHandItem;

    public ItemActivatedEvent onItemActivated = new ItemActivatedEvent();
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
        player = GetComponent<Player>();
		InputManager.I.onPickUpKeyPressed.AddListener(PickUpItem);
		//InputManager.I.onActivateItemKeyPressed.AddListener(InteractItem);
		InputManager.I.onActivateItemKeyPressed.AddListener(PlayerInteract);

		UIManager.I.onToolsInventoryClosedEvent.AddListener((hand) => { AllowPickup = true; AllowActivate = true; });
        UIManager.I.onToolsInventoryOpenedEvent.AddListener((hand) => { AllowPickup = false; AllowActivate = false; });

        ToolsInventoryManager.I.onToolSelected.AddListener(PutItemInHand);

    }

    public List<ItemData> Inventory => _playerInventory;
    public void AddItemToInventory(ItemData itemData)
    {
        Inventory.Add(itemData);
    }

	#region @ Olivier
	// Les interactions dans un jeu peuvent être de plein de type, avec un item, une stone, une porte, ect...
	// Hors en général on à un seul bouton d'interaction, chez nous "Y", donc il va falloir définir avec quoi 
	// on veut intéragir et ce que l'on fait en fonction.
	// Je propose la modification suivante pour les interactions du Player si tu es d'accord:
	//	- PlayerInteract() pilote les interactions en fonction du "hitInfo".
	//	- InteractItem() reçoit dorénavant son RaycastHit. (je n'ai pas bouger à sa logique)
	//	- Rattaché l'event "onActivateItemKeyPressed" à PlayerInteract() (l'event pourrait aussi être renommé)
	// Dans ce cas le LayerMask définit plus haut n'est plus utile, on précise dans le code ce que l'on doit
	// faire en fonction de ce que l'on touche avec le RaycastHit.
	// Si tu n'es pas ok avec cette modification, l'ancien InteractItem() est tjs présent en commentaire plus 
	// bas et PlayerInteract() ainsi que le "nouveau" InteractItem() peuvent être supprimés.
	#endregion
	void PlayerInteract()
	{
        if (!AllowActivate)
            return;

		RaycastHit hitInfo;
		if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hitInfo, GameManager.I._data.PlayerItemDistanceInteraction, ItemLayer))
		{
            Item it = hitInfo.transform.GetComponent<Item>();
            if (it._itemData.IsActivable)
                InteractItem(hitInfo);

			// Interact with Door (Add a Layer for "Door", now on "Ground")
			if (hitInfo.transform.gameObject.layer == 8)
				hitInfo.transform.GetComponentInParent<Door>().InteractWithDoor();
		}
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

	void InteractItem(RaycastHit hitInfo)
	{
		if (!AllowActivate)
			return;

		if (!AmbiantManager.I.IsUsableNow(GameManager.I._data.StonesActivationUsable))
		{
			UIManager.I.AlertMessage("Unable to activate the stone now.");
			return;
		}

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