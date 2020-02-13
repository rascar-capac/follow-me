using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Borodar.FarlandSkies.LowPoly;

public class ToolsInventoryEvent : UnityEvent<Hand> { }
public class UIManager : Singleton<UIManager>
{
	public Camera CameraMain;
	public Camera CameraUI;

    #region References Player
    Player _Player;
    PlayerMovement _PlayerMovement;
	PlayerLook _PlayerLook;
	PlayerInventory _PlayerInventory;
    #endregion

    #region References Tribe
    Tribe _Tribe;
    #endregion

    #region References UI Prefabs
    [Header("HUD Panel")]
	public GameObject HudPanel;
	[Header("Main Menu")]
	public GameObject MainMenu;
	//[Header("Tout les Panels du Menu (sauf MainMenu)")]
	//public List<GameObject> MenuPanels;

	[Header("HUD Components")]
    public Text _HudTribeDistanceText;
    public Text _HudTribeEnergyText;
    public Text _HudCurrentTimeText;
    public Text _HudPlayerEnergyText;
    public Text _HudPlayerRunStaminaText;
    public Text _HudTribeDocilityText;


    //public Text _HudAlertMessageText;
    //public Text _HudTribeSaysText;
    //public Text _HudPlayerSaysText;

	//[Header("Inventory Prefabs")]
	//public Transform _inventoryContent;
	//public GameObject _inventoryCellAsset;
	//public GameObject PanelViewInventory;
	//public GameObject ItemViewer;

	//[Header("Quest Prefabs")]
	//public Transform _questContent;
	//public GameObject _questCellAsset;
	//public GameObject PanelViewQuest;

    //[Header("Tools Inventory gameobject")]
    //public GameObject ToolsInventory;
	#endregion

	#region Archives

	//#region Quest Variables

	//List<GameObject> _QuestCellList = new List<GameObject>();

	//#endregion


	//#region Inventory Variables

	//List<GameObject> _InventoryCellList = new List<GameObject>();
	//   public ToolsInventoryEvent onToolsInventoryOpenedEvent = new ToolsInventoryEvent();
	//   public ToolsInventoryEvent onToolsInventoryClosedEvent = new ToolsInventoryEvent();
	//   public bool ToolsInvetoryOpened = false;
	//   bool AllowOpenInventory = true;

	//#endregion

	#endregion

	Hand CurrentOpenedHand;


    #region start, update, awake...

    protected override void Start()
	{
		base.Start();

        _Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
		_PlayerMovement = _Player.GetComponent<PlayerMovement>();
		_PlayerInventory = _Player.GetComponent<PlayerInventory>();
		_PlayerLook = _Player.transform.GetChild(0).GetComponent<PlayerLook>();

		_Tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();

		#region Event Listeners

		_Tribe.onTribeEnergyEnterCritical.AddListener(AlertTribeEnergyCritical);

		InputManager.I.onUIOptionsKeyPressed.AddListener(OpenCloseMainMenu);
		//InputManager.I.onUIOpenCloseInventoryKeyPressed.AddListener(OpenCloseMainMenu);
        //InputManager.I.onLeftHandOpenToolInventory.AddListener(() => { OpenToolsInventory(Hand.Left); }) ;
        //InputManager.I.onLeftHandCloseToolInventory.AddListener(() => { CloseToolsInventory(Hand.Left); });
        //InputManager.I.onRightHandOpenToolInventory.AddListener(() => { OpenToolsInventory(Hand.Right); });
        //InputManager.I.onRightHandCloseToolInventory.AddListener(() => { CloseToolsInventory(Hand.Right); });

        //onToolsInventoryOpenedEvent.AddListener((hand) => { AllowOpenInventory = false; });
        //onToolsInventoryClosedEvent.AddListener((hand) => { AllowOpenInventory = true; });

		#endregion

		#region Messages System

		Callbacks = new List<System.Action>();
        Messages[(int)MessageOrigin.System] = new List<Message>();
        MessageText[(int)MessageOrigin.System].gameObject.SetActive(false);
        Callbacks.Add(DisableMessageSystem);

        Messages[(int)MessageOrigin.Tribe] = new List<Message>();
        MessageText[(int)MessageOrigin.Tribe].gameObject.SetActive(false);
        Callbacks.Add(DisableMessageTribe);

        Messages[(int)MessageOrigin.Player] = new List<Message>();
        MessageText[(int)MessageOrigin.Player].gameObject.SetActive(false);
        Callbacks.Add(DisableMessagePlayer);

		#endregion

		#region Hud Messages

		ShowTime(false);
        ShowPlayerEnergy(false);
        ShowTribeDistance(false);
        ShowPlayerRunStamina(false);
        ShowTribeDocility(false);
        ShowTribeEnergy(false);
        ShowPlayerRunStamina(false);
        #endregion

        CloseMenu();
	}
    #endregion

    #region Hud Functions

    public void ShowTime(bool show)
    {
        _HudCurrentTimeText.gameObject.SetActive(show);
    }
    public void ShowPlayerEnergy(bool show)
    {
        _HudPlayerEnergyText.gameObject.SetActive(show);
    }
    public void ShowPlayerRunStamina(bool show)
    {
        _HudPlayerRunStaminaText.gameObject.SetActive(show);
    }
    public void ShowTribeEnergy(bool show)
    {
        _HudTribeEnergyText.gameObject.SetActive(show);
    }
    public void ShowTribeDistance(bool show)
    {
        _HudTribeDistanceText.gameObject.SetActive(show);
    }
    public void ShowTribeDocility(bool show)
    {
        _HudTribeDocilityText.gameObject.SetActive(show);
    }

    public void SetTimeOfDay()
    {
        float hour = SkyboxDayNightCycle.Instance.TimeOfDay / 100 * 24; 
        _HudCurrentTimeText.text = $"Current time " + Mathf.Round(hour) + " h (" + AmbiantManager.I.currentStateOfDay.ToString() + ")";
    }
	public void SetPlayerEnergy()
	{
	    _HudPlayerEnergyText.text = $"Player Energy " + Mathf.Floor(_Player.Energy);
	}
    public void SetRunStamina(float runStamina)
    {
        _HudPlayerRunStaminaText.text = $"Run Stamina " + Mathf.Floor(runStamina);
    }
	public void SetTribeEnergy()
    {
	    _HudTribeEnergyText.text = $"Glaucus Energy " + Mathf.Floor(_Tribe.Energy);
    }
    public void SetTribeDistance()
    {
        _HudTribeDistanceText.text = $"Tribe distance " + Mathf.Floor(_PlayerMovement.TribeDistance) + " m";
        if (_PlayerMovement.IsTooFar)
            _HudTribeDistanceText.text += " - Too far.";
    }
    public void SetTribeDocility(bool isIgnoring)
    {
        if(isIgnoring)
        {
            _HudTribeDocilityText.text = "Glaucus is ignoring you";
        }
        else
        {
            _HudTribeDocilityText.text = "";
        }
    }

	public void AlertTribeEnergyCritical()
	{
		AlertMessage("Danger : Tribe energy is critical.", 3f);
	}

	List<Message>[] Messages = new List<Message>[3];
    List<System.Action> Callbacks = new List<System.Action>();
    [Header("0 : System, 1 : Tribe, 2 : Player")]
    public Text[] MessageText;

    public void AlertMessage(string message, float duration = 3f, MessageOrigin WhoTalks = MessageOrigin.System)
    {

        Message m = new Message() { Text = message, Duration = duration, Origin = WhoTalks };
        Messages[(int)WhoTalks].Add(m);
        if (MessageText[(int)WhoTalks].gameObject.activeSelf)
            return;
        AlertMessageShow(WhoTalks);
    }
    void AlertMessageShow(MessageOrigin WhoTalks = MessageOrigin.System)
    {
        Message m = Messages[(int)WhoTalks][0];
        Messages[(int)WhoTalks].RemoveAt(0);
        MessageText[(int)WhoTalks].text = m.Text;
        MessageText[(int)WhoTalks].gameObject.SetActive(true);
        StartChrono(m.Duration, Callbacks[(int)WhoTalks]);
    }

    void DisableMessageSystem()
    {
        MessageText[(int)MessageOrigin.System].gameObject.SetActive(false);
        if (Messages[(int)MessageOrigin.System].Count > 0)
            AlertMessageShow(MessageOrigin.System);
    }
    void DisableMessageTribe()
    {
        MessageText[(int)MessageOrigin.Tribe].gameObject.SetActive(false);
        if (Messages[(int)MessageOrigin.Tribe].Count > 0)
            AlertMessageShow(MessageOrigin.Tribe);
    }
    void DisableMessagePlayer()
    {
        MessageText[(int)MessageOrigin.Player].gameObject.SetActive(false);
        if (Messages[(int)MessageOrigin.Player].Count > 0)
            AlertMessageShow(MessageOrigin.Player);
    }

	#endregion

	#region Menu Functions

	public void OpenCloseMainMenu()
	{
		if (MainMenu.activeSelf == false)
		{
			OpenMenu();
		}
		else
		{
			CloseMenu();
		}
	}
	void OpenMenu()
	{
		MainMenu.SetActive(true);

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
	void CloseMenu()
	{
		MainMenu.SetActive(false);

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	//public void OpenCloseMainMenu()
	//{
	//	if (MainMenu.activeSelf == false)
	//	{
	//		CameraMain.gameObject.SetActive(false);
	//		CameraUI.gameObject.SetActive(true);
	//		//OpenQuestPanel();
	//		_PlayerMovement.InGame = false;
	//		_PlayerLook.AllowLook = false;
	//	}
	//	else
	//	{
	//		CameraMain.gameObject.SetActive(true);
	//		CameraUI.gameObject.SetActive(false);

	//		CloseMenu();
	//		_PlayerMovement.InGame = true;
	//		_PlayerLook.AllowLook = true;
	//	}
	//}
	//   void OpenMenu(string MenuName, bool CloseOthers = true)
	//   {
	//       if (MainMenu == null || MenuPanels == null)
	//           return;

	//	MainMenu.SetActive(true);
	//	HudPanel.SetActive(false);

	//       for (int i = 0; i < MenuPanels.Count; i++)
	//       {
	//           if (CloseOthers && MenuName != MenuPanels[i].name)
	//               MenuPanels[i].SetActive(false);
	//           if (MenuPanels[i].name == MenuName)
	//               MenuPanels[i].SetActive(true);
	//       }

	//	Cursor.visible = true;
	//	Cursor.lockState = CursorLockMode.None;
	//}
	//void CloseMenu()
	//{
	//	if (MainMenu == null || MenuPanels == null)
	//		return;

	//	MainMenu.SetActive(false);
	//	HudPanel.SetActive(true);

	//	for (int i = 0; i < MenuPanels.Count; i++)
	//		MenuPanels[i].SetActive(false);

	//	//CleanCellsInventory();
	//	//CleanCellsQuest();
	//	//ItemViewer.SetActive(false);

	//	Cursor.visible = false;
	//	Cursor.lockState = CursorLockMode.Locked;
	//}

	//public void OpenInventoryPanel()
	//{
	//       if (!AllowOpenInventory)
	//           return;

	//	ItemViewer.SetActive(false);
	//	OpenMenu("InventoryPanel");
	//	GenerateCellsInventory();
	//}
	//public void OpenQuestPanel()
	//{
	//	ItemViewer.SetActive(false);
	//	OpenMenu("QuestPanel");
	//	GenerateCellsQuest();
	//}
	//public void OpenOptionsPanel()
	//{
	//	ItemViewer.SetActive(false);
	//	OpenMenu("OptionsPanel");
	//}

	//public void OpenToolsInventory(Hand hand)
	//{
	//    ToolsInventory.gameObject.SetActive(true);
	//    ToolsInventory.transform.position = CameraManager.I._MainCamera.transform.position + CameraManager.I._MainCamera.transform.forward * 7f;
	//    ToolsInventory.transform.rotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, -CameraManager.I._MainCamera.transform.forward);
	//    ToolsInvetoryOpened = true;
	//    onToolsInventoryOpenedEvent?.Invoke(hand);
	//}
	//public void CloseToolsInventory(Hand hand)
	//{
	//    onToolsInventoryClosedEvent?.Invoke(hand);
	//    ToolsInventory.gameObject.SetActive(false);
	//    ToolsInvetoryOpened = false;
	//}

	#endregion


	#region Archives

	//#region Quest Functions

	//void GenerateCellsQuest()
	//{
	//	// Checking QuestCellList is empty
	//	if (_QuestCellList.Count > 0)
	//		CleanCellsQuest();

	//	// Instantiate cells for each quest.
	//	List<Quest> playerQuestsList = QuestManager.I.Quests;
	//	for (int i = 0; i < playerQuestsList.Count; i++)
	//	{
	//           _QuestCellList.Add(Instantiate(_questCellAsset, _questContent));

	//		// Set-up for each cell.
	//		QuestCell questCell = _QuestCellList[i].GetComponent<QuestCell>();
	//		questCell.Quest = playerQuestsList[i];
	//		questCell.PanelViewQuest = PanelViewQuest;
	//		questCell.InitQuestCell();
	//	}
	//}
	//void CleanCellsQuest()
	//{
	//	// Destroy all gameobjects cells in QuestCellList
	//	foreach (GameObject cell in _QuestCellList)
	//	{
	//		Destroy(cell);
	//	}

	//	// Clean QuestCellList.
	//	_QuestCellList.Clear();

	//	// Desactivate PanelView for Quest.
	//	PanelViewQuest.SetActive(false);
	//}

	//#endregion


	//   #region Inventory Functions

	//   void GenerateCellsInventory()
	//{
	//	// Checking InventoryCellList is empty
	//	if (_InventoryCellList.Count > 0)
	//		CleanCellsInventory();

	//	// Instantiate cells for each items.
	//	List<ItemData> playerInventoryList = _PlayerInventory.Inventory;
	//	for (int i = 0; i < playerInventoryList.Count; i++)
	//	{
	//		_InventoryCellList.Add(Instantiate(_inventoryCellAsset, _inventoryContent));

	//		// Checking QuestCellList is empty
	//		InventoryCell inventoryCell = _InventoryCellList[i].GetComponent<InventoryCell>();
	//		inventoryCell.ItemData = playerInventoryList[i];
	//		inventoryCell.PanelViewInventory = PanelViewInventory;
	//		inventoryCell.ItemViewer = ItemViewer;
	//		inventoryCell.InitInventoryCell();
	//	}
	//}
	//void CleanCellsInventory()
	//{
	//	foreach (GameObject cell in _InventoryCellList)
	//	{
	//		Destroy(cell);
	//	}

	//	// Clean InventoryCellList.
	//	_InventoryCellList.Clear();

	//	// Desactivate PanelView for Inventory.
	//	PanelViewInventory.SetActive(false);
	//}

	//#endregion

	#endregion
}
public struct Message
{
    public string Text;
    public float Duration;
    public MessageOrigin Origin;
}

public enum MessageOrigin
{
    System,
    Tribe,
    Player
}