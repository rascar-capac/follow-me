using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Game Parameters", menuName = "New game LD", order = 1)]
public class GameData : ScriptableObject
{
	[Title("Camera")]
	[TabGroup("Player")][Tooltip("Camera rotation speed")]
    public float CameraRotationSpeed = 100f;

	[Title("Energy")]
	[TabGroup("Player")][Tooltip("The energy of the player when the game starts.")]
	public float InitialPlayerEnergy = 100f;
	[TabGroup("Player")][Tooltip("Energy player lost per second in the day")]
	public float EnergyPlayerLostPerSecond = 1f;
	[TabGroup("Player")][Tooltip("Energy player gain per second in the night")]
	public float EnergyPlayerGainPerSecond = 1f;

	[Title("Speed")]
	[TabGroup("Player")][Tooltip("The speed of the player when the game starts.")]
    public float InitialPlayerSpeed = 12.0f;
	[TabGroup("Player")][Tooltip("speed multiplicator value when running (Speed * SpeedMultiplicator = Run).")]
    public float SpeedMultiplicator = 2f;
	[TabGroup("Player")][Tooltip("when the player may run")]
    public List<DayState> PlayerRunUsable;
    [TabGroup("Player")][Tooltip("Player run gauge maximum")]
    public float PlayerRunGaugeMax = 100f;
    [TabGroup("Player")][Tooltip("Player run is unusable when energy low")]
    public bool PlayerRunEnergyLowUnusable;
	[TabGroup("Player")][Tooltip("player run gauge cost by second when running")]
    public float PlayerRunCostBySecond = 1f;
    [TabGroup("Player")][Tooltip("player run gauge gain by second when walking")]
    public float PlayerRunGainBySecond = 1f;

    [Title("Items interactions")]
	[TabGroup("Player")][Tooltip("The distance minimum betwees player and item for interaction.")]
    public float PlayerItemDistanceInteraction = 2f;
	[TabGroup("Player")][Tooltip("When the player may activate the stones.")]
    public List<DayState> StonesActivationUsable;

	[Title("Compass")]
	[TabGroup("Tools")][Tooltip("when compass is usable")]
    public List<DayState> CompassUsable;
	[TabGroup("Tools")][Tooltip("compass is unusable when energy low")]
    public bool CompassEnergyLowUnusable;

	[Title("Beacon")]
	[TabGroup("Tools")][Tooltip("when beacon placer is usable")]
    public List<DayState> BeaconPlacerUsable;
	[TabGroup("Tools")][Tooltip("beacon placer is unusable when energy low")]
    public bool BeaconPlacerEnergyLowUnusable;
	[TabGroup("Tools")][Tooltip("The maximum distance between player and tribe allowing beacon drop")]
    public float MaximumDistanceOfTribe = 300.0f;
	[TabGroup("Tools")][Tooltip("distance where the beacon is dropped by the player")]
    public float BeaconPlacementDistance = 3f;
    [TabGroup("Tools")][Tooltip("The count of beacon that may be placed at same time (0 = no limit).")]
    public int BeaconPlacementCount = 5;

    [Title("Map")]
	[TabGroup("Tools")][Tooltip("when map is usable")]
    public List<DayState> MapUsable;

	[Title("Sun dial")]
	[TabGroup("Tools")][Tooltip("when sundial is usable")]
    public List<DayState> SundialUsable;
	[TabGroup("Tools")][Tooltip("sundial is unusable when energy low")]
    public bool SundialEnergyLowUnusable;

	[Title("Energy meter")]
	[TabGroup("Tools")][Tooltip("when player energy meter is usable")]
    public List<DayState> PlayerEnergyMeterUsable;
	[TabGroup("Tools")][Tooltip("player energy meter is unusable when energy low")]
    public bool PlayerEnergyMeterEnergyLowUnusable;

	[Title("Energy meter")]
	[TabGroup("Tools")][Tooltip("when tribe energy meter is usable")]
    public List<DayState> TribeEnergyMeterUsable;
	[TabGroup("Tools")][Tooltip("tribe energy meter is unusable when energy low")]
    public bool TribeEnergyMeterEnergyLowUnusable;

	[Title("Distance meter")]
	[TabGroup("Tools")][Tooltip("when tribe distance meter is usable")]
    public List<DayState> TribeDistanceMeterUsable;
	[TabGroup("Tools")][Tooltip("tribe distance meter is unusable when energy low")]
    public bool TribeDistanceMeterEnergyLowUnusable;

    [Title("Tribe energy")]
	[TabGroup("Tribe")][Tooltip("The energy of the tribe when the game starts.")]
	public float InitialTribeEnergy = 100f;
	[TabGroup("Tribe")][Tooltip("Percent of energy for entering in critical state")][Range(0, 100)]
	public float PercentEnergyTribeForCritical = 5f;
	[TabGroup("Tribe")][Tooltip("Energy tribe lost per second in the day")]
	public float EnergyTribeLostPerSecond = 1f;
	[TabGroup("Tribe")][Tooltip("Energy tribe gain per second in the night")]
	public float EnergyTribeGainPerSecond = 1f;

	[Title("Tribe speed")]
	[TabGroup("Tribe")]
	[Tooltip("Futur Tribe Properties")]
	public TribeProperties TribeProperties;
	//[TabGroup("Tribe")][Tooltip("Default speed of Tribe")]
	//public float InitialSpeedTribe = 20f;
	//[TabGroup("Tribe")][Tooltip("Critical (energy low) speed multiplicator of Tribe (speed * multiplicator = new speed)")]
	//public float CriticalSpeedTribeMultiplicator = 0.5f;
	//[TabGroup("Tribe")][Tooltip("Default speed rotation of Tribe")]
 //   public float InitialSpeedRotationTribe = 10f;

    [Title("Ambience parameters")]
	[TabGroup("Ambience")][Tooltip("Day states (night and day)")]
    public DayStatesProperties[] States;

    [Title("Scene parameters")]
    [TabGroup("Zones")]
    [Tooltip("Final Turtle rock position after appearing (elevation position)")]
    public GameObject TurtleRockFinalPosition;

    [Title("Inputs Parameters")]
    [TabGroup("Inputs")]
    [Header("Player Move is readonly (Left stick / Arrows)")]
    [Header("Player Look is readonly (Right stick / Mouse)")]
    [Tooltip("Pause the game")]
    public PlayerInputs Input_Pause;
    [TabGroup("Inputs")]
    [Tooltip("Player jump")]
    public PlayerInputs Input_PlayerJump;
    [TabGroup("Inputs")]
    [Tooltip("Make the player run")]
    public PlayerInputs Input_PlayerRun;
    [TabGroup("Inputs")]
    [Tooltip("Order tribe to follow the player")]
    public PlayerInputs Input_TribeOrder;
    [TabGroup("Inputs")]
    [Tooltip("Open left hand inventory tools")]
    public PlayerInputs Input_LeftHand_SelectTool;
    [TabGroup("Inputs")]
    [Tooltip("Open right hand inventory tools")]
    public PlayerInputs Input_RightHand_SelectTool;
    [TabGroup("Inputs")]
    [Tooltip("Raise/Lower left hand")]
    public PlayerInputs Input_LeftHand_ShowHide;
    [TabGroup("Inputs")]
    [Tooltip("Raise/Lower right hand")]
    public PlayerInputs Input_RightHand_ShowHide;
    [TabGroup("Inputs")]
    [Tooltip("Place a beacon")]
    public PlayerInputs Input_BeaconPlace;
    [TabGroup("Inputs")]
    [Tooltip("Activate a beacon")]
    public PlayerInputs Input_BeaconActivate;
    [TabGroup("Inputs")]
    [Tooltip("Activate / Pickup an item")]
    public PlayerInputs Input_Item_ActivationPickup;
    [TabGroup("Inputs")]
    [Tooltip("Open items inventory")]
    public PlayerInputs Input_InventoryOpenClose;
    [TabGroup("Inputs")]
    [Tooltip("Select in inventory")]
    public PlayerInputs Input_InventorySelect;
    [TabGroup("Inputs")]
    [Tooltip("back in inventory")]
    public PlayerInputs Input_InventoryBack;

}

[System.Serializable]
public class PlayerInputs
{
    public XBoxInputs XBox = XBoxInputs.None;
    public KeyCode Keyboard = KeyCode.None;
}

public enum XBoxInputs
{
    None,
    A,
    B,
    X,
    Y,
    LB,
    RB,
    LT,
    RT,
    Back,
    Start,
    LeftStickPush,
    RightStickPush
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
