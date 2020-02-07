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
    [TabGroup("Player")][Tooltip("Player run is unusable when energy low")]
    public bool PlayerRunEnergyLowUnusable;
    [TabGroup("Player")][Tooltip("Running is limited by a run stamina")]
    public bool PlayerRunStamina;
    [TabGroup("Player")][Tooltip("Player run stamina maximum")]
    public float PlayerRunStaminaMax = 100f;
	[TabGroup("Player")][Tooltip("player run stamina cost by second when running")]
    public float PlayerRunCostBySecond = 1f;
    [TabGroup("Player")][Tooltip("player run stamina gain by second when walking")]
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
	public int InitialTribeEnergy = 100;
	[TabGroup("Tribe")][Tooltip("Percent of energy for entering in critical state")][Range(0, 100)]
	public float PercentEnergyTribeForCritical = 5f;
	[TabGroup("Tribe")][Tooltip("Energy tribe looses per second when waiting")]
	public float TribeEnergyLossWaiting = .5f;
	[TabGroup("Tribe")][Tooltip("Energy tribe looses per second when moving")]
	public float TribeEnergyLossMoving = 3f;

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

	[Title("Tribe docility")]

	[TabGroup("Tribe")]
    [Tooltip("Docility score at start up")]
    public int InitialDocilityScore = 50;

    [TabGroup("Tribe")]
    [Tooltip("Permanent bonus when an item is activated")]
    public int ItemActivationBonus = 100;

    [TabGroup("Tribe")]
    [Tooltip("Permanent bonus for each new day")]
    public int NewDayBonus = 10;

    [TabGroup("Tribe")]
    [Tooltip("Permanent malus when the player is too far from the tribe")]
    public int TooFarFromTribeMalus = 10;

    [TabGroup("Tribe")]
    [Tooltip("Temporary malus when the tribe energy is critical")]
    public int CriticalEnergyMalus = 50;

    [TabGroup("Tribe")]
    [Tooltip("Temporary bonus at night")]
    public int NightBonus = 50;

    [TabGroup("Tribe")]
	[Tooltip("Params at level 0 (score < 0)")]
    public TribeDocilityParams Level0Params;

    [TabGroup("Tribe")]
	[Tooltip("Params at level 1 (score < 100)")]
    public TribeDocilityParams Level1Params;

    [TabGroup("Tribe")]
	[Tooltip("Params at level 2 (score < 200)")]
    public TribeDocilityParams Level2Params;

    [TabGroup("Tribe")]
	[Tooltip("Params at level 3 (score < 300)")]
    public TribeDocilityParams Level3Params;

    [TabGroup("Tribe")]
	[Tooltip("Params at level 4 (score >= 300)")]
    public TribeDocilityParams Level4Params;

	[TabGroup("Tribe")]
	[Tooltip("Minimal timer duration between potential spontaneous actions")]
    public int SpontaneityCheckTimerMinDuration = 30;

	[TabGroup("Tribe")]
	[Tooltip("Maximal timer duration between potential spontaneous actions")]
    public int SpontaneityCheckTimerMaxDuration = 60;

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
    [Tooltip("Interaction with an item")]
    public PlayerInputs Input_Interaction;
    [TabGroup("Inputs")]
    [Tooltip("Open items inventory")]
    public PlayerInputs Input_InventoryOpenClose;
    [TabGroup("Inputs")]
    [Tooltip("Select in inventory")]
    public PlayerInputs Input_InventorySelect;
    [TabGroup("Inputs")]
    [Tooltip("back in inventory")]
    public PlayerInputs Input_InventoryBack;

    [TabGroup("UI")]
    [Tooltip("Messages duration")]
    public float MessageDuration;
}

[System.Serializable]
public class TribeDocilityParams
{
    [Range(0, 1f)] public float IgnoranceProbability = 0.5f;
    public int IgnoranceDuration = 10;
    [Range(0, 1f)] public float SpontaneityProbability = 0.5f;
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
	public float TimeStateChanged;
	//public Color StateColor;
	//public Vector3 EnterSunRotation;
	//public Vector3 ExitSunRotation;
}
