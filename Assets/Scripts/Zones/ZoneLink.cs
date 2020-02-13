﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneLink : Zone
{
    public ZoneEgg LinkedZone;
    Player player;
    Tribe tribe;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        player.onZoneEnter.AddListener(EnteredZone);
        tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();
    }

    public void EnteredZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this)
        {
            //LinkedZone.gameObject.SetActive(true);
            tribe.StopAll();
            LinkedZone.gameObject.SetActive(true);
            LinkedZone.Ray.gameObject.SetActive(false);
            LinkedZone.AllowActivate = true;
            tribe.SetMode(TribeEmotionMode.Happy);
            tribe.StartRotating(LinkedZone.Egg.transform);
        }
    }
}