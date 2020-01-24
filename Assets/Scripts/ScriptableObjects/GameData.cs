using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Game Parameters", menuName = "New game LD", order = 1)]
public class GameData : ScriptableObject
{
	[Title("Controls parameters")]
    [Tooltip("Player look mouse sensitivity")]
    public float mouseSensitivity = 100f;

	[Title("Player Energy")]
	[Tooltip("The energy of the player when the game starts.")]
	public float _initialPlayerEnergy = 100f;
	[Tooltip("Percent of energy for entering in critical state")] [Range(0, 100)]
	public float _percentLifePlayerForCritical = 5f;
	[Tooltip("Energy player lost per second in the day")]
	public float _energyPlayerLostPerSeconde = 1f;
	[Tooltip("Energy player gain per second in the night")]
	public float _energyPlayerGainPerSeconde = 1f;
	//[Tooltip("The life of the player when the game starts.")]
	//public float InitialPlayerLife = 100.0f;
	//[Tooltip("The oxygen of the player when the game starts.")]
	//public float InitialPlayerOxygen = 100.0f;
	//[Tooltip("Oxygen loss by second walking.")]
	//public float OxygenLossWalk = 0.2f;
	//[Tooltip("Oxygen loss by second running.")]
	//public float OxygenLossRun = 0.4f;

	[Title("Player Speed")]
	[Tooltip("The speed of the player when the game starts.")]
    public float InitialPlayerSpeed = 12.0f;
    [Tooltip("speed multiplicator value when running.")]
    public float SpeedMultiplicator = 2f;
    [Tooltip("The percentage of speed decrease when life is under thresshold.")]
    [Range(0, 100)]
    public float PlayerSpeedDecreasePercentage = 50f;
    //[Tooltip("The life thresshold under which the speed decreases.")]
    //[Range(0, 1)]
    //public float PlayerLifeThresshold = 0.1f;

	[Title(" Tools parameters")]
    [Tooltip("The maximum distance between player and tribe allowing beacon drop")]
    public float MaximumDistanceOfTribe = 300.0f;
    [Tooltip("The Angle tresshold of the compass")]
    public float CompassThresshold = 15f;
    [Tooltip("Activate/deactivate compass feature")]
    public bool CompassActive = true;
    [Tooltip("when compass is usable")]
    public List<DayState> CompassUsable;

    [Title("Tribe parameters")]
	[Tooltip("The energy of the tribe when the game starts.")]
	public float _initialTribeEnergy = 100f;
	[Tooltip("Percent of energy for entering in critical state")]
	[Range(0, 100)]
	public float _percentLifeTribeForCritical = 5f;
	[Tooltip("Energy tribe lost per second in the day")]
	public float _energyTribeLostPerSeconde = 1f;
	[Tooltip("Energy tribe gain per second in the night")]
	public float _energyTribeGainPerSeconde = 1f;
	[Tooltip("Default speed of Tribe")]
	public float _defaultSpeedTribe = 20f;
	[Tooltip("Critical speed of Tribe")]
	public float _criticalSpeedTribe = 0f;
	//[Tooltip("Initial Tribe energy when the game starts.")]
	//public float InitialTribeEnergy = 100.0f;
	//[Tooltip("Alert player when life is under threshold in ratio (between 0 and 1).")]
	//[Range(0, 1)]
	//public float TribeEnergyThresshold = 0.1f;
	//[Tooltip("Initial Tribe life when the game starts.")]
	//public float InitialTribeLife = 100.0f;
	//[Tooltip("Initial Tribe fuel when the game starts.")]
	//public float InitialTribeFuel = 100.0f;
	//[Tooltip("Fuel loss by second walking.")]
	//public float FuelLossSpeed = 0.2f;
	//[Tooltip("Alert player when life is under threshold in ratio (between 0 and 1).")]
	//[Range(0, 1)]
	//public float TribeLifeThresshold = 0.1f;

	[Title("Fog")]
    [Tooltip("Seconds between 2 fogs apparition (0 = no fog)")]
    public float MinimumTimeBetweenFog = 0;

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
