using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArksPortal : MonoBehaviour
{

    public bool IsInside;
    public bool IsBeingActivated;
    public bool IsActivated;
    public GameObject Target;

    public void Start()
    {
        IsInside = false;
        IsBeingActivated = false;
        IsActivated = false;

        Target.SetActive(false);
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
