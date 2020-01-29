using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public enum TribeMovementsMode
{
	GoToBeacon,
	FollowPlayer,
	Wait
}

[System.Serializable]
public struct TribeProperties
{
	public float MaxSpeed;
	public float MaxAngularSpeed;
	public float AccelerationForce;
	public float DecelerationForce;
	public float MinDistForAcceleration;
	public float MinDistForDeceleration;
	public float CriticalSpeedMultiplicator;
	//[TabGroup("Tribe")]
	//[Tooltip("Critical (energy low) speed multiplicator of Tribe (speed * multiplicator = new speed)")]

}

public class Tribe : ZoneInteractable
{
	public TribeProperties TribeProperties;

	[ShowInInspector][ReadOnly]
	TribeMovementsMode _TribeMovementsMode;
	#region Tribe Energy

	[Header("Current Tribe energy")]
	public float Energy = 100f;
	float CriticalEnergy;
    bool IsEnergyCritical => Energy <= CriticalEnergy;
    bool PreviousIsEnergyCritical = false;

	#endregion

    #region Desobedience System

    [Header("Desobedience system")]
    [Range(0, 1f)] public float level1IgnoranceProbability = 0.5f;
    public int level1IgnoranceDuration = 3;
    public float level1DesobedienceProbability = 0.1f;
    public int desobedienceMinDuration = 30;
    public int desobedienceMaxDuration = 120;

    #endregion

	#region Tribe Events

	public UnityEvent onTribeEnergyEnterCritical = new UnityEvent();
	public UnityEvent onTribeEnergyExitCritical = new UnityEvent();

	#endregion

	NavMeshAgent _TribeNavAgent;
	Player _Player;
	PlayerMovement _PlayerMovement;
    PlayerInventory _PlayerInventory;

    int _DocilityScore;
    bool _IsIgnoring;
    float _IgnoranceProbability;
    int _IgnoranceDuration;
    float _IgnoranceTimer;
    float _DisobedienceProbability;
    int _DisobedienceTimer;

	protected override void Start()
    {
        base.Start();

		TribeProperties = GameManager.I._data.TribeProperties;

		_TribeNavAgent = GetComponentInParent<NavMeshAgent>();
		_Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
		_PlayerMovement = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerMovement>();
        _PlayerInventory = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerInventory>();

		Energy = GameManager.I._data.InitialTribeEnergy;
		CriticalEnergy = (Energy / 100) * GameManager.I._data.PercentEnergyTribeForCritical;

		TribeInDefaultSpeed();

		onTribeEnergyEnterCritical.AddListener(TribeInCriticalSpeed);
		onTribeEnergyExitCritical.AddListener(TribeInDefaultSpeed);
		InputManager.I.onTribeOrderKeyPressed.AddListener(SwitchModeFollowAndWait);
        _PlayerInventory.onItemActivated.AddListener(AddItemActivationDocilityBonus);
        AmbiantManager.I.onDayStateChanged.AddListener(AddNewDayDocilityBonus);

        _DocilityScore = 50;
        _IsIgnoring = false;
		ModeStopAndWait();
	}

	protected override void Update()
    {
        base.Update();

		UpdateEnergy();
		EnergyCritical();

        // à changer quand l’énergie du convoi sera affichée autrement que par du texte
        UIManager.I.SetTribeEnergy();

		// Acceleration and deceleration controls of Tribe
		if (_TribeNavAgent.hasPath)
			_TribeNavAgent.acceleration = (_TribeNavAgent.remainingDistance < TribeProperties.MinDistForDeceleration) ?     TribeProperties.DecelerationForce : TribeProperties.AccelerationForce;


        // if (_DisobedienceTimer -=
        if (_IsIgnoring)
        {
            _IgnoranceTimer -= Time.deltaTime;
            if (_IgnoranceTimer <= 0)
            {
                _IsIgnoring = false;
            }
        }
        else
        {
            UpdateDocility();
        }
	}

	#region Tribe Movements

	public void SwitchModeFollowAndWait()
	{
        if(!_IsIgnoring)
        {
            if (_TribeMovementsMode == TribeMovementsMode.FollowPlayer)
                ModeStopAndWait();
            else if (_TribeMovementsMode == TribeMovementsMode.Wait || _TribeMovementsMode == TribeMovementsMode.GoToBeacon)
                ModeFollowPlayer();
        }
	}

	public void ModeStopAndWait()
	{
        _TribeMovementsMode = TribeMovementsMode.Wait;
        _PlayerMovement.onPlayerHasMoved.RemoveListener(TribeFollowPlayer);
        _TribeNavAgent.ResetPath();
	}

	public void ModeFollowPlayer()
	{
        _TribeMovementsMode = TribeMovementsMode.FollowPlayer;
        _PlayerMovement.onPlayerHasMoved.AddListener(TribeFollowPlayer);
	}

	void TribeFollowPlayer(Vector3 playerPosition)
	{
        Vector3 playerPositionXZ = new Vector3(playerPosition.x, 0, playerPosition.z);
        if (_TribeNavAgent.destination != playerPositionXZ)
            _TribeNavAgent.SetDestination(playerPositionXZ);
	}

    public void ModeGoToBeacon(Vector3 destination)
	{
        if(!_IsIgnoring)
        {
            ModeStopAndWait();

            _TribeMovementsMode = TribeMovementsMode.GoToBeacon;
            _TribeNavAgent.SetDestination(new Vector3(destination.x, 0, destination.z));
        }
	}

    public void UpdateDocility()
    {
        switch(ComputeDocilityLevel())
        {
            case 0:
                _IgnoranceProbability = .6f;
                _IgnoranceDuration = 30;
                _DisobedienceProbability = .5f;
                break;
            case 1:
                _IgnoranceProbability = .5f;
                _IgnoranceDuration = 25;
                _DisobedienceProbability = .25f;
                break;
            case 2:
                _IgnoranceProbability = .4f;
                _IgnoranceDuration = 20;
                _DisobedienceProbability = .2f;
                break;
            case 3:
                _IgnoranceProbability = .3f;
                _IgnoranceDuration = 15;
                _DisobedienceProbability = .15f;
                break;
            case 4:
                _IgnoranceProbability = .2f;
                _IgnoranceDuration = 10;
                _DisobedienceProbability = .8f;
                break;
            case 5:
                _IgnoranceProbability = .1f;
                _IgnoranceDuration = 5;
                _DisobedienceProbability = .5f;
                break;
            case 6:
                _IgnoranceProbability = 0;
                _IgnoranceDuration = 0;
                _DisobedienceProbability = .3f;
                break;
        }



        UIManager.I.ShowTribeDocility(true);
        UIManager.I.SetTribeDocility(_DocilityScore);
    }

    public void AddItemActivationDocilityBonus(Item item)
    {
        _DocilityScore += 100;
    }

    // public void AddObstacleDocilityMalus() {}

    public void AddNewDayDocilityBonus(DayStatesProperties currentDayState, DayStatesProperties nextDayState)
    {
        if(currentDayState.State == DayState.Day)
        {
            _DocilityScore += 10;
        }
    }

    public int ComputeDocilityLevel()
    {
        int bonusMalus = 0;
        bonusMalus -= 100 - Mathf.RoundToInt(Energy);
        if(IsEnergyCritical)
        {
            bonusMalus -= 50;
        }
        if(AmbiantManager.I.IsNight)
        {
            bonusMalus += 50;
        }

        int finalScore = _DocilityScore + bonusMalus;
        if(finalScore <    0) { return 0; }
        if(finalScore <   50) { return 1; }
        if(finalScore <  100) { return 2; }
        if(finalScore <  150) { return 3; }
        if(finalScore <  200) { return 4; }
        if(finalScore <  300) { return 5; }
        if(finalScore >= 300) { return 6; }
        return 0;
    }

    public void IsIgnoring()
    {
        if(Random.Range(0, 1f) < _IgnoranceProbability)
        {
            _IsIgnoring = true;
            _IgnoranceTimer = _IgnoranceDuration;
        }
    }

	#endregion

	#region Tribe energy

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

	#endregion


	#region Tribe speed

	void TribeInDefaultSpeed()
	{
		//_TribeNavAgent.speed = GameManager.I._data.InitialSpeedTribe;
		//_TribeNavAgent.angularSpeed = GameManager.I._data.InitialSpeedRotationTribe;
		_TribeNavAgent.speed = GameManager.I._data.TribeProperties.MaxSpeed;
		_TribeNavAgent.angularSpeed = GameManager.I._data.TribeProperties.MaxAngularSpeed;
	}

	void TribeInCriticalSpeed()
	{
		//_TribeNavAgent.speed = GameManager.I._data.InitialSpeedTribe * GameManager.I._data.CriticalSpeedTribeMultiplicator;
		_TribeNavAgent.speed = GameManager.I._data.TribeProperties.MaxSpeed * GameManager.I._data.TribeProperties.CriticalSpeedMultiplicator;
	}

	#endregion


	#region Tribe in zone

	public override void ApplyZoneEffect(Zone zone)
    {
        base.ApplyZoneEffect(zone);
        GainTribeEnergy(zone);
        LooseEnergy(zone);
    }

    public void GainTribeEnergy(Zone zone)
	{
		Energy += zone.GainEnergySpeed * Time.deltaTime;
        Energy = Mathf.Clamp(Energy, 0f, GameManager.I._data.InitialTribeEnergy);
    }

    public void LooseEnergy(Zone zone)
    {
        Energy += zone.LooseEnergySpeed * Time.deltaTime;
        Energy = Mathf.Clamp(Energy, 0f, GameManager.I._data.InitialTribeEnergy);
    }

	#endregion


	#region Debug NavMesh

	//NavMeshPath path = new NavMeshPath();
	//_TribeNavAgent.CalculatePath(new Vector3(_Player.transform.position.x, 0, _Player.transform.position.z), path);
	//_TribeNavAgent.SetPath(path);

	//void DebugNavMesh()
	//{
	//	Debug.Log("Speed = " + _TribeNavAgent.speed);
	//	Debug.Log("Acceleration = " + _TribeNavAgent.acceleration);
	//	Debug.Log("Angular speed = " + _TribeNavAgent.angularSpeed);
	//	Debug.Log("Velocity = " + _TribeNavAgent.velocity);
	//	Debug.Log("Desired Velocity = " + _TribeNavAgent.desiredVelocity);
	//	Debug.Log("Destination = " + _TribeNavAgent.destination);
	//	Debug.Log("Remaining Distance = " + _TribeNavAgent.remainingDistance);
	//	Debug.Log("Stopping Distance = " + _TribeNavAgent.stoppingDistance);
	//	Debug.Log("Steering Target = " + _TribeNavAgent.steeringTarget);
	//	Debug.Log("Next Position = " + _TribeNavAgent.nextPosition);
	//	Debug.Log("Update Position = " + _TribeNavAgent.updatePosition);
	//	Debug.Log("Update Rotation = " + _TribeNavAgent.updateRotation);
	//	Debug.Log("Auto Braking = " + _TribeNavAgent.autoBraking);
	//	Debug.Log("Auro Repath = " + _TribeNavAgent.autoRepath);
	//	Debug.Log("Has Path = " + _TribeNavAgent.hasPath);
	//	Debug.Log("Is Stopped = " + _TribeNavAgent.isStopped);
	//	Debug.Log("Path Status = " + _TribeNavAgent.path.status);
	//	Debug.Log("Path Status = " + _TribeNavAgent.pathStatus);
	//	Debug.Log("PathPending = " + _TribeNavAgent.pathPending);
	//	Debug.Log("**********************************************");
	//	Debug.Log("**********************************************");
	//}

	#endregion
}
