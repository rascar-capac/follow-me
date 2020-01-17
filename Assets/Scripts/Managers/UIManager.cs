using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
	GameObject _backgroundImage;
	GameObject _inventoryPanel;
	RectTransform _inventoryContent;
	public GameObject _inventoryCellPrefab;
	public float _inventoryCellHeight;
	List<GameObject> _inventoryUiItemList = new List<GameObject>();
	PlayerInventory _refPlayerInventory;

	protected override void Start()
	{
		base.Start();
		InputManager.I.onOpenInventoryKeyPressed.AddListener(OpenInventory);

		_refPlayerInventory = GameObject.FindObjectOfType<PlayerInventory>();

		_backgroundImage = GameObject.Find("BackgroundImage");
		_inventoryPanel = GameObject.Find("InventoryPanel");
		_inventoryContent = GameObject.Find("InventoryPanel/Scroll View/Viewport/Content").GetComponent<RectTransform>();
		_inventoryCellHeight = _inventoryCellPrefab.GetComponent<RectTransform>().sizeDelta.y;

		_backgroundImage.SetActive(false);
		_inventoryPanel.SetActive(false);
	}

	void OpenInventory()
	{
		// Inventory Open
		if (!_inventoryPanel.activeSelf)
		{
			//PauseGame(true);
			_backgroundImage.SetActive(true);
			_inventoryPanel.SetActive(true);

			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;

			// Set height of InventoryPanel/Scroll View/Viewport/Content
			_inventoryContent.sizeDelta = new Vector2(_inventoryContent.sizeDelta.x, _inventoryCellHeight * _refPlayerInventory._playerInventory.Count);

			// Instantiate all cells for each items in inventory and set-up cell
			for (int i = 0; i < _refPlayerInventory._playerInventory.Count; i++)
			{
				_inventoryUiItemList.Add(Instantiate(_inventoryCellPrefab, _inventoryContent));

				_inventoryUiItemList[i].transform.GetChild(0).GetComponent<Image>().sprite = _refPlayerInventory._playerInventory[i]._itemIcon;
				_inventoryUiItemList[i].transform.GetChild(1).GetComponent<Text>().text = _refPlayerInventory._playerInventory[i]._itemName;
				//_inventoryUiItemList[i].transform.GetChild(2).GetComponent<Image>().sprite = _refPlayerInventory._playerInventory[i]._itemIcon;
			}
		}
		// Inventory Closed
		else
		{
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
	}
}
