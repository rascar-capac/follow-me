using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneEgg : Zone
{
    public GameObject Egg;
    public GameObject Ray;
    public int ColorIndex;
    public bool AllowActivate = false;
    public bool IsActivate => Ray.activeSelf;
    Player player;
    Tribe tribe;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        player.onZoneEnter.AddListener(EnteredZone);
        player.onZoneExit.AddListener(ExitedZone);
        tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();
        GameData gd = GameManager.I._data;
        Color rayColor = ColorIndex == gd.Phases.Count ? gd.SpecialPhase.color : gd.Phases[ColorIndex].color;
        Ray.GetComponent<Renderer>().material.SetColor("_Color", rayColor);
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
