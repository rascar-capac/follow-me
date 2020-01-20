using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RessourcesType
{
    Food,
    Water
}

public class Zone : BaseMonoBehaviour
{
    [Header("Zone radius")]
    public float Radius = 5f;
    [Header("Disable use of compass")]
    public bool AllowCompass = false;
    [Header("Ressources")]
    public RessourcesType[] Ressources;
    [Header("Player layer")]
    public LayerMask PlayerLayer;
    [Header("Tribe layer")]
    public LayerMask TribeLayer;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerLayer == (PlayerLayer | (1 << other.gameObject.layer)))
        {
            Debug.Log("Player Entered zone !");
        }
        else if (TribeLayer == (TribeLayer | (1 << other.gameObject.layer)))
        {
            Debug.Log("Tribe Entered zone !");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PlayerLayer == (PlayerLayer | (1 << other.gameObject.layer)))
        {
            Debug.Log("Player Exited zone !");
        }
        else if (TribeLayer == (TribeLayer | (1 << other.gameObject.layer)))
        {
            Debug.Log("Tribe Exited zone !");
        }
    }
}
