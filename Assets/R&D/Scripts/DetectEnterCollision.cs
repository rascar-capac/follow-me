using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectEnterCollision : MonoBehaviour
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

        parent.IsBeingActivated = parent.IsInside = !parent.IsInside;
    }
}
