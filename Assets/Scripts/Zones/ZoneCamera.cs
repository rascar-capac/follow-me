using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneCamera : Zone
{
    Player player;
    public Transform TargetObject;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<Player>();
        player.onZoneEnter.AddListener(EnterAZone);
    }

    public void EnterAZone(ZoneInteractable who, Zone zone)
    {
        if (zone == this)
        {
            CameraManager.I.SwitchCamera(duration: 10, target: TargetObject);
        }
    }
}
