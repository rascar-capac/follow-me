using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class HandWrapper
{
    public Item Item = null;
    public bool Show = true;
    public Vector3 ItemPosition = Vector3.zero;
    public Quaternion ItemRotation = Quaternion.identity;
    public Hand hand;
}

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

    HandWrapper[] Hands;

    public ItemActivatedEvent onItemActivated = new ItemActivatedEvent();
    float xPixels = 300;
    float yPixels = 300;

    protected override void Start()
	{
		base.Start();
		_mainCamera = CameraManager.I._MainCamera;
        tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();
        player = GetComponent<Player>();
		InputManager.I.onPickUpKeyPressed.AddListener(PickUpItem);
		InputManager.I.onActivateItemKeyPressed.AddListener(PlayerInteract);

        InputManager.I.onLeftHandShowHide.AddListener(() => { ShowHideHand(Hand.Left); });
        InputManager.I.onRightHandShowHide.AddListener(() => { ShowHideHand(Hand.Right); });

        UIManager.I.onToolsInventoryClosedEvent.AddListener((hand) => { AllowPickup = true; AllowActivate = true; });
        UIManager.I.onToolsInventoryOpenedEvent.AddListener((hand) => { AllowPickup = false; AllowActivate = false; });

        ToolsInventoryManager.I.onToolSelected.AddListener(PutItemInHand);

        Hands = new HandWrapper[2];
        Hands[0] = new HandWrapper() { hand = Hand.Left };
        Hands[1] = new HandWrapper() { hand = Hand.Right };
    }

    public void ShowHideHand(Hand hand)
    {
        Hands[(int)hand].Show = !Hands[(int)hand].Show;
        if (Hands[(int)hand].Item)
        {
            Hands[(int)hand].Item.IsEnabled = Hands[(int)hand].Show;
            Hands[(int)hand].Item.gameObject.SetActive(Hands[(int)hand].Show);
        }
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
        HandWrapper CurrentHand = Hands[(int)hand];
        HandWrapper OtherHand = Hands[(int)Mathf.Repeat(((int)hand) + 1, 2)];

        Hands[0].ItemPosition = CameraManager.I._MainCamera.ScreenToWorldPoint(new Vector3(xPixels, yPixels, CameraManager.I._MainCamera.nearClipPlane * 3));
        Hands[0].ItemRotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, CameraManager.I._MainCamera.transform.position - Hands[0].ItemPosition);

        Hands[1].ItemPosition = CameraManager.I._MainCamera.ScreenToWorldPoint(new Vector3(Screen.width - xPixels, yPixels, CameraManager.I._MainCamera.nearClipPlane * 3));
        Hands[1].ItemRotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, CameraManager.I._MainCamera.transform.position - Hands[1].ItemPosition);

        if (OtherHand.Item && OtherHand.Item._itemData._itemName == o.GetComponent<Item>()._itemData._itemName)
        {
            SwapHands();
            return;
        }

        GameObject newone = Instantiate(o);
        newone.transform.SetParent(CameraManager.I._MainCamera.transform);

        if (CurrentHand.Item)
            Destroy(CurrentHand.Item.gameObject);

        CurrentHand.Item = newone.GetComponent<Item>();
        CurrentHand.Item.IsEnabled = true;

        CurrentHand.Item.transform.position = CurrentHand.ItemPosition;
        CurrentHand.Item.transform.rotation = CurrentHand.ItemRotation;

    }

    public void SwapHands() {

        (Hands[0].Item, Hands[1].Item) = (Hands[1].Item, Hands[0].Item);

        if (Hands[0].Item)
        {
            Hands[0].Item.transform.position = Hands[0].ItemPosition;
            Hands[0].Item.transform.rotation = Hands[0].ItemRotation;
        }
        if (Hands[1].Item)
        {
            Hands[1].Item.transform.position = Hands[1].ItemPosition;
            Hands[1].Item.transform.rotation = Hands[1].ItemRotation;
        }
    }
}
public class ItemActivatedEvent : UnityEvent<Item> { }
public enum Hand
{
    Left,
    Right
}