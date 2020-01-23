using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Parameters", menuName = "New game LD", order = 1)]
public class GameData : ScriptableObject
{
    [Header("Player parameters")]
    [Tooltip("Player look mouse sensitivity")]
    public float mouseSensitivity = 100f;
	[Tooltip("The energy of the player when the game starts.")]
	public float _initialPlayerEnergy = 100f;
	[Tooltip("Percent of energy for entering in critical state")] [Range(0, 100)]
	public float _initialCriticalEnergy = 5f;
	[Tooltip("Energy player lost per second")]
	public float _energyLostPerSeconde = 1f;
    //[Tooltip("The life of the player when the game starts.")]
    //public float InitialPlayerLife = 100.0f;
    //[Tooltip("The oxygen of the player when the game starts.")]
    //public float InitialPlayerOxygen = 100.0f;
    //[Tooltip("Oxygen loss by second walking.")]
    //public float OxygenLossWalk = 0.2f;
    //[Tooltip("Oxygen loss by second running.")]
    //public float OxygenLossRun = 0.4f;
    [Tooltip("The speed of the player when the game starts.")]
    public float InitialPlayerSpeed = 12.0f;
    [Tooltip("speed multiplicator value when running.")]
    public float SpeedMultiplicator = 2f;
    [Tooltip("The percentage of speed decrease when life is under thresshold.")]
    [Range(0, 100)]
    public float PlayerSpeedDecreasePercentage = 50f;
    [Tooltip("The life thresshold under which the speed decreases.")]
    [Range(0, 1)]
    public float PlayerLifeThresshold = 0.1f;
    [Tooltip("The maximum distance between player and tribe allowing beacon drop")]
    public float MaximumDistanceOfTribe = 300.0f;
    [Tooltip("The Angle tresshold of the compass")]
    public float CompassThresshold = 15f;
    [Tooltip("Activate/deactivate compass feature")]
    public bool CompassActive = true;
    [Tooltip("when compass is usable")]
    public List<DayState> CompassUsable;

    [Header("Tribe parameters")]
    [Tooltip("Initial Tribe life when the game starts.")]
    public float InitialTribeLife = 100.0f;
    [Tooltip("Initial Tribe fuel when the game starts.")]
    public float InitialTribeFuel = 100.0f;
    [Tooltip("Fuel loss by second walking.")]
    public float FuelLossSpeed = 0.2f;
    [Tooltip("Alert player when life is under threshold in ratio (between 0 and 1).")]
    [Range(0, 1)]
    public float TribeLifeThresshold = 0.1f;

    [Header("Fog")]
    [Tooltip("Seconds between 2 fogs apparition (0 = no fog)")]
    public float MinimumTimeBetweenFog = 0;

    [Header("Ambiance parameters")]
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
