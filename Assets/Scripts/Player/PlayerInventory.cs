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

    [Header("Beacon Prefab to spawn at Ground")]
	public Item BeaconPrefab;

    [Header("The layers on which the beacons can be placed.")]
	public LayerMask DropableLayers;

    [Header("The depth of the tools from camera.")]
    [Range(1, 10)]
    public float NearClipPlaneMultiply = 3f;

    Tribe tribe;
    Player player;
    PlayerMovement playerMovement;

    bool isInteractionAllowed = true;
	Camera _mainCamera;

    HandWrapper[] Hands;

    public ItemActivatedEvent onItemActivated = new ItemActivatedEvent();
    public ItemActivatedEvent onItemPickedUp = new ItemActivatedEvent();
    float xPixels = 300;
    float yPixels = 300;

    protected override void Start()
	{
		base.Start();
		_mainCamera = CameraManager.I._MainCamera;
        tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();
        player = GetComponent<Player>();
        playerMovement = GetComponent<PlayerMovement>();

		InputManager.I.onInteractionKeyPressed.AddListener(PlayerInteract);
        InputManager.I.onLeftHandShowHide.AddListener(() => { ShowHideHand(Hand.Left); });
        InputManager.I.onRightHandShowHide.AddListener(() => { ShowHideHand(Hand.Right); });
		InputManager.I.onBeaconPlaceButtonPressed.AddListener(PlaceBeacon);
        InputManager.I.onBeaconActivateKeyPressed.AddListener(BeaconActivation);

        UIManager.I.onToolsInventoryClosedEvent.AddListener((hand) => { isInteractionAllowed = true; });
        UIManager.I.onToolsInventoryOpenedEvent.AddListener((hand) => { isInteractionAllowed = false; });


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
        if (!isInteractionAllowed)
            return;

		RaycastHit hitInfo;
		if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out hitInfo, 10000, ItemLayer))
		{
            Item it = hitInfo.transform.GetComponent<Item>();
            if (it)
            {
                if (it._itemData.IsActivable)
                    InteractItem(it);
                else if (it._itemData.IsCatchable)
                    PickUpItem(it);
            }
            else
            {
                Door door = hitInfo.transform.GetComponentInParent<Door>();
                if (door)
                    door.InteractWithDoor();
            }
		}
	}

    void PickUpItem(Item it)
	{
        // à améliorer, gérer autrement l’inventaire peut être?
        if (it._itemData._itemName == "Beacon")
        {
            player.PlacedBeacon.Remove(it);
            tribe.ModeStopAndWait();
        }
        else
        {
            _playerInventory.Add(it._itemData);
        }
        onItemPickedUp.Invoke(it);
        UIManager.I.AlertMessage(it._itemData._itemDescription, WhoTalks: MessageOrigin.Player);

        Destroy(it.gameObject);
	}

	void InteractItem(Item it)
	{
		if (!AmbiantManager.I.IsUsableNow(GameManager.I._data.StonesActivationUsable))
		{
			UIManager.I.AlertMessage("Unable to activate the stone now.");
			return;
		}

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

        Hands[0].ItemPosition = CameraManager.I._MainCamera.ScreenToWorldPoint(new Vector3(xPixels, yPixels, CameraManager.I._MainCamera.nearClipPlane * NearClipPlaneMultiply));
        Hands[0].ItemRotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, CameraManager.I._MainCamera.transform.position - Hands[0].ItemPosition);

        Hands[1].ItemPosition = CameraManager.I._MainCamera.ScreenToWorldPoint(new Vector3(Screen.width - xPixels, yPixels, CameraManager.I._MainCamera.nearClipPlane * NearClipPlaneMultiply));
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

    void PlaceBeacon()
	{
        if (GameManager.I._data.BeaconPlacementCount > 0 && player.PlacedBeacon.Count >= GameManager.I._data.BeaconPlacementCount)
        {
            UIManager.I.AlertMessage("The maximum beacon count has been reached.");
            return;
        }

        if (!AmbiantManager.I.IsUsableNow(GameManager.I._data.BeaconPlacerUsable))
        {
            UIManager.I.AlertMessage("Unable to place beacon now.");
            return;
        }

        if (playerMovement.IsTooFar)
        {
            UIManager.I.AlertMessage("You are too far from tribe.");
            return;
        }

        RaycastHit hitInfo = new RaycastHit();
		if (Physics.Raycast(_mainCamera.transform.position + Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized * GameManager.I._data.BeaconPlacementDistance, Vector3.down, out hitInfo, 100.0f, DropableLayers))
		{
			Item beacon =	Instantiate(BeaconPrefab);

            beacon.transform.position = hitInfo.point;
            player.PlacedBeacon.Insert(0, beacon);
            player.CurrentBeaconIndex = -1;
		}
	}

    public void BeaconActivation()
    {
        if (player.PlacedBeacon == null || player.PlacedBeacon.Count <= 0)
            return;

        player.PlacedBeacon.ForEach(b => DeactivateBeacon(b));
        player.CurrentBeaconIndex++;
        if (player.CurrentBeaconIndex == player.PlacedBeacon.Count)
        {
            player.CurrentBeaconIndex = -1;
            tribe.ModeStopAndWait();
            return;
        }
        ActivateBeacon(player.PlacedBeacon[(int)Mathf.Repeat(player.CurrentBeaconIndex, player.PlacedBeacon.Count)]);
    }

    public void ActivateBeacon(Item beacon)
    {
		//_tribeAgent.destination = new Vector3(beacon.transform.position.x, 0, beacon.transform.position.z);
		tribe.ModeGoToBeacon(new Vector3(beacon.transform.position.x, 0, beacon.transform.position.z));
        beacon.GetComponentInChildren<Animator>().SetBool("IsOpened", true);
    }
    public void DeactivateBeacon(Item beacon)
    {
        beacon.GetComponentInChildren<Animator>().SetBool("IsOpened", false);
    }
}

public class ItemActivatedEvent : UnityEvent<Item> { }
public enum Hand
{
    Left,
    Right
}