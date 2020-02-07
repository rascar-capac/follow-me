using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public enum TribeMovementsMode
{
	Wait,
	FollowPlayer,
	GoToBeacon,
    BeSpontaneous
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
    public UnityEvent onEnergyFull = new UnityEvent();
    public Color IgnoringEmissionColor;

	[ShowInInspector][ReadOnly]
	TribeMovementsMode _TribeMovementsMode;
	#region Tribe Energy

	[Header("Current Tribe energy")]
	public float Energy = 100f;
	float CriticalEnergy;
    bool IsEnergyCritical => Energy <= CriticalEnergy;
    bool PreviousIsEnergyCritical = false;

    #endregion

	#region Tribe Events

	public UnityEvent onTribeEnergyEnterCritical = new UnityEvent();
	public UnityEvent onTribeEnergyExitCritical = new UnityEvent();

	#endregion

	NavMeshAgent _TribeNavAgent;
	Player _Player;
	PlayerMovement _PlayerMovement;
    PlayerInventory _PlayerInventory;
    Terrain _Terrain;
    Animator animator;

    [HideInInspector]
    public int DocilityScore;
    int _DocilityLevel;
    bool _IsIgnoring;
    float _IgnoranceProbability;
    int _IgnoranceDuration;
    float _IgnoranceTimer;
    float _SpontaneityProbability;
    float _SpontaneityCheckTimer;
    Color _DefaultEmissionColor;

	protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
		TribeProperties = GameManager.I._data.TribeProperties;

		_TribeNavAgent = GetComponentInParent<NavMeshAgent>();
        _Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
		_PlayerMovement = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerMovement>();
        _PlayerInventory = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerInventory>();
        _Terrain = ((GameObject)ObjectsManager.I["Terrain"]).GetComponent<Terrain>();

		Energy = GameManager.I._data.InitialTribeEnergy;
		CriticalEnergy = (Energy / 100) * GameManager.I._data.PercentEnergyTribeForCritical;

		//TribeInDefaultSpeed();
		SetNavMeshAgent();

		onTribeEnergyEnterCritical.AddListener(TribeInCriticalSpeed);
		onTribeEnergyExitCritical.AddListener(TribeInDefaultSpeed);
		InputManager.I.onTribeOrderKeyPressed.AddListener(SwitchModeFollowAndWait);
        _PlayerInventory.onItemActivated.AddListener(AddItemActivationBonus);
        AmbiantManager.I.onDayStateChanged.AddListener(AddNewDayBonus);
        _PlayerMovement.onPlayerTooFarFromTribe.AddListener(AddTooFarMalus);

        DocilityScore = GameManager.I._data.InitialDocilityScore;
        _DocilityLevel = 1;
        _IsIgnoring = false;
        _SpontaneityCheckTimer = ComputeRandomSpontaneityCheckTimer();
		ModeStopAndWait();
        Random.InitState(System.DateTime.Now.Millisecond);
        StartChrono(Random.Range(5, 10), PlayFlip);

        _DefaultEmissionColor = transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material.GetColor("_EmissionColor");
	}

    void PlayFlip()
    {
        if (Random.Range(0, 2) == 0)
            animator.Play("@loopingSide");
        else
            animator.Play("@rear");

        StartChrono(Random.Range(30, 100), PlayFlip);
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
			_TribeNavAgent.acceleration = (_TribeNavAgent.remainingDistance < TribeProperties.MinDistForDeceleration) ? TribeProperties.DecelerationForce : TribeProperties.AccelerationForce;

		if (_IsIgnoring)
		{
			_IgnoranceTimer -= Time.deltaTime;
			if (_IgnoranceTimer <= 0)
			{
                ChangeEmissive(_DefaultEmissionColor);
				_IsIgnoring = false;
			}
		}
		else
		{
			UpdateDocility();
			_SpontaneityCheckTimer -= Time.deltaTime;
			if (_SpontaneityCheckTimer <= 0)
			{
				CheckSpontaneity();
			}
		}

		UIManager.I.SetTribeDocility(_IsIgnoring);

		DebugNavMesh();

		if (IsInMeleeRangeOf())
		{
			RotateTowards();
		}


	}

	#region Tribe Movements
	// Renock Test
	void SetNavMeshAgent()
	{
		_TribeNavAgent.acceleration = TribeProperties.AccelerationForce;
		_TribeNavAgent.stoppingDistance = TribeProperties.MinDistForAcceleration;
		TribeInDefaultSpeed();
	}
	bool IsInMeleeRangeOf()
	{
		float distance = Vector3.Distance(transform.position, _Player.transform.position);
		return distance < TribeProperties.MinDistForAcceleration;
	}
	void RotateTowards()
	{
		Vector3 direction = (_Player.transform.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * TribeProperties.MaxAngularSpeed);
	}
	// Fin Test

	public void SwitchModeFollowAndWait()
	{
        if(!IsIgnoring())
        {
            if (_PlayerMovement.TribeDistance > GameManager.I._data.MaximumDistanceOfTribe)
            {
                return;
            }
            if (_TribeMovementsMode == TribeMovementsMode.FollowPlayer)
                ModeStopAndWait();
            else
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
        NavMeshHit hit;
        Vector3 playerPositionOnNavMesh;
        if(NavMesh.SamplePosition(playerPosition, out hit, 500, NavMesh.AllAreas))
        {
            playerPositionOnNavMesh = hit.position;
            if (_TribeNavAgent.destination != playerPositionOnNavMesh)
                _TribeNavAgent.SetDestination(playerPositionOnNavMesh);
            _TribeNavAgent.SetDestination(Vector3.Slerp(transform.position, playerPositionOnNavMesh, 5));
        }
	}

    public void ModeGoToBeacon(Vector3 destination)
	{
        if(!IsIgnoring())
        {
            ModeStopAndWait();
            _TribeMovementsMode = TribeMovementsMode.GoToBeacon;
            NavMeshHit hit;
            if(NavMesh.SamplePosition(destination, out hit, 500, NavMesh.AllAreas))
            {
                _TribeNavAgent.SetDestination(hit.position);
            }
        }
	}

    public void ModeBeSpontaneous(Vector2 destination)
    {
        ModeStopAndWait();
        _TribeMovementsMode = TribeMovementsMode.BeSpontaneous;
        NavMeshHit hit;
        if(NavMesh.SamplePosition(new Vector3(destination.x, 0, destination.y), out hit, 500, NavMesh.AllAreas))
        {
            _TribeNavAgent.SetDestination(hit.position);
        }
    }

    public void UpdateDocility()
    {
        switch(ComputeDocilityLevel())
        {
            case 0:
                _IgnoranceProbability = GameManager.I._data.Level0Params.IgnoranceProbability;
                _IgnoranceDuration = GameManager.I._data.Level0Params.IgnoranceDuration;
                _SpontaneityProbability = GameManager.I._data.Level0Params.SpontaneityProbability;
                break;
            case 1:
                _IgnoranceProbability = GameManager.I._data.Level1Params.IgnoranceProbability;
                _IgnoranceDuration = GameManager.I._data.Level1Params.IgnoranceDuration;
                _SpontaneityProbability = GameManager.I._data.Level1Params.SpontaneityProbability;
                break;
            case 2:
                _IgnoranceProbability = GameManager.I._data.Level2Params.IgnoranceProbability;
                _IgnoranceDuration = GameManager.I._data.Level2Params.IgnoranceDuration;
                _SpontaneityProbability = GameManager.I._data.Level2Params.SpontaneityProbability;
                break;
            case 3:
                _IgnoranceProbability = GameManager.I._data.Level3Params.IgnoranceProbability;
                _IgnoranceDuration = GameManager.I._data.Level3Params.IgnoranceDuration;
                _SpontaneityProbability = GameManager.I._data.Level3Params.SpontaneityProbability;
                break;
            case 4:
                _IgnoranceProbability = GameManager.I._data.Level4Params.IgnoranceProbability;
                _IgnoranceDuration = GameManager.I._data.Level4Params.IgnoranceDuration;
                _SpontaneityProbability = GameManager.I._data.Level4Params.SpontaneityProbability;
                break;
        }
    }

    public void AddItemActivationBonus(Item item)
    {
        DocilityScore += GameManager.I._data.ItemActivationBonus;
    }

    public void AddNewDayBonus(DayStatesProperties currentDayState, DayStatesProperties nextDayState)
    {
        if(currentDayState.State == DayState.Day)
        {
            DocilityScore += GameManager.I._data.NewDayBonus;
        }
    }

    public void AddTooFarMalus()
    {
        DocilityScore -= GameManager.I._data.TooFarFromTribeMalus;
    }

    // public void AddObstacleDocilityMalus() {}

    public int ComputeDocilityLevel()
    {
        int bonusMalus = 0;
        bonusMalus -= GameManager.I._data.InitialTribeEnergy - Mathf.RoundToInt(Energy);
        if(IsEnergyCritical)
        {
            bonusMalus -= GameManager.I._data.CriticalEnergyMalus;
        }
        if(AmbiantManager.I.IsNight)
        {
            bonusMalus += GameManager.I._data.NightBonus;
        }

        int finalScore = DocilityScore + bonusMalus;
        if(finalScore <         0) { _DocilityLevel = 0; }
        else if(finalScore <  100) { _DocilityLevel = 1; }
        else if(finalScore <  200) { _DocilityLevel = 2; }
        else if(finalScore <  300) { _DocilityLevel = 3; }
        else if(finalScore >= 300) { _DocilityLevel = 4; }
        return _DocilityLevel;
    }

    public bool IsIgnoring()
    {
        if(!_IsIgnoring && Random.Range(0, 1f) < _IgnoranceProbability)
        {
            ChangeEmissive(IgnoringEmissionColor);
            _IsIgnoring = true;
            _IgnoranceTimer = _IgnoranceDuration;
        }
        return _IsIgnoring;
    }

    public bool CheckSpontaneity()
    {
        if(Random.Range(0, 1f) < _SpontaneityProbability)
        {
            _SpontaneityCheckTimer = ComputeRandomSpontaneityCheckTimer();
            Bounds terrainDimensions = _Terrain.terrainData.bounds;
            Vector2 randomDestination = new Vector2(terrainDimensions.center.x, terrainDimensions.center.z)
                    + Random.insideUnitCircle * (terrainDimensions.size / 2);
            ModeBeSpontaneous(randomDestination);
            ChangeEmissive(IgnoringEmissionColor);
            _IsIgnoring = true;
            _IgnoranceTimer = _IgnoranceDuration;
            return true;
        }
        return false;
    }

    public int ComputeRandomSpontaneityCheckTimer()
    {
        return Random.Range(GameManager.I._data.SpontaneityCheckTimerMinDuration, GameManager.I._data.SpontaneityCheckTimerMaxDuration);
    }

    public void ChangeEmissive(Color emissionColor)
    {
        transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetColor("_EmissionColor", emissionColor);
    }

    #endregion


    #region Tribe energy
    bool EventCalled = false;
	public void UpdateEnergy()
	{
		// Lost energy
		if (!InZones.Exists(z => z.GainEnergySpeed > 0))
        {
            if(_TribeMovementsMode == TribeMovementsMode.Wait)
            {
                Energy -= GameManager.I._data.TribeEnergyLossWaiting * Time.deltaTime;
            }
            else
            {
			    Energy -= GameManager.I._data.TribeEnergyLossMoving * Time.deltaTime;
            }
        }

        // Clamp if life is out of range
        Energy = Mathf.Clamp(Energy, 0f, GameManager.I._data.InitialTribeEnergy);
        if (Energy == GameManager.I._data.InitialTribeEnergy && !EventCalled)
        {
            EventCalled = true;
            onEnergyFull.Invoke();
        }
        else if (Energy != GameManager.I._data.InitialTribeEnergy)
            EventCalled = false;

        if(IsEnergyCritical)
        {
            animator.SetFloat("Blend", 1);
        }
        else
        {
            animator.SetFloat("Blend", 1 - (Energy / GameManager.I._data.InitialTribeEnergy * 2));
        }
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

	void DebugNavMesh()
	{
		//Debug.Log("Speed = " + _TribeNavAgent.speed);
		//Debug.Log("Acceleration = " + _TribeNavAgent.acceleration);
		//Debug.Log("Angular speed = " + _TribeNavAgent.angularSpeed);
		//Debug.Log("Velocity = " + _TribeNavAgent.velocity);
		//Debug.Log("Desired Velocity = " + _TribeNavAgent.desiredVelocity);
		//Debug.Log("Destination = " + _TribeNavAgent.destination);
		//Debug.Log("Remaining Distance = " + _TribeNavAgent.remainingDistance);
		//Debug.Log("Stopping Distance = " + _TribeNavAgent.stoppingDistance);
		//Debug.Log("Steering Target = " + _TribeNavAgent.steeringTarget);
		//Debug.Log("Next Position = " + _TribeNavAgent.nextPosition);
		//Debug.Log("Update Position = " + _TribeNavAgent.updatePosition);
		//Debug.Log("Update Rotation = " + _TribeNavAgent.updateRotation);
		//Debug.Log("Auto Braking = " + _TribeNavAgent.autoBraking);
		//Debug.Log("Auro Repath = " + _TribeNavAgent.autoRepath);
		//Debug.Log("Has Path = " + _TribeNavAgent.hasPath);
		//Debug.Log("Is Stopped = " + _TribeNavAgent.isStopped);
		//Debug.Log("Path Status = " + _TribeNavAgent.path.status);
		//Debug.Log("Path Status = " + _TribeNavAgent.pathStatus);
		//Debug.Log("PathPending = " + _TribeNavAgent.pathPending);
		//Debug.Log("**********************************************");
		//Debug.Log("**********************************************");
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, TribeProperties.MinDistForAcceleration);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, TribeProperties.MinDistForDeceleration);
	}

	#endregion
}
