using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Pedestal2 : MonoBehaviour
{
	// Events?
	public bool IsActivated = false;
	public Transform CurrentStonePosition;
	public ItemData CurrentStone;
	public ItemData NeededStone;

	public void CheckStonesMatching()
	{
		if (CurrentStone == NeededStone)
			IsActivated = true;
		else
			IsActivated = false;
	}

}
