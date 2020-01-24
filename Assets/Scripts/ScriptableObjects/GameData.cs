using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Game Parameters", menuName = "New game LD", order = 1)]
public class GameData : ScriptableObject
{
	[Title("Controls parameters")]
    [Tooltip("Camera rotation speed")]
    public float CameraRotationSpeed = 100f;

    [Space(2)]
    [Title("Player Energy")]
	[Tooltip("The energy of the player when the game starts.")]
	public float InitialPlayerEnergy = 100f;
	[Tooltip("Energy player lost per second in the day")]
	public float EnergyPlayerLostPerSecond = 1f;
	[Tooltip("Energy player gain per second in the night")]
	public float EnergyPlayerGainPerSecond = 1f;

    [Title("Player Speed")]
	[Tooltip("The speed of the player when the game starts.")]
    public float InitialPlayerSpeed = 12.0f;
    [Tooltip("speed multiplicator value when running (Speed * SpeedMultiplicator = Run).")]
    public float SpeedMultiplicator = 2f;

    [Title("Player Items interactions")]
    [Tooltip("The distance minimum betwees player and item for interaction.")]
    public float PlayerItemDistanceInteraction = 2f;
    [Tooltip("When the player may activate the stones.")]
    public List<DayState> StonesActivationUsable;

    [Title("Tools parameters")]
    [Tooltip("The maximum distance between player and tribe allowing beacon drop")]
    public float MaximumDistanceOfTribe = 300.0f;
    [Tooltip("when compass is usable")]
    public List<DayState> CompassUsable;
    [Tooltip("compass is unusable when energy low")]
    public bool CompassEnergyLowUnusable;

    [Tooltip("when beacon placer is usable")]
    public List<DayState> BeaconPlacerUsable;
    [Tooltip("beacon placer is unusable when energy low")]
    public bool BeaconPlacerEnergyLowUnusable;
    [Tooltip("distance where the beacon is dropped by the player")]
    public float BeaconPlacementDistance = 3f;

    [Tooltip("when map is usable")]
    public List<DayState> MapUsable;

    [Tooltip("when the player may run")]
    public List<DayState> PlayerRunUsable;
    [Tooltip("Player run is unusable when energy low")]
    public bool PlayerRunEnergyLowUnusable;
    [Tooltip("Energy player cost per second when running")]
    public float EnergyPlayerRunCostPerSecond = 1f;


    [Tooltip("when sundial is usable")]
    public List<DayState> SundialUsable;
    [Tooltip("sundial is unusable when energy low")]
    public bool SundialEnergyLowUnusable;

    [Tooltip("when player energy meter is usable")]
    public List<DayState> PlayerEnergyMeterUsable;
    [Tooltip("player energy meter is unusable when energy low")]
    public bool PlayerEnergyMeterEnergyLowUnusable;

    [Tooltip("when tribe energy meter is usable")]
    public List<DayState> TribeEnergyMeterUsable;
    [Tooltip("tribe energy meter is unusable when energy low")]
    public bool TribeEnergyMeterEnergyLowUnusable;

    [Tooltip("when tribe distance meter is usable")]
    public List<DayState> TribeDistanceMeterUsable;
    [Tooltip("tribe distance meter is unusable when energy low")]
    public bool TribeDistanceMeterEnergyLowUnusable;

    [Title("Tribe parameters")]
	[Tooltip("The energy of the tribe when the game starts.")]
	public float InitialTribeEnergy = 100f;
	[Tooltip("Percent of energy for entering in critical state")]
	[Range(0, 100)]
	public float PercentEnergyTribeForCritical = 5f;
	[Tooltip("Energy tribe lost per second in the day")]
	public float EnergyTribeLostPerSecond = 1f;
	[Tooltip("Energy tribe gain per second in the night")]
	public float EnergyTribeGainPerSecond = 1f;
	[Tooltip("Default speed of Tribe")]
	public float InitialSpeedTribe = 20f;
	[Tooltip("Critical (energy low) speed multiplicator of Tribe (speed * multiplicator = new speed)")]
	public float CriticalSpeedTribeMultiplicator = 0.5f;
    [Tooltip("Default speed rotation of Tribe")]
    public float InitialSpeedRotationTribe = 10f;

    [Title("Ambiance parameters")]
    [Tooltip("Day states (night and day)")]
    public DayStatesProperties[] States;
}

public enum DayState
{
    Day,
    Night,
    _count
}

[System.Serializable]
public struct DayStatesProperties
{
    public DayState State;
    public float DayStateDurationInSecond;
    public float StateHoursCount;
    public Color StateColor;
    public float TimeStateChanged;
    public Vector3 EnterSunRotation;
    public Vector3 ExitSunRotation;
}
