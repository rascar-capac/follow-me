using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
	[Header("Place Item Data")]
	public ScriptableObject _itemData;

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == 9)
		{
			bool itemPickUp = false;
			itemPickUp = other.GetComponent<PlayerInventory>().PickUpItem(_itemData);

			if (itemPickUp)
			{
				Debug.Log("Item pick-up");
				Destroy(this.gameObject);
			}
		}
	}
}
