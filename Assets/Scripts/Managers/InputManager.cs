using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : Singleton<InputManager>
{
	// Main Keys
    [Header("Key for pausing the game")]
    public KeyCode PauseKey = KeyCode.Escape;
	// Player Keys
    [Header("Key for placing Beacon")]
    public KeyCode BeaconKey = KeyCode.G;
	[Header("Key for pick-up item")]
	public KeyCode PickUpKey = KeyCode.E;
	// UI Keys
	[Header("Key for open Player Menu")]
	public KeyCode UIPlayerKey = KeyCode.P;
	[Header("Key for open Tribe Menu")]
	public KeyCode UITribeKey = KeyCode.T;
	[Header("Key for open Inventory Menu")]
	public KeyCode UIInventoryKey = KeyCode.I;
	[Header("Key for open Quest Menu")]
	public KeyCode UIQuestKey = KeyCode.O;
	[Header("Key for open Map Menu")]
	public KeyCode UIMapKey = KeyCode.M;
	[Header("Key for open Options Menu")]
	public KeyCode UIOptionsKey = KeyCode.F1;

	// Main Events
    public UnityEvent onPauseKeyPressed = new UnityEvent();
	// Player Events
	public InputAxisUnityEvent onInputAxisEvent = new InputAxisUnityEvent();
    public UnityEvent onBeaconKeyPressed = new UnityEvent();
	public UnityEvent onPickUpKeyPressed = new UnityEvent();
	// UI Events
	public UnityEvent onUIPlayerKeyPressed = new UnityEvent();
	public UnityEvent onUITribeKeyPressed = new UnityEvent();
	public UnityEvent onUIInventoryKeyPressed = new UnityEvent();
	public UnityEvent onUIQuestKeyPressed = new UnityEvent();
	public UnityEvent onUIMapKeyPressed = new UnityEvent();
	public UnityEvent onUIOptionsKeyPressed = new UnityEvent();

	// Update is called once per frame
	void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

		// Main
		if (Input.GetKeyDown(PauseKey))
			onPauseKeyPressed?.Invoke();

		// Player
		if (Input.GetAxis("Horizontal") != 0)
            onInputAxisEvent?.Invoke(new InputAxisUnityEventArg() { Direction = "Horizontal", Value = horizontalAxis });
        if (Input.GetAxis("Vertical") != 0)
            onInputAxisEvent?.Invoke(new InputAxisUnityEventArg() { Direction = "Vertical", Value = verticalAxis });
        if (Input.GetKeyDown(BeaconKey))
            onBeaconKeyPressed?.Invoke();
		if (Input.GetKeyDown(PickUpKey))
			onPickUpKeyPressed?.Invoke();

		// UI 
		if (Input.GetKeyDown(UIPlayerKey))
			onUIPlayerKeyPressed?.Invoke();
		if (Input.GetKeyDown(UITribeKey))
			onUITribeKeyPressed?.Invoke();
		if (Input.GetKeyDown(UIInventoryKey))
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
    public string Direction;
    public float Value;
}
public class InputAxisUnityEvent : UnityEvent<InputAxisUnityEventArg> { }