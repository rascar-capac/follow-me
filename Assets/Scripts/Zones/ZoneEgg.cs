using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneEgg : Zone
{
    public GameObject Egg;
    public GameObject Ray;
    public int ColorIndex;
    public bool AllowActivate = false;

    Player player;
    Tribe tribe;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        player.onZoneEnter.AddListener(EnteredZone);
        player.onZoneExit.AddListener(ExitedZone);
        tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();
        Ray.GetComponent<Renderer>().material.SetColor("_Color", GameManager.I._data.PhasesColors[ColorIndex]);
    }

    public void EnteredZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this && AllowActivate)
        {
            tribe.StopAll();
            Ray.gameObject.SetActive(true);
            tribe.StartLive();
        }
        else if (zone == this)
        {
            tribe.StopAll();
            tribe.StartAggress();
        }
    }

    public void ExitedZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this && !AllowActivate)
        {
            tribe.StopAll();
            tribe.StartLive();
        }
    }
}
