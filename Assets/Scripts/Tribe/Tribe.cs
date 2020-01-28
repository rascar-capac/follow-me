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

public class Tribe : ZoneInteractable
{
	#region Tribe Modes

	[ShowInInspector][ReadOnly]
	TribeMovementsMode _TribeMovementsMode;

	#endregion

	#region Tribe Movements

	public float AccelerationForce;
	public float DecelerationForce;
	public float CloseEnoughMeters;

	#endregion

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


	protected override void Start()
    {
        base.Start();

		_TribeNavAgent = GetComponentInParent<NavMeshAgent>();
		_Player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();

		Energy = GameManager.I._data.InitialTribeEnergy;
		CriticalEnergy = (Energy / 100) * GameManager.I._data.PercentEnergyTribeForCritical;

		TribeInDefaultSpeed();

		onTribeEnergyEnterCritical.AddListener(TribeInCriticalSpeed);
		onTribeEnergyExitCritical.AddListener(TribeInDefaultSpeed);
		InputManager.I.onTribeOrderKeyPressed.AddListener(SwitchModeFollowAndWait);

		ModeStopAndWait();
	}

	protected override void Update()
    {
        base.Update();

		UpdateEnergy();
		EnergyCritical();

		// Acceleration and deceleration controls of Tribe
		if (_TribeNavAgent.hasPath)
			_TribeNavAgent.acceleration = (_TribeNavAgent.remainingDistance < CloseEnoughMeters) ? DecelerationForce : AccelerationForce;
	}

	#region Tribe Movements

	public void SwitchModeFollowAndWait()
	{
		if (_TribeMovementsMode == TribeMovementsMode.FollowPlayer)
			ModeStopAndWait();
		else if (_TribeMovementsMode == TribeMovementsMode.Wait || _TribeMovementsMode == TribeMovementsMode.GoToBeacon)
			ModeFollowPlayer();
	}

	public void ModeGoToBeacon(Vector3 destination)
	{
		ModeStopAndWait();

		_TribeMovementsMode = TribeMovementsMode.GoToBeacon;
		_TribeNavAgent.SetDestination(new Vector3(destination.x, 0, destination.z));
	}

	public void ModeFollowPlayer()
	{
		_TribeMovementsMode = TribeMovementsMode.FollowPlayer;
		AmbiantManager.I.onHourChanged.AddListener(TribeFollowPlayer);
	}
	void TribeFollowPlayer(int currentHours, DayState currentDayState)
	{
		Vector3 playerPositionXZ = new Vector3(_Player.transform.position.x, 0, _Player.transform.position.z);
		if (_TribeNavAgent.destination != playerPositionXZ)
			_TribeNavAgent.SetDestination(playerPositionXZ);
	}

	public void ModeStopAndWait()
	{
		_TribeMovementsMode = TribeMovementsMode.Wait;
		AmbiantManager.I.onHourChanged.RemoveListener(TribeFollowPlayer);

		//if (_TribeMovementsMode == TribeMovementsMode.FollowPlayer)
		//	AmbiantManager.I.onHourChanged.RemoveListener(TribeFollowPlayer);
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
		_TribeNavAgent.speed = GameManager.I._data.InitialSpeedTribe;
        _TribeNavAgent.angularSpeed = GameManager.I._data.InitialSpeedRotationTribe;
    }

	void TribeInCriticalSpeed()
	{
		_TribeNavAgent.speed = GameManager.I._data.InitialSpeedTribe * GameManager.I._data.CriticalSpeedTribeMultiplicator;
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
