using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : Singleton<InputManager>
{
    [Header("Key for pausing the game")]
    public KeyCode PauseKey = KeyCode.Escape;
    [Header("Key for placing Beacon")]
    public KeyCode BeaconKey = KeyCode.G;
	[Header("Key for pick-up item")]
	public KeyCode PickUpKey = KeyCode.E;

	public InputAxisUnityEvent onInputAxisEvent = new InputAxisUnityEvent();
    public UnityEvent onBeaconKeyPressed = new UnityEvent();
    public UnityEvent onPauseKeyPressed = new UnityEvent();
	public UnityEvent onPickUpKeyPressed = new UnityEvent();

	// Update is called once per frame
	void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        if (Input.GetAxis("Horizontal") != 0) {
            onInputAxisEvent?.Invoke(new InputAxisUnityEventArg() { Direction = "Horizontal", Value = horizontalAxis });
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            onInputAxisEvent?.Invoke(new InputAxisUnityEventArg() { Direction = "Vertical", Value = verticalAxis });
        }

        if (Input.GetKeyDown(BeaconKey))
        {
            onBeaconKeyPressed?.Invoke();
        }

        if (Input.GetKeyDown(PauseKey))
        {
            onPauseKeyPressed?.Invoke();
        }

		if (Input.GetKeyDown(PickUpKey))
		{
			onPickUpKeyPressed?.Invoke();
		}
	}
}

public class InputAxisUnityEventArg 
{
    public string Direction;
    public float Value;
}
public class InputAxisUnityEvent : UnityEvent<InputAxisUnityEventArg> { }