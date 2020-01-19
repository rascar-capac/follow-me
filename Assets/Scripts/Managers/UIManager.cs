using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region References Player
    Player _refPlayer;
    PlayerMovement _refPlayerMovement;
	PlayerInventory _refPlayerInventory;
	#endregion

	#region References UI Elements
	[Header("UI Réferences")]
	public GameObject _inventoryPanel;
    public GameObject _inventoryContent;
    public Text _HudTribeDistanceText;
    public Text _HudAlertMessageText;
    public Text _HudCurrentTimeText;
    public Text _HudPlayerLifeText;
    public GameObject _backgroundImage;
	#endregion

	#region Prefabs UI Elements
	[Header("Assets Réferences")]
	public GameObject _inventoryCellPrefab;
	#endregion
	[Header("HUD Panel")]
	public GameObject _hudPanel;
	[Header("Main Menu")]
	public GameObject _mainMenu;
	[Header("Tout les Panels du Menu")]
	public List<GameObject> MenuPanels;

	#region Inventory Variables
	List<GameObject> _inventoryUiItemList = new List<GameObject>();
	#endregion


	protected override void Start()
	{
		base.Start();

        _refPlayerMovement = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerMovement>();
		_refPlayerInventory = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerInventory>();
        _refPlayer = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();

		InputManager.I.onUIPlayerKeyPressed.AddListener(OpenPlayerPanel);
		InputManager.I.onUITribeKeyPressed.AddListener(OpenTribePanel);
		InputManager.I.onUIInventoryKeyPressed.AddListener(OpenInventoryPanel);
		InputManager.I.onUIQuestKeyPressed.AddListener(OpenQuestPanel);
		InputManager.I.onUIMapKeyPressed.AddListener(OpenMapPanel);
		InputManager.I.onUIOptionsKeyPressed.AddListener(OpenOptionsPanel);

		CloseMenu();
	}

    private void Update()
    {
        SetTribeDistance();
        SetTimeOfDay();
        SetPlayerLife();
    }

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
		OpenMenu("InventoryPanel");
	}
	public void OpenQuestPanel()
	{
		OpenMenu("QuestPanel");
	}
	public void OpenMapPanel()
	{
		OpenMenu("MapPanel");
	}
	public void OpenOptionsPanel()
	{
		OpenMenu("OptionsPanel");
	}

	#region Inventory Functions
	void InventoryOpenClose()
	{
		if (_inventoryPanel.activeSelf == false)
			OpenInventory();
		else
			CloseInventory();
	}
	void OpenInventory()
	{
			RectTransform invContentRectT = _inventoryContent.GetComponent<RectTransform>();
			float invCellHeight = _inventoryCellPrefab.GetComponent<RectTransform>().sizeDelta.y;

			// Set height of InventoryPanel/Scroll View/Viewport/Content
			invContentRectT.sizeDelta = new Vector2(invContentRectT.sizeDelta.x, invCellHeight * _refPlayerInventory._playerInventory.Count);

			// Instantiate all cells for each items in inventory and set-up cell
			for (int i = 0; i < _refPlayerInventory._playerInventory.Count; i++)
			{
				_inventoryUiItemList.Add(Instantiate(_inventoryCellPrefab, invContentRectT));

				_inventoryUiItemList[i].transform.GetChild(0).GetComponent<Image>().sprite = _refPlayerInventory._playerInventory[i]._itemIcon;
				_inventoryUiItemList[i].transform.GetChild(1).GetComponent<Text>().text = _refPlayerInventory._playerInventory[i]._itemName;
				//_inventoryUiItemList[i].transform.GetChild(2).GetComponent<Image>().sprite = _refPlayerInventory._playerInventory[i]._itemIcon;
			}
	}
	void CloseInventory()
	{
		foreach (GameObject cell in _inventoryUiItemList)
		{
			Destroy(cell);
		}
		_inventoryUiItemList.Clear();
	}
	#endregion

	#region Hud Functions

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

    public void SetPlayerLife()
    {
        _HudPlayerLifeText.text = $"Player life " + _refPlayer.PlayerLife;
    }
    public void AlertMessage(string message)
    {
        _HudAlertMessageText.text = message;
        _HudAlertMessageText.gameObject.SetActive(true);
        StartChrono(3, DisableMessage);
    }
    void DisableMessage()
    {
        _HudAlertMessageText.gameObject.SetActive(false);
    }

    #endregion
}
