using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneEgg : Zone
{
    public GameObject Egg;
    public GameObject Ray;

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
            tribe.StopAll();
            Ray.gameObject.SetActive(true);
            tribe.StartLive();
        }
    }
}
