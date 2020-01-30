using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ToolsInventoryEvent : UnityEvent<Hand> { }
public class UIManager : Singleton<UIManager>
{
    #region References Player
    Player Player;
    PlayerMovement _refPlayerMovement;
	PlayerInventory _refPlayerInventory;
	//PlayerQuest _refPlayerQuest;
    #endregion

    #region References Tribe
    Tribe _refTribe;
    #endregion

    #region References UI Prefabs
    [Header("HUD Panel")]
	public GameObject _hudPanel;
	[Header("Main Menu")]
	public GameObject _mainMenu;
	[Header("Tout les Panels du Menu (sauf MainMenu)")]
	public List<GameObject> MenuPanels;

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

    [Header("Inventory Prefabs")]
	public Transform _inventoryContent;
	public GameObject _inventoryCellAsset;

	[Header("Quest Prefabs")]
	public Transform _questContent;
	public GameObject _questCellAsset;

    [Header("Tools Inventory gameobject")]
    public GameObject ToolsInventory;
	#endregion

	#region Inventory Variables
	List<GameObject> _inventoryCellList = new List<GameObject>();
    public ToolsInventoryEvent onToolsInventoryOpenedEvent = new ToolsInventoryEvent();
    public ToolsInventoryEvent onToolsInventoryClosedEvent = new ToolsInventoryEvent();
    public bool ToolsInvetoryOpened = false;
    bool AllowOpenInventory = true;
    #endregion
    #region Quest Variables
    List<GameObject> _questCellList = new List<GameObject>();
    #endregion

    Hand CurrentOpenedHand;


    #region start, update, awake...
    protected override void Start()
	{
		base.Start();

        Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        _refPlayerMovement = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerMovement>();
		_refPlayerInventory = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerInventory>();
		//_refPlayerQuest = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerQuest>();
        _refTribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();

        _refTribe.onTribeEnergyEnterCritical.AddListener(AlertTribeEnergyCritical);

  //      InputManager.I.onUIPlayerKeyPressed.AddListener(OpenPlayerPanel);
		//InputManager.I.onUITribeKeyPressed.AddListener(OpenTribePanel);
		InputManager.I.onUIOpenCloseInventoryKeyPressed.AddListener(OpenInventoryPanel);
        //InputManager.I.onUIQuestKeyPressed.AddListener(OpenQuestPanel);
        //InputManager.I.onUIMapKeyPressed.AddListener(OpenMapPanel);
        //InputManager.I.onUIOptionsKeyPressed.AddListener(OpenOptionsPanel);

        InputManager.I.onLeftHandOpenToolInventory.AddListener(() => { OpenToolsInventory(Hand.Left); }) ;
        InputManager.I.onLeftHandCloseToolInventory.AddListener(() => { CloseToolsInventory(Hand.Left); });
        InputManager.I.onRightHandOpenToolInventory.AddListener(() => { OpenToolsInventory(Hand.Right); });
        InputManager.I.onRightHandCloseToolInventory.AddListener(() => { CloseToolsInventory(Hand.Right); });

        onToolsInventoryOpenedEvent.AddListener((hand) => { AllowOpenInventory = false; });
        onToolsInventoryClosedEvent.AddListener((hand) => { AllowOpenInventory = true; });

        Callbacks = new List<System.Action>();
        Messages[(int)MessageOrigin.System] = new List<Message>();
        Callbacks.Add(DisableMessageSystem);

        Messages[(int)MessageOrigin.Tribe] = new List<Message>();
        Callbacks.Add(DisableMessageTribe);

        Messages[(int)MessageOrigin.Player] = new List<Message>();
        Callbacks.Add(DisableMessagePlayer);

        CloseMenu();

        ShowTime(false);
        ShowPlayerEnergy(false);
        ShowTribeDistance(false);
        ShowPlayerRunStamina(false);
        ShowTribeDocility(false);
    }

    #endregion

    #region Menu Functions
    void OpenMenu(string MenuName, bool CloseOthers = true)
    {
        if (_mainMenu == null || MenuPanels == null)
            return;

		_mainMenu.SetActive(true);
		_hudPanel.SetActive(false);

        for (int i = 0; i < MenuPanels.Count; i++)
        {
            if (CloseOthers && MenuName != MenuPanels[i].name)
                MenuPanels[i].SetActive(false);
            if (MenuPanels[i].name == MenuName)
                MenuPanels[i].SetActive(true);
        }

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
	public void CloseMenu()
	{
		if (_mainMenu == null || MenuPanels == null)
			return;

		_mainMenu.SetActive(false);
		_hudPanel.SetActive(true);

		for (int i = 0; i < MenuPanels.Count; i++)
			MenuPanels[i].SetActive(false);
		CleanCellsInventory();
		CleanCellsQuest();

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	public void OpenPlayerPanel()
	{
		OpenMenu("PlayerPanel");
	}
	public void OpenTribePanel()
	{
		OpenMenu("TribePanel");
	}
	public void OpenInventoryPanel()
	{
        if (!AllowOpenInventory)
            return;

		OpenMenu("InventoryPanel");
		GenerateCellsInventory();
	}
	public void OpenQuestPanel()
	{
		OpenMenu("QuestPanel");
		GenerateCellsQuest();
	}
	public void OpenMapPanel()
	{
		OpenMenu("MapPanel");
	}
	public void OpenOptionsPanel()
	{
		OpenMenu("OptionsPanel");
	}
    public void OpenToolsInventory(Hand hand)
    {
        ToolsInventory.gameObject.SetActive(true);
        ToolsInventory.transform.position = CameraManager.I._MainCamera.transform.position + CameraManager.I._MainCamera.transform.forward * 7f;
        ToolsInventory.transform.rotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, -CameraManager.I._MainCamera.transform.forward);
        ToolsInvetoryOpened = true;
        onToolsInventoryOpenedEvent?.Invoke(hand);
    }
    public void CloseToolsInventory(Hand hand)
    {
        onToolsInventoryClosedEvent?.Invoke(hand);
        ToolsInventory.gameObject.SetActive(false);
        ToolsInvetoryOpened = false;
    }
    #endregion

    #region Hud Functions
    public void ShowTribeEnergy(bool show)
    {
        _HudTribeEnergyText.gameObject.SetActive(show);
    }
    public void ShowTime(bool show)
    {
        _HudCurrentTimeText.gameObject.SetActive(show);
    }
    public void ShowPlayerEnergy(bool show)
    {
        _HudPlayerEnergyText.gameObject.SetActive(show);
    }
    public void ShowTribeDistance(bool show)
    {
        _HudTribeDistanceText.gameObject.SetActive(show);
    }

    public void ShowPlayerRunStamina(bool show)
    {
        _HudPlayerRunStaminaText.gameObject.SetActive(show);
    }

    public void ShowTribeDocility(bool show)
    {
        _HudTribeDocilityText.gameObject.SetActive(show);
    }

    public void SetTribeDistance()
    {
        _HudTribeDistanceText.text = $"Tribe distance " + Mathf.Floor(_refPlayerMovement.TribeDistance) + " m";
        if (_refPlayerMovement.IsTooFar)
            _HudTribeDistanceText.text += " - Too far.";
    }
    public void SetTimeOfDay()
    {
        _HudCurrentTimeText.text = $"Current time " + (int)AmbiantManager.I.CurrentTimeOfDay + " h (" + AmbiantManager.I.CurrentDayState.State.ToString() + ")";
    }
	public void SetPlayerEnergy()
	{
	    _HudPlayerEnergyText.text = $"Player Energy " + Mathf.Floor(Player.Energy);
	}
	public void SetTribeEnergy()
    {
	    _HudTribeEnergyText.text = $"Tribe Energy " + Mathf.Floor(_refTribe.Energy);
    }
    public void SetRunStamina(float runStamina)
    {
        _HudPlayerRunStaminaText.text = $"Run Stamina " + Mathf.Floor(runStamina);
    }
    public void SetTribeDocility(float tribeDocility, int docilityLevel, string movementMode,
            float spontaneityCheckTimer, bool isIgnoring, float ignoranceTimer)
    {
        _HudTribeDocilityText.text =
                $"Tribe Docility " + Mathf.Floor(tribeDocility) + " level " + docilityLevel + " mode " + movementMode + "\n"
                + "Spontaneity timer " + spontaneityCheckTimer + "\n"
                + "Ignorance " + (isIgnoring ? "yes ":"no ") + ignoranceTimer;
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
        MessageText[(int)WhoTalks].text = WhoTalks.ToString() + ": " + m.Text;
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


    #region Inventory Functions
    void GenerateCellsInventory()
	{
		if (_inventoryCellList.Count > 0)
			CleanCellsInventory();

		// Instantiate all cells for each items in inventory and set-up cell
		for (int i = 0; i < _refPlayerInventory.Inventory.Count; i++)
		{
			_inventoryCellList.Add(Instantiate(_inventoryCellAsset, _inventoryContent));

			_inventoryCellList[i].transform.GetChild(0).GetComponent<Image>().sprite = _refPlayerInventory.Inventory[i]._itemIcon;
			_inventoryCellList[i].transform.GetChild(1).GetComponent<Text>().text = _refPlayerInventory.Inventory[i]._itemName;
			//_inventoryUiItemList[i].transform.GetChild(2).GetComponent<Image>().sprite = _refPlayerInventory._playerInventory[i]._itemIcon;
		}
	}
	void CleanCellsInventory()
	{
		foreach (GameObject cell in _inventoryCellList)
		{
			Destroy(cell);
		}
		_inventoryCellList.Clear();
	}
	#endregion


	#region Quest Functions
	void GenerateCellsQuest()
	{
		if (_questCellList.Count > 0)
			CleanCellsQuest();

		// Instantiate all cells for each items in inventory and set-up cell
		for (int i = 0; i < Player.Quests.Count; i++)
		{
            _questCellList.Add(Instantiate(_questCellAsset, _questContent));
            _questCellList[i].transform.GetChild(0).GetComponent<Text>().text = Player.Quests[i].Data.QuestTitle;
		}
	}
	void CleanCellsQuest()
	{
		foreach (GameObject cell in _questCellList)
		{
			Destroy(cell);
		}
		_questCellList.Clear();
	}
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