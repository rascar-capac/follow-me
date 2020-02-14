using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZoneLink : Zone
{
    public ZoneEgg LinkedZone;
    Player player;
    Tribe tribe;
    bool IsActivated = false;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        player.onZoneEnter.AddListener(EnteredZone);
        tribe = ((GameObject)ObjectsManager.I["Tribe"]).GetComponent<Tribe>();
    }

    public void EnteredZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this && !IsActivated)
        {
            tribe.StopAll();
            LinkedZone.gameObject.SetActive(true);
            LinkedZone.Ray.gameObject.SetActive(false);
            tribe.SetMode(TribeEmotionMode.Happy);
            tribe.StartRotating(LinkedZone.Egg.transform);
            AmbiantManager.I.SkipDayTimeToPhase(LinkedZone.PhaseIndex);
            IsActivated = true;
        }
    }
}
