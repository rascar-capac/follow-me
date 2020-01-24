using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Tribe : ZoneInteractable
{
	[Header("Current Tribe energy")]
	public float _tribeCurrentEnergy = 100f;
	float _energyRatio;
	bool _tribeOnCriticalEnergy = false;
	public UnityEvent onTribeEnergyEnterCritical = new UnityEvent();
	public UnityEvent onTribeEnergyExitCritical = new UnityEvent();

	NavMeshAgent _tribeNavAgent;

	float lastTime;

	//[Header("Current Tribe Life.")]
	//public float Life = 100.0f;
	//[Header("Current Tribe Fuel.")]
	//public float Fuel = 100.0f;

	//public UnityEvent onTribeLifeEnterCritical = new UnityEvent();
	//public UnityEvent onTribeLifeExitCritical = new UnityEvent();

	//bool TribeLifeCritical = false;

	protected override void Start()
    {
        base.Start();

		_tribeCurrentEnergy = GameManager.I._data._initialTribeEnergy;
		_energyRatio = (_tribeCurrentEnergy / 100) * GameManager.I._data._percentLifeTribeForCritical;

		_tribeNavAgent = GetComponentInParent<NavMeshAgent>();
		TribeInDefaultSpeed();
		onTribeEnergyEnterCritical.AddListener(TribeInCriticalSpeed);
		onTribeEnergyExitCritical.AddListener(TribeInDefaultSpeed);

		lastTime = Time.time;
		//Life = GameManager.I._data.InitialTribeLife;
		//Fuel = GameManager.I._data.InitialTribeFuel;
	}

	protected override void Update()
    {
        base.Update();

		UpdateEnergy();
		EnergyCritical();

		//CriticalLife();
		//UpdateFuel();
	}

	public void UpdateEnergy()
	{
		// Lost energy in journey
		if (Time.time >= lastTime + 1 && _tribeCurrentEnergy > 0 && AmbiantManager.I.CurrentDayState.State == DayState.Day)
		{
			_tribeCurrentEnergy -= GameManager.I._data._energyTribeLostPerSeconde;
			lastTime = Time.time;
		}
		// Gain energy in the night
		if (Time.time >= lastTime + 1 && _tribeCurrentEnergy <= GameManager.I._data._initialTribeEnergy && AmbiantManager.I.CurrentDayState.State == DayState.Night)
		{
			_tribeCurrentEnergy += GameManager.I._data._energyTribeGainPerSeconde;
			lastTime = Time.time;
		}
		// Checking if life is out of range
		if (_tribeCurrentEnergy < 0)
			_tribeCurrentEnergy = 0;
		if (_tribeCurrentEnergy > GameManager.I._data._initialTribeEnergy)
			_tribeCurrentEnergy = GameManager.I._data._initialTribeEnergy;
	}
	void EnergyCritical()
	{
		if (_tribeCurrentEnergy <= _energyRatio)
		{
			_tribeOnCriticalEnergy = true;
			onTribeEnergyEnterCritical.Invoke();
		}
		else if (_tribeCurrentEnergy > _energyRatio)
		{
			_tribeOnCriticalEnergy = false;
			onTribeEnergyExitCritical.Invoke();
		}
	}
	void TribeInDefaultSpeed()
	{
		_tribeNavAgent.speed = GameManager.I._data._defaultSpeedTribe;
	}
	void TribeInCriticalSpeed()
	{
		_tribeNavAgent.speed = GameManager.I._data._criticalSpeedTribe;
	}

	//public void UpdateFuel()
	//{
	//	Fuel -= GameManager.I._data.FuelLossSpeed * Time.deltaTime;
	//}

	//public void CriticalLife()
	//{
	//	float TribeLifeRatio = Life / GameManager.I._data.InitialTribeLife;

	//	if (TribeLifeRatio <= GameManager.I._data.TribeLifeThresshold && !TribeLifeCritical)
	//	{
	//		TribeLifeCritical = true;
	//		onTribeLifeEnterCritical.Invoke();
	//	}
	//	else if (TribeLifeRatio > GameManager.I._data.TribeLifeThresshold && TribeLifeCritical)
	//	{
	//		TribeEnergyCritical = false;
	//		onTribeEnergyExitCritical.Invoke();
	//	}
	//}

	//public void CriticalEnergy()
	//{
	//	float TribeEnergyRatio = _energy / GameManager.I._data.InitialTribeEnergy;

	//	if (TribeEnergyRatio <= GameManager.I._data.TribeEnergyThresshold && !TribeEnergyCritical)
	//	{
	//		TribeEnergyCritical = true;
	//		onTribeEnergyEnterCritical.Invoke();
	//	}
	//	else if (TribeEnergyRatio > GameManager.I._data.TribeEnergyThresshold && TribeEnergyCritical)
	//	{
	//		TribeEnergyCritical = false;
	//		onTribeEnergyExitCritical.Invoke();
	//	}
	//}

	public override void ApplyZoneEffect(Zone zone)
    {
        base.ApplyZoneEffect(zone);

		GainTribeEnergy(zone);
        //LooseLife(zone);
        //GainLife(zone);
        //FuelZone(zone);
    }

	public void GainTribeEnergy(Zone zone)
	{
		_tribeCurrentEnergy += zone.GainEnergySpeed * Time.deltaTime;
	}

	//public void FuelZone(Zone zone)
	//{
	//    Fuel -= zone.LooseFuelSpeed * Time.deltaTime;
	//    Fuel += zone.GainFuelSpeed * Time.deltaTime;
	//}

	//public void LooseLife(Zone zone)
	//{
	//    Life -= zone.HurtSpeed * Time.deltaTime;
	//}

	//public void GainLife(Zone zone)
	//{
	//    Life += zone.GainSpeed * Time.deltaTime;
	//}
}
