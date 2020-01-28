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
    [Header("Ressources")]
    public RessourcesType[] Ressources;
    [Header("Player loose energy speed by second")]
    public float LooseEnergySpeed = 0.0f;
	[Header("Player gain energy speed by second")]
	public float GainEnergySpeed = 0.0f;
	[Header("Authorized layer")]
    public LayerMask Layers;

	[Header("Placer Coaster")]
	public GameObject _coaster; // Réference à coaster comme tu voulais.
	[Header("Placer SpawnPositionItem")]
	public Transform _spawnPositionItem; // Position de spawn d'un item dans la zone.
	[HideInInspector]
	public bool _itemOnCoaster = false; // Si le coaster possède un item.

	private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collide " + other.name);
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
