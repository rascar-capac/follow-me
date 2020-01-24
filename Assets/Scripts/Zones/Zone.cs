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
    public float Radius = 25f; // Radius d'une zone (pour Capsulecollider et Cylinder)
	[Header("Zone height")]
	public float Height = 200f; // Height d'une zone (pour Capsulecollider et Cylinder)
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


	// Pour adapter automatiquement la taille de la CapsuleCollider et de Cylinder(enfant) en fonction de Rius et Height de Zone.cs
	// Appelé uniquement quand une valeur est changée dans l'inspecteur.
	// Attention ne pas changer l'ordre des enfants dans les prefabs zones pour cette fonction.
	private void OnValidate()
	{
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.radius = Radius / 2;
        capsuleCollider.height = Height * 2;
        //Transform cylinder = transform.GetChild(1);
        //cylinder.localScale = new Vector3(Radius, Height, Radius);
    }

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
