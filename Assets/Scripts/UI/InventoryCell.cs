using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
	public ItemData ItemData;
	public GameObject PanelViewInventory;
	public GameObject ItemViewer;


	public void InitInventoryCell()
	{
		// Set-up Button on Inventory List.
		transform.GetChild(0).GetComponent<Image>().sprite = ItemData.Icon;
		transform.GetChild(1).GetComponent<Text>().text = ItemData.Name;
		transform.GetComponent<Button>().onClick.AddListener(ClickInventoryCell);
	}

	public void ClickInventoryCell()
	{
		// Activate PanelView for Item.
		if (PanelViewInventory.activeSelf == false)
			PanelViewInventory.SetActive(true);

		// Update PanelView for Item with infos of this Item.
		PanelViewInventory.transform.GetChild(0).GetComponent<Text>().text = ItemData.Name;
		PanelViewInventory.transform.GetChild(1).GetComponent<Text>().text = ItemData.Description;

		// Activate ItemViewer and Set-up with ItemData.
		ItemViewer.SetActive(true);
		ItemViewer.GetComponent<MeshFilter>().mesh = ItemData.MeshBase;
		ItemViewer.GetComponent<MeshRenderer>().material = ItemData.MaterialBase;
	}
}
