using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct EventToggle
{
    public List<UnityEvent> Pressed;
    public List<UnityEvent> Maintain;
    public List<UnityEvent> Released;
}
public class InputManager : Singleton<InputManager>
{
	
	[Header("Key for open Tribe Menu")]
	public KeyCode UITribeKey = KeyCode.T;
	[Header("Key for open Quest Menu")]
	public KeyCode UIQuestKey = KeyCode.O;
	[Header("Key for open Map Menu")]
	public KeyCode UIMapKey = KeyCode.M;
	[Header("Key for open Options Menu")]
	public KeyCode UIOptionsKey = KeyCode.F1;

	// Main Events

    //public UnityEvent onToolInventoryButtonPressed = new UnityEvent();
    //public UnityEvent onToolInventoryButtonReleased = new UnityEvent();

    // UI Events
 //   public UnityEvent onUIPlayerKeyPressed = new UnityEvent();
	//public UnityEvent onUITribeKeyPressed = new UnityEvent();
	//public UnityEvent onUIQuestKeyPressed = new UnityEvent();
	//public UnityEvent onUIMapKeyPressed = new UnityEvent();
	//public UnityEvent onUIOptionsKeyPressed = new UnityEvent();

    float lastLTRT = 0;

	// Player Events
	public InputAxisUnityEvent onMoveInputAxisEvent = new InputAxisUnityEvent();
    public InputAxisUnityEvent onLookInputAxisEvent = new InputAxisUnityEvent();

    public UnityEvent onPauseKeyPressed = new UnityEvent();

    public UnityEvent onRunButtonPressed = new UnityEvent();
    public UnityEvent onRunButtonReleased = new UnityEvent();

    public UnityEvent onBeaconPlaceButtonPressed = new UnityEvent();
    public UnityEvent onBeaconActivateKeyPressed = new UnityEvent();

    public UnityEvent onPickUpKeyPressed = new UnityEvent();
    public UnityEvent onActivateItemKeyPressed = new UnityEvent();

	public UnityEvent onUIOpenCloseInventoryKeyPressed = new UnityEvent();

    public UnityEvent onTribeOrderKeyPressed = new UnityEvent();

    public UnityEvent onLeftHandOpenToolInventory = new UnityEvent();
    public UnityEvent onLeftHandCloseToolInventory = new UnityEvent();
    public UnityEvent onLeftHandShowHide = new UnityEvent();

    public UnityEvent onRightHandOpenToolInventory = new UnityEvent();
    public UnityEvent onRightHandCloseToolInventory = new UnityEvent();
    public UnityEvent onRightHandShowHide = new UnityEvent();

    public Dictionary<PlayerInputs, EventToggle> EventInputMapping = new Dictionary<PlayerInputs, EventToggle>();

    protected override void Start()
    {
        base.Start();

        EventInputMapping.Add(GameManager.I._data.Input_Pause, new EventToggle() { Pressed = new List<UnityEvent> { onPauseKeyPressed } });

        EventInputMapping.Add(GameManager.I._data.Input_PlayerRun, new EventToggle() { Pressed = new List<UnityEvent> { onRunButtonPressed }, Released = new List<UnityEvent> { onRunButtonReleased } });

        EventInputMapping.Add(GameManager.I._data.Input_BeaconPlace, new EventToggle() { Pressed = new List<UnityEvent> { onBeaconPlaceButtonPressed } });
        EventInputMapping.Add(GameManager.I._data.Input_BeaconActivate, new EventToggle() { Pressed = new List<UnityEvent> { onBeaconActivateKeyPressed } });

        EventInputMapping.Add(GameManager.I._data.Input_Item_ActivationPickup, new EventToggle() { Pressed = new List<UnityEvent> { onPickUpKeyPressed, onActivateItemKeyPressed } });

        EventInputMapping.Add(GameManager.I._data.Input_TribeOrder, new EventToggle() { Pressed = new List<UnityEvent> { onTribeOrderKeyPressed } });

        EventInputMapping.Add(GameManager.I._data.Input_LeftHand_SelectTool, new EventToggle() { Pressed = new List<UnityEvent> { onLeftHandOpenToolInventory }, Released = new List<UnityEvent> { onLeftHandCloseToolInventory } });
        EventInputMapping.Add(GameManager.I._data.Input_LeftHand_ShowHide, new EventToggle() { Pressed = new List<UnityEvent> { onLeftHandShowHide } });

        EventInputMapping.Add(GameManager.I._data.Input_RightHand_SelectTool, new EventToggle() { Pressed = new List<UnityEvent> { onRightHandOpenToolInventory }, Released = new List<UnityEvent> { onRightHandCloseToolInventory } });
        EventInputMapping.Add(GameManager.I._data.Input_RightHand_ShowHide, new EventToggle() { Pressed = new List<UnityEvent> { onRightHandShowHide } });

        EventInputMapping.Add(GameManager.I._data.Input_InventoryOpenClose, new EventToggle() { Pressed = new List<UnityEvent> { onUIOpenCloseInventoryKeyPressed } });
    }

    // Update is called once per frame
    void Update()
    {
        //Player moving (keyboard)
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");
        if (horizontalAxis != 0 || verticalAxis != 0)
            onMoveInputAxisEvent?.Invoke(new InputAxisUnityEventArg() { XDirection = "Horizontal", XValue = horizontalAxis, YDirection = "Vertical", YValue = verticalAxis });
        //Player moving (XBox controller)
        horizontalAxis = Input.GetAxis("XBoxLeftStickHorizontal");
        verticalAxis = Input.GetAxis("XBoxLeftStickVertical");
        if (horizontalAxis != 0 || verticalAxis != 0)
            onMoveInputAxisEvent?.Invoke(new InputAxisUnityEventArg() { XDirection = "XBoxLeftStickHorizontal", XValue = horizontalAxis, YDirection = "XBoxLeftStickVertical", YValue = verticalAxis });

        //Player Look (mouse)
        float CameraLookX = Input.GetAxis("Mouse X");
        float CameraLookY = Input.GetAxis("Mouse Y");
        if (CameraLookX != 0 || CameraLookY != 0)
            onLookInputAxisEvent?.Invoke(new InputAxisUnityEventArg() { XDirection = "Mouse X", XValue = CameraLookX, YDirection = "MouseY", YValue = CameraLookY });

        //Player Look (XBox controller)
        CameraLookX = Input.GetAxis("XBoxRightStickHorizontal");
        CameraLookY = Input.GetAxis("XBoxRightStickVertical");
        if (CameraLookX != 0 || CameraLookY != 0)
            onLookInputAxisEvent?.Invoke(new InputAxisUnityEventArg() { XDirection = "XBoxRightStickHorizontal", XValue = CameraLookX, YDirection = "XBoxRightStickVertical", YValue = CameraLookY * -1 });

        //Prepare LT / RT Values (LT / RT are 2 buttons but works like an axis)
        float LTRTAxis = Input.GetAxis("XBoxLTRT");
        bool LTPressed = LTRTAxis < 0 && lastLTRT >= 0;
        bool LTReleased = LTRTAxis >= 0 && lastLTRT < 0;
        bool RTPressed = LTRTAxis > 0 && lastLTRT <= 0;
        bool RTReleased = LTRTAxis <= 0 && lastLTRT > 0;
        lastLTRT = LTRTAxis;

        bool pressed = false;
        bool released = false;

        //CUSTOMISABLE INPUTS
        foreach (KeyValuePair<PlayerInputs, EventToggle> pair in EventInputMapping)
        {
            if (pair.Key.XBox == XBoxInputs.None || pair.Key.Keyboard == KeyCode.None)
                continue;

            pressed = false;
            released = false;
            if (pair.Key.XBox == XBoxInputs.LT)
            {
                pressed = LTPressed || Input.GetKeyDown(pair.Key.Keyboard);
                released = LTReleased || Input.GetKeyUp(pair.Key.Keyboard);
            }
            else if (pair.Key.XBox == XBoxInputs.RT)
            {
                pressed = RTPressed || Input.GetKeyDown(pair.Key.Keyboard);
                released = RTReleased || Input.GetKeyUp(pair.Key.Keyboard);
            }
            else
            {
                pressed =   Input.GetButtonDown($"XBox{pair.Key.XBox}") ||
                            Input.GetKeyDown(pair.Key.Keyboard);
                released = Input.GetButtonUp($"XBox{pair.Key.XBox}") ||
                            Input.GetKeyUp(pair.Key.Keyboard);
            }

            if (pressed && pair.Value.Pressed != null)
                pair.Value.Pressed.ForEach (e => e.Invoke());

            if (released && pair.Value.Released != null)
                pair.Value.Released.ForEach(e => e.Invoke());
        }

		//if (Input.GetKeyDown(UITribeKey))
		//	onUITribeKeyPressed?.Invoke();
		//if (Input.GetKeyDown(UIQuestKey))
		//	onUIQuestKeyPressed?.Invoke();
		//if (Input.GetKeyDown(UIMapKey))
		//	onUIMapKeyPressed?.Invoke();
		//if (Input.GetKeyDown(UIOptionsKey))
		//	onUIOptionsKeyPressed?.Invoke();
	}
}

public class InputAxisUnityEventArg 
{
    public string XDirection;
    public float XValue;
    public string YDirection;
    public float YValue;
}
public class InputAxisUnityEvent : UnityEvent<InputAxisUnityEventArg> { }