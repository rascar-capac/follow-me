using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMaster : Singleton<QuestMaster>
{
	[Header("All Zone Spot")]
	public List<Zone> _zonesSpot; // Liste de toute les zones de Spot, à glisser depuis la scène.

	[Header("Items to randomly spawn on Zones Spot")]
	public List<ItemData> _itemsToSpawnOnSpot; // Liste d'items à spawn au Start(), à glisser depuis le dossier Assets


	protected override void Start()
	{
		base.Start();

		RandomSpawnInZoneSpot();
	}

	// Fonction qui spawn les items dans les zones choisies aléatoirement.
	void RandomSpawnInZoneSpot()
	{
		List<int> zonesSelected = RandomDraw(_itemsToSpawnOnSpot.Count, 0, _zonesSpot.Count - 1);

		for (int i = 0; i < zonesSelected.Count; i++)
			Instantiate(_itemsToSpawnOnSpot[i]._itemBasePrefab, _zonesSpot[zonesSelected[i]]._spawnPositionItem.position, Quaternion.identity, _zonesSpot[zonesSelected[i]].transform);
	}


	// Tain.. j'ai chier pourtant c'est con au final...
	//Fonction pour renvoyer X (drawCount) nombres aléatoire toujours différents dans une liste. Pratique.  
	List<int> RandomDraw(int drawCount, int minRandomNumber, int maxRandomNumber)
	{
		List<int> randomNumbers = new List<int>();

		for (int i = 0; i < drawCount; i++)
		{
			int randomNum;
			do
				randomNum = Random.Range(minRandomNumber, maxRandomNumber);
			while (randomNumbers.Contains(randomNum));

			randomNumbers.Add(randomNum);
		}

		return randomNumbers;
	}
}
