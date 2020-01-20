using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tribe : ZoneInteractable
{
    public override void EnterZone(Zone zone)
    {
        Debug.Log("Tribe in zone");
    }

    public override void ExitZone(Zone zone)
    {
        Debug.Log("Tribe out zone");
    }
}
