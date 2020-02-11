using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectLateralCollisions : MonoBehaviour
{

    private ArksPortal parent;

    private void Start()
    {
        parent = transform.GetComponentInParent<ArksPortal>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            return;
        }

        if(parent.IsBeingActivated)
        {
            parent.IsBeingActivated = false;
        }
        parent.IsInside = !parent.IsInside;
    }
}
