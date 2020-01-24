using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : ZoneInteractable
{
    //[Header("Current player life")]
    //public float PlayerLife = 100.0f;
    //[Header("Current player oxygen")]
    //public float PlayerOxygen = 100.0f;
    //bool CriticalLife = false;
    PlayerLook playerLook;
    PlayerMovement playerMove;
	//public UnityEvent onPlayerLifeEnterCritical = new UnityEvent();
	//public UnityEvent onPlayerLifeExitCritical = new UnityEvent();

	[Header("Current player energy")]
	public float _playerCurrentEnergy = 100f;
	float _energyRatio;
	bool _playerOnCriticalEnergy = false;
	public UnityEvent onPlayerEnergyEnterCritical = new UnityEvent();
	public UnityEvent onPlayerEnergyExitCritical = new UnityEvent();

	float lastTime;


	protected override void Start()
    {
        base.Start();
        //PlayerLife = GameManager.I._data.InitialPlayerLife;
        //PlayerOxygen = GameManager.I._data.InitialPlayerOxygen;
        playerLook = CameraManager.I._MainCamera.GetComponent<PlayerLook>();
        playerMove = GetComponent<PlayerMovement>();

		_playerCurrentEnergy = GameManager.I._data._initialPlayerEnergy;
		_energyRatio = (_playerCurrentEnergy / 100) * GameManager.I._data._percentLifePlayerForCritical;

		lastTime = Time.time;
	}

    protected override void Update()
    {
        base.Update();
		//LifeCritical();
		//UpdateOxygen();
		UpdateEnergy();
		EnergyCritical();
    }

	public void UpdateEnergy()
	{
		// Lost energy in journey
		if(Time.time >= lastTime + 1 && _playerCurrentEnergy > 0 && AmbiantManager.I.CurrentDayState.State == DayState.Day)
		{
			_playerCurrentEnergy -= GameManager.I._data._energyPlayerLostPerSeconde;
			lastTime = Time.time;
		}
		// Gain energy in the night
		if(Time.time >= lastTime + 1 && _playerCurrentEnergy <= GameManager.I._data._initialPlayerEnergy && AmbiantManager.I.CurrentDayState.State == DayState.Night)
		{
			_playerCurrentEnergy += GameManager.I._data._energyPlayerGainPerSeconde;
			lastTime = Time.time;
		}
		// Checking if life is out of range
		if(_playerCurrentEnergy < 0)
			_playerCurrentEnergy = 0;
		if(_playerCurrentEnergy > GameManager.I._data._initialPlayerEnergy)
			_playerCurrentEnergy = GameManager.I._data._initialPlayerEnergy;
	}
	void EnergyCritical()
	{
		if(_playerCurrentEnergy <= _energyRatio)
		{
			_playerOnCriticalEnergy = true;
			onPlayerEnergyEnterCritical.Invoke();
		}
		else if (_playerCurrentEnergy > _energyRatio)
		{
			_playerOnCriticalEnergy = false;
			onPlayerEnergyExitCritical.Invoke();
		}
	}

    public override void ApplyZoneEffect(Zone zone)
    {
        base.ApplyZoneEffect(zone);

		GainEnergy(zone);
		//LooseLife(zone);
		//GainLife(zone);
		//ApplyZoneOxygen(zone);
    }

	public void GainEnergy(Zone zone)
	{
		_playerCurrentEnergy += zone.GainEnergySpeed * Time.deltaTime;
	}

    //public void LooseLife(Zone zone)
    //{
    //    PlayerLife -= zone.HurtSpeed * Time.deltaTime;
    //}

    //public void GainLife(Zone zone)
    //{
    //    PlayerLife += zone.GainSpeed * Time.deltaTime;
    //}

    //public void ApplyZoneOxygen(Zone zone)
    //{
    //    PlayerOxygen -= zone.LooseOxygenSpeed * Time.deltaTime;
    //    PlayerOxygen += zone.GainOxygenSpeed * Time.deltaTime;
    //}

    //public void UpdateOxygen()
    //{
    //    if (playerMove.IsRunning)
    //        PlayerOxygen -= GameManager.I._data.OxygenLossRun * Time.deltaTime;
    //    else
    //        PlayerOxygen -= GameManager.I._data.OxygenLossWalk * Time.deltaTime;
    //}

    //public void LifeCritical()
    //{
    //    float lifeRatio = PlayerLife / GameManager.I._data.InitialPlayerLife;
    //    if (lifeRatio <= GameManager.I._data.PlayerLifeThresshold && !CriticalLife)
    //    {
    //        CriticalLife = true;
    //        onPlayerLifeEnterCritical.Invoke();
    //    }
    //    else if (lifeRatio > GameManager.I._data.PlayerLifeThresshold && CriticalLife)
    //    {
    //        CriticalLife = false;
    //        onPlayerLifeExitCritical.Invoke();
    //    }
    //}
}