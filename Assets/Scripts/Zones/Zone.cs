using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RessourcesType
{
    Food,
    Water
}

[System.Serializable]
public class TribePlayerDialog
{
    [Multiline()]
    public string Message = "";
    public MessageOrigin Who = MessageOrigin.System;
}

public class Zone : BaseMonoBehaviour
{
    public AudioClip Music;
    protected bool MusicPlayed = false;

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

    [Header("The dialog between tribe and player when player enter zone")]
    public List<TribePlayerDialog> Dialogs;

    private void OnTriggerEnter(Collider other)
    {
        if (Layers == (Layers | (1 << other.gameObject.layer)))
        {
            other.gameObject.GetComponent<ZoneInteractable>().EnterZone(this);
            if (!MusicPlayed && other.gameObject.GetComponent<ZoneInteractable>() is Player)
            {
                SoundManager.I.PlayZoneAmbiance(Music);
                MusicPlayed = true;
            }
   //         if (Dialogs != null && other.gameObject.GetComponent<ZoneInteractable>() is Player)
   //         {
			//	//foreach (TribePlayerDialog dialog in Dialogs)
			//	//{
			//	//	//UIManager.I.AlertMessage(dialog.Message, WhoTalks: dialog.Who);
			//	//}
			//}
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
