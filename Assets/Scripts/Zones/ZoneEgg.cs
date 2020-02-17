using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneEgg : Zone
{
    public Item Egg;
    public GameObject Ray;
    public int PhaseIndex;
    public bool HasActivationAllowed = false;
    public bool IsTileActivated = false;

    public bool IsActivate => Ray.activeSelf;
    Player player;
    Tribe tribe;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        player.onZoneEnter.AddListener(EnteredZone);
        player.onZoneExit.AddListener(ExitedZone);
        AmbiantManager.I.onTimePhaseChanged.AddListener(AllowActivation);
        tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();
        GameData gd = GameManager.I._data;
        Color rayColor = PhaseIndex == gd.Phases.Count ? gd.SpecialPhase.color : gd.Phases[PhaseIndex].color;
        Ray.transform.GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_Color", rayColor);
    }

    public void EnteredZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this && HasActivationAllowed)
        {
            tribe.StopAll();
            Ray.gameObject.SetActive(true);
            tribe.StartLive();
            Egg.ActivateItem();
        }
        else if (zone == this)
        {
            tribe.StopAll();
            tribe.StartAggress();
        }
    }

    public void ExitedZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this && !HasActivationAllowed)
        {
            tribe.StopAll();
            tribe.StartLive();
        }
    }

    public void AllowActivation(int currentPhaseIndex)
    {
        HasActivationAllowed = currentPhaseIndex == PhaseIndex && IsTileActivated;
    }
}
