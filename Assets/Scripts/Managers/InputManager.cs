using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : Singleton<InputManager>
{
	// Main Keys
    //[Header("Key for pausing the game")]
    //public KeyCode PauseKey = KeyCode.Escape;
	// Player Keys
    //[Header("Key for placing Beacon")]
    //public KeyCode BeaconKey = KeyCode.G;
	//[Header("Key for pick-up item")]
	//public KeyCode PickUpKey = KeyCode.E;
	// UI Keys
	//[Header("Key for open Player Menu")]
	//public KeyCode UIPlayerKey = KeyCode.P;
	[Header("Key for open Tribe Menu")]
	public KeyCode UITribeKey = KeyCode.T;
	//[Header("Key for open Inventory Menu")]
	//public KeyCode UIInventoryKey = KeyCode.I;
	[Header("Key for open Quest Menu")]
	public KeyCode UIQuestKey = KeyCode.O;
	[Header("Key for open Map Menu")]
	public KeyCode UIMapKey = KeyCode.M;
	[Header("Key for open Options Menu")]
	public KeyCode UIOptionsKey = KeyCode.F1;

	// Main Events
    public UnityEvent onPauseKeyPressed = new UnityEvent();
	// Player Events
	public InputAxisUnityEvent onMoveInputAxisEvent = new InputAxisUnityEvent();
    public InputAxisUnityEvent onLookInputAxisEvent = new InputAxisUnityEvent();

    public UnityEvent onToolInventoryButtonPressed = new UnityEvent();
    public UnityEvent onToolInventoryButtonReleased = new UnityEvent();
    public UnityEvent onBeaconKeyPressed = new UnityEvent();
	public UnityEvent onPickUpKeyPressed = new UnityEvent();
    public UnityEvent onActivateItemKeyPressed = new UnityEvent();
    public UnityEvent onRunButtonPressed = new UnityEvent();
    public UnityEvent onRunButtonReleased = new UnityEvent();

    // UI Events
    public UnityEvent onUIPlayerKeyPressed = new UnityEvent();
	public UnityEvent onUITribeKeyPressed = new UnityEvent();
	public UnityEvent onUIInventoryKeyPressed = new UnityEvent();
	public UnityEvent onUIQuestKeyPressed = new UnityEvent();
	public UnityEvent onUIMapKeyPressed = new UnityEvent();
	public UnityEvent onUIOptionsKeyPressed = new UnityEvent();

    float lastLTRT = 0;
	// Update is called once per frame
	void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        float CameraLookX = Input.GetAxis("XBoxHorizontal4Axis");
        float CameraLookY = Input.GetAxis("XBoxVertical5Axis");

        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");

        // Main
        if (Input.GetButtonDown("Cancel"))
			onPauseKeyPressed?.Invoke();

		// Player
		if (horizontalAxis != 0 || verticalAxis != 0)
            onMoveInputAxisEvent?.Invoke(new InputAxisUnityEventArg() { XDirection = "Horizontal", XValue = horizontalAxis, YDirection = "Vertical", YValue = verticalAxis });
        if (CameraLookX != 0 || CameraLookY != 0)
            onLookInputAxisEvent?.Invoke(new InputAxisUnityEventArg() { XDirection = "XBoxHorizontal4Axis", XValue = CameraLookX, YDirection = "XBoxVertical5Axis", YValue = CameraLookY * -1});
        if (MouseX != 0 || MouseY != 0)
            onLookInputAxisEvent?.Invoke(new InputAxisUnityEventArg() { XDirection = "Mouse X", XValue = MouseX, YDirection = "MouseY", YValue = MouseY });

        float LTRTAxis = Input.GetAxis("XBoxLTRT");
        if (LTRTAxis != 0 && lastLTRT == 0)
        {
            lastLTRT = LTRTAxis;
            onToolInventoryButtonPressed?.Invoke();
        }
        else if (LTRTAxis == 0 && lastLTRT != 0)
        {
            lastLTRT = LTRTAxis;
            onToolInventoryButtonReleased?.Invoke();
        }

        if (Input.GetButtonDown("XBoxA"))
            onBeaconKeyPressed?.Invoke();
		if (Input.GetButtonDown("XBoxX"))
			onPickUpKeyPressed?.Invoke();
        if (Input.GetButtonDown("XBoxY"))
            onActivateItemKeyPressed?.Invoke();
        if (Input.GetButtonDown("XBoxLB"))
            onRunButtonPressed?.Invoke();
        if (Input.GetButtonUp("XBoxLB"))
            onRunButtonReleased?.Invoke();

        // UI 
        if (Input.GetButtonDown("XBoxStartButton"))
			onUIPlayerKeyPressed?.Invoke();
		if (Input.GetKeyDown(UITribeKey))
			onUITribeKeyPressed?.Invoke();
		if (Input.GetButtonDown("XBoxB"))
			onUIInventoryKeyPressed?.Invoke();
		if (Input.GetKeyDown(UIQuestKey))
			onUIQuestKeyPressed?.Invoke();
		if (Input.GetKeyDown(UIMapKey))
			onUIMapKeyPressed?.Invoke();
		if (Input.GetKeyDown(UIOptionsKey))
			onUIOptionsKeyPressed?.Invoke();
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