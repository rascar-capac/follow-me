using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    #region References Player
    Player _refPlayer;
    PlayerMovement _refPlayerMovement;
	PlayerInventory _refPlayerInventory;
	PlayerQuest _refPlayerQuest;
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
    public Text _HudAlertMessageText;
    public Text _HudCurrentTimeText;
    public Text _HudPlayerEnergyText;

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
    public UnityEvent onToolsInventoryOpenedEvent = new UnityEvent();
    public UnityEvent onToolsInventoryClosedEvent = new UnityEvent();
    public bool ToolsInvetoryOpened = false;
    bool AllowOpenInventory = true;
    #endregion
    #region Quest Variables
    List<GameObject> _questCellList = new List<GameObject>();
    #endregion



    #region start, update, awake...
    protected override void Start()
	{
		base.Start();

        _refPlayer = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        _refPlayerMovement = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerMovement>();
		_refPlayerInventory = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerInventory>();
		_refPlayerQuest = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerQuest>();
        _refTribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();

        _refTribe.onTribeEnergyEnterCritical.AddListener(AlertTribeEnergyCritical);

        InputManager.I.onUIPlayerKeyPressed.AddListener(OpenPlayerPanel);
		InputManager.I.onUITribeKeyPressed.AddListener(OpenTribePanel);
		InputManager.I.onUIInventoryKeyPressed.AddListener(OpenInventoryPanel);
		InputManager.I.onUIQuestKeyPressed.AddListener(OpenQuestPanel);
		InputManager.I.onUIMapKeyPressed.AddListener(OpenMapPanel);
		InputManager.I.onUIOptionsKeyPressed.AddListener(OpenOptionsPanel);
        InputManager.I.onToolInventoryButtonPressed.AddListener(OpenToolsInventory);
        InputManager.I.onToolInventoryButtonReleased.AddListener(CloseToolsInventory);
        onToolsInventoryOpenedEvent.AddListener(() => { AllowOpenInventory = false;  });
        onToolsInventoryClosedEvent.AddListener(() => { AllowOpenInventory = true; });

        CloseMenu();

        ShowTime(false);
        ShowPlayerEnergy(false);
        ShowTribeDistance(false);
        ShowTribeEnergy(false);
    }

    private void Update()
    {
        SetTribeDistance();
        SetTimeOfDay();
        SetPlayerEnergy();
        SetTribeEnergy();
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
    public void OpenToolsInventory()
    {
        ToolsInventory.gameObject.SetActive(true);
        ToolsInventory.transform.position = CameraManager.I._MainCamera.transform.position + CameraManager.I._MainCamera.transform.forward * 7f;
        ToolsInventory.transform.rotation = Quaternion.LookRotation(CameraManager.I._MainCamera.transform.up, -CameraManager.I._MainCamera.transform.forward);
        ToolsInvetoryOpened = true;
        onToolsInventoryOpenedEvent?.Invoke();
    }
    public void CloseToolsInventory()
    {
        ToolsInventory.gameObject.SetActive(false);
        ToolsInvetoryOpened = false;
        onToolsInventoryClosedEvent?.Invoke();
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

    public void SetTribeDistance()
    {
        _HudTribeDistanceText.text = $"Tribe distance " + _refPlayerMovement.TribeDistance + " m";
        if (_refPlayerMovement.IsTooFar)
            _HudTribeDistanceText.text += " - Too far.";
    }
    public void SetTimeOfDay()
    {
        _HudCurrentTimeText.text = $"Current time " + (int)AmbiantManager.I.CurrentTimeOfDay + " h (" + AmbiantManager.I.CurrentDayState.State.ToString() + ")";
    }
	public void SetPlayerEnergy()
	{
	    _HudPlayerEnergyText.text = $"Player Energy " + _refPlayer.Energy;
	}
	public void SetTribeEnergy()
    {
	    _HudTribeEnergyText.text = $"Tribe Energy " + _refTribe.Energy;
	}
	public void AlertTribeEnergyCritical()
	{
		AlertMessage("Danger : Tribe energy is critical.", 10f);
	}

	public List<Message> Messages = new List<Message>();
    public void AlertMessage(string message, float duration = 3f)
    {
        Message m = new Message() { Text = message, Duration = duration };
        Messages.Add(m);
        if (_HudAlertMessageText.gameObject.activeSelf)
            return;
        AlertMessageShow();
    }
    void AlertMessageShow()
    {
        Message m = Messages[0];
        Messages.RemoveAt(0);
        _HudAlertMessageText.text = m.Text;
        _HudAlertMessageText.gameObject.SetActive(true);
        StartChrono(m.Duration, DisableMessage);
    }

    void DisableMessage()
    {
        _HudAlertMessageText.gameObject.SetActive(false);
        if (Messages.Count > 0)
            AlertMessageShow();
    }
    #endregion


	#region Inventory Functions
	void GenerateCellsInventory()
	{
		if (_inventoryCellList.Count > 0)
			CleanCellsInventory();

		// Instantiate all cells for each items in inventory and set-up cell
		for (int i = 0; i < _refPlayerInventory._playerInventory.Count; i++)
		{
			_inventoryCellList.Add(Instantiate(_inventoryCellAsset, _inventoryContent));

			_inventoryCellList[i].transform.GetChild(0).GetComponent<Image>().sprite = _refPlayerInventory._playerInventory[i]._itemIcon;
			_inventoryCellList[i].transform.GetChild(1).GetComponent<Text>().text = _refPlayerInventory._playerInventory[i]._itemName;
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
		for (int i = 0; i < _refPlayerQuest._questsPlayer.Count; i++)
		{
			_questCellList.Add(Instantiate(_questCellAsset, _questContent));

			_questCellList[i].transform.GetChild(0).GetComponent<Text>().text = _refPlayerQuest._questsPlayer[i]._questTitle;
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
}
