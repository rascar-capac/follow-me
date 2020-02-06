using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestCell : MonoBehaviour
{
	public Quest Quest;
	public GameObject PanelViewQuest;

	public void InitQuestCell()
	{
		// Set-up Button on Quest List
		transform.GetChild(0).GetComponent<Text>().text = Quest.Data.QuestTitle;
		transform.GetComponent<Button>().onClick.AddListener(ClickQuestCell);
	}

	void ClickQuestCell()
	{
		// Activate PanelView for Quest
		if (PanelViewQuest.activeSelf == false)
			PanelViewQuest.SetActive(true);

		// Update PanelView for Quest with infos of this Quest.
		PanelViewQuest.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Quest.Data.QuestTitle;
		PanelViewQuest.transform.GetChild(1).GetComponent<Text>().text = Quest.Data.QuestDescription;
	}
}
