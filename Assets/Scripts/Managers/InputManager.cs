using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : Singleton<InputManager>
{
    public InputAxisUnityEvent onInputAxisEvent = new InputAxisUnityEvent();
    public UnityEvent onGKeyPressed = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        
    }

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

        if (Input.GetKeyDown(KeyCode.G))
        {
            onGKeyPressed?.Invoke();
        }
    }
}

public class InputAxisUnityEventArg 
{
    public string Direction;
    public float Value;
}
public class InputAxisUnityEvent : UnityEvent<InputAxisUnityEventArg> { }