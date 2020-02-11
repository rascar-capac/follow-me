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

    private void OnTriggerStay(Collider other)
    {
        //if(other)
        if(!parent.IsBeingActivated)
        {
            parent.IsBeingActivated = true;
        }
    }
}
