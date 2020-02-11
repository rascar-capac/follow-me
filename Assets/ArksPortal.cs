using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArksPortal : MonoBehaviour
{

    public bool IsBeingActivated;
    public bool IsActivated;
    public GameObject Target;

    public void Start()
    {
        IsBeingActivated = false;
        IsActivated = false;
    }

    public void Update()
    {
        if(IsBeingActivated)
        {

        }
        if(IsActivated)
        {
            Target.SetActive(true);
        }
    }
}
