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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            return;
        }

        if(parent.IsInside && parent.IsBeingActivated)
        {
            parent.IsBeingActivated = false;
            parent.IsActivated = true;
        }
        parent.IsInside = !parent.IsInside;
    }
}
