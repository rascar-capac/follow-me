using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{

		if(other.gameObject.layer == 9)
		{
			Debug.Log(other.gameObject.layer);
		}
	}
}
