using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Tribe : ZoneInteractable
{
	[Header("Current Tribe energy")]
	public float Energy = 100f;

	float CriticalEnergy;
    bool IsEnergyCritical => Energy <= CriticalEnergy;
    bool PreviousIsEnergyCritical = false;

    public UnityEvent onTribeEnergyEnterCritical = new UnityEvent();
	public UnityEvent onTribeEnergyExitCritical = new UnityEvent();

	NavMeshAgent _tribeNavAgent;

	protected override void Start()
    {
        base.Start();

		Energy = GameManager.I._data.InitialTribeEnergy;
		CriticalEnergy = (Energy / 100) * GameManager.I._data.PercentEnergyTribeForCritical;

		_tribeNavAgent = GetComponentInParent<NavMeshAgent>();
		TribeInDefaultSpeed();
		onTribeEnergyEnterCritical.AddListener(TribeInCriticalSpeed);
		onTribeEnergyExitCritical.AddListener(TribeInDefaultSpeed);
	}

	protected override void Update()
    {
        base.Update();

		UpdateEnergy();
		EnergyCritical();
	}

	public void UpdateEnergy()
	{

		// Lost energy in journey
		if (AmbiantManager.I.IsDay && !InZones.Exists(z => z.GainEnergySpeed > 0))
			Energy -= GameManager.I._data.EnergyTribeLostPerSecond * Time.deltaTime;
		
        // Gain energy in the night
		if (AmbiantManager.I.IsNight)
			Energy += GameManager.I._data.EnergyTribeGainPerSecond * Time.deltaTime;

        // Clamp if life is out of range
        Energy = Mathf.Clamp(Energy, 0f, GameManager.I._data.InitialTribeEnergy);
	}
	void EnergyCritical()
	{
        bool PreviousIsEnergyCritical = false;
		if (IsEnergyCritical && !PreviousIsEnergyCritical)
		{
            PreviousIsEnergyCritical = true;
			onTribeEnergyEnterCritical.Invoke();
		}
		else if (!IsEnergyCritical && PreviousIsEnergyCritical)
		{
            PreviousIsEnergyCritical = false;
			onTribeEnergyExitCritical.Invoke();
		}
	}
	void TribeInDefaultSpeed()
	{
		_tribeNavAgent.speed = GameManager.I._data.InitialSpeedTribe;
        _tribeNavAgent.angularSpeed = GameManager.I._data.InitialSpeedRotationTribe;
    }

	void TribeInCriticalSpeed()
	{
		_tribeNavAgent.speed = GameManager.I._data.InitialSpeedTribe * GameManager.I._data.CriticalSpeedTribeMultiplicator;
	}

	public override void ApplyZoneEffect(Zone zone)
    {
        base.ApplyZoneEffect(zone);
		GainTribeEnergy(zone);
    }

	public void GainTribeEnergy(Zone zone)
	{
		Energy += zone.GainEnergySpeed * Time.deltaTime;
        Energy = Mathf.Clamp(Energy, 0f, GameManager.I._data.InitialTribeEnergy);
    }


}
