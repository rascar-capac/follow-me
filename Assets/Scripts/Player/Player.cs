using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : ZoneInteractable
{
    PlayerLook playerLook;
    PlayerInventory playerInventory;

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
        playerInventory = GetComponent<PlayerInventory>();
        PlacedBeacon.Clear();

        Energy = GameManager.I._data.InitialPlayerEnergy;
	}

    protected override void Update()
    {
        base.Update();
        //UpdateEnergy();
        //EnergyCritical();
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
		//GainEnergy(zone);
  //      LooseEnergy(zone);
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



}