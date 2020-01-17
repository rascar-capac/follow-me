using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
	#region References Player
    PlayerMovement _refPlayerMovement;
	PlayerInventory _refPlayerInventory;
	#endregion

	#region References UI Elements
	[Header("UI Réferences")]
	public GameObject _inventoryPanel;
    public GameObject _inventoryContent;
	public GameObject _hudPanel;
    public Text _HudTribeDistanceText;
    public Text _HudAlertMessageText;
    public Text _HudCurrentTimeText;
	public GameObject _backgroundImage;
	#endregion

	#region Prefabs UI Elements
	[Header("Assets Réferences")]
	public GameObject _inventoryCellPrefab;
	#endregion

	#region UI HUD Variables
	public List<GameObject> Menus;
	#endregion

	#region Inventory Variables
	List<GameObject> _inventoryUiItemList = new List<GameObject>();
	#endregion


	protected override void Start()
	{
		base.Start();

        _refPlayerMovement = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerMovement>();
		_refPlayerInventory = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerInventory>();

		InputManager.I.onOpenInventoryKeyPressed.AddListener(InventoryOpenClose);

		_backgroundImage.SetActive(false);
		_inventoryPanel.SetActive(false);

        OpenMenu("HudPanel");
	}

    private void Update()
    {
        SetTribeDistance();
        SetTimeOfDay();
    }

    void OpenMenu(string MenuName, bool CloseOthers = true)
    {
        if (Menus == null)
            return;

        for (int i = 0; i < Menus.Count; i++)
        {
            if (CloseOthers && MenuName != Menus[i].name)
                Menus[i].SetActive(false);
            if (Menus[i].name == MenuName)
                Menus[i].SetActive(true);
        }
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

			//PauseGame(true);
			_backgroundImage.SetActive(true);
			_inventoryPanel.SetActive(true);

			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;

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
		

		//// Inventory Open
		//if (!_inventoryPanel.activeSelf)
		//{
		//	RectTransform invContentRectT = _inventoryContent.GetComponent<RectTransform>();
		//	float invCellHeight = _inventoryCellPrefab.GetComponent<RectTransform>().sizeDelta.y;

		//	//PauseGame(true);
		//	_backgroundImage.SetActive(true);
		//	_inventoryPanel.SetActive(true);

		//	Cursor.visible = true;
		//	Cursor.lockState = CursorLockMode.None;

		//	// Set height of InventoryPanel/Scroll View/Viewport/Content
		//	invContentRectT.sizeDelta = new Vector2(invContentRectT.sizeDelta.x, invCellHeight * _refPlayerInventory._playerInventory.Count);

		//	// Instantiate all cells for each items in inventory and set-up cell
		//	for (int i = 0; i < _refPlayerInventory._playerInventory.Count; i++)
		//	{
		//		_inventoryUiItemList.Add(Instantiate(_inventoryCellPrefab, invContentRectT));

		//		_inventoryUiItemList[i].transform.GetChild(0).GetComponent<Image>().sprite = _refPlayerInventory._playerInventory[i]._itemIcon;
		//		_inventoryUiItemList[i].transform.GetChild(1).GetComponent<Text>().text = _refPlayerInventory._playerInventory[i]._itemName;
		//		//_inventoryUiItemList[i].transform.GetChild(2).GetComponent<Image>().sprite = _refPlayerInventory._playerInventory[i]._itemIcon;
		//	}
		//}
		//// Inventory Closed
		//else
		//{
		//	//PauseGame(false);
		//	_backgroundImage.SetActive(false);
		//	_inventoryPanel.SetActive(false);

		//	Cursor.visible = false;
		//	Cursor.lockState = CursorLockMode.Locked;

		//	foreach (GameObject cell in _inventoryUiItemList)
		//	{
		//		Destroy(cell);
		//	}
		//	_inventoryUiItemList.Clear();
		//}
	}
	void CloseInventory()
	{
		// Inventory Closed
		//PauseGame(false);
		_backgroundImage.SetActive(false);
		_inventoryPanel.SetActive(false);

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

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
