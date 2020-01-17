using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : BaseMonoBehaviour
{
    [Header("Zone radius")]
    public float Radius = 5f;
    [Header("Zone radius")]
    public bool AllowCompass = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
