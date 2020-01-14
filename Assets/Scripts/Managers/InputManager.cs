using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : Singleton<InputManager>
{
    public InputAxisUnityEvent InputAxisEvent = new InputAxisUnityEvent();

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
            InputAxisEvent?.Invoke(new InputAxisUnityEventArg() { Direction = "Horizontal", Value = horizontalAxis });
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            InputAxisEvent?.Invoke(new InputAxisUnityEventArg() { Direction = "Vertical", Value = verticalAxis });
        }
    }
}

public class InputAxisUnityEventArg 
{
    public string Direction;
    public float Value;
}
public class InputAxisUnityEvent : UnityEvent<InputAxisUnityEventArg> { }