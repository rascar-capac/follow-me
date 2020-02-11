using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectEndCollision : MonoBehaviour
{
    private ArksPortal parent;

    private void Start()
    {
        parent = transform.GetComponentInParent<ArksPortal>();
    }

    private void OnTriggerStay()
    {
        if(parent.IsBeingActivated)
        {
            parent.IsBeingActivated = false;
            parent.IsActivated = true;
        }
    }
}
