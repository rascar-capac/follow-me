using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : ZoneInteractable
{
    PlayerLook playerLook;
    PlayerMovement playerMove;
    PlayerInventory playerInventory;

    [Header("List of the player quests data scriptable object")]
    public List<QuestData> QuestData;
    [HideInInspector]
    public List<Quest> Quests;

    [Header("Current player energy")]
	public float Energy = 100f;
    bool IsEnergyNull => Energy == 0;
    bool PreviousEnergyNull = false;

    public List<Item> PlacedBeacon = new List<Item>();
    public int CurrentBeaconIndex = -1;
    public Item CurrentActiveBeacon => (CurrentBeaconIndex >= 0 && CurrentBeaconIndex < PlacedBeacon.Count) ? PlacedBeacon[CurrentBeaconIndex] : null;

    public UnityEvent onPlayerEnergyNullEnter = new UnityEvent();
    public UnityEvent onPlayerEnergyNullExit = new UnityEvent();

	protected override void Start()
    {
        base.Start();
        playerLook = CameraManager.I._MainCamera.GetComponent<PlayerLook>();
        playerMove = GetComponent<PlayerMovement>();
        playerInventory = GetComponent<PlayerInventory>();
        PlacedBeacon.Clear();

        Energy = GameManager.I._data.InitialPlayerEnergy;
        if (QuestData != null)
        {
            Quests = new List<Quest>();
            foreach (QuestData data in QuestData)
            {
                LoadQuest(data);
            }
        }

	}

    protected override void Update()
    {
        base.Update();
		UpdateEnergy();
		EnergyCritical();
        UpdateRunGauge();
    }


    public void UpdateRunGauge()
    {
        if (playerMove.IsRunning)
            playerMove.PlayerRunGauge -= GameManager.I._data.PlayerRunCostBySecond * Time.deltaTime;
        else
            playerMove.PlayerRunGauge += GameManager.I._data.PlayerRunGainBySecond * Time.deltaTime;

        playerMove.PlayerRunGauge = Mathf.Clamp(playerMove.PlayerRunGauge, 0, GameManager.I._data.PlayerRunGaugeMax);
    }

    public void UpdateEnergy()
	{
        // Lost energy in journey
        if (AmbiantManager.I.IsDay && !InZones.Exists(z => z.GainEnergySpeed > 0))
            Energy -= GameManager.I._data.EnergyPlayerLostPerSecond * Time.deltaTime;

        // Gain energy in the night
        if (AmbiantManager.I.IsNight)
            Energy += GameManager.I._data.EnergyPlayerGainPerSecond * Time.deltaTime;

        // Checking if life is out of range
        Energy = Mathf.Clamp(Energy, 0, GameManager.I._data.InitialPlayerEnergy);
	}
	void EnergyCritical()
	{
		if(IsEnergyNull && !PreviousEnergyNull)
		{
            PreviousEnergyNull = true;
			onPlayerEnergyNullEnter.Invoke();
		}
		else if (!IsEnergyNull && PreviousEnergyNull)
		{
            PreviousEnergyNull = false;
            onPlayerEnergyNullExit.Invoke();
		}
	}

    public override void ApplyZoneEffect(Zone zone)
    {
        base.ApplyZoneEffect(zone);
		GainEnergy(zone);
        LooseEnergy(zone);
    }

	public void GainEnergy(Zone zone)
	{
		Energy += zone.GainEnergySpeed * Time.deltaTime;
        Energy = Mathf.Clamp(Energy, 0f, GameManager.I._data.InitialPlayerEnergy);
    }
    public void LooseEnergy(Zone zone)
    {
        Energy += zone.LooseEnergySpeed * Time.deltaTime;
        Energy = Mathf.Clamp(Energy, 0f, GameManager.I._data.InitialPlayerEnergy);
    }

    public void LoadQuest(QuestData data)
    {
        Quest quest = new Quest(this, data);
        Quests.Add(quest);
        quest.onQuestCompleted.AddListener(QuestCompleted);
    }

    void QuestCompleted(Quest quest)
    {
        List<QuestRewardData> rewards = quest.Data.QuestRewardData;
        if (rewards == null || rewards.Count <= 0)
            return;

        for (int i = 0; i < rewards.Count; i++)
        {
            if (rewards[i].Items != null)
            {
                foreach (ItemData it in rewards[i].Items)
                {
                    playerInventory.AddItemToInventory(it);
                }
            }
            if (rewards[i].Quests != null)
            {
                foreach (QuestData questdata in rewards[i].Quests)
                {
                    LoadQuest(questdata);
                }
            }
            if (rewards[i].Zones != null)
            {
                foreach (ZoneApparition zone in rewards[i].Zones)
                {
                    GameObject newZone = Instantiate(zone.Zone.gameObject);
                    newZone.transform.position = zone.PositionToAppear.transform.position;
                    newZone.transform.rotation = Quaternion.identity;
                }
            }
        }
    }


}