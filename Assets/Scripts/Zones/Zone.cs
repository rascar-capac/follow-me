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
    [Header("Player toxicity speed by second")]
    public float HurtSpeed = 0.0f;
    [Header("Player gain life speed by second")]
    public float GainSpeed = 0.0f;
    [Header("Authorized layer")]
    public LayerMask Layers;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collide");
        if (Layers == (Layers | (1 << other.gameObject.layer)))
        {
            other.gameObject.GetComponent<ZoneInteractable>().EnterZone(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Layers == (Layers | (1 << other.gameObject.layer)))
        {
            other.gameObject.GetComponent<ZoneInteractable>().ExitZone(this);
        }
    }
}
